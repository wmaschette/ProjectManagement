using ProjectManagementAPI.Domain.Enums;

namespace ProjectManagementAPI.Domain.Entities
{
    public class WorkItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public WorkItemStatus Status { get; set; }
        public WorkItemPriority Priority { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        // Relacionamento com o histórico
        public List<WorkItemHistory> Histories { get; set; } = new List<WorkItemHistory>();

        // Relacionamento com os comentários
        public List<WorkItemComment> Comments { get; set; } = new List<WorkItemComment>();
    }
}
