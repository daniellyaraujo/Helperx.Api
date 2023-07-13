using AutoMapper;
using Helperx.Application.Constants;
using Helperx.Application.Services;
using Helperz.Application.Contracts;
using Helperz.Domain.Entities;
using Helperz.Domain.Enums;
using Helperz.Domain.Interfaces;
using Moq;
using System.Net;

namespace Helperx.Unit.Tests.Services
{
    public class HelperServiceTests
    {
        private readonly Mock<IJobRepository> _jobRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly HelperService _helperService;

        public HelperServiceTests()
        {
            _jobRepositoryMock = new Mock<IJobRepository>();
            _mapperMock = new Mock<IMapper>();
            _helperService = new HelperService(_jobRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task ProcessJobs_DoesNotCallCreateAsync_WhenNoJobsPending()
        {
            // Arrange
            var jobsPending = new List<Job>();

            // Act
            await _helperService.ProcessJobs(jobsPending);

            // Assert
            _jobRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Job>(), CancellationToken.None), Times.Never());
        }

        [Fact]
        public async Task CreateAsync_ReturnsJobResponseWithLateJobMessage_WhenExecutionTimeInTheFuture()
        {
            // Arrange
            var jobRequest = new Job { ExecutionTime = DateTime.UtcNow.AddHours(1) };
            var response = new JobResponse
            {
                Message = JobResponseMessages.LATE_JOB
            };

            // Act
            var result = await _helperService.CreateAsync(jobRequest);

            // Assert
            Assert.Equal(response.Message, result.Message);
        }

        [Fact]
        public async Task CreateAsync_ReturnsJobResponseWithConcludedStatusAndStatusCodeOK_WhenExecutionTimeIsNull()
        {
            // Arrange
            var jobRequest = new Job { ExecutionTime = DateTime.Now };
            var response = new JobResponse
            {
                JobStatus = JobStatus.Concluded,
                StatusCode = HttpStatusCode.OK
            };

            _jobRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Job>(), CancellationToken.None)).Returns(Task.CompletedTask);

            // Act
            var result = await _helperService.CreateAsync(jobRequest);

            // Assert
            Assert.Equal(response.JobStatus, result.JobStatus);
            Assert.Equal(response.StatusCode, result.StatusCode);
        }

        [Fact]
        public async Task RegisterJobAsync_ReturnsJobResponseWithDuplicityMessageAndStatusCodeBadRequest_WhenDuplicityAndScheduleJob()
        {
            // Arrange
            var jobRequest = new JobRequest { Description = "Descrição do Job", IsScheduleJob = true };
            var job = new Job { Description = "Descrição do Job" };
            var response = new JobResponse
            {
                Message = JobResponseMessages.DUPLICITY_JOB,
                StatusCode = HttpStatusCode.BadRequest
            };

            _jobRepositoryMock.Setup(repo => repo.GetAll()).Returns(new List<Job> { job });

            // Act
            var result = await _helperService.RegisterJobAsync(jobRequest);

            // Assert
            Assert.Equal(response.Message, result.Message);
            Assert.Equal(response.StatusCode, result.StatusCode);
            _jobRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Job>(), CancellationToken.None), Times.Never());
        }

        [Fact]
        public async Task RegisterJobAsync_ReturnsJobResponseWithDuplicityMessageAndStatusCodeBadRequest()
        {
            // Arrange
            var jobRequest = new JobRequest { Description = "Descrição do Job" };
            var job = new Job { Description = "Descrição do Job" };
            var response = new JobResponse
            {
                Message = JobResponseMessages.DUPLICITY_JOB,
                StatusCode = HttpStatusCode.BadRequest
            };

            _jobRepositoryMock.Setup(repo => repo.GetAll()).Returns(new List<Job> { job });

            // Act
            var result = await _helperService.RegisterJobAsync(jobRequest);

            // Assert
            Assert.Equal(response.Message, result.Message);
            Assert.Equal(response.StatusCode, result.StatusCode);
        }

        [Fact]
        public void GetAllJobsAsync_ReturnsEmptyListWhenNoJobs()
        {
            // Arrange
            var jobs = new List<Job>();

            _jobRepositoryMock.Setup(repo => repo.GetAll()).Returns(jobs);

            // Act
            var result = _helperService.GetAllJobsAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetAllJobsAsync_ReturnsListWithAllJobs()
        {
            // Arrange
            var jobs = new List<Job>
        {
            new Job { Description = "Teste", ExecutionTime = DateTime.UtcNow, IsScheduleJob = true, Status = JobStatus.Pending },
            new Job { Description = "Teste2", ExecutionTime = DateTime.UtcNow, IsScheduleJob = true, Status = JobStatus.Concluded },
            new Job { Description = "Teste3", ExecutionTime = DateTime.UtcNow, IsScheduleJob = true, Status = JobStatus.Pending }
        };

            _jobRepositoryMock.Setup(repo => repo.GetAll()).Returns(jobs);

            // Act
            var result = _helperService.GetAllJobsAsync();

            // Assert
            Assert.Equal(jobs, result);
        }

        [Fact]
        public void GetAllJobsAsync_CallsGetAllOnRepository()
        {
            // Arrange
            var jobs = new List<Job>();

            _jobRepositoryMock.Setup(repo => repo.GetAll()).Returns(jobs);

            // Act
            var result = _helperService.GetAllJobsAsync();

            // Assert
            _jobRepositoryMock.Verify(repo => repo.GetAll(), Times.Once());
        }

        [Fact]
        public void GetAllPendingJobsAsync_CallsGetAllOnRepository()
        {
            // Arrange
            var jobs = new List<Job>();

            _jobRepositoryMock.Setup(repo => repo.GetAll()).Returns(jobs);

            // Act
            var result = _helperService.GetAllPendingJobsAsync();

            // Assert
            _jobRepositoryMock.Verify(repo => repo.GetAll(), Times.Once());
        }

        [Fact]
        public void GetAllPendingJobsAsync_ReturnsEmptyListWhenNoPendingJobs()
        {
            // Arrange
            var jobs = new List<Job>
            {
            new Job { Status = JobStatus.Concluded },
            new Job { Status = JobStatus.Concluded }
            };

            _jobRepositoryMock.Setup(repo => repo.GetAll()).Returns(jobs);

            // Act
            var result = _helperService.GetAllPendingJobsAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task UpdateJobByIdAsync_ReturnsJobResponseWithAlreadyCompletedMessageAndStatusCode400()
        {
            // Arrange
            int jobId = 1;
            var jobRequest = new JobRequest { IsScheduleJob = true, Description = "Teste", ExecutionTime = DateTime.UtcNow };
            var job = new Job { Description = "Descrição do Job", Status = JobStatus.Concluded };

            _jobRepositoryMock.Setup(repo => repo.GetByIdAsync(jobId)).ReturnsAsync(job);

            // Act
            var response = await _helperService.UpdateJobByIdAsync(jobId, jobRequest);

            // Assert
            Assert.Equal(JobResponseMessages.ALREADY_COMPLETED_JOB, response.Message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateJobByIdAsync_ReturnsJobResponseWithConcludedStatusAndStatusCode200()
        {
            // Arrange
            int jobId = 1;
            var jobRequest = new JobRequest { IsScheduleJob = true, Description = "Teste", ExecutionTime = DateTime.UtcNow };
            var job = new Job { Description = "Descrição do Job", Status = JobStatus.Pending };

            _jobRepositoryMock.Setup(repo => repo.GetByIdAsync(jobId)).ReturnsAsync(job);
            _jobRepositoryMock.Setup(repo => repo.UpdateAsync(job, CancellationToken.None)).Returns(Task.CompletedTask);

            // Act
            var response = await _helperService.UpdateJobByIdAsync(jobId, jobRequest);

            // Assert
            Assert.Equal(JobStatus.Concluded, response.JobStatus);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateJobByIdAsync_ReturnsJobResponseWithNotFoundMessageAndStatusCode400()
        {
            // Arrange
            int jobId = 1;
            var jobRequest = new JobRequest {IsScheduleJob = true, Description = "Teste", ExecutionTime = DateTime.UtcNow };
            var job = new Job(); 

            _jobRepositoryMock.Setup(repo => repo.GetByIdAsync(jobId)).ReturnsAsync(job);

            // Act
            var response = await _helperService.UpdateJobByIdAsync(jobId, jobRequest);

            // Assert
            Assert.Equal(JobResponseMessages.NOT_FOUND_JOB, response.Message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RemoveJobByIdAsync_CallsDeleteAsyncOnRepository()
        {
            // Arrange
            int jobId = 1;
            var job = new Job { Description = "Descrição do Job" };

            _jobRepositoryMock.Setup(repo => repo.GetByIdAsync(jobId)).ReturnsAsync(job);

            // Act
            var response = await _helperService.RemoveJobByIdAsync(jobId);

            // Assert
            _jobRepositoryMock.Verify(repo => repo.DeleteAsync(job, CancellationToken.None), Times.Once());
        }

        [Fact]
        public async Task RemoveJobByIdAsync_ReturnsJobResponseWithNotFoundMessageAndStatusCode400()
        {
            // Arrange
            int jobId = 1;
            var job = new Job();

            _jobRepositoryMock.Setup(repo => repo.GetByIdAsync(jobId)).ReturnsAsync(job);

            // Act
            var response = await _helperService.RemoveJobByIdAsync(jobId);

            // Assert
            Assert.Equal(JobResponseMessages.NOT_FOUND_JOB, response.Message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RemoveJobByIdAsync_ReturnsJobResponseWithDeletedMessageAndStatusCode200()
        {
            // Arrange
            int jobId = 1;
            var job = new Job { Description = "Descrição do Job" };

            _jobRepositoryMock.Setup(repo => repo.GetByIdAsync(jobId)).ReturnsAsync(job);
            _jobRepositoryMock.Setup(repo => repo.DeleteAsync(job, CancellationToken.None)).Returns(Task.CompletedTask);

            // Act
            var response = await _helperService.RemoveJobByIdAsync(jobId);

            // Assert
            Assert.Equal(JobResponseMessages.DELETED_JOB, response.Message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void ChecksForDuplicityInJobDescription_ReturnsTrueWhenDuplicateExists()
        {
            // Arrange
            string jobDescription = "Descrição do Job";
            var jobs = new List<Job>
            {
            new Job { Description = "Descrição do Job" },
            new Job { Description = "Outra Descrição" }
            };

            _jobRepositoryMock.Setup(repo => repo.GetAll()).Returns(jobs);

            // Act
            bool result = _helperService.ChecksForDuplicityInJobDescription(jobDescription);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ChecksForDuplicityInJobDescription_ReturnsFalseWhenNoDuplicateExists()
        {
            // Arrange
            string jobDescription = "Descrição do Job";
            var jobs = new List<Job>
        {
            new Job { Description = "Outra Descrição" },
            new Job { Description = "Mais uma Descrição" }
        };

            _jobRepositoryMock.Setup(repo => repo.GetAll()).Returns(jobs);

            // Act
            bool result = _helperService.ChecksForDuplicityInJobDescription(jobDescription);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ChecksForDuplicityInJobDescription_ReturnsFalseWhenRepositoryIsEmpty()
        {
            // Arrange
            string jobDescription = "Descrição do Job";
            var jobs = new List<Job>();

            _jobRepositoryMock.Setup(repo => repo.GetAll()).Returns(jobs);

            // Act
            bool result = _helperService.ChecksForDuplicityInJobDescription(jobDescription);

            // Assert
            Assert.False(result);
        }
    }
}