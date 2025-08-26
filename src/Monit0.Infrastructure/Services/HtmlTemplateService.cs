using Microsoft.Extensions.Logging;

using System.Text;

using Monit0.Core.Interfaces;

using Monit0.Core.Models.WorldCheck;

using Monit0.Core.Models.Veos;

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

        // Ajouter cette méthode à votre classe HtmlTemplateService
        public async Task<string> GenerateSingleReportAsync<T>(T data, string fileName)
        {
            // Convertir l'objet unique en liste pour utiliser GenerateReportAsync
            var monitoringData = new List<object> { data };
            return await GenerateReportAsync(monitoringData);
        }

        private string GetHtmlHeader()

        {

            return @" <!DOCTYPE html> <html lang='fr'> <head> <meta charset='UTF-8'> <meta name='viewport' content='width=device-width, initial-scale=1.0'> <title>RAPPORT DE MONITORING</title> <style>

        body {

            font-family: Arial, sans-serif;

            margin: 20px;

            display: flex;

            justify-content: center;

            flex-direction: column;

            align-items: center;

        }

        table {

            border-collapse: collapse;

            width: 800px;

            font-size: 14px;

            margin-bottom: 30px;

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

        h2 {

            margin-top: 30px;

            color: #333;

        } </style> </head>";

        }

        private async Task<string> GenerateSection(object data)

        {

            return data switch

            {

                WorldCheckMonitoring worldCheck => GenerateWorldCheckSection(worldCheck),

                VeosMonitoring veos => GenerateVeosSection(veos),

                _ => $"<div>Type de données non supporté: {data.GetType().Name}</div>"

            };

        }

        private string GenerateWorldCheckSection(WorldCheckMonitoring data)

        {

            string statut = data.ErrorCount > 0 ? "KO" : "OK";

            string statutClass = statut == "KO" ? "status-ko" : "status-ok";

            string dateFormatted = data.LastDate == DateTime.MinValue ? "-" : data.LastDate.ToString("dd/MM/yyyy HH:mm");

            return $@" <table> <!-- Ligne 1 : fusion des cellules A1 et B1 --> <tr> <td colspan='2'><strong>MONITORING WORLDCHECK</strong></td> <td class='{statutClass}'>{statut}</td> <td class='date'>{dateFormatted}</td> </tr> <!-- Ligne vide --> <tr><td></td><td></td><td></td><td></td></tr> <!-- Partie indicateurs (4 colonnes comme Excel) --> <tr><td></td><td>Nombre de lignes traitées</td><td>{data.TotalCount}</td><td></td></tr> <tr><td></td><td>Nombre d'erreurs</td><td>{data.ErrorCount}</td><td></td></tr> <tr><td></td><td>Pourcentage erreurs</td><td>{Math.Round(data.ErrorPercentage)}%</td><td></td></tr> </table>";

        }

        private string GenerateVeosSection(VeosMonitoring data)

        {

            string statut = data.GlobalStatus;

            string statutClass = statut == "KO" ? "status-ko" : "status-ok";

            var sb = new StringBuilder();

            // En-tête du tableau récapitulatif

            sb.Append($@" <table> <tr> <td colspan='2'><strong>MONITORING VEOS</strong></td> <td class='{statutClass}'>{statut}</td> <td class='date'>{data.CheckTime:dd/MM/yyyy HH:mm}</td> </tr> <!-- Ligne vide --> <tr><td></td><td></td><td></td><td></td></tr> <!-- Récapitulatif --> <tr><td></td><td>Total enregistrements</td><td>{data.TotalRecords}</td><td></td></tr> <tr><td></td><td>Total erreurs</td><td>{data.TotalErrors}</td><td></td></tr> </table>");

            // Tableau des détails

            sb.Append($@" <table> <tr> <td colspan='4'><strong>DÉTAILS PAR ENTITÉ</strong></td> </tr> <tr> <td><strong>Entité</strong></td> <td><strong>Dernière mise à jour</strong></td> <td><strong>Enregistrements</strong></td> <td><strong>Erreurs</strong></td> </tr>");

            // Détails pour chaque item

            foreach (var item in data.Items)

            {

                string itemStatusClass = item.Status == "KO" ? "status-ko" : "status-ok";

                string itemDateFormatted = item.MaxDate == DateTime.MinValue ? "-" : item.MaxDate.ToString("dd/MM/yyyy HH:mm");

                sb.Append($@" <tr> <td>{item.Libelle}</td> <td>{itemDateFormatted}</td> <td>{item.NbEnregistrements}</td> <td class='{itemStatusClass}'>{item.NbErr}</td> </tr>");

            }

            sb.Append("</table>");

            return sb.ToString();

        }

    }

}
