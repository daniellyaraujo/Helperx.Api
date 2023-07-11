using Helperz.Application.Contracts;

namespace Helperx.Application.Services
{
    public interface IQueueSenderService
    {
        Task SendAsync(JobRequest job);
    }
}