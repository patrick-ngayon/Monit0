// Monit0.Core/Interfaces/IHtmlTemplateService.cs
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Monit0.Core.Interfaces
{
    public interface IHtmlTemplateService
    {
        // Méthode existante pour un rapport combiné
        Task<string> GenerateReportAsync(List<object> monitoringData);
        // Nouvelle méthode pour un rapport individuel
        Task<string> GenerateSingleReportAsync<T>(T data, string fileName);
    }
}