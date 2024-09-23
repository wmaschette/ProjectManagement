using ProjectManagementAPI.Domain.Entities;

namespace ProjectManagementAPI.Application.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetAllProjectsAsync();
        Task<Project> GetProjectByIdAsync(int id);
        Task<Project> CreateProjectAsync(Project project);
        Task UpdateProjectAsync(Project project);
        Task DeleteProjectAsync(int id);
    }
}
