using AutoMapper;
using Helperx.Application.Constants;
using Helperz.Application.Contracts;
using Helperz.Domain.Entities;
using Helperz.Domain.Enums;
using Helperz.Domain.Interfaces;
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

        /// <summary>
        /// Constructor of the Job processing service class, which allows the injection of other services.
        /// </summary>
        /// <param name="jobRepository"></param>
        /// <param name="mapper"></param>
        public HelperService(IJobRepository jobRepository,
            IMapper mapper)
        {
            _jobRepository = jobRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the list of pending jobs and sends them for processing.
        /// </summary>
        /// <returns></returns>
        public async Task ExecuteJobsPendingAsync()
        {
            var jobs = GetAllPendingJobsAsync();
            await ProcessJobs(jobs);
        }

        /// <summary>
        /// Processes the list of pending jobs by sending them to be created.
        /// </summary>
        /// <param name="jobsPending"></param>
        /// <returns></returns>
        public async Task ProcessJobs(List<Job> jobsPending)
        {
            foreach (var job in jobsPending)
            {
                await CreateAsync(job);
            }
        }

        /// <summary>
        /// Creates in the database and performs the specified job.
        /// </summary>
        /// <param name="jobRequest"></param>
        /// <returns></returns>
        public async Task<JobResponse> CreateAsync(Job jobRequest)
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
            job.ExecutionTime = jobRequest.ExecutionTime;

            await _jobRepository.CreateAsync(job);

            response.JobStatus = JobStatus.Concluded;
            response.StatusCode = HttpStatusCode.OK;

            return response;
        }

        /// <summary>
        /// Registers and executes the job in the database, if it is a scheduled job, saves it with pending status, to be executed later.
        /// </summary>
        /// <param name="jobRequest"></param>
        /// <returns></returns>
        public async Task<JobResponse> RegisterJobAsync(JobRequest jobRequest)
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

            job.Status = JobStatus.Concluded;

            await CreateAsync(job);

            response.JobStatus = JobStatus.Concluded;
            response.StatusCode = HttpStatusCode.OK;

            return response;
        }

        /// <summary>
        /// Query all jobs listed.
        /// </summary>
        /// <returns></returns>
        public List<Job> GetAllJobsAsync()
        {
            List<Job> jobs = _jobRepository.GetAll();
            return jobs;
        }

        /// <summary>
        /// Queries all jobs that are in pending status in the database.
        /// </summary>
        /// <returns></returns>
        public List<Job> GetAllPendingJobsAsync()
        {
            List<Job> jobs = _jobRepository.GetAll().Where(x => x.Status == JobStatus.Pending).OrderBy(x => x.ExecutionTime).ToList();
            return jobs;
        }

        /// <summary>
        /// Updates the job with pending status by informed id.
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="jobRequest"></param>
        /// <returns></returns>
        public async Task<JobResponse> UpdateJobByIdAsync(int jobId, JobRequest jobRequest)
        {
            var response = new JobResponse();

            Job job = await _jobRepository.GetByIdAsync(jobId);

            if (job.Description == null)
            {
                response.Message = JobResponseMessages.NOT_FOUND_JOB;
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            if (job.Status != JobStatus.Pending || job.Status == JobStatus.Concluded)
            {
                response.Message = JobResponseMessages.ALREADY_COMPLETED_JOB;
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            job.IsScheduleJob = jobRequest.IsScheduleJob;
            job.Description = jobRequest.Description;
            job.ExecutionTime = jobRequest.ExecutionTime ?? DateTime.UtcNow;

            await _jobRepository.UpdateAsync(job);

            response.JobStatus = JobStatus.Concluded;
            response.StatusCode = HttpStatusCode.OK;

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

            Job job = await _jobRepository.GetByIdAsync(jobId);
            if (job.Description == null)
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

        /// <summary>
        /// Checks if there is duplicity between any existing job and the new job entered.
        /// </summary>
        /// <param name="jobDescription"></param>
        /// <returns></returns>
        public bool ChecksForDuplicityInJobDescription(string jobDescription)
        {
            bool duplicity = _jobRepository.GetAll().Any(x => x.Description == jobDescription);
            return duplicity;
        }
    }
}