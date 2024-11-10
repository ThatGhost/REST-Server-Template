
using Backend;
using Backend.core;
using Backend.migrations;
using Backend.migrations.core;

namespace Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Dont use certificates for local development
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    name: "_AllowLocalDevelopment",
                    policy =>
                    {
                        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                    }
                );
            });

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            builder.Services.RegisterJobs();
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(config =>
            {
                config.OperationFilter<AuthorizationHeaderFilter>();
            });

            DiContainer.registerServices(builder.Services);
            MigrationRegistry.registerServices(builder.Services);
            builder.Services.AddHostedService<Startup>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseMiddleware<ExeptionHandler>();
            }

            app.UseHttpsRedirection();
            app.UseCors("_AllowLocalDevelopment");
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
