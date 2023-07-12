using Helperz.Application.Contracts;
using Helperz.Domain.Entities;

namespace Helperx.Application.Services
{
    public interface IHelperService
    {
        Task<List<Job>> GetAllJobsAsync();
        Task<JobResponse> RegisterNewJobAsync(JobRequest jobRequest);
        Task ProcessJobAsync(JobRequest jobRequest);
        Task<JobResponse> UpdateJobByIdAsync(long jobId, JobRequest jobRequest);
        Task<JobResponse> RemoveJobByIdAsync(long jobId);
        bool ChecksForDuplicityInJobDescription(string jobDescription);
        bool VerifyJobById(long jobId);
    }
}