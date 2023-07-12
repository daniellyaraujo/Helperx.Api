using Helperz.Domain.Entities;

namespace Helperx.Application.Services
{
    public interface IQueueSenderService
    {
        Task SendToQueueAsync(Job job);
    }
}