namespace ProjectManagementAPI.Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<WorkItem> WorkItems { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }

}
