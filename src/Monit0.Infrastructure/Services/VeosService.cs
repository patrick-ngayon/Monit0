using Microsoft.Extensions.Logging;
using Monit0.Core.Interfaces;
using Monit0.Core.Models.Veos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Monit0.Infrastructure.Services
{
    public class VeosService : IVeosService
    {
        private readonly IDataService _dataService;
        private readonly IHtmlTemplateService _htmlTemplateService;
        private readonly ILogger<VeosService> _logger;
        public VeosService(
            IDataService dataService,
            IHtmlTemplateService htmlTemplateService,
            ILogger<VeosService> logger)
        {
            _dataService = dataService;
            _htmlTemplateService = htmlTemplateService;
            _logger = logger;
        }
        public async Task<VeosMonitoring> GetVeosMonitoringAsync()
        {
            // La requête Oracle pour VEOS
            const string query = @"
SELECT 
    POSITIONS,
    LIBELLE,
    MAX_DATE,
    NB_ENREGISTREMENTS,
    NB_ERR
FROM (
    SELECT 1 AS POSITIONS,
    'ACTEURS' AS LIBELLE,
    max(act.DATE_CREATION_JRN) AS MAX_DATE,
    count(1) AS NB_ENREGISTREMENTS,
    max(err.NB_ERR) AS NB_ERR
    FROM VEOS_OWNER.ITR_ACTEUR_JRN act
    INNER JOIN (
        SELECT count(1) AS NB_ERR
        FROM VEOS_OWNER.TMP_ACTEUR_ERR errA
        WHERE TRUNC(errA.ERR_DATE) = TRUNC(CASE 
            WHEN TO_NUMBER(TO_CHAR(SYSDATE, 'D')) = 1 THEN SYSDATE - 3 -- Lundi (D=1)
            ELSE SYSDATE - 1 -- Autres jours
        END)
        AND errA.ERR_CODE = 'ACTEUR_ALREADY_EXPORTED'
    ) err
    ON 1 = 1
    WHERE TRUNC(DATE_CREATION_JRN) = TRUNC(CASE 
        WHEN TO_NUMBER(TO_CHAR(SYSDATE, 'D')) = 1 THEN SYSDATE - 3 -- Lundi (D=1)
        ELSE SYSDATE - 1 -- Autres jours
    END)
 
    UNION
 
    SELECT 2 AS POSITIONS,
    'CONTRATS' AS LIBELLE,
    max(cont.DATE_CREATION_JRN) AS MAX_DATE,
    count(*) AS NB_ENREGISTREMENTS,
    max(err.NB_ERR) AS NB_ERR
    FROM VEOS_OWNER.ITR_CONTRAT_JRN cont
    INNER JOIN (
        SELECT count(1) AS NB_ERR
        FROM VEOS_OWNER.TMP_CONTRAT_ERR errCo
        WHERE TRUNC(errCo.ERR_DATE) = TRUNC(CASE 
            WHEN TO_NUMBER(TO_CHAR(SYSDATE, 'D')) = 1 THEN SYSDATE - 3 -- Lundi (D=1)
            ELSE SYSDATE - 1 -- Autres jours
        END)
        AND errCo.ERR_CODE = 'CONTRAT_COASS_ALREADY_EXPORTED'
    ) err
    ON 1 = 1
    WHERE TRUNC(DATE_CREATION_JRN) = TRUNC(CASE 
        WHEN TO_NUMBER(TO_CHAR(SYSDATE, 'D')) = 1 THEN SYSDATE - 3 -- Lundi (D=1)
        ELSE SYSDATE - 1 -- Autres jours
    END)
 
    UNION
 
    SELECT 3 AS POSITIONS,
    'COASSURANCES' AS LIBELLE,
    max(coas.DATE_CREATION_JRN) AS MAX_DATE,
    count(*) AS NB_ENREGISTREMENTS,
    max(err.NB_ERR) AS NB_ERR
    FROM VEOS_OWNER.ITR_CONTRAT_COASS_JRN coas
    INNER JOIN (
        SELECT count(1) AS NB_ERR
        FROM VEOS_OWNER.TMP_CONTRAT_COASS_ERR errC
        WHERE TRUNC(errC.ERR_DATE) = TRUNC(CASE 
            WHEN TO_NUMBER(TO_CHAR(SYSDATE, 'D')) = 1 THEN SYSDATE - 3 -- Lundi (D=1)
            ELSE SYSDATE - 1 -- Autres jours
        END)
        AND errC.ERR_CODE != 'CONTRAT_COASS_ALREADY_EXPORTED'
    ) err
    ON 1 = 1
    WHERE TRUNC(DATE_CREATION_JRN) = TRUNC(CASE 
        WHEN TO_NUMBER(TO_CHAR(SYSDATE, 'D')) = 1 THEN SYSDATE - 3 -- Lundi (D=1)
        ELSE SYSDATE - 1 -- Autres jours
    END)
 
    UNION
 
    SELECT 4 AS POSITIONS,
    'SINISTRES' AS LIBELLE,
    max(sini.DATE_CREATION_JRN) AS MAX_DATE,
    count(*) AS NB_ENREGISTREMENTS,
    max(err.NB_ERR) AS NB_ERR
    FROM VEOS_OWNER.ITR_SINISTRE_JRN sini
    INNER JOIN (
        SELECT count(1) AS NB_ERR
        FROM VEOS_OWNER.TMP_SINISTRE_ERR errS
        WHERE TRUNC(errS.CHECK_DATE) = TRUNC(CASE 
            WHEN TO_NUMBER(TO_CHAR(SYSDATE, 'D')) = 1 THEN SYSDATE - 3 -- Lundi (D=1)
            ELSE SYSDATE - 1 -- Autres jours
        END)
        AND errS.ERR_CODE != 'SINISTRE_ALREADY_EXPORTED'
    ) err
    ON 1 = 1
    WHERE TRUNC(sini.DATE_MODIFICATION_JRN) = TRUNC(CASE 
        WHEN TO_NUMBER(TO_CHAR(SYSDATE, 'D')) = 1 THEN SYSDATE - 3 -- Lundi (D=1)
        ELSE SYSDATE - 1 -- Autres jours
    END)
 
    UNION
 
    SELECT 5 AS POSITIONS,
    'REGLEMENTS' AS LIBELLE,
    max(reg.DATE_CREATION_JRN) AS MAX_DATE,
    count(1) AS NB_ENREGISTREMENTS,
    max(err.NB_ERR) AS NB_ERR
    FROM VEOS_OWNER.ITR_REGLEMENT_JRN reg
    INNER JOIN(
        SELECT count(1) AS NB_ERR
        FROM VEOS_OWNER.TMP_REGLEMENT_ERR errR
        WHERE TRUNC(errR.CHECK_DATE) = TRUNC(CASE 
            WHEN TO_NUMBER(TO_CHAR(SYSDATE, 'D')) = 1 THEN SYSDATE - 3 -- Lundi (D=1)
            ELSE SYSDATE - 1 -- Autres jours
        END)
        AND errR.ERR_CODE != 'REGLEMENT_ALREADY_EXPORTED'
    ) err
    ON 1 = 1
    WHERE TRUNC(reg.DATE_CREATION_JRN) = TRUNC(CASE 
        WHEN TO_NUMBER(TO_CHAR(SYSDATE, 'D')) = 1 THEN SYSDATE - 3 -- Lundi (D=1)
        ELSE SYSDATE - 1 -- Autres jours
    END)
 
    UNION
 
    SELECT 6 AS POSITIONS,
    'VENTILLATIONS' AS LIBELLE,
    max(vent.DATE_CREATION_JRN) AS MAX_DATE,
    count(*) AS NB_ENREGISTREMENTS,
    max(err.NB_ERR) AS NB_ERR
    FROM VEOS_OWNER.ITR_VENTILLATION_JRN vent
    INNER JOIN (
        SELECT count(1) AS NB_ERR
        FROM VEOS_OWNER.TMP_VENTILLATION_ERR errV
        WHERE TRUNC(errV.CHECK_DATE) = TRUNC(CASE 
            WHEN TO_NUMBER(TO_CHAR(SYSDATE, 'D')) = 1 THEN SYSDATE - 3 -- Lundi (D=1)
            ELSE SYSDATE - 1 -- Autres jours
        END)
        AND errV.ERR_CODE != 'VENTILLATION_ALREADY_EXPORTED'
    ) err
    ON 1 = 1
    WHERE TRUNC(vent.DATE_CREATION_JRN) = TRUNC(CASE 
        WHEN TO_NUMBER(TO_CHAR(SYSDATE, 'D')) = 1 THEN SYSDATE - 3 -- Lundi (D=1)
        ELSE SYSDATE - 1 -- Autres jours
    END)
) data
ORDER BY POSITIONS";
            try
            {
                _logger.LogInformation("Executing VEOS monitoring query against Oracle database");
                var result = await _dataService.ExecuteQueryAsync("OracleDb1", query);
                if (!result.IsSuccess)
                {
                    _logger.LogError("Failed to execute VEOS query: {Error}", result.ErrorMessage);
                    return null;
                }
                if (result.Data?.Count > 0)
                {
                    var veosMonitoring = new VeosMonitoring
                    {
                        CheckTime = DateTime.Now,
                        Items = new List<VeosMonitoringItem>()
                    };
                    foreach (var row in result.Data)
                    {
                        var item = new VeosMonitoringItem
                        {
                            Position = row.ContainsKey("POSITIONS") && row["POSITIONS"] != DBNull.Value
                                ? Convert.ToInt32(row["POSITIONS"])
                                : 0,
                            Libelle = row.ContainsKey("LIBELLE") && row["LIBELLE"] != DBNull.Value
                                ? Convert.ToString(row["LIBELLE"])
                                : string.Empty,
                            MaxDate = row.ContainsKey("MAX_DATE") && row["MAX_DATE"] != DBNull.Value
                                ? Convert.ToDateTime(row["MAX_DATE"])
                                : DateTime.MinValue,
                            NbEnregistrements = row.ContainsKey("NB_ENREGISTREMENTS") && row["NB_ENREGISTREMENTS"] != DBNull.Value
                                ? Convert.ToInt32(row["NB_ENREGISTREMENTS"])
                                : 0,
                            NbErr = row.ContainsKey("NB_ERR") && row["NB_ERR"] != DBNull.Value
                                ? Convert.ToInt32(row["NB_ERR"])
                                : 0
                        };
                        veosMonitoring.Items.Add(item);
                    }
                    _logger.LogInformation("VEOS data retrieved: Total Items={TotalItems}, Total Records={TotalRecords}, Total Errors={TotalErrors}, Status={Status}",
                        veosMonitoring.Items.Count, veosMonitoring.TotalRecords, veosMonitoring.TotalErrors, veosMonitoring.GlobalStatus);
                    return veosMonitoring;
                }
                _logger.LogWarning("No VEOS monitoring data found");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving VEOS monitoring data");
                throw;
            }
        }
        public async Task<string> GenerateHtmlReportAsync(VeosMonitoring data)
        {
            var monitoringData = new List<object> { data };
            return await _htmlTemplateService.GenerateReportAsync(monitoringData);
        }
        public async Task SaveReportAsync(VeosMonitoring data, string outputPath)
        {
            try
            {
                Directory.CreateDirectory(outputPath);
                var fileName = $"VEOS_Report_{DateTime.Now:yyyyMMdd_HHmmss}.html";
                var fullPath = Path.Combine(outputPath, fileName);
                // Utiliser GenerateReportAsync
                var monitoringData = new List<object> { data };
                var htmlContent = await _htmlTemplateService.GenerateReportAsync(monitoringData);
                await File.WriteAllTextAsync(fullPath, htmlContent);
                _logger.LogInformation("VEOS report saved to: {FilePath}", fullPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving VEOS report");
                throw;
            }
        }
    }
}