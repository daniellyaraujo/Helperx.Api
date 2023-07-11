using Helperx.Consumer.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace Helperx.Consumer
{
    [ExcludeFromCodeCoverage]
    public class Worker : BackgroundService
    {
        private readonly IServiceBusQueueListener _serviceBusQueueListener;

        public Worker(
        IServiceBusQueueListener serviceBusQueueListener)
        {
            _serviceBusQueueListener = serviceBusQueueListener;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _ = _serviceBusQueueListener.StartAsync(stoppingToken);
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}