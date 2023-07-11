using Helperx.Consumer.Jobs.Services.Interfaces;

namespace Helperx.Consumer.Jobs
{
    public class Worker : BackgroundService
    {
        private readonly IListenerService _listenerService;

        public Worker(IListenerService listenerService)
        {
            _listenerService = listenerService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _listenerService.StartConsumingAsync();
        }
    }
}