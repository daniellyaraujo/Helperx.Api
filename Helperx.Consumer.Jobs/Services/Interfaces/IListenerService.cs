namespace Helperx.Consumer.Jobs.Services.Interfaces
{
    public interface IListenerService
    {
        Task StartConsumingAsync();
        Task StopConsumingAsync();
    }
}