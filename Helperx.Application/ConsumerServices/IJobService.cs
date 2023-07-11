using Azure.Messaging.ServiceBus;
using Helperx.Application.Contracts.Common;

namespace Helperx.Application.ConsumerServices
{
    public interface IJobService
    {
        Task<JobResponse> RunJobAsync(JobRequest baseRequest);
    }
}