using Backend.Services;
using Backend.Services.Users;

namespace Backend.DiContainer
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

            // Register your services and repositories here
        }
    }
}
