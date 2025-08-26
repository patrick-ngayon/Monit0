// Monit0.Core/Interfaces/IVeosService.cs
using Monit0.Core.Models.Veos;
using System.Threading.Tasks;

namespace Monit0.Core.Interfaces
{
    public interface IVeosService
    {
        Task<VeosMonitoring> GetVeosMonitoringAsync();
        Task<string> GenerateHtmlReportAsync(VeosMonitoring data);
        Task SaveReportAsync(VeosMonitoring data, string outputPath);
    }
}