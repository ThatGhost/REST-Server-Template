using BreadAPI.Services;
using BreadAPI.Services.Orders;
using Quartz;

namespace BREADAPI.Jobs
{
    public class FailedOrdersJob : IJob
    {
        private readonly OrdersRepository _ordersRepository;

        public FailedOrdersJob(OrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            DateTime expiredDate = DateTime.Now;
            List<Order> expiredOrders = await _ordersRepository.GetExpiredOrders(expiredDate);
            await _ordersRepository.SetStateOfOrders(expiredOrders.Select(o => o.OrderId).ToList(), OrderState.Failed);

            Console.WriteLine($"PastOrderJob: number of failed jobs = {expiredOrders.Count}");
        }
    }
}
