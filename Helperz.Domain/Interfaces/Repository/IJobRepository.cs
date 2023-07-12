using Helperz.Domain.Entities;

namespace Helperz.Domain.Interfaces.Repository
{
    public interface IJobRepository
    {
        List<Job> GetJobs();
        Task<Job> GetJobByIdAsync(long jobId);
        Task CreateAsync(Job job, CancellationToken cancellationToken = default);
        Task UpdateAsync(Job job, CancellationToken cancellationToken = default);
        Task DeleteAsync(Job job, CancellationToken cancellationToken = default);
    }
}