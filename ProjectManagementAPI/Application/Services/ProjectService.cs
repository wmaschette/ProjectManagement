using ProjectManagementAPI.Application.Interfaces;
using ProjectManagementAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ProjectManagementAPI.Domain.Enums;
using ProjectManagementAPI.Infra.Data;

namespace ProjectManagementAPI.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;

        public ProjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            return await _context.Projects.Include(p => p.WorkItems).ToListAsync();
        }

        public async Task<Project> GetProjectByIdAsync(int id) 
            => await _context.Projects.Include(p => p.WorkItems).FirstOrDefaultAsync(p => p.Id == id);

        public async Task<Project> CreateProjectAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public async Task UpdateProjectAsync(Project updatedProject)
        {
            var existingProject = await _context.Projects.FindAsync(updatedProject.Id);

            if (existingProject == null)
            {
                throw new InvalidOperationException("Projeto não encontrado.");
            }

            existingProject.Name = updatedProject.Name;
            existingProject.Description = updatedProject.Description;

            await _context.SaveChangesAsync();
        }


        public async Task DeleteProjectAsync(int id)
        {
            var project = await _context.Projects.Include(p => p.WorkItems).FirstOrDefaultAsync(p => p.Id == id);

            if (project == null || project.WorkItems.Any(w => w.Status != WorkItemStatus.Completed))
                throw new InvalidOperationException("O projeto contém tarefas pendentes e não pode ser excluído.");

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }
    }
}
