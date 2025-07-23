using Monit0.Core.Models.WorldCheck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


 
namespace Monit0.Core.Interfaces
{
    public interface IWorldCheckService
    {
        Task<WorldCheckMonitoring> GetWorldCheckMonitoringAsync();
        Task<string> GenerateHtmlReportAsync(WorldCheckMonitoring data);
        Task SaveReportAsync(WorldCheckMonitoring data, string outputPath);
    }
}