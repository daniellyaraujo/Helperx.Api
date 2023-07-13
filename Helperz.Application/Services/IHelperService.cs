using Helperz.Application.Contracts;
using Helperz.Domain.Entities;

namespace Helperx.Application.Services
{
    public interface IHelperService
    {
        Task ExecuteJobsPendingAsync();
        Task ProcessJobs(List<Job> jobsPending);
        Task<JobResponse> CreateAsync(Job jobRequest);
        Task<JobResponse> RegisterJobAsync(JobRequest jobRequest);
        List<Job> GetAllJobsAsync();
        List<Job> GetAllPendingJobsAsync();
        Task<JobResponse> UpdateJobByIdAsync(int jobId, JobRequest jobRequest);
        Task<JobResponse> RemoveJobByIdAsync(int jobId);
        bool ChecksForDuplicityInJobDescription(string jobDescription);
    }
}