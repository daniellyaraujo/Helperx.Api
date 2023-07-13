using Helperz.Domain.Entities;
using Helperz.Domain.Interfaces;

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

        public async Task<Job> GetByIdAsync(int jobId)
        {
            return (Job)_jobContext.Job!.Where(x => x.Id == jobId);
        }

        public List<Job> GetAll()
        {
            return _jobContext.Set<Job>().ToList();
        }

        public async Task CreateAsync(Job job, CancellationToken cancellationToken = default)
        {
            await _jobContext.Job!.AddAsync(job, cancellationToken);
            await _jobContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Job job, CancellationToken cancellationToken = default)
        {
            _jobContext.Job!.Update(job);
            await _jobContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Job job, CancellationToken cancellationToken = default)
        {
            _jobContext.Job!.Remove(job);
            await _jobContext.SaveChangesAsync(cancellationToken);
        }
    }
}