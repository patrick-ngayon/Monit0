namespace Monit0.Core.Interfaces
{
    public interface IHtmlTemplateService
    {
        Task<string> GenerateReportAsync(List<object> monitoringData);
    }
}