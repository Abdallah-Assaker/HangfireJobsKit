using HangfireJobsKit.Abstractions;
using HangfireJobsKit.Models;
using HangfireJobsKit.Sample.Jobs;

namespace HangfireJobsKit.Sample.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder ConfigureRecurringJobs(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var jobManager = scope.ServiceProvider.GetRequiredService<IRecurrenceJobManager>();
    
        // Add daily report job
        jobManager.AddOrUpdateRecurring(
            "daily-report",
            new GenerateDailyReportJob(DateTime.Today),
            new JobContext(
                correlationId: "system"
            ),
            "0 0 * * *" // Daily at midnight
        );
        
        return app;
    }
}