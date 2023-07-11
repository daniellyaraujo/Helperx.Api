using Azure.Messaging.ServiceBus;
using Helperx.Application.Contracts.Common;

namespace Helperx.Application.ConsumerServices
{
    public interface IJobListenerService
    {
        Task<JobResponse> StartConsumingAsync(JobRequest baseRequest);
        Task StopConsumingAsync();
        Task StartConsumingAsync(Func<ServiceBusReceivedMessage, Task> processMessageCallback);
    }
}