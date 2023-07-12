using Helperx.Application.Services;

namespace Helperx.Application.ConsumerServices
{
    public class ScheduledJobService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IHelperService _helperService;

        public ScheduledJobService(IHelperService helperService)
        {
            _helperService = helperService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(ExecuteJob, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
            return Task.CompletedTask;
        }

        private void ExecuteJob(object state)
        {
            _helperService.ProcessJobAsync(state);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}