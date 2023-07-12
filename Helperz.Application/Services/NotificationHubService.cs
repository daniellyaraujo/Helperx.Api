using Microsoft.AspNetCore.SignalR;

namespace Helperx.Application.Services
{
    public class NotificationHubService : Hub
    {
        public async Task SendToScreenJobUpdatesAsync(string job)
        {
            await Clients.All.SendAsync("JobUpdate", job);
        }
    }
}