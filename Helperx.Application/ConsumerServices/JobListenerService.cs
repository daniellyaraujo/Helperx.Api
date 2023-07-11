using Azure.Messaging.ServiceBus;
using Helper.Domain.Entities;
using Helperx.Application.Constants;
using Helperx.Application.Contracts.Common;
using Helperx.Application.Services;

namespace Helperx.Application.ConsumerServices
{
    public class JobListenerService : IJobListenerService
    {
        private readonly IHelperService _helperService;
        private readonly NotificationHubService _hubContext;
        private readonly ServiceBusProcessor _processor;

        public JobListenerService(IHelperService helperService, NotificationHubService hubContext, ServiceBusProcessor processor)
        {
            _helperService = helperService;
            _hubContext = hubContext;
            _processor = processor;
        }

        public async Task StartConsumingAsync()
        {
            _processor.ProcessMessageAsync += async args =>
            {
                await StartConsumingAsync(args.Message);
            };

            _processor.ProcessErrorAsync += args =>
            {
                // Tratar erros de processamento de mensagens
                return Task.CompletedTask;
            };

            await _processor.StartProcessingAsync();
        }

        public async Task StopConsumingAsync()
        {
            await _processor.StopProcessingAsync();
        }

        public async Task<JobResponse> StartConsumingAsync(Job jobRequest)
        {
            var response = new JobResponse();

            if (_helperService.VerifyDuplicityBetwenJobs(jobRequest))
            {
                response.Status = Helper.Domain.Enums.JobStatus.Canceled;
                response.Message = JobResponseMessages.DUPLICITY_JOB;
                await _hubContext.SendJobUpdate(jobRequest.ToString());
                return response;
            }

            await _helperService.ProcessJobAsync(jobRequest);
            return response;
        }
    }
}