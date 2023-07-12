using Helperz.Application.Contracts;
using Helperz.Domain.Entities;

namespace Helperx.Application.Services
{
    public interface IHelperService
    {
        Task<List<Job>> GetAllJobsAsync();
        Task<JobResponse> RegisterNewJobAsync(JobRequest jobRequest);
        Task ProcessJobAsync(JobRequest jobRequest);
        Task<JobResponse> UpdateJobByIdAsync(Guid jobId, JobRequest jobRequest);
        Task<JobResponse> RemoveJobByIdAsync(Guid jobId);
        bool ChecksForDuplicityInJobDescription(string jobDescription);
        bool VerifyJobById(Guid jobId);
    }
}