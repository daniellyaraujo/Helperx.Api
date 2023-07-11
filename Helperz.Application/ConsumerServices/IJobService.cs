using Helperz.Application.Contracts;

namespace Helperx.Application.ConsumerServices
{
    public interface IJobService
    {
        Task<JobResponse> RunJobAsync(JobRequest baseRequest);
    }
}