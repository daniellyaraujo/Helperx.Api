using Helperx.Api.Controllers;
using Helperx.Application.Services;
using Helperz.Application.Contracts;
using Helperz.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Helperx.Unit.Tests.Controllers
{
    public class HelperControllerTests
    {
        private readonly Mock<IHelperService> _helperServiceMock;
        private readonly HelperController _controller;

        public HelperControllerTests()
        {
            _helperServiceMock = new Mock<IHelperService>();
            _controller = new HelperController(_helperServiceMock.Object);
        }

        [Fact]
        public async Task GetAsync_ReturnsListOfJobs()
        {
            // Arrange
            var expectedJobs = new List<Job> {  };
            _helperServiceMock.Setup(service => service.GetAllJobsAsync()).Returns(await Task.FromResult(expectedJobs));

            // Act
            var result = await _controller.GetAsync();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<Job>>(result);
            Assert.Equal(expectedJobs, result);
        }

        [Fact]
        public async Task GetAsync_ReturnsEmptyList_WhenNoJobs()
        {
            // Arrange
            var expectedJobs = new List<Job>();
            _helperServiceMock.Setup(service => service.GetAllJobsAsync()).Returns(await Task.FromResult(expectedJobs));

            // Act
            var result = await _controller.GetAsync();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<Job>>(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task PostAsync_ReturnsJobResponse_WhenJobRegistrationIsSuccessful()
        {
            // Arrange
            var jobRequest = new JobRequest {IsScheduleJob = true, Description = "Teste", ExecutionTime = DateTime.UtcNow };
            var expectedResponse = new JobResponse {  };
            _helperServiceMock.Setup(service => service.RegisterJobAsync(jobRequest)).Returns(Task.FromResult(expectedResponse));

            // Act
            var result = await _controller.PostAsync(jobRequest);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<JobResponse>(result);
            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task PutAsync_ReturnsJobResponse_WhenJobUpdateIsSuccessful()
        {
            // Arrange
            var id = 1;
            var jobRequest = new JobRequest {IsScheduleJob = true, Description = "Teste", ExecutionTime = DateTime.UtcNow };
            var expectedResponse = new JobResponse {  };
            _helperServiceMock.Setup(service => service.UpdateJobByIdAsync(id, jobRequest)).Returns(Task.FromResult(expectedResponse));

            // Act
            var result = await _controller.PutAsync(id, jobRequest);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<JobResponse>(result);
            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsJobResponse_WhenJobRemovalIsSuccessful()
        {
            // Arrange
            var id = 1;
            var expectedResponse = new JobResponse {  };
            _helperServiceMock.Setup(service => service.RemoveJobByIdAsync(id)).Returns(Task.FromResult(expectedResponse));

            // Act
            var result = await _controller.DeleteAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<JobResponse>(result);
            Assert.Equal(expectedResponse, result);
        }
    }
}