using Monit0.Core.Models.WorldCheck;
using Monit0.Core.Interfaces;
using Monit0.Infrastructure.Services;
using Monit0.Api.Mocks;


namespace Monit0.Api.Mocks
{
    public class MockWorldCheckService : IWorldCheckService
    {
        public async Task<WorldCheckMonitoring> GetWorldCheckMonitoringAsync()
        {
            return new WorldCheckMonitoring
            {
                LastDate = DateTime.Now,

                TotalCount = 10,

                ErrorCount = 1,

                ErrorPercentage = 10
            };
        }
        public async Task<string> GenerateHtmlReportAsync(WorldCheckMonitoring data)
        {
            await Task.Delay(2000);
            return string.Empty;
        }

        public async Task SaveReportAsync(WorldCheckMonitoring data, string outputPath)
        {
            await Task.Delay(2000); 
        }
    }
}
