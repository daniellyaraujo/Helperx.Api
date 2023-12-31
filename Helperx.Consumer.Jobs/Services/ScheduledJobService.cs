﻿using Helperx.Application.Services;
using Helperz.Domain.Interfaces;

namespace Helperx.Application.ConsumerServices
{
    /// <summary>
    /// Service class for scheduled jobs that need to run.
    /// </summary>
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
            _timer = new Timer(ExecuteAsync, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
            return Task.CompletedTask;
        }

        public async void ExecuteAsync(object state)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var helperService = scope.ServiceProvider.GetRequiredService<IHelperService>();

                await helperService.ProcessQueueAsync();
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