using Azure.Messaging.ServiceBus;
using Helperx.Consumer.Jobs.Services.Interfaces;
using Helperz.Application.Contracts;
using Newtonsoft.Json;

namespace Helperx.Application.ConsumerServices
{
    public class ListenerService : IListenerService
    {
        private readonly ServiceBusProcessor _processor;
        private readonly IJobService _jobService;

        public ListenerService(ServiceBusProcessor processor, IJobService jobService)
        {
            _processor = processor;
            _jobService = jobService;
        }

        public async Task StartConsumingAsync()
        {
            _processor.ProcessMessageAsync += async args =>
            {
                string message = args.Message.Body.ToString();
                var jobRequest = JsonConvert.DeserializeObject<JobRequest>(message);

                await _jobService.RunJobAsync(jobRequest);
            };

            _processor.ProcessErrorAsync += args =>
            {
                return Task.CompletedTask;
            };

            await _processor.StartProcessingAsync();
        }

        public async Task StopConsumingAsync()
        {
            await _processor.StopProcessingAsync();
        }
    }
}