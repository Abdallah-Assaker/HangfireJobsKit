using HangfireJobsKit.Abstractions;
using HangfireJobsKit.Configuration;
using HangfireJobsKit.Handlers;
using HangfireJobsKit.Sample.Services;

namespace HangfireJobsKit.Sample.Jobs;

[JobConfiguration("Daily Report Job", 
    retryAttempts: 4, 
    queue: "default", logEvents: true)]
public record GenerateDailyReportJob(DateTime ReportDate) : IRecurrenceJob;

public class GenerateDailyReportJobHandler(IReportService reportService)
    : RecurrenceJobHandlerBase<GenerateDailyReportJob>
{
    protected override async Task Handle(GenerateDailyReportJob job)
    {
        var rnd = new Random().Next(1, 10);
        if (rnd % 3 == 0)
            throw new NotImplementedException();
        
        await reportService.GenerateReportAsync(job.ReportDate);
    }
}