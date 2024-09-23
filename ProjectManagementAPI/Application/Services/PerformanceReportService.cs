using Microsoft.EntityFrameworkCore;
using ProjectManagementAPI.Application.Interfaces;
using ProjectManagementAPI.Domain.Enums;
using ProjectManagementAPI.Infra.Data;

namespace ProjectManagementAPI.Application.Services
{
    public class PerformanceReportService : IPerformanceReportService
    {
        private readonly ApplicationDbContext _context;

        public PerformanceReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<double> GetAverageCompletedTasksAsync()
        {
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);

            var totalCompletedTasks = await _context.WorkItems
                .Where(w => w.Status == WorkItemStatus.Completed && w.DueDate >= thirtyDaysAgo)
                .CountAsync();

            var totalUsers = await _context.Users.CountAsync();

            if (totalUsers == 0)
                return 0;

            return (double)totalCompletedTasks / totalUsers;
        }
    }
}
