﻿using Helperx.Application.Constants;
using Helperx.Application.Contracts.Common;
using Helperx.Application.Services;

namespace Helperx.Application.ConsumerServices
{
    public class JobService : IJobService
    {
        private readonly IHelperService _helperService;
        private readonly NotificationHubService _hubContext;

        public JobService(IHelperService helperService, NotificationHubService hubContext)
        {
            _helperService = helperService;
            _hubContext = hubContext;
        }

        public async Task<JobResponse> RunJobAsync(JobRequest baseRequest)
        {
            var response = new JobResponse();

            if (_helperService.VerifyDuplicityBetwenJobs(baseRequest))
            {
                response.Status = Helperz.Domain.Enums.JobStatus.Canceled;
                response.Message = JobResponseMessages.DUPLICITY_JOB;
                await _hubContext.SendJobUpdate(baseRequest.ToString());
                return response;
            }

            await _helperService.ProcessJobAsync(baseRequest);
            return response;
        }
    }
}