namespace ProjectManagementAPI.Application.Interfaces
{
    public interface IPerformanceReportService
    {
        Task<double> GetAverageCompletedTasksAsync();
    }
}
