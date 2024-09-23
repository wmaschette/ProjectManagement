using System;

namespace ProjectManagementAPI.Domain.Entities
{
    public class WorkItemHistory
    {
        public int Id { get; set; }
        public int WorkItemId { get; set; }
        public WorkItem WorkItem { get; set; }
        public string ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; }
        public string ChangeDescription { get; set; }
    }
}
