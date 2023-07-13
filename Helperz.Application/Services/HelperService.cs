using AutoMapper;
using Helperx.Application.Constants;
using Helperx.Application.Contracts;
using Helperz.Application.Contracts;
using Helperz.Domain.Entities;
using Helperz.Domain.Enums;
using Helperz.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Helperx.Application.Services
{
    /// <summary>
    /// Job processing service class, update, create, delete and query jobs.
    /// </summary>
    public class HelperService : IHelperService
    {
        private readonly IJobRepository _jobRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<HelperService> _logger;

        /// <summary>
        /// Constructor of the Job processing service class, which allows the injection of other services.
        /// </summary>
        /// <param name="jobRepository"></param>
        /// <param name="mapper"></param>
        public HelperService(IJobRepository jobRepository,
            IMapper mapper,
            ILogger<HelperService> logger)
        {
            _jobRepository = jobRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Gets the list of pending jobs and sends them for processing.
        /// </summary>
        /// <returns></returns>
        public async Task ExecuteJobsPendingAsync()
        {
            _logger.LogInformation("Starting the query of the list of pending jobs.");
            var jobs = GetAllPendingJobsAsync();

            _logger.LogInformation("Submitting the list of jobs for processing.");
            await ProcessJobs(jobs);
        }

        /// <summary>
        /// Processes the list of pending jobs by sending them to be updated.
        /// </summary>
        /// <param name="jobsPending"></param>
        /// <returns></returns>
        public async Task ProcessJobs(List<Job> jobsPending)
        {
            _logger.LogInformation("Processing one by one from the queue.");
            foreach (var job in jobsPending)
            {
                _logger.LogInformation($"Submitting {job} to be updated.");
                await UpdatesJobStatusByQueueAsync(job);
            }
        }

        /// <summary>
        /// Updates work that came from the queue in the database.
        /// </summary>
        /// <param name="jobRequest"></param>
        /// <returns></returns>
        public async Task UpdatesJobStatusByQueueAsync(Job job)
        {
            if (job.IsScheduleJob && job.Status == JobStatus.Pending)
            {
                _logger.LogInformation("Checked that the job is late.");
                job.Status = JobStatus.Late;
            }

            _logger.LogInformation("");
            await _jobRepository.UpdateAsync(job);
        }

        /// <summary>
        /// Registers the job in the database.
        /// </summary>
        /// <param name="jobRequest"></param>
        /// <returns></returns>
        public async Task<JobResponse> RegisterJobAsync(JobRequest jobRequest)
        {
            var response = new JobResponse();
            var job = new Job();
            _mapper.Map(jobRequest, job);

            _logger.LogInformation($"Checking for duplicity between the description of the new job: '{jobRequest.Description}', and the existing ones.");
            if (ChecksForDuplicityInJobDescription(jobRequest.Description))
            {
                _logger.LogInformation("Confirmed duplicity");
                response.Message = JobResponseMessages.DUPLICITY_JOB;
                response.StatusCode = HttpStatusCode.BadRequest;

                _logger.LogInformation($"Return StatusCode: {response.StatusCode}");
                return response;
            }

            _logger.LogInformation("Creating job in the database with pending status.");
            job.Status = JobStatus.Pending;
            await _jobRepository.CreateAsync(job);

            response.JobStatus = JobStatus.Pending;
            response.StatusCode = HttpStatusCode.OK;
            response.Message = JobResponseMessages.SENT_JOB_TO_QUEUE;

            _logger.LogInformation($"Return StatusCode: {response.StatusCode}");
            return response;
        }

        /// <summary>
        /// Updates the job with pending status by informed id.
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="jobRequest"></param>
        /// <returns></returns>
        public async Task<JobResponse> UpdateJobByIdAsync(int jobId, UpdateJobRequest jobRequest)
        {
            var response = new JobResponse();

            _logger.LogInformation($"Checking if a job exists by id:{jobId}.");
            var job = await _jobRepository.GetByIdAsync(jobId);

            if (job == null)
            {
                response.Message = JobResponseMessages.NOT_FOUND_JOB;
                response.StatusCode = HttpStatusCode.BadRequest;

                _logger.LogInformation($"Return StatusCode: {response.StatusCode}");
                return response;
            }

            job.IsScheduleJob = jobRequest.IsScheduleJob;
            job.Status = jobRequest.CompletedJob;
            job.Description = jobRequest.Description ?? job.Description;
            job.ExecutionTime = jobRequest.ExecutionTime ?? DateTime.UtcNow;

            _logger.LogInformation($"Updating the job: {jobId} in the database.");
            await _jobRepository.UpdateAsync(job);

            response.JobStatus = JobStatus.Concluded;
            response.StatusCode = HttpStatusCode.OK;

            _logger.LogInformation($"Return StatusCode: {response.StatusCode}");
            return response;
        }

        /// <summary>
        /// Removes the job by id from the database.
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public async Task<JobResponse> RemoveJobByIdAsync(int jobId)
        {
            var response = new JobResponse();

            _logger.LogInformation($"Checking if a job exists by id:{jobId}.");
            var job = await _jobRepository.GetByIdAsync(jobId);

            if (job == null)
            {
                response.Message = JobResponseMessages.NOT_FOUND_JOB;
                response.StatusCode = HttpStatusCode.BadRequest;

                _logger.LogInformation($"Return StatusCode: {response.StatusCode}");
                return response;
            }

            _logger.LogInformation($"Deleting the job: {jobId} from the database.");
            await _jobRepository.DeleteAsync(job);

            response.Message = JobResponseMessages.DELETED_JOB;
            response.StatusCode = HttpStatusCode.OK;

            _logger.LogInformation($"Return StatusCode: {response.StatusCode}");
            return response;
        }

        /// <summary>
        /// Query all jobs listed.
        /// </summary>
        /// <returns></returns>
        public List<Job> GetAllJobsAsync()
        {
            _logger.LogInformation("Querying all database jobs.");
            List<Job> jobs = _jobRepository.GetAll();

            _logger.LogInformation("Returning all jobs in a list.");
            return jobs;
        }

        /// <summary>
        /// Queries all jobs that are in pending status in the database.
        /// </summary>
        /// <returns></returns>
        public List<Job> GetAllPendingJobsAsync()
        {
            _logger.LogInformation("Querying all jobs with a pending status in the database.");
            List<Job> jobs = _jobRepository.GetAllPendingAsync();

            _logger.LogInformation("Returning all jobs with a pending status in a list.");
            return jobs;
        }

        /// <summary>
        /// Queries all jobs that are in late status in the database.
        /// </summary>
        /// <returns></returns>
        public List<Job> VerifyLateJobsAsync()
        {
            _logger.LogInformation("Querying all jobs with a late status in the database.");
            List<Job> jobs = _jobRepository.GetAllLateAsync();

            _logger.LogInformation("Returning all jobs with a late status in a list.");
            return jobs;
        }

        /// <summary>
        /// Checks if there is duplicity between any existing job and the new job entered.
        /// </summary>
        /// <param name="jobDescription"></param>
        /// <returns></returns>
        public bool ChecksForDuplicityInJobDescription(string jobDescription)
        {
            bool duplicity = _jobRepository.GetByDescriptionAsync(jobDescription);
            return duplicity;
        }
    }
}