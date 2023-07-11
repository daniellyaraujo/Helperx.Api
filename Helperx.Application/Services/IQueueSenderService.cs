using Helperx.Application.Contracts.Common;

namespace Helperx.Application.Services
{
    public interface IQueueSenderService
    {
        Task SendAsync(JobRequest job);
    }
}