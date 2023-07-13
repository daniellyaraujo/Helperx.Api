using Helperx.Application.Contracts;
using Helperz.Application.Contracts;
using Helperz.Domain.Entities;

namespace Helperx.Application.Services
{
    public interface IHelperService
    {
        Task ExecuteJobsPendingAsync();
        Task ProcessJobs(List<Job> jobsPending);
        Task UpdatesJobStatusByQueueAsync(Job jobRequest);
        Task<JobResponse> RegisterJobAsync(JobRequest jobRequest);
        List<Job> GetAllJobsAsync();
        List<Job> GetAllPendingJobsAsync();
        Task<JobResponse> UpdateJobByIdAsync(int jobId, UpdateJobRequest jobRequest);
        Task<JobResponse> RemoveJobByIdAsync(int jobId);
        bool ChecksForDuplicityInJobDescription(string jobDescription);
    }
}