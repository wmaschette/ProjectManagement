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
    public class ProjectServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ProjectService _projectService;

        public ProjectServiceTests()
        {
            // Usando InMemoryDatabase para simular o banco de dados
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "ProjectManagementTest")
                .Options;

            _context = new ApplicationDbContext(options);
            _projectService = new ProjectService(_context);
        }
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task CreateProject_WithValidData_ShouldAddProject()
        {
            // Arrange
            var project = new Project
            {
                Name = "Novo Projeto",
                Description = "Descrição do projeto"
            };

            // Act
            var createdProject = await _projectService.CreateProjectAsync(project);

            // Assert
            Assert.NotEqual(0, createdProject.Id);
            Assert.True(_context.Projects.Count() > 0);
        }

        [Fact]
        public async Task GetProjectById_WithValidId_ShouldReturnProject()
        {
            // Arrange
            var project = new Project
            {
                Name = "Projeto Existente",
                Description = "Descrição do projeto"
            };

            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();

            // Act
            var result = await _projectService.GetProjectByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(project.Id, result.Id);
        }

        [Fact]
        public async Task GetProjectById_WithInvalidId_ShouldReturnNull()
        {
            // Act
            var result = await _projectService.GetProjectByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllProjects_ShouldReturnAllProjects()
        {
            // Arrange
            var project1 = new Project { Name = "Projeto 1", Description = "Descrição 1" };
            var project2 = new Project { Name = "Projeto 2", Description = "Descrição 2" };

            await _context.Projects.AddRangeAsync(project1, project2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _projectService.GetAllProjectsAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task UpdateProject_WithValidData_ShouldUpdateProject()
        {
            // Arrange
            var project = new Project { Name = "Projeto Original", Description = "Descrição original" };
            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();

            var updatedProject = new Project { Id = project.Id, Name = "Projeto Atualizado", Description = "Nova descrição" };

            // Act
            await _projectService.UpdateProjectAsync(updatedProject);

            // Assert
            var result = await _context.Projects.FindAsync(project.Id);
            Assert.Equal("Projeto Atualizado", result.Name);
            Assert.Equal("Nova descrição", result.Description);
        }

        [Fact]
        public async Task DeleteProject_WithCompletedWorkItems_ShouldDeleteProject()
        {
            // Arrange
            var project = new Project
            {
                Name = "Projeto",
                Description = "Projeto Teste", 
                WorkItems = new List<WorkItem>
                {
                    new WorkItem { Title = "Tarefa Completa", Status = WorkItemStatus.Completed, Description = "Descrição", DueDate = DateTime.Now.AddDays(5), Priority = WorkItemPriority.Medium }
                }
            };

            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();

            // Act
            await _projectService.DeleteProjectAsync(project.Id);

            // Assert
            Assert.Null(await _context.Projects.FindAsync(project.Id));
        }

        [Fact]
        public async Task DeleteProject_WithPendingWorkItems_ShouldThrowException()
        {
            // Arrange
            var project = new Project
            {
                Name = "Projeto",
                Description = "Projeto Teste",
                WorkItems = new List<WorkItem>
                {
                    new WorkItem { Title = "Tarefa Pendente", Status = WorkItemStatus.Pending, Description = "Descrição", DueDate = DateTime.Now.AddDays(5), Priority = WorkItemPriority.Medium }
                }
            };

            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _projectService.DeleteProjectAsync(project.Id));
        }

        [Fact]
        public async Task DeleteProject_WithInvalidId_ShouldThrowException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _projectService.DeleteProjectAsync(100));
        }
    }
}
