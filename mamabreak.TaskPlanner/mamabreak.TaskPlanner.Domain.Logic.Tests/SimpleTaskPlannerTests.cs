using Moq;

using mamabreak.TaskPlanner.Domain.Models;
using mamabreak.TaskPlanner.Domain.Models.enums;
using mamabreak.TaskPlanner.DataAccess.Abstractions;

namespace mamabreak.TaskPlanner.Domain.Logic.Tests
{
    public class SimpleTaskPlannerTests
    {
        [Fact]
        public void CreatePlan_SortsTasksCorrectly()
        {
            // Arrange
            var mockRepository = new Mock<IWorkItemsRepository>();
            var taskPlanner = new SimpleTaskPlanner(mockRepository.Object);

            // Mock data for testing
            var tasks = new[]
            {
                new WorkItem { Id = Guid.NewGuid(), Priority = Priority.High, DueDate = DateTime.Now.AddDays(2), IsCompleted = false },
                new WorkItem { Id = Guid.NewGuid(), Priority = Priority.Low, DueDate = DateTime.Now.AddDays(1), IsCompleted = false },
                new WorkItem { Id = Guid.NewGuid(), Priority = Priority.Medium, DueDate = DateTime.Now.AddDays(3), IsCompleted = false }
            };

            // Setup the mock repository
            mockRepository.Setup(repo => repo.GetAll()).Returns(tasks);

            // Act
            var result = taskPlanner.CreatePlan();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Length);
            Assert.Equal(Priority.Low, result[0].Priority);
            Assert.Equal(Priority.Medium, result[1].Priority);
            Assert.Equal(Priority.High, result[2].Priority);
        }

        [Fact]
        public void CreatePlan_ExcludesCompletedTasks()
        {
            // Arrange
            var mockRepository = new Mock<IWorkItemsRepository>();
            var taskPlanner = new SimpleTaskPlanner(mockRepository.Object);

            // Mock data for testing, including one completed task
            var tasks = new[]
            {
                new WorkItem { Id = Guid.NewGuid(), Priority = Priority.High, DueDate = DateTime.Now.AddDays(2), IsCompleted = false },
                new WorkItem { Id = Guid.NewGuid(), Priority = Priority.Low, DueDate = DateTime.Now.AddDays(1), IsCompleted = true }, // Completed task
                new WorkItem { Id = Guid.NewGuid(), Priority = Priority.Medium, DueDate = DateTime.Now.AddDays(3), IsCompleted = false }
            };

            // Setup the mock repository
            mockRepository.Setup(repo => repo.GetAll()).Returns(tasks);

            // Act
            var result = taskPlanner.CreatePlan();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Length);
            Assert.DoesNotContain(result, task => task.IsCompleted);
        }
    }
}