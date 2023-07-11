using Helperx.Application.ConsumerServices;
using Helperx.Consumer.Jobs.Services;

namespace Helperx.Consumer.Jobs
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger,
        IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var serviceBusConsumer = scope.ServiceProvider.GetRequiredService<JobListenerService>();
                await serviceBusConsumer.StartConsumingAsync();

                // Mantenha o Worker em execução
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }

                await serviceBusConsumer.StopConsumingAsync();
            }
        }
    }
}