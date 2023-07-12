using Helperx.Application.Services;
using Helperz.Application.Contracts;

namespace Helperx.Application.ConsumerServices
{
    public class JobService : IJobService
    {
        private readonly IHelperService _helperService;

        public JobService(IHelperService helperService)
        {
            _helperService = helperService;
        }

        public async Task RunJobAsync(JobRequest baseRequest)
        {
            await _helperService.ProcessJobAsync(baseRequest);
        }
    }
}