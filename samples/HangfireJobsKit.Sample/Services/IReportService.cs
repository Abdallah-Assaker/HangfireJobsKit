namespace HangfireJobsKit.Sample.Services;

public interface IReportService
{
    Task GenerateReportAsync(DateTime reportDate);
}