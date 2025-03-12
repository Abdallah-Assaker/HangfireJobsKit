# HangfireJobsKit

HangfireJobsKit is a comprehensive framework for structuring and managing background jobs in ASP.NET applications using Hangfire. It provides a clean, dependency injection-friendly architecture for defining, scheduling, and processing jobs with robust error handling, transaction management, and logging.

## Features

- **Structured Job Architecture**: Clear separation between job definitions, handlers, and management
- **First-Class Dependency Injection**: Full DI support for job handlers and filters
- **Filter Pipeline**: Middleware-like approach for cross-cutting concerns like transactions and logging
- **Automatic Retry Configuration**: Configure retry behavior at the job class level using attributes
- **Queue Support**: Fine-grained control over which queues jobs are sent to
- **Transaction Support**: Built-in transaction handling for job processing
- **Extensible Architecture**: Easy to add custom filters for your specific needs

## Installation

```bash
dotnet add package HangfireJobsKit
```

## Quick Start

### 1. Register Services

In your `Program.cs` or `Startup.cs`:

```csharp
// Add Hangfire with HangfireJobsKit
builder.Services.AddHangfireWithJobsKit(
    builder.Configuration.GetConnectionString("Hangfire"));

// Register job handlers
builder.Services.AddScoped<IDelayedJobHandlerBase<YourDelayedJob>, YourDelayedJobHandler>();
builder.Services.AddScoped<IRecurrenceJobHandlerBase<YourRecurringJob>, YourRecurringJobHandler>();

// Register a server
builder.Services.AddHangfireJobsKitServer(
    "MainServer", 
    new[] { "critical", "default" });

// Optional: Add custom filter
builder.Services.AddHangfireJobsKitFilter<YourCustomFilter>();
```

### 2. Configure Middleware

```csharp
// Add the Hangfire middleware
app.UseHangfireDashboard();

// Add HangfireJobsKit middleware
app.UseHangfireJobsKit();
```

### 3. Define Your Jobs

#### Delayed Job

```csharp
using HangfireJobsKit.Abstractions;
using HangfireJobsKit.Configuration;
using HangfireJobsKit.Models;

[JobConfiguration("Email Notification Job", 
    retryAttempts: 3, 
    queue: "emails", 
    logEvents: true)]
public record SendEmailJob(string Email, string Subject, string Body) : IDelayedJob;
```

#### Recurring Job

```csharp
using HangfireJobsKit.Abstractions;
using HangfireJobsKit.Configuration;

[JobConfiguration("Daily Report Job", 
    retryAttempts: 1, 
    queue: "reports")]
public record GenerateDailyReportJob(DateTime ReportDate) : IRecurrenceJob;
```

### 4. Implement Handlers

#### Delayed Job Handler

```csharp
using HangfireJobsKit.Abstractions;
using HangfireJobsKit.Abstractions.Handlers;
using HangfireJobsKit.Handlers;
using Microsoft.Extensions.Logging;

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
```

#### Recurring Job Handler

```csharp
using HangfireJobsKit.Abstractions;
using HangfireJobsKit.Abstractions.Handlers;
using HangfireJobsKit.Handlers;
using Microsoft.Extensions.Logging;

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
```

### 5. Schedule Jobs

```csharp
// For one-time jobs
[ApiController]
[Route("api/[controller]")]
public class EmailController
{
    private readonly IDelayedJobManager _jobManager;
    
    public EmailController(IDelayedJobManager jobManager)
    {
        _jobManager = jobManager;
    }
    
    [HttpPost]
    public IActionResult SendEmail(EmailRequest request)
    {
        var context = new JobContext(
            correlationId: Guid.NewGuid().ToString()
        );
        
        // Send immediately
        _jobManager.Enqueue(
            new SendEmailJob(request.Email, request.Subject, request.Body),
            context
        );
        
        // Or schedule for later
        _jobManager.Schedule(
            new SendEmailJob(request.Email, request.Subject, request.Body),
            context,
            delayedMilliseconds: 60000 // 1 minute
        );
        
        return Ok();
    }
}

// For recurring jobs (typically in startup code)
public static void ConfigureRecurringJobs(IApplicationBuilder app)
{
    using var scope = app.ApplicationServices.CreateScope();
    var jobManager = scope.ServiceProvider.GetRequiredService<IRecurrenceJobManager>();
    
    jobManager.AddOrUpdateRecurring(
        "daily-report",
        new GenerateDailyReportJob(DateTime.Today),
        new JobContext(correlationId: "system"),
        "0 0 * * *" // Daily at midnight
    );
}
```

## Architecture

### Core Components

#### Jobs

- **IJob**: Base interface for all jobs
- **IDelayedJob**: Interface for jobs that run once
- **IRecurrenceJob**: Interface for recurring jobs

#### Job Handlers

- **BackgroundJobHandlerBase<TJob>**: Base implementation for all job handlers
- **DelayedJobHandlerBase<TJob>**: Base implementation for delayed job handlers
- **RecurrenceJobHandlerBase<TJob>**: Base implementation for recurring job handlers

#### Job Managers

- **IJobManager**: Combined interface for all job operations
- **IDelayedJobManager**: Interface for managing delayed jobs
- **IRecurrenceJobManager**: Interface for managing recurring jobs

#### Filters

- **IHangfireJobFilter**: Interface for creating custom filters
- **JobConfigurationFilter**: Applies job configuration attributes
- **HangfireJobFilter**: Orchestrates filter execution

### Filter Pipeline

HangfireJobsKit implements a filter pipeline similar to ASP.NET middleware. Filters are executed in order of their `ExecutionOrder` property (ascending) when a job starts and in reverse order when a job completes.

This allows for implementing cross-cutting concerns such as:

- Transaction management
- Logging
- Error handling
- Performance monitoring
- User context initialization

****

## Job Configuration

Jobs can be configured using the `JobConfigurationAttribute`:

```csharp
[JobConfiguration(
    displayName: "My Job",
    retryAttempts: 3,
    retryDelaysInSeconds: new[] { 10, 30, 60 },
    onAttemptsExceeded: AttemptsExceededAction.Delete,
    exceptOn: new[] { typeof(InvalidOperationException) },
    queue: "critical",
    logEvents: true)]
public record MyJob(string Data) : IDelayedJob;
```

### Configuration Options

- **displayName**: The name to display in the Hangfire dashboard
- **retryAttempts**: Number of times to retry the job if it fails
- **retryDelaysInSeconds**: Delay between retry attempts in seconds
- **onAttemptsExceeded**: Action to take when retry attempts are exceeded (Fail or Delete)
- **exceptOn**: Exception types that should not trigger retries
- **queue**: Default queue for the job
- **logEvents**: Whether to log job execution events

## Custom Filters

You can create custom filters by implementing the `IHangfireJobFilter` interface:

```csharp
public class PerformanceMonitorFilter : IHangfireJobFilter
{
    private readonly Stopwatch _stopwatch = new();
    private readonly ILogger<PerformanceMonitorFilter> _logger;
    
    public PerformanceMonitorFilter(ILogger<PerformanceMonitorFilter> logger)
    {
        _logger = logger;
    }
    
    public double ExecutionOrder => 5;
    
    public void OnPerforming(PerformingContext context)
    {
        _stopwatch.Start();
    }
    
    public void OnPerformed(PerformedContext context)
    {
        _stopwatch.Stop();
        _logger.LogInformation(
            "Job {JobId} executed in {ElapsedMs}ms",
            context.BackgroundJob.Id,
            _stopwatch.ElapsedMilliseconds);
    }
    
    // Other methods with empty implementations...
    public void OnCreating(CreatingContext context) { }
    public void OnCreated(CreatedContext context) { }
    public void OnStateElection(ElectStateContext context) { }
    public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction) { }
    public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction) { }
}

// Register the filter
services.AddHangfireJobsKitFilter<PerformanceMonitorFilter>();
```

## Multiple Queue Support

HangfireJobsKit supports multiple Hangfire servers processing different queues:

```csharp
// Server for critical jobs
builder.Services.AddHangfireJobsKitServer(
    "CriticalWorker",
    new[] { "critical" });
    
// Server for IO-bound jobs
builder.Services.AddHangfireJobsKitServer(
    "IOWorker",
    new[] { "io-bound" });
```

## Advanced Configuration

### Custom Json Serialization

```csharp
builder.Services.AddHangfireWithJobsKit(
    connectionString,
    config => 
    {
        config.UseSerializerSettings(new JsonSerializerSettings
        {
            // Custom serialization settings
        });
    });
```

### Worker Count

```csharp
builder.Services.AddHangfireJobsKitServer(
    "HighThroughputWorker",
    new[] { "high-throughput" },
    options => 
    {
        options.WorkerCount = 20;
    });
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.