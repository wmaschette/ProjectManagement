using System;

namespace ProjectManagementAPI.Domain.Entities
{
    public class WorkItemComment
    {
        public int Id { get; set; }
        public int WorkItemId { get; set; }
        public WorkItem WorkItem { get; set; }
        public string CommentText { get; set; }
        public string CommentedBy { get; set; }
        public DateTime CommentedAt { get; set; }
    }
}
