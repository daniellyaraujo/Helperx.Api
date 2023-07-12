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
            
            await RegisterNewJobAsync(jobRequest);
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
                //ver como cancelar na fila e enviar dnv
            }

            job.Status = JobStatus.Concluded;
            job.Description = jobRequest.Description;
            job.ExecutionTime = jobRequest.ExecutionTime ?? DateTime.UtcNow;
            job.Action = JobActions.Update;

            await _jobRepository.UpdateAsync(job);

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

            response.JobStatus = JobStatus.Concluded;
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }

        public async Task<JobResponse> RegisterNewJobAsync(JobRequest jobRequest)
        {
            var response = new JobResponse();
            var job = new Job();
            _mapper.Map(jobRequest, job);

            if (ChecksForDuplicityInJobDescription(jobRequest.Description))
            {
                response.JobStatus = JobStatus.Canceled;
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

                //await _queueSenderService.SendToQueueAsync(job);

                return response;
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

        public bool ChecksForDuplicityInJobDescription(string jobDescription)
        {
            
            bool duplicity = _jobRepository.GetJobs().Any(x => x.Description == jobDescription);
            return duplicity;
        }

        public bool VerifyJobById(int jobId)
        {
            bool job = _jobRepository.GetJobs().Any(x => x.Id == jobId);
            return job;
        }
    }
}