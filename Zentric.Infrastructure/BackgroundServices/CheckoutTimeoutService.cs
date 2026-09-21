using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Zentric.Domain.Orders.Ports;
using Zentric.Domain.Orders.Enums;
using Zentric.Application.Common.Ports;
using Zentric.Domain.Inventories.Events;

namespace Zentric.Infrastructure.BackgroundServices
{
    public class CheckoutTimeoutService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CheckoutTimeoutService> _logger;

        public CheckoutTimeoutService(IServiceProvider serviceProvider, ILogger<CheckoutTimeoutService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var orderRepo = scope.ServiceProvider.GetRequiredService<ICustomerOrderRepository>();
                    var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    
                    _logger.LogInformation("CheckoutTimeoutService running.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing CheckoutTimeoutService.");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
