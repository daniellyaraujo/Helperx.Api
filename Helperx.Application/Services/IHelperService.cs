using Helperx.Application.Contracts.Common;

namespace Helperx.Application.Services
{
    public interface IHelperService
    {
        Task<JobResponse> SendJobToQueueAsync(JobRequest createJobRequest);
        Task<JobResponse> ProcessJobAsync(JobRequest createJobRequest);
        bool VerifyDuplicityBetwenJobs(JobRequest newJob);
    }
}