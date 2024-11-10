using Backend.Services;
using Backend.Services.Core;
using Backend.Services.Users;

namespace REST_Template
{
    class DiContainer
    {
        public static void registerServices(IServiceCollection container)
        {
            container.AddTransient<HttpClient>();
            container.AddTransient<UsersRepository>();
            container.AddTransient<UsersService>();
            container.AddTransient<UsersAuthenticationService>();
            container.AddTransient<UsersAuthorazationService>();
            container.AddTransient<FileService>();

            // Register your services and repositories here
        }
    }
}
