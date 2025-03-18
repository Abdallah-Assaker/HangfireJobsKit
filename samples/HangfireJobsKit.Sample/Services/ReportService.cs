namespace HangfireJobsKit.Sample.Services;

public class ReportService : IReportService
{
    private readonly ILogger<ReportService> _logger;
    
    public ReportService(ILogger<ReportService> logger)
    {
        _logger = logger;
    }
    
    public Task GenerateReportAsync(DateTime reportDate)
    {
        _logger.LogInformation("Generating report for date {ReportDate}", reportDate);
        // Actual report generation logic would go here
        return Task.CompletedTask;
    }
}