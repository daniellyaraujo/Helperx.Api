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

            DirectToActionJob(job);

            response.JobStatus = JobStatus.Concluded;

            await _hubContext.SendToScreenJobUpdatesAsync(jobRequest.ToString());
            return response;
        }

        public async Task<List<Job>> GetJobByIdAsync(long jobId)
        {
            var job = _jobRepository.GetJobById(jobId).ToList();
            return job;
        }

        public async Task<JobResponse> RegisterNewJobAsync(JobRequest jobRequest)
        {
            var response = new JobResponse();
            var job = new Job();

            if (ChecksForDuplicityInJobDescription(jobRequest.Description))
            {
                response.JobStatus = JobStatus.Canceled;
                response.Message = JobResponseMessages.DUPLICITY_JOB;
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;

                return response;
            }

            if (jobRequest.ScheduleTime == default)
            {
                await _queueSenderService.SendToQueueAsync(job);

                response.JobStatus = JobStatus.Pending;
                response.Message = JobResponseMessages.SENT_JOB_TO_QUEUE;
                response.StatusCode = System.Net.HttpStatusCode.Created;
            }

            _mapper.Map(jobRequest, job);
            job.Status = JobStatus.Concluded;

            await _jobRepository.CreateAsync(job);
            //await _hubContext.SendToScreenJobUpdatesAsync(jobRequest.ToString());

            response.JobStatus = JobStatus.Concluded;
            response.StatusCode = System.Net.HttpStatusCode.OK;

            return response;
        }

        public bool ChecksForDuplicityInJobDescription(string jobDescription)
        {
            bool duplicity = _jobRepository.GetAllJobs().Any(x => x.Description == jobDescription);
            return duplicity;
        }

        public async Task DirectToActionJob(Job job)
        {
            if (job.Action == JobActions.Create)
            {
                await _jobRepository.CreateAsync(job);
            }
            if (job.Action == JobActions.Delete)
            {
                await _jobRepository.DeleteAsync(job);
            }
            if (job.Action == JobActions.Update)
            {
                await _jobRepository.UpdateAsync(job);
            }
            if (job.Action == JobActions.Read)
            {
                _jobRepository.GetAllJobs();
            }
        }
    }
}