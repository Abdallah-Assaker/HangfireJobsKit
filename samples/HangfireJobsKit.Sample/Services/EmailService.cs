namespace HangfireJobsKit.Sample.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    
    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }
    
    public Task SendEmailAsync(string email, string subject, string body)
    {
        _logger.LogInformation("Sending email to {Email} with subject {Subject}", email, subject);
        // Actual email sending logic would go here
        return Task.CompletedTask;
    }
}