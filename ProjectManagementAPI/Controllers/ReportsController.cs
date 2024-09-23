using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementAPI.Application.Interfaces;

namespace ProjectManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IPerformanceReportService _performanceReportService;

        public ReportsController(IPerformanceReportService performanceReportService)
        {
            _performanceReportService = performanceReportService;
        }

        [HttpGet("Performance")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetPerformanceReport()
        {
            var averageCompletedTasks = await _performanceReportService.GetAverageCompletedTasksAsync();
            return Ok(new { AverageCompletedTasksPerUser = averageCompletedTasks });
        }
    }
}
