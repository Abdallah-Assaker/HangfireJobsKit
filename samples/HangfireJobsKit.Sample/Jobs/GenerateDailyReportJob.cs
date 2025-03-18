using HangfireJobsKit.Abstractions;
using HangfireJobsKit.Configuration;
using HangfireJobsKit.Handlers;
using HangfireJobsKit.Sample.Services;

namespace HangfireJobsKit.Sample.Jobs;

[JobConfiguration("Daily Report Job", 
    retryAttempts: 4, 
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
        var rnd = new Random().Next(1, 10);
        if (rnd % 3 == 0)
            throw new NotImplementedException();
        
        await _reportService.GenerateReportAsync(job.ReportDate);
    }
}