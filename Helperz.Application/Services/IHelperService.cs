using Helperz.Application.Contracts;
using Helperz.Domain.Entities;

namespace Helperx.Application.Services
{
    public interface IHelperService
    {
        Task<List<Job>> GetAllJobsAsync();
        Task<JobResponse> RegisterNewJobAsync(JobRequest jobRequest);
        Task ProcessJobAsync(object jobRequest);
        Task<JobResponse> UpdateJobByIdAsync(int jobId, JobRequest jobRequest);
        Task<JobResponse> RemoveJobByIdAsync(int jobId);
        bool ChecksForDuplicityInJobDescription(string jobDescription);
        bool VerifyJobById(int jobId);
    }
}