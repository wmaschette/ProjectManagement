using Microsoft.EntityFrameworkCore;
using ProjectManagementAPI.Application.Interfaces;
using ProjectManagementAPI.Domain.Entities;
using ProjectManagementAPI.Domain.Enums;
using ProjectManagementAPI.Infra.Data;

namespace ProjectManagementAPI.Application.Services
{
    public class WorkItemService : IWorkItemService
    {
        private readonly ApplicationDbContext _context;

        public WorkItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkItem>> GetWorkItemsByProjectIdAsync(int projectId)
        {
            return await _context.WorkItems.Where(w => w.ProjectId == projectId).ToListAsync();
        }

        public async Task<WorkItem> CreateWorkItemAsync(WorkItem workItem)
        {
            var project = await _context.Projects.Include(p => p.WorkItems)
                                                 .FirstOrDefaultAsync(p => p.Id == workItem.ProjectId);

            if (!Enum.IsDefined(typeof(WorkItemPriority), workItem.Priority))
                throw new InvalidOperationException("Prioridade informada é inválida!");


            if (project.WorkItems.Count >= 20)
                throw new InvalidOperationException("O projeto já atingiu o limite máximo de 20 tarefas.");

            _context.WorkItems.Add(workItem);
            await _context.SaveChangesAsync();
            return workItem;
        }

        public async Task UpdateWorkItemAsync(WorkItem workItem, string updatedBy)
        {
            var existingWorkItem = await _context.WorkItems.FindAsync(workItem.Id);

            if (existingWorkItem == null)
                throw new InvalidOperationException("Tarefa não encontrada.");

            if (existingWorkItem.Priority != workItem.Priority)
                throw new InvalidOperationException("Não é permitido alterar a prioridade.");

            var historyEntry = new WorkItemHistory
            {
                WorkItemId = workItem.Id,
                ChangedBy = updatedBy,
                ChangedAt = DateTime.Now,
                ChangeDescription = GetChangeDescription(existingWorkItem, workItem)
            };

            existingWorkItem.Title = workItem.Title;
            existingWorkItem.Description = workItem.Description;
            existingWorkItem.Status = workItem.Status;
            existingWorkItem.DueDate = workItem.DueDate;

            _context.WorkItemHistories.Add(historyEntry);
            await _context.SaveChangesAsync();
        }


        public async Task AddCommentAsync(int workItemId, string commentText, string commentedBy)
        {
            var workItem = await _context.WorkItems.FindAsync(workItemId);
            if (workItem == null)
                throw new InvalidOperationException("Tarefa não encontrada.");

            var comment = new WorkItemComment
            {
                WorkItemId = workItemId,
                CommentText = commentText,
                CommentedBy = commentedBy,
                CommentedAt = DateTime.Now
            };

            _context.WorkItemComments.Add(comment);

            var historyEntry = new WorkItemHistory
            {
                WorkItemId = workItemId,
                ChangedBy = commentedBy,
                ChangedAt = DateTime.Now,
                ChangeDescription = $"Comentário adicionado: {commentText}"
            };

            _context.WorkItemHistories.Add(historyEntry);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteWorkItemAsync(int id)
        {
            var workItem = await _context.WorkItems.FindAsync(id);

            if (workItem == null)
            {
                throw new InvalidOperationException("Tarefa não encontrada.");
            }

            _context.WorkItems.Remove(workItem);
            await _context.SaveChangesAsync();
        }

        private string GetChangeDescription(WorkItem oldItem, WorkItem newItem)
        {
            var changes = new List<string>();

            if (oldItem.Status != newItem.Status)
            {
                changes.Add($"Status alterado de {oldItem.Status} para {newItem.Status}");
            }

            if (oldItem.Description != newItem.Description)
            {
                changes.Add("Descrição alterada.");
            }

            if (oldItem.DueDate != newItem.DueDate)
            {
                changes.Add($"Data de vencimento alterada de {oldItem.DueDate.ToShortDateString()} para {newItem.DueDate.ToShortDateString()}");
            }

            return string.Join(", ", changes);
        }
    }
}
