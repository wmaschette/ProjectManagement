using Microsoft.EntityFrameworkCore;
using ProjectManagementAPI.Domain.Entities;

namespace ProjectManagementAPI.Infra.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<WorkItem> WorkItems { get; set; }
        public DbSet<WorkItemHistory> WorkItemHistories { get; set; }
        public DbSet<WorkItemComment> WorkItemComments { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

}
