using HangfireJobsKit.Abstractions;
using HangfireJobsKit.Configuration;
using HangfireJobsKit.Handlers;
using SampleApp.Services;

namespace HangfireJobsKit.Sample.Jobs;

[JobConfiguration("Daily Report Job", 
    retryAttempts: 1, 
    queue: "default")]
public record GenerateDailyReportJob(DateTime ReportDate) : IRecurrenceJob;

public class GenerateDailyReportJobHandler : RecurrenceJobHandlerBase<GenerateDailyReportJob>
{
    private readonly IReportService _reportService;
    
    public GenerateDailyReportJobHandler(
        IReportService reportService,
        ILogger<GenerateDailyReportJobHandler> logger) : base(logger)
    {
        _reportService = reportService;
    }
    
    protected override async Task Handle(GenerateDailyReportJob job)
    {
        await _reportService.GenerateReportAsync(job.ReportDate);
    }
}