using Helperz.Application.Contracts;
using Microsoft.Azure.ServiceBus;
using System.Text;

namespace Helperx.Application.Services
{
    public class QueueSenderService : IQueueSenderService
    {
        private readonly QueueClient _queueClient;

        public QueueSenderService(QueueClient queueClient)
        {
            _queueClient = queueClient;
        }

        public async Task SendAsync(JobRequest job)
        {
            var message = new Message(Encoding.UTF8.GetBytes(job.ToString()));
            message.ScheduledEnqueueTimeUtc = job.TimeToExecute;
            await _queueClient.SendAsync(message);
        }
    }
}