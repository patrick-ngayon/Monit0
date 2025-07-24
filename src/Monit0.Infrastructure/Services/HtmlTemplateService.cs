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

            htmlBuilder.Append(GetHtmlHeader());

            htmlBuilder.Append("<body>");

            foreach (var data in monitoringData)

            {

                htmlBuilder.Append(await GenerateSection(data));

            }

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
<title>MONITORING WORDL CHECK</title>
<style>

        body { 

            font-family: Arial, sans-serif; 

            margin: 20px; 

            display: flex; 

            justify-content: center; 

        }

        table { 

            border-collapse: collapse; 

            width: 800px; 

            font-size: 14px; 

        }

        th, td { 

            border: 1px solid #999; 

            padding: 8px 12px; 

            text-align: left; 

        }

        th { 

            background-color: #f0f0f0; 

            width: 200px; 

        }

        .label-cell { 

            background-color: #e0e0e0; 

            font-weight: normal; 

        }

        .status-ko { 

            color: #c00; 

            font-weight: bold; 

            text-align: center; 

        }

        .status-ok { 

            color: #080; 

            font-weight: bold; 

            text-align: center; 

        }

        .date { 

            color: #555; 

            text-align: center; 

        }
</style>
</head>";

        }

        private async Task<string> GenerateSection(object data)

        {

            return data switch

            {

                WorldCheckMonitoring worldCheck => GenerateWorldCheckSection(worldCheck),

                _ => $"<div>Type de données non supporté: {data.GetType().Name}</div>"

            };

        }

        private string GenerateWorldCheckSection(WorldCheckMonitoring data)

        {

            string statut = data.ErrorCount > 0 ? "KO" : "OK";

            string statutClass = statut == "KO" ? "status-ko" : "status-ok";

            string dateFormatted = data.LastDate == DateTime.MinValue ? "-" : data.LastDate.ToString("dd/MM/yyyy HH:mm");

            return $@"
<table>
<!-- Ligne 1 : fusion des cellules A1 et B1 -->
<tr>
<td colspan='2'><strong>MONITORING WORDL CHECK</strong></td>
<td class='{statutClass}'>{statut}</td>
<td class='date'>{dateFormatted}</td>
</tr>
<!-- Ligne vide -->
<tr><td></td><td></td><td></td><td></td></tr>
<!-- Partie indicateurs (4 colonnes comme Excel) -->
<tr><td></td><td>Nombre de lignes traitées</td><td>{data.TotalCount}</td><td></td></tr>
<tr><td></td><td>Nombre d'erreurs</td><td>{data.ErrorCount}</td><td></td></tr>
<tr><td></td><td>Pourcentage erreurs</td><td>{Math.Round(data.ErrorPercentage)}%</td><td></td></tr>
</table>";

        }

    }

}
 