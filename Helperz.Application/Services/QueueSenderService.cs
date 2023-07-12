﻿using Helperz.Domain.Entities;
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

        public async Task SendToQueueAsync(Job job)
        {
            var message = new Message(Encoding.UTF8.GetBytes(job.ToString()));
            message.ScheduledEnqueueTimeUtc = job.ScheduleTime;
            await _queueClient.SendAsync(message);
        }
    }
}