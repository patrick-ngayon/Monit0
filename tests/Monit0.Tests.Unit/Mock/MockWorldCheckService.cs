using Monit0.Core.Interfaces;
using Monit0.Core.Models.WorldCheck;
using Monit0.Infrastructure.Services;

namespace Monit0.Infrastructure.Services
{
    public class MockWorldCheckService : IWorldCheckService
    {
        public async Task<WorldCheckMonitoring> GetWorldCheckMonitoringAsync()
        {
            await Task.Delay(100);
            return new WorldCheckMonitoring
            {
                LastDate = DateTime.Now,
                TotalCount = 10,
                ErrorCount = 0,
                ErrorPercentage = 0
            };
        }

        public async Task<string> GenerateHtmlReportAsync(WorldCheckMonitoring data)
        {
            await Task.Delay(100);
            return "<html>Mock Report</html>";
        }

        public async Task SaveReportAsync(WorldCheckMonitoring data, string outputPath)
        {
            await Task.Delay(100);
        }
    }
}