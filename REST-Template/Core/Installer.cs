using BreadAPI.Services;
using BreadAPI.Services.Users;
using BreadAPI.Services.Orders;
using BREADAPI.Services.Notifications;
using BREADAPI.Services.Emails;
using BREADAPI.Services.Core;
using BREADAPI.Services.Statistics;
using BREADAPI.Services.Products;
using BREADAPI.Services.Bakers;

namespace BreadAPI.DiContainer
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
            container.AddTransient<OrderService>();
            container.AddTransient<OrdersRepository>();
            container.AddTransient<MachinesRepository>();
            container.AddTransient<MachinesService>();
            container.AddTransient<NotificationService>();
            container.AddTransient<NotificationRepository>();
            container.AddTransient<EmailService>();
            container.AddTransient<FileService>();
            container.AddTransient<StatisticsRepository>();
            container.AddTransient<StatisticsService>();
            container.AddTransient<ProductsService>();
            container.AddTransient<ProductsRepository>();
            container.AddTransient<BakersRepository>();
            container.AddTransient<BakersService>();
        }
    }
}
