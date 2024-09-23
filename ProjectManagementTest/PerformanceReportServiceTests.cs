using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectManagementAPI.Application.Services;
using ProjectManagementAPI.Domain.Entities;
using ProjectManagementAPI.Domain.Enums;
using ProjectManagementAPI.Infra.Data;
using Xunit;

namespace ProjectManagementAPI.Tests.Services
{
    public class PerformanceReportServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly PerformanceReportService _performanceReportService;

        public PerformanceReportServiceTests()
        {
            // Usando InMemoryDatabase para simular o banco de dados
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "ProjectManagementTest")
                .Options;

            _context = new ApplicationDbContext(options);
            _performanceReportService = new PerformanceReportService(_context);
        }
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetAverageCompletedTasksAsync_WhenNoUsers_ShouldReturnZero()
        {
            // Arrange
            // Act
            var result = await _performanceReportService.GetAverageCompletedTasksAsync();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task GetAverageCompletedTasksAsync_WhenNoCompletedTasks_ShouldReturnZero()
        {
            // Arrange
            // Adicionar usuários sem tarefas concluídas
            var users = new List<User>
            {
                new User { Id = 1, Name = "User 1", Email = "user1@test.com", Role = "Manager" },
                new User { Id = 2, Name = "User 2", Email = "user2@test.com", Role = "Stackholder" }
            };

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            // Act
            var result = await _performanceReportService.GetAverageCompletedTasksAsync();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task GetAverageCompletedTasksAsync_WithCompletedTasks_ShouldReturnCorrectAverage()
        {
            // Arrange
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);

            // Adicionar usuários
            var users = new List<User>
            {
                new User { Id = 1, Name = "User 1", Email = "user1@test.com", Role = "Manager" },
                new User { Id = 2, Name = "User 2", Email = "user2@test.com", Role = "Stackholder" }
            };

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            // Adicionar tarefas concluídas nos últimos 30 dias
            var workItems = new List<WorkItem>
            {
                new WorkItem
                {
                    Id = 1,
                    Title = "Tarefa 1",
                    Status = WorkItemStatus.Completed,
                    DueDate = DateTime.Now.AddDays(-10),
                    ProjectId = 1,
                    Description = "Task completed within 30 days",
                    Priority = WorkItemPriority.Medium
                },
                new WorkItem
                {
                    Id = 2,
                    Title = "Tarefa 2",
                    Status = WorkItemStatus.Completed,
                    DueDate = DateTime.Now.AddDays(-5),
                    ProjectId = 1,
                    Description = "Task completed within 30 days",
                    Priority = WorkItemPriority.Medium
                }
            };

            await _context.WorkItems.AddRangeAsync(workItems);
            await _context.SaveChangesAsync();

            // Act
            var result = await _performanceReportService.GetAverageCompletedTasksAsync();

            // Assert
            Assert.Equal(1.0, result);
        }

        [Fact]
        public async Task GetAverageCompletedTasksAsync_ExcludeTasksOutsideThirtyDays_ShouldReturnCorrectAverage()
        {
            // Arrange
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);

            // Adicionar usuários
            var users = new List<User>
            {
                new User { Id = 1, Name = "User 1", Email = "user1@test.com", Role = "Manager" },
                new User { Id = 2, Name = "User 2", Email = "user2@test.com", Role = "Stackholder" }
            };

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            var workItems = new List<WorkItem>
            {
                new WorkItem
                {
                    Id = 1,
                    Title = "Tarefa 1",
                    Status = WorkItemStatus.Completed,
                    DueDate = DateTime.Now.AddDays(-10),
                    ProjectId = 1,
                    Description = "Task completed within 30 days",
                    Priority = WorkItemPriority.Medium
                },
                new WorkItem
                {
                    Id = 2,
                    Title = "Tarefa 2",
                    Status = WorkItemStatus.Completed,
                    DueDate = DateTime.Now.AddDays(-31),
                    ProjectId = 1,
                    Description = "Task completed outside 30 days",
                    Priority = WorkItemPriority.Medium
                }
            };

            await _context.WorkItems.AddRangeAsync(workItems);
            await _context.SaveChangesAsync();

            // Act
            var result = await _performanceReportService.GetAverageCompletedTasksAsync();

            // Assert
            Assert.Equal(0.5, result); // 1 tarefa válida nos últimos 30 dias / 2 usuários
        }
    }
}
