namespace Helperz.Domain.Interfaces.Repository
{
    public interface IJobRepository
    {
        List<Entities.Job> GetAllJobs();
        List<Entities.Job> GetJobById(long jobId);
        Task CreateAsync(Entities.Job job, CancellationToken cancellationToken = default);
        Task UpdateAsync(Entities.Job job, CancellationToken cancellationToken = default);
        Task DeleteAsync(Entities.Job job, CancellationToken cancellationToken = default);
    }
}
