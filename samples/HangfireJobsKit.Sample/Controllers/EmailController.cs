using HangfireJobsKit.Abstractions;
using HangfireJobsKit.Models;
using HangfireJobsKit.Sample.Jobs;
using Microsoft.AspNetCore.Mvc;

namespace SampleApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly IDelayedJobManager _jobManager;
    
    public EmailController(IDelayedJobManager jobManager)
    {
        _jobManager = jobManager;
    }
    
    [HttpPost]
    public IActionResult SendEmail([FromBody] EmailRequest request)
    {
        var context = new JobContext(
            correlationId: Guid.NewGuid().ToString()
        );
        
        if (request.Delayed)
        {
            _jobManager.Schedule(
                new SendEmailJob(request.Email, request.Subject, request.Body),
                context,
                delayedMilliseconds: 60000 // 1 minute
            );
            
            return Ok(new { status = "scheduled", message = "Email scheduled to be sent in 1 minute" });
        }
        else
        {
            _jobManager.Enqueue(
                new SendEmailJob(request.Email, request.Subject, request.Body),
                context
            );
            
            return Ok(new { status = "queued", message = "Email queued for immediate delivery" });
        }
    }
}

public class EmailRequest
{
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool Delayed { get; set; }
}