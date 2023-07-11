namespace Helperx.Consumer.Services.Interfaces
{
    public interface IListener
    {
        Task StartAsync(CancellationToken cancellationToken);
    }
}