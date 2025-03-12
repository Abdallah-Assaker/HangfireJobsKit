using Hangfire;
using HangfireJobsKit.Abstractions.Handlers;
using HangfireJobsKit.Configuration;
using HangfireJobsKit.Sample.Extensions;
using HangfireJobsKit.Sample.Jobs;
using HangfireJobsKit.Sample.Services;
using HangfireJobsKit.SampleApp.Filters;
using SampleApp.Jobs;
using SampleApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// Add Hangfire with HangfireJobsKit
builder.Services.AddHangfireWithJobsKit(
    builder.Configuration.GetConnectionString("Hangfire")!);

// Add application services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IReportService, ReportService>();

// Register job handlers
builder.Services.AddScoped<IDelayedJobHandlerBase<SendEmailJob>, SendEmailJobHandler>();
builder.Services.AddScoped<IRecurrenceJobHandlerBase<GenerateDailyReportJob>, GenerateDailyReportJobHandler>();

// Register servers
builder.Services.AddHangfireJobsKitServer(
    "MainWorker", 
    new[] { "default", "critical" });
    
builder.Services.AddHangfireJobsKitServer(
    "EmailWorker", 
    new[] { "emails" });

// Add custom filter
builder.Services.AddHangfireJobsKitFilter<AuditLogFilter>();

var app = builder.Build();

// Configure middleware
app.UseRouting();
app.UseAuthorization();

// Add Hangfire dashboard
app.UseHangfireDashboard();

// Configure HangfireJobsKit
app.UseHangfireJobsKit();

// Map controllers
app.MapControllers();

// Configure recurring jobs
app.ConfigureRecurringJobs();

app.Run();