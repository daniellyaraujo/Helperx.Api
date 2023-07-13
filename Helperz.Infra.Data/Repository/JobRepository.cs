using Helperz.Domain.Entities;
using Helperz.Domain.Enums;
using Helperz.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Helperx.Infra.Data.Repository
{
    public class JobRepository : IJobRepository
    {
        private readonly JobContext _jobContext;

        public JobRepository(JobContext taskContext)
        {
            _jobContext = taskContext;
        }

        public async Task<Job> GetByIdAsync(int jobId)
        {
            return await _jobContext.Job!.FirstOrDefaultAsync(x => x.Id == jobId);
        }

        public bool GetByDescriptionAsync(string description)
        {
            return _jobContext.Job!.Any(x => x.Description == description);
        }

        public async Task UpdatePendingJobsToLateStatusAsync(CancellationToken cancellationToken = default)
        {
            await _jobContext.Job!.
                 Where(x => x.Status == JobStatus.Pending && x.IsScheduleJob && x.ExecutionTime < DateTime.UtcNow)
                .ExecuteUpdateAsync(x => x.SetProperty(c => c.Status, c => JobStatus.Late));

            await _jobContext.SaveChangesAsync(cancellationToken);
        }

        public List<Job> GetAllLateAsync()
        {
            return _jobContext.Job.Where(x => x.Status == JobStatus.Late).OrderBy(x => x.ExecutionTime).ToList();
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