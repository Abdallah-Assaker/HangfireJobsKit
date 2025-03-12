namespace SampleApp.Services;

public interface IReportService
{
    Task GenerateReportAsync(DateTime reportDate);
}