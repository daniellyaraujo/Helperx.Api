﻿using Helperz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Helperx.Infra.Data
{
    [ExcludeFromCodeCoverage]
    public class JobContext : DbContext
    {
        public DbSet<Job>? Job { get; set; }

        public JobContext(DbContextOptions<JobContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(JobContext).Assembly);
        }
    }
}