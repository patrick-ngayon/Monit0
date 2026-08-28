using Microsoft.Extensions.Logging;
using Monit0.Core.Interfaces;
using Monit0.Core.Models.WorldCheck;

namespace Monit0.Infrastructure.Services
{
    public class WorldCheckService : IWorldCheckService
    {
        private readonly IDataService _dataService;
        private readonly IHtmlTemplateService _htmlTemplateService;
        private readonly ILogger<WorldCheckService> _logger;

        public WorldCheckService(
            IDataService dataService,
            IHtmlTemplateService htmlTemplateService,
        ILogger<WorldCheckService> logger)
        {
            _dataService = dataService;
            _htmlTemplateService = htmlTemplateService;
            _logger = logger;
        }

        public async Task<WorldCheckMonitoring> GetWorldCheckMonitoringAsync()
        {
            // VOTRE VRAIE REQU�TE ORACLE
            const string query = @"
                SELECT MAX(MAX_DATE_CREATION_JRN) LAST_DATE,
                       SUM(NB) NB_TOTAL,
                       SUM(CASE WHEN STATUT_JRN = 'EXPORTED' THEN NB ELSE 0 END) NB_ERR,
                       ROUND(
                           SUM(CASE WHEN STATUT_JRN = 'EXPORTED' THEN NB ELSE 0 END) / SUM(NB) * 100, 2
                       ) ""% ERR""
                FROM (
                    SELECT JRN.STATUT_JRN,
                           COUNT(1) NB,
                           MAX(JRN.DATE_CREATION_JRN) MAX_DATE_CREATION_JRN
                    FROM WORLDCHECK_OWNER.ITR_ACTEUR_JRN JRN
                    WHERE TRUNC(JRN.DATE_CREATION_JRN) BETWEEN 
                          TO_DATE(TO_CHAR(SYSDATE - 1, 'YYYYMMDD') || '10', 'YYYYMMDDHH') AND
                          TO_DATE(TO_CHAR(SYSDATE, 'YYYYMMDD') || '10', 'YYYYMMDDHH')
                    GROUP BY JRN.STATUT_JRN
                )";

            try
            {
                _logger.LogInformation("Executing WorldCheck monitoring query against Oracle database");

                var result = await _dataService.ExecuteQueryAsync("OracleDb1", query);
                if (!result.IsSuccess)
                {
                    _logger.LogError("Failed to execute WorldCheck query: {Error}", result.ErrorMessage);
                    return null;
                }

                if (result.Data?.Count > 0)
                {
                    var row = result.Data[0];
                    var monitoring = new WorldCheckMonitoring
                    {
                        LastDate = row.ContainsKey("LAST_DATE") && row["LAST_DATE"] != DBNull.Value
                            ? Convert.ToDateTime(row["LAST_DATE"])
                            : DateTime.MinValue,
                        TotalCount = row.ContainsKey("NB_TOTAL") && row["NB_TOTAL"] != DBNull.Value
                            ? Convert.ToInt32(row["NB_TOTAL"])
                            : 0,
                        ErrorCount = row.ContainsKey("NB_ERR") && row["NB_ERR"] != DBNull.Value
                            ? Convert.ToInt32(row["NB_ERR"])
                            : 0,
                        ErrorPercentage = row.ContainsKey("% ERR") && row["% ERR"] != DBNull.Value
                            ? Convert.ToDecimal(row["% ERR"])
                            : 0
                    };

                    _logger.LogInformation("WorldCheck data retrieved: Total={Total}, Errors={Errors}, Status={Status}",
                        monitoring.TotalCount, monitoring.ErrorCount, monitoring.GlobalStatus);

                    return monitoring;
                }

                _logger.LogWarning("No WorldCheck monitoring data found");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving WorldCheck monitoring data");
                throw;
            }
        }

        public async Task<string> GenerateHtmlReportAsync(WorldCheckMonitoring data)
        {
            var monitoringData = new List<object> { data };
            return await _htmlTemplateService.GenerateReportAsync(monitoringData);
        }

        public async Task SaveReportAsync(WorldCheckMonitoring data, string outputPath)
        {
            try
            {
                Directory.CreateDirectory(outputPath);
                var fileName = $"WorldCheck_Report_{DateTime.Now:yyyyMMdd_HHmmss}.html";
                var fullPath = Path.Combine(outputPath, fileName);
                var htmlContent = await GenerateHtmlReportAsync(data);
                await File.WriteAllTextAsync(fullPath, htmlContent);
                _logger.LogInformation("WorldCheck report saved to: {FilePath}", fullPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving WorldCheck report");
                throw;
            }
        }
    }
}