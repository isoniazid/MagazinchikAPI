using MagazinchikAPI.Services;
using Quartz;

namespace MagazinchikAPI.Services.Jobs
{
    public class PaymentChecker : IJob
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public PaymentChecker(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            /* Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine("Executed!");
            Console.ResetColor(); */

            using var scope = _serviceScopeFactory.CreateScope();
            IOrderService service = scope.ServiceProvider.GetService<IOrderService>() ?? throw new Exception("something wrong!");

            await service.CheckPaymentsForOrders();

        }
    }
}