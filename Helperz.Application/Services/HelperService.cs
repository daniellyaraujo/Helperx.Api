using AutoMapper;
using Helperx.Application.Constants;
using Helperz.Application.Contracts;
using Helperz.Domain.Entities;
using Helperz.Domain.Enums;
using Helperz.Domain.Interfaces.Repository;
using System.Net;

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

        public async Task ProcessJobAsync(JobRequest jobRequest)
        {
            await RegisterNewJobAsync(jobRequest);
        }

        public async Task<List<Job>> GetAllJobsAsync()
        {
            List<Job> jobs = _jobRepository.GetJobs();
            return jobs;
        }

        public async Task<JobResponse> UpdateJobByIdAsync(long jobId, JobRequest jobRequest)
        {
            var response = new JobResponse();

            Job job = await _jobRepository.GetJobByIdAsync(jobId);

            if (job.Description == null)
            {
                response.Message = JobResponseMessages.NOT_FOUND_JOB;
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            job.Status = JobStatus.Concluded;
            job.Description = jobRequest.Description;
            job.ScheduleTime = jobRequest.ScheduleTime;
            job.Action = jobRequest.Action;

            await _jobRepository.UpdateAsync(job);

            return response;
        }
        
        public async Task<JobResponse> RemoveJobByIdAsync(long jobId)
        {
            var response = new JobResponse();

            Job job = await _jobRepository.GetJobByIdAsync(jobId);
            if (job.Description != null)
            {
                response.Message = JobResponseMessages.NOT_FOUND_JOB;
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            await _jobRepository.DeleteAsync(job);

            response.JobStatus = JobStatus.Concluded;
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }

        public async Task<JobResponse> RegisterNewJobAsync(JobRequest jobRequest)
        {
            var response = new JobResponse();
            var job = new Job();

            if (ChecksForDuplicityInJobDescription(jobRequest.Description))
            {
                response.JobStatus = JobStatus.Canceled;
                response.Message = JobResponseMessages.DUPLICITY_JOB;
                response.StatusCode = HttpStatusCode.BadRequest;

                return response;
            }

            if (jobRequest.ScheduleTime != default)
            {
                await _queueSenderService.SendToQueueAsync(job);

                response.JobStatus = JobStatus.Pending;
                response.Message = JobResponseMessages.SENT_JOB_TO_QUEUE;
                response.StatusCode = HttpStatusCode.Created;
                return response;
            }

            _mapper.Map(jobRequest, job);
            job.Status = JobStatus.Concluded;

            await _jobRepository.CreateAsync(job);
            //await _hubContext.SendToScreenJobUpdatesAsync(jobRequest.ToString());

            response.JobStatus = JobStatus.Concluded;
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }


        public bool ChecksForDuplicityInJobDescription(string jobDescription)
        {
            bool duplicity = _jobRepository.GetJobs().Any(x => x.Description == jobDescription);
            return duplicity;
        }

        public bool VerifyJobById(long jobId)
        {
            bool job = _jobRepository.GetJobs().Any(x => x.Id == jobId);
            return job;
        }
    }
}