using HangfireJobsKit.Abstractions;
using HangfireJobsKit.Configuration;
using HangfireJobsKit.Handlers;
using HangfireJobsKit.Sample.Services;

namespace HangfireJobsKit.Sample.Jobs;

[JobConfiguration("Email Notification Job", 
    retryAttempts: 3, 
    queue: "emails", 
    logEvents: true)]
public record SendEmailJob(string Email, string Subject, string Body) : IDelayedJob;

public class SendEmailJobHandler(IEmailService emailService) : DelayedJobHandlerBase<SendEmailJob>
{
    protected override async Task Handle(SendEmailJob job)
    {
        var rnd = new Random().Next(1, 10);
        if (rnd % 3 == 0)
            throw new NotImplementedException();
        
        await emailService.SendEmailAsync(job.Email, job.Subject, job.Body);
    }
}