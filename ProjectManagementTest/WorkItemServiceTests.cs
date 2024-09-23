using Microsoft.EntityFrameworkCore;
using ProjectManagementAPI.Application.Services;
using ProjectManagementAPI.Domain.Entities;
using ProjectManagementAPI.Domain.Enums;
using ProjectManagementAPI.Infra.Data;
using Xunit;

namespace ProjectManagementAPI.Tests.Services
{
    public class WorkItemServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly WorkItemService _workItemService;
        private readonly Project _project;

        public WorkItemServiceTests()
        {
            // Usando InMemoryDatabase para simular o banco de dados
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Gera um banco único para cada teste
                .Options;

            _context = new ApplicationDbContext(options);
            _workItemService = new WorkItemService(_context);

            _project = CreateTestProject().Result;
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task CreateWorkItem_WithValidData_ShouldAddWorkItem()
        {
            // Arrange
            var workItem = new WorkItem
            {
                Title = "Nova Tarefa", // Deixe o EF gerar o Id automaticamente
                Description = "Descrição da tarefa",
                Priority = WorkItemPriority.Medium,
                Status = WorkItemStatus.Pending,
                DueDate = DateTime.Now.AddDays(7),
                ProjectId = _project.Id
            };

            // Act
            var createdWorkItem = await _workItemService.CreateWorkItemAsync(workItem);

            // Assert
            Assert.NotEqual(0, createdWorkItem.Id); // O Id foi gerado corretamente
            Assert.Equal(1, _context.WorkItems.Count());
        }

        [Fact]
        public async Task CreateWorkItem_WithInvalidPriority_ShouldThrowException()
        {
            // Arrange
            var workItem = new WorkItem
            {
                Title = "Nova Tarefa",
                Description = "Descrição da tarefa",
                Priority = (WorkItemPriority)999, // Prioridade inválida
                Status = WorkItemStatus.Pending,
                DueDate = DateTime.Now.AddDays(7),
                ProjectId = _project.Id
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _workItemService.CreateWorkItemAsync(workItem));
        }

        [Fact]
        public async Task CreateWorkItem_WithMaxWorkItems_ShouldThrowException()
        {
            // Arrange
            for (int i = 1; i <= 20; i++)
            {
                var workItemLoop = new WorkItem
                {
                    Title = $"Tarefa {i}",
                    Description = $"Descrição da tarefa {i}",
                    Priority = WorkItemPriority.Medium,
                    Status = WorkItemStatus.Pending,
                    DueDate = DateTime.Now.AddDays(7),
                    ProjectId = _project.Id
                };

                await _context.WorkItems.AddAsync(workItemLoop);
                await _context.SaveChangesAsync();
            }

            var workItem = new WorkItem
            {
                Title = "Tarefa extra",
                Description = "Descrição da tarefa extra",
                Priority = WorkItemPriority.Medium,
                Status = WorkItemStatus.Pending,
                DueDate = DateTime.Now.AddDays(7),
                ProjectId = _project.Id
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _workItemService.CreateWorkItemAsync(workItem));
        }

        [Fact]
        public async Task UpdateWorkItem_WithValidData_ShouldUpdateWorkItemAndAddHistory()
        {
            // Arrange
            var workItem = new WorkItem
            {
                Title = "Tarefa Original",
                Description = "Descrição original",
                Priority = WorkItemPriority.Medium,
                Status = WorkItemStatus.Pending,
                DueDate = DateTime.Now.AddDays(7),
                ProjectId = _project.Id
            };

            await _context.WorkItems.AddAsync(workItem);
            await _context.SaveChangesAsync();

            var updatedWorkItem = new WorkItem
            {
                Id = workItem.Id, // Usar o Id gerado pelo EF
                Title = "Tarefa Atualizada",
                Description = "Nova descrição",
                Priority = WorkItemPriority.Medium,
                Status = WorkItemStatus.InProgress,
                DueDate = DateTime.Now.AddDays(10),
                ProjectId = _project.Id
            };

            // Act
            await _workItemService.UpdateWorkItemAsync(updatedWorkItem, "John Doe");

            // Assert
            var history = _context.WorkItemHistories.FirstOrDefault(h => h.WorkItemId == updatedWorkItem.Id);
            Assert.NotNull(history);
            Assert.Contains("Descrição alterada", history.ChangeDescription);
        }

        [Fact]
        public async Task AddComment_WithValidData_ShouldAddCommentAndRecordHistory()
        {
            // Arrange
            var workItem = new WorkItem
            {
                Title = "Tarefa",
                Description = "Descrição da tarefa",
                Priority = WorkItemPriority.Medium,
                Status = WorkItemStatus.Pending,
                DueDate = DateTime.Now.AddDays(7),
                ProjectId = _project.Id,
                Comments = new List<WorkItemComment>()
            };

            await _context.WorkItems.AddAsync(workItem);
            await _context.SaveChangesAsync();

            // Act
            await _workItemService.AddCommentAsync(workItem.Id, "Novo comentário", "John Doe");

            // Assert
            var comment = _context.WorkItemComments.FirstOrDefault(c => c.WorkItemId == workItem.Id);
            Assert.NotNull(comment);
            Assert.Equal("Novo comentário", comment.CommentText);

            var history = _context.WorkItemHistories.FirstOrDefault(h => h.WorkItemId == workItem.Id);
            Assert.NotNull(history);
            Assert.Contains("Comentário adicionado", history.ChangeDescription);
        }

        [Fact]
        public async Task DeleteWorkItem_WithValidData_ShouldRemoveWorkItem()
        {
            // Arrange
            var workItem = new WorkItem
            {
                Title = "Tarefa",
                Description = "Descrição da tarefa",
                Priority = WorkItemPriority.Medium,
                Status = WorkItemStatus.Completed,
                DueDate = DateTime.Now.AddDays(7),
                ProjectId = _project.Id
            };

            await _context.WorkItems.AddAsync(workItem);
            await _context.SaveChangesAsync();

            // Act
            await _workItemService.DeleteWorkItemAsync(workItem.Id);

            // Assert
            Assert.Null(await _context.WorkItems.FindAsync(workItem.Id));
        }

        [Fact]
        public async Task DeleteWorkItem_WithInvalidId_ShouldThrowException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _workItemService.DeleteWorkItemAsync(100)); // Id que não existe
        }

        private async Task<Project> CreateTestProject()
        {
            var project = new Project
            {
                Name = "Novo Projeto",
                Description = "Descrição do projeto"
            };
            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();

            return project;
        }
    }
}
