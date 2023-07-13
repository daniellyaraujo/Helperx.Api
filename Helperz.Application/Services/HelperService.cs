using AutoMapper;
using Azure;
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
        private readonly IMapper _mapper;

        public HelperService(IJobRepository jobRepository,
            IMapper mapper)
        {
            _jobRepository = jobRepository;
            _mapper = mapper;
        }

        public async Task ProcessJobAsync(object jobFromQueue)
        {
            var jobRequest = new JobRequest();
            _mapper.Map(jobFromQueue, jobRequest);
            
            await CreateJobAsync(jobRequest);
        }

        public async Task<JobResponse> CreateJobAsync(JobRequest jobRequest)
        {
            var response = new JobResponse();
            var job = new Job();

            _mapper.Map(jobRequest, job);

            if (jobRequest.ExecutionTime > DateTime.UtcNow)
            {
                response.Message = JobResponseMessages.LATE_JOB;
            }

            job.Status = JobStatus.Concluded;
            job.IsScheduleJob = jobRequest.IsScheduleJob;
            job.ExecutionTime = jobRequest.ExecutionTime ?? DateTime.UtcNow;
            job.Action = JobActions.Create;

            await _jobRepository.CreateAsync(job);

            response.JobStatus = JobStatus.Concluded;
            response.StatusCode = HttpStatusCode.OK;

            return response;
        }

        public async Task<JobResponse> RegisterJobInQueueAsync(JobRequest jobRequest)
        {
            var response = new JobResponse();
            var job = new Job();
            _mapper.Map(jobRequest, job);

            if (ChecksForDuplicityInJobDescription(jobRequest.Description))
            {
                response.Message = JobResponseMessages.DUPLICITY_JOB;
                response.StatusCode = HttpStatusCode.BadRequest;

                return response;
            }

            if (jobRequest.IsScheduleJob)
            {
                await _jobRepository.CreateAsync(job);

                response.JobStatus = JobStatus.Pending;
                response.Message = JobResponseMessages.SENT_JOB_TO_QUEUE;
                response.StatusCode = HttpStatusCode.Created;

                return response;
            }

            await CreateJobAsync(jobRequest);

            response.JobStatus = JobStatus.Concluded;
            response.StatusCode = HttpStatusCode.OK;

            return response;
        }

        public async Task<List<Job>> GetAllJobsAsync()
        {
            List<Job> jobs = _jobRepository.GetJobs();
            return jobs;
        }

        public async Task<JobResponse> UpdateJobByIdAsync(int jobId, JobRequest jobRequest)
        {
            var response = new JobResponse();

            Job job = await _jobRepository.GetJobByIdAsync(jobId);

            if (job.Description == null)
            {
                response.Message = JobResponseMessages.NOT_FOUND_JOB;
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            if (job.Status == JobStatus.Pending)
            {
                job.IsScheduleJob = jobRequest.IsScheduleJob;
                job.Description = jobRequest.Description;
                job.ExecutionTime = jobRequest.ExecutionTime ?? DateTime.UtcNow;

                await _jobRepository.UpdateAsync(job);

                response.JobStatus = JobStatus.Concluded;
                response.StatusCode = HttpStatusCode.OK;
                return response;
            }

            if (job.Status == JobStatus.Concluded)
            {
                response.Message = JobResponseMessages.ALREADY_COMPLETED_JOB;
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            return response;
        }
        
        public async Task<JobResponse> RemoveJobByIdAsync(int jobId)
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

            response.Message = JobResponseMessages.DELETED_JOB;
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }
        
        public bool ChecksForDuplicityInJobDescription(string jobDescription)
        {
            bool duplicity = _jobRepository.GetJobs().Any(x => x.Description == jobDescription);
            return duplicity;
        }
    }
}