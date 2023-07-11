using Helper.Domain.Interfaces.Repository;

namespace Helperx.Infra.Data.Repository
{
    public class JobRepository : IJobRepository
    {
        private readonly JobContext _jobContext;

        public JobRepository(JobContext taskContext)
        {
            _jobContext = taskContext;
        }

        public void Dispose()
        {
            _jobContext.Dispose();
        }

        public List<Helper.Domain.Entities.Job> GetAllJobs()
        {
            return _jobContext.Job!.ToList();
        }

        public async Task CreateAsync(Helper.Domain.Entities.Job job, CancellationToken cancellationToken = default)
        {
            await _jobContext.Job!.AddAsync(job, cancellationToken);
            await _jobContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Helper.Domain.Entities.Job job, CancellationToken cancellationToken = default)
        {
            _jobContext.Job!.Update(job);
            await _jobContext.SaveChangesAsync(cancellationToken);
        }
        
        public async Task DeleteAsync(Helper.Domain.Entities.Job job, CancellationToken cancellationToken = default)
        {
            _jobContext.Job!.Remove(job);
            await _jobContext.SaveChangesAsync(cancellationToken);
        }
    }
}