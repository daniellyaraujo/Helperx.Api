using Helperx.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace Helperx.Application.ConsumerServices
{
    public class ScheduledJobService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ScheduledJobService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(ExecuteJob, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
            return Task.CompletedTask;
        }

        private async void ExecuteJob(object state)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var helperService = scope.ServiceProvider.GetRequiredService<IHelperService>();

                await helperService.ProcessJobAsync(state);
            }
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