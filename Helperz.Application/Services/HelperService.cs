using AutoMapper;
using Helperx.Application.Constants;
using Helperz.Application.Contracts;
using Helperz.Domain.Entities;
using Helperz.Domain.Enums;
using Helperz.Domain.Interfaces.Repository;

namespace Helperx.Application.Services
{
    public class HelperService : IHelperService
    {
        private readonly IJobRepository _jobRepository;
        private readonly IQueueSenderService _queueSenderService;
        private readonly NotificationHubService _hubContext;
        private readonly IMapper _mapper;

        public HelperService(IJobRepository jobRepository,
            IQueueSenderService queueSenderService,
            NotificationHubService hubContext,
            IMapper mapper)
        {
            _jobRepository = jobRepository;
            _queueSenderService = queueSenderService;
            _hubContext = hubContext;
            _mapper = mapper;
        }

        public async Task<JobResponse> ProcessJobAsync(JobRequest jobRequest)
        {
            var response = new JobResponse();
            var job = new Job();

            _mapper.Map(jobRequest, job);

            switch (jobRequest.Action)
            {
                case JobActions.Creation:
                    await _jobRepository.CreateAsync(job);
                    response.Message = JobResponseMessages.CREATED_JOB;
                    break;

                case JobActions.Delete:
                    await _jobRepository.DeleteAsync(job);
                    response.Message = JobResponseMessages.DELETED_JOB;
                    break;

                case JobActions.Update:
                    await _jobRepository.UpdateAsync(job);
                    response.Message = JobResponseMessages.UPDATED_JOB;
                    break;

                default:
                    _jobRepository.GetAllJobs();
                    break;
            }

            response.Status = Helperz.Domain.Enums.JobStatus.Concluded;

            await _hubContext.SendJobUpdate(jobRequest.ToString());
            return response;
        }

        public async Task<JobResponse> SendJobToQueueAsync(JobRequest jobRequest)
        {
            var response = new JobResponse();

            if (VerifyDuplicityBetwenJobs(jobRequest))
            {
                response.Status = Helperz.Domain.Enums.JobStatus.Canceled;
                response.Message = JobResponseMessages.DUPLICITY_JOB;
                await _hubContext.SendJobUpdate(jobRequest.ToString());
                return response;
            }

            await _queueSenderService.SendAsync(jobRequest);

            response.Status = Helperz.Domain.Enums.JobStatus.Pending;
            response.Message = JobResponseMessages.CREATED_JOB;

            var job = new Job();
            _mapper.Map(jobRequest, job);
            job.Id = Guid.NewGuid().ToString();

            await _jobRepository.CreateAsync(job);

            //await _hubContext.SendJobUpdate(jobRequest.ToString());
            return response;
        }

        public bool VerifyDuplicityBetwenJobs(JobRequest newJob)
        {
            List<Job> jobsRegisters = _jobRepository.GetAllJobs();
            var duplicity = jobsRegisters.FirstOrDefault(x => x.Description == newJob.Description);

            if (duplicity != null)
            {
                return true;
            }
            return false;
        }
    }
}