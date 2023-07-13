using Helperx.Application.Contracts;
using Helperz.Application.Contracts;
using Helperz.Domain.Entities;

namespace Helperx.Application.Services
{
    public interface IHelperService
    {
        Task<JobResponse> RegisterJobAsync(JobRequest jobRequest);
        List<Job> GetAllJobsAsync();
        Task<JobResponse> UpdateJobByIdAsync(int jobId, UpdateJobRequest jobRequest);
        Task<JobResponse> RemoveJobByIdAsync(int jobId);
        bool ChecksForDuplicityInJobDescription(string jobDescription);
        Task ProcessQueueAsync();
    }
}