﻿using Helperz.Domain.Entities;

namespace Helperz.Domain.Interfaces
{
    public interface IJobRepository
    {
        List<Job> GetAll();
        Task<Job> GetByIdAsync(int jobId);
        Task CreateAsync(Job job, CancellationToken cancellationToken = default);
        Task UpdateAsync(Job job, CancellationToken cancellationToken = default);
        Task DeleteAsync(Job job, CancellationToken cancellationToken = default);
    }
}