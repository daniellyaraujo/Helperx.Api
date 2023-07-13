using Helperx.Infra.Data;
using Helperx.Infra.Data.Repository;
using Helperz.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Helperx.Unit.Tests.Repository
{
    public class JobRepositoryTests
    {
        private readonly DbContextOptions<JobContext> _dbContextOptions;

        public JobRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<JobContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;
        }

        [Fact]
        public void GetAll_ReturnsNotNullList()
        {
            // Arrange
            using var dbContext = new JobContext(_dbContextOptions);
            var jobRepository = new JobRepository(dbContext);

            // Act
            var result = jobRepository.GetAll();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task CreateAsync_AddsJobToDatabase()
        {
            // Arrange
            using var dbContext = new JobContext(_dbContextOptions);
            var job = new Job { Description = "Test Job" };
            var jobRepository = new JobRepository(dbContext);

            // Act
            await jobRepository.CreateAsync(job);
            var createdJob = await dbContext.Job.FindAsync(job.Id);

            // Assert
            Assert.Equal(job, createdJob);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesJobInDatabase()
        {
            // Arrange
            using var dbContext = new JobContext(_dbContextOptions);
            var job = new Job { Description = "Test Job" };
            var jobRepository = new JobRepository(dbContext);
            await dbContext.Job.AddAsync(job);
            await dbContext.SaveChangesAsync();

            // Act
            job.Description = "Updated Job";
            await jobRepository.UpdateAsync(job);
            var updatedJob = await dbContext.Job.FindAsync(job.Id);

            // Assert
            Assert.Equal(job.Description, updatedJob.Description);
        }

        [Fact]
        public async Task DeleteAsync_RemovesJobFromDatabase()
        {
            // Arrange
            using var dbContext = new JobContext(_dbContextOptions);
            var job = new Job { Description = "Test Job" };
            var jobRepository = new JobRepository(dbContext);
            await dbContext.Job.AddAsync(job);
            await dbContext.SaveChangesAsync();

            // Act
            await jobRepository.DeleteAsync(job);
            var deletedJob = await dbContext.Job.FindAsync(job.Id);

            // Assert
            Assert.Null(deletedJob);
        }
    }
}