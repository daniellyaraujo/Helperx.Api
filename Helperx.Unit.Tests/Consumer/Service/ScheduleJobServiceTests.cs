using FluentAssertions.Common;
using Helperx.Application.ConsumerServices;
using Helperx.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Helperx.Unit.Tests.Consumer.Service
{
    public class ScheduleJobServiceTests
    {
        private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
        private readonly Mock<IHelperService> _helperServiceMock;
        private readonly ScheduledJobService _scheduledJobService;

        public ScheduleJobServiceTests()
        {
            _helperServiceMock = new Mock<IHelperService>();
            _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            _scheduledJobService = new ScheduledJobService(_serviceScopeFactoryMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_Should_CallProcessQueueAsync()
        {
            // Arrange
            var serviceProvider = new ServiceCollection().AddSingleton(_helperServiceMock.Object).BuildServiceProvider();
            var serviceScope = Mock.Of<IServiceScope>(scope => scope.ServiceProvider == serviceProvider);
            _serviceScopeFactoryMock.Setup(factory => factory.CreateScope()).Returns(serviceScope);

            // Act
            _scheduledJobService.ExecuteAsync(null);

            // Assert
            _helperServiceMock.Verify(helper => helper.ProcessQueueAsync(), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_Should_DisposeServiceScope()
        {
            // Arrange
            var serviceScopeMock = new Mock<IServiceScope>();
            _serviceScopeFactoryMock.Setup(factory => factory.CreateScope()).Returns(serviceScopeMock.Object);

            // Act
            _scheduledJobService.ExecuteAsync(null);

            // Assert
            serviceScopeMock.Verify(scope => scope.Dispose(), Times.Once);
        }

        [Fact]
        public async Task StartAsync_Should_ReturnCompletedTask()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;

            // Act
            var result = _scheduledJobService.StartAsync(cancellationToken);

            // Assert
            Assert.Equal(Task.CompletedTask, result);
        }

        [Fact]
        public async Task StopAsync_DoesNotChangeTimer_WhenTimerIsNull()
        {
            // Arrange
            var cancellationToken = new CancellationToken();
            var scheduleJobService = new ScheduledJobService(null);

            // Act
            await scheduleJobService.StopAsync(cancellationToken);

            // Assert
            // No need to verify anything since the timer is null
        }
    }
}
