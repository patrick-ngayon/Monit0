using Microsoft.Extensions.Logging;

using System.Text;

using Monit0.Core.Interfaces;

using Monit0.Core.Models.WorldCheck;

namespace Monit0.Infrastructure.Services

{

    public class HtmlTemplateService : IHtmlTemplateService

    {

        private readonly ILogger<HtmlTemplateService> _logger;

        public HtmlTemplateService(ILogger<HtmlTemplateService> logger)

        {

            _logger = logger;

        }

        public async Task<string> GenerateReportAsync(List<object> monitoringData)

        {

            var htmlBuilder = new StringBuilder();

            // Header HTML commun

            htmlBuilder.Append(GetHtmlHeader());

            // Contenu dynamique

            htmlBuilder.Append("<body>");

            htmlBuilder.Append(GetReportHeader());

            foreach (var data in monitoringData)

            {

                htmlBuilder.Append(await GenerateSection(data));

            }

            htmlBuilder.Append(GetReportFooter());

            htmlBuilder.Append("</body></html>");

            return htmlBuilder.ToString();

        }

        private string GetHtmlHeader()

        {

            return @"
<!DOCTYPE html>
<html lang='fr'>
<head>
<meta charset='UTF-8'>
<meta name='viewport' content='width=device-width, initial-scale=1.0'>
<title>Monit0 - Rapport de Monitoring</title>
<style>

        :root {

            --color-success: #28a745;

            --color-error: #dc3545;

            --color-warning: #ffc107;

            --color-info: #17a2b8;

        }

        body {

            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;

            margin: 0;

            padding: 20px;

            background-color: #f8f9fa;

        }

        .container {

            max-width: 1200px;

            margin: 0 auto;

            background: white;

            border-radius: 8px;

            box-shadow: 0 2px 10px rgba(0,0,0,0.1);

            overflow: hidden;

        }

        .header {

            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);

            color: white;

            padding: 30px;

            text-align: center;

        }

        .header h1 {

            margin: 0;

            font-size: 2.5rem;

            font-weight: 300;

        }

        .header .timestamp {

            margin-top: 10px;

            opacity: 0.9;

            font-size: 0.9rem;

        }

        .monitoring-section {

            padding: 30px;

            border-bottom: 1px solid #eee;

        }

        .monitoring-section:last-child {

            border-bottom: none;

        }

        .section-header {

            display: flex;

            justify-content: space-between;

            align-items: center;

            margin-bottom: 20px;

        }

        .section-title {

            font-size: 1.5rem;

            font-weight: 600;

            color: #333;

        }

        .status-badge {

            padding: 8px 16px;

            border-radius: 20px;

            font-weight: bold;

            font-size: 0.9rem;

            text-transform: uppercase;

        }

        .status-ok {

            background-color: var(--color-success);

            color: white;

        }

        .status-ko {

            background-color: var(--color-error);

            color: white;

        }

        .metrics-grid {

            display: grid;

            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));

            gap: 20px;

            margin-top: 20px;

        }

        .metric-card {

            background: #f8f9fa;

            border: 1px solid #dee2e6;

            border-radius: 8px;

            padding: 20px;

            text-align: center;

        }

        .metric-value {

            font-size: 2rem;

            font-weight: bold;

            color: #495057;

            margin-bottom: 5px;

        }

        .metric-label {

            color: #6c757d;

            font-size: 0.9rem;

            text-transform: uppercase;

            letter-spacing: 0.5px;

        }

        .footer {

            background-color: #f8f9fa;

            padding: 20px;

            text-align: center;

            color: #6c757d;

            font-size: 0.9rem;

        }

        @media (max-width: 768px) {

            .container {

                margin: 10px;

                border-radius: 0;

            }

            .header {

                padding: 20px;

            }

            .header h1 {

                font-size: 2rem;

            }

            .monitoring-section {

                padding: 20px;

            }

        }
</style>
</head>";

        }

        private string GetReportHeader()

        {

            return $@"
<div class='container'>
<div class='header'>
<h1>?? Monit0 - Rapport de Monitoring</h1>
<div class='timestamp'>Généré le {DateTime.Now:dddd dd MMMM yyyy à HH:mm}</div>
</div>";

        }

        private async Task<string> GenerateSection(object data)

        {

            return data switch

            {

                WorldCheckMonitoring worldCheck => GenerateWorldCheckSection(worldCheck),

                _ => $"<div class='monitoring-section'><p>Type de données non supporté: {data.GetType().Name}</p></div>"

            };

        }

        private string GenerateWorldCheckSection(WorldCheckMonitoring data)

        {

            var statusClass = data.GlobalStatus.ToLower() == "ok" ? "status-ok" : "status-ko";

            var lastDateFormatted = data.LastDate == DateTime.MinValue ? "N/A" : data.LastDate.ToString("dd/MM/yyyy HH:mm");

            return $@"
<div class='monitoring-section'>
<div class='section-header'>
<h2 class='section-title'>?? WorldCheck Monitoring</h2>
<span class='status-badge {statusClass}'>{data.GlobalStatus}</span>
</div>
<div class='metrics-grid'>
<div class='metric-card'>
<div class='metric-value'>{data.TotalCount:N0}</div>
<div class='metric-label'>Lignes traitées</div>
</div>
<div class='metric-card'>
<div class='metric-value'>{data.ErrorCount:N0}</div>
<div class='metric-label'>Erreurs détectées</div>
</div>
<div class='metric-card'>
<div class='metric-value'>{Math.Round(data.ErrorPercentage, 1)}%</div>
<div class='metric-label'>Taux d'erreur</div>
</div>
<div class='metric-card'>
<div class='metric-value' style='font-size: 1.2rem;'>{lastDateFormatted}</div>
<div class='metric-label'>Dernière mise à jour</div>
</div>
</div>
</div>";

        }

        private string GetReportFooter()

        {

            return $@"
<div class='footer'>
<p>Rapport généré automatiquement par <strong>Monit0</strong> • {DateTime.Now:yyyy}</p>
</div>
</div>";

        }

    }

}
