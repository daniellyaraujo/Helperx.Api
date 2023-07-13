using AutoMapper;
using Helperx.Application.Constants;
using Helperx.Application.Contracts;
using Helperx.Application.Services;
using Helperz.Application.Contracts;
using Helperz.Domain.Entities;
using Helperz.Domain.Enums;
using Helperz.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

public class HelperServiceTests
{
    private readonly Mock<IJobRepository> _jobRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<HelperService>> _loggerMock;
    private readonly HelperService _helperService;

    public HelperServiceTests()
    {
        _jobRepositoryMock = new Mock<IJobRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<HelperService>>();
        _helperService = new HelperService(_jobRepositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task RegisterJobAsync_Should_ReturnBadRequest_When_JobDescriptionIsDuplicate()
    {
        // Arrange
        var jobRequest = new JobRequest { Description = "Sample Job" };
        var existingJob = new Job { Description = "Sample Job" };

        _mapperMock.Setup(mapper => mapper.Map(jobRequest, It.IsAny<Job>()));
        _jobRepositoryMock.Setup(repository => repository.GetByDescriptionAsync(jobRequest.Description)).Returns(true);

        // Act
        var result = await _helperService.RegisterJobAsync(jobRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Equal(JobResponseMessages.DUPLICITY_JOB, result.Message);
    }

    [Fact]
    public async Task RegisterJobAsync_Should_CreateJobAndReturnOk_When_JobDescriptionIsNotDuplicate()
    {
        // Arrange
        var jobRequest = new JobRequest { Description = "Sample Job" };
        var job = new Job { Description = "Sample Job" };

        _mapperMock.Setup(mapper => mapper.Map(jobRequest, It.IsAny<Job>()));
        _jobRepositoryMock.Setup(repository => repository.GetByDescriptionAsync(jobRequest.Description)).Returns(false);
        _jobRepositoryMock.Setup(repository => repository.CreateAsync(job, CancellationToken.None)).Returns(Task.CompletedTask);

        // Act
        var result = await _helperService.RegisterJobAsync(jobRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(JobResponseMessages.SENT_JOB_TO_QUEUE, result.Message);
    }

    [Fact]
    public async Task UpdateJobByIdAsync_Should_ReturnBadRequest_When_JobNotFound()
    {
        // Arrange
        var jobId = 1;
        var jobRequest = new UpdateJobRequest { IsScheduleJob = true, CompletedJob = JobStatus.Concluded, Description = "Updated Job" };

        _jobRepositoryMock.Setup(repository => repository.GetByIdAsync(jobId)).ReturnsAsync((Job)null);

        // Act
        var result = await _helperService.UpdateJobByIdAsync(jobId, jobRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Equal(JobResponseMessages.NOT_FOUND_JOB, result.Message);
    }

    [Fact]
    public async Task UpdateJobByIdAsync_Should_UpdateJobAndReturnOk()
    {
        // Arrange
        var jobId = 1;
        var jobRequest = new UpdateJobRequest { IsScheduleJob = true, CompletedJob = JobStatus.Concluded, Description = "Updated Job" };
        var job = new Job { IsScheduleJob = false, Status = JobStatus.Pending, Description = "Old Job" };

        _jobRepositoryMock.Setup(repository => repository.GetByIdAsync(jobId)).ReturnsAsync(job);
        _jobRepositoryMock.Setup(repository => repository.UpdateAsync(job, CancellationToken.None)).Returns(Task.CompletedTask);

        // Act
        var result = await _helperService.UpdateJobByIdAsync(jobId, jobRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task RemoveJobByIdAsync_Should_ReturnBadRequest_When_JobNotFound()
    {
        // Arrange
        var jobId = 1;

        _jobRepositoryMock.Setup(repository => repository.GetByIdAsync(jobId)).ReturnsAsync((Job)null);

        // Act
        var result = await _helperService.RemoveJobByIdAsync(jobId);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Equal(JobResponseMessages.NOT_FOUND_JOB, result.Message);
    }

    [Fact]
    public async Task RemoveJobByIdAsync_Should_DeleteJobAndReturnOk()
    {
        // Arrange
        var jobId = 1;
        var job = new Job {  };

        _jobRepositoryMock.Setup(repository => repository.GetByIdAsync(jobId)).ReturnsAsync(job);
        _jobRepositoryMock.Setup(repository => repository.DeleteAsync(job, CancellationToken.None)).Returns(Task.CompletedTask);

        // Act
        var result = await _helperService.RemoveJobByIdAsync(jobId);

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(JobResponseMessages.DELETED_JOB, result.Message);
    }

    [Fact]
    public void GetAllJobsAsync_Should_ReturnAllJobs()
    {
        // Arrange
        var jobs = new List<Job> { new Job(), new Job(), new Job() };

        _jobRepositoryMock.Setup(repository => repository.GetAll()).Returns(jobs);

        // Act
        var result = _helperService.GetAllJobsAsync();

        // Assert
        Assert.Equal(jobs, result);
    }

    [Fact]
    public void GetAllJobsAsync_Should_ReturnEmptyList_When_NoJobsFound()
    {
        // Arrange
        var jobs = new List<Job>();

        _jobRepositoryMock.Setup(repository => repository.GetAll()).Returns(jobs);

        // Act
        var result = _helperService.GetAllJobsAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void ChecksForDuplicityInJobDescription_Should_ReturnTrue_When_DuplicateJobFound()
    {
        // Arrange
        var jobDescription = "Sample Job";
        _jobRepositoryMock.Setup(repository => repository.GetByDescriptionAsync(jobDescription)).Returns(true);

        // Act
        var result = _helperService.ChecksForDuplicityInJobDescription(jobDescription);

        // Assert
        Assert.True(result);
        _jobRepositoryMock.Verify(repository => repository.GetByDescriptionAsync(jobDescription), Times.Once);
    }

    [Fact]
    public void ChecksForDuplicityInJobDescription_Should_ReturnFalse_When_NoDuplicateJobFound()
    {
        // Arrange
        var jobDescription = "Sample Job";
        _jobRepositoryMock.Setup(repository => repository.GetByDescriptionAsync(jobDescription)).Returns(false);

        // Act
        var result = _helperService.ChecksForDuplicityInJobDescription(jobDescription);

        // Assert
        Assert.False(result);
        _jobRepositoryMock.Verify(repository => repository.GetByDescriptionAsync(jobDescription), Times.Once);
    }
}