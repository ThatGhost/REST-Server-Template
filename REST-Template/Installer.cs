using Backend.Services;
using Backend.Services.Core;
using Backend.Services.Users;

namespace Backend
{
    class DiContainer
    {
        public static void registerServices(IServiceCollection container)
        {
            container.AddTransient<IFileService, FileService>();
            container.AddTransient<IUsersRepository, UsersRepository>();
            container.AddTransient<IUsersService, UsersService>();
            container.AddTransient<IUsersAuthenticationService, UsersAuthenticationService>();
            container.AddTransient<IUsersAuthorazationService, UsersAuthorazationService>();

            // Register your services and repositories here
        }
    }
}
