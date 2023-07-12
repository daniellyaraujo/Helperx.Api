using Helperx.Application.Services;
using Helperz.Application.Contracts;

namespace Helperx.Application.ConsumerServices
{
    public class JobService : IJobService
    {
        private readonly IHelperService _helperService;
        private readonly NotificationHubService _hubContext;

        public JobService(IHelperService helperService, NotificationHubService hubContext)
        {
            _helperService = helperService;
            _hubContext = hubContext;
        }

        public async Task RunJobAsync(JobRequest baseRequest)
        {
            await _helperService.ProcessJobAsync(baseRequest);
        }
    }
}