using Helperz.Application.Contracts;
using Helperz.Domain.Entities;

namespace Helperx.Application.Services
{
    public interface IHelperService
    {
        Task<JobResponse> RegisterNewJobAsync(JobRequest createJobRequest);
        Task<JobResponse> ProcessJobAsync(JobRequest createJobRequest);
        Task<List<Job>> GetJobByIdAsync(long jobId);
        bool ChecksForDuplicityInJobDescription(string jobDescription);
    }
}