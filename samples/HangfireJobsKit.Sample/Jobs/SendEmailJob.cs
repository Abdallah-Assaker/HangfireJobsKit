using HangfireJobsKit.Abstractions;
using HangfireJobsKit.Configuration;
using HangfireJobsKit.Handlers;
using HangfireJobsKit.Sample.Services;
using Microsoft.Extensions.Logging;
using SampleApp.Services;

namespace SampleApp.Jobs;

[JobConfiguration("Email Notification Job", 
    retryAttempts: 3, 
    queue: "emails", 
    logEvents: true)]
public record SendEmailJob(string Email, string Subject, string Body) : IDelayedJob;

public class SendEmailJobHandler : DelayedJobHandlerBase<SendEmailJob>
{
    private readonly IEmailService _emailService;
    
    public SendEmailJobHandler(
        IEmailService emailService,
        ILogger<SendEmailJobHandler> logger) : base(logger)
    {
        _emailService = emailService;
    }
    
    protected override async Task Handle(SendEmailJob job)
    {
        await _emailService.SendEmailAsync(job.Email, job.Subject, job.Body);
    }
}