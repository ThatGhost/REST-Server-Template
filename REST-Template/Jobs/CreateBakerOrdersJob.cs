using BreadAPI.Services;
using BreadAPI.Services.Orders;
using Quartz;

namespace BREADAPI.Jobs
{
    public class CreateBakerOrdersJob : IJob
    {
        private readonly OrdersRepository _ordersRepository;

        public CreateBakerOrdersJob(OrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            DateTime expiredDate = DateTime.Now.AddDays(1);
            List<Order> ordersToBeMade = await _ordersRepository.GetTommorowsOrdersToBeMade(expiredDate);
            await _ordersRepository.SetStateOfOrders(ordersToBeMade.Select(o => o.OrderId).ToList(), OrderState.Making);

            Console.WriteLine($"Todays orders: {ordersToBeMade.Count}");
        }
    }
}
