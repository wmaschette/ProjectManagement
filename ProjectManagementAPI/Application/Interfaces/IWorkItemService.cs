using Microsoft.EntityFrameworkCore;
using ProjectManagementAPI.Domain.Entities;

namespace ProjectManagementAPI.Application.Interfaces
{
    public interface IWorkItemService
    {
        Task<IEnumerable<WorkItem>> GetWorkItemsByProjectIdAsync(int projectId);
        Task<WorkItem> CreateWorkItemAsync(WorkItem workItem);
        Task UpdateWorkItemAsync(WorkItem workItem, string updateBy);
        Task DeleteWorkItemAsync(int id);
        Task AddCommentAsync(int workItemId, string commentText, string commentedBy);
    }
}
