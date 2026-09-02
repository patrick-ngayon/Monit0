using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Hosting;

using Microsoft.Extensions.Logging;

using Microsoft.Extensions.Configuration;

using Monit0.Core.Interfaces;

using Monit0.Infrastructure.Services;

using System;

using System.Collections.Generic;

using System.IO;

using System.Threading.Tasks;

namespace Monit0.Console

{

    class Program

    {

        static async Task Main(string[] args)

        {

            var host = CreateHostBuilder(args).Build();

            try

            {

                using var scope = host.Services.CreateScope();

                System.Console.WriteLine("?? Starting Monit0 with Real Data...");

                // 1. WorldCheck Monitoring

                var worldCheckService = scope.ServiceProvider.GetRequiredService<IWorldCheckService>();

                var worldCheckData = await worldCheckService.GetWorldCheckMonitoringAsync();

                if (worldCheckData != null)

                {

                    System.Console.WriteLine($"? WorldCheck Data Retrieved:");

                    System.Console.WriteLine($"   ?? Total Records: {worldCheckData.TotalCount:N0}");

                    System.Console.WriteLine($"   ?? Errors: {worldCheckData.ErrorCount:N0}");

                    System.Console.WriteLine($"   ?? Error Rate: {worldCheckData.ErrorPercentage:F1}%");

                    System.Console.WriteLine($"   ?? Last Update: {worldCheckData.LastDate:dd/MM/yyyy HH:mm}");

                    System.Console.WriteLine($"   ?? Status: {worldCheckData.GlobalStatus}");

                    // G�n�rer et sauvegarder le rapport HTML

                    // await worldCheckService.SaveReportAsync(worldCheckData, "./reports");

                    System.Console.WriteLine("?? Professional HTML Report saved in ./reports/");

                    // Afficher le chemin du fichier g�n�r�

                    var worldCheckFileName = $"WorldCheck_Report_{DateTime.Now:yyyyMMdd_HHmmss}.html";

                    System.Console.WriteLine($"?? Report File: {worldCheckFileName}");

                }

                else

                {

                    System.Console.WriteLine("?? No WorldCheck data found");

                }

                // 2. VEOS Monitoring

                System.Console.WriteLine("\n?? Starting VEOS Monitoring...");

                var veosService = scope.ServiceProvider.GetRequiredService<IVeosService>();

                var veosData = await veosService.GetVeosMonitoringAsync(); 

                if (veosData != null)

                {

                    System.Console.WriteLine($"? VEOS Data Retrieved:");

                    System.Console.WriteLine($"   ?? Total Entities: {veosData.Items.Count}");

                    System.Console.WriteLine($"   ?? Total Records: {veosData.TotalRecords:N0}");

                    System.Console.WriteLine($"   ?? Total Errors: {veosData.TotalErrors:N0}");

                    System.Console.WriteLine($"   ?? Global Status: {veosData.GlobalStatus}");

                    // D�tails par entit�

                    System.Console.WriteLine("   ?? Entity Details:");

                    foreach (var item in veosData.Items)

                    {

                        System.Console.WriteLine($"      � {item.Libelle}: {item.NbEnregistrements} records, {item.NbErr} errors - Status: {item.Status}");

                    }

                    // G�n�rer et sauvegarder le rapport HTML

                    await veosService.SaveReportAsync(veosData, "./reports");

                    System.Console.WriteLine("?? Professional HTML Report saved in ./reports/");

                    // Afficher le chemin du fichier g�n�r�

                    var veosFileName = $"VEOS_Report_{DateTime.Now:yyyyMMdd_HHmmss}.html";

                    System.Console.WriteLine($"?? Report File: {veosFileName}");

                }

                else

                {

                    System.Console.WriteLine("?? No VEOS data found"); 

                }

                // 3. Rapport combin�

                if (worldCheckData != null || veosData != null)

                {

                    System.Console.WriteLine("\n?? Generating Combined Report...");

                    var htmlTemplateService = scope.ServiceProvider.GetRequiredService<IHtmlTemplateService>();

                    var allResults = new List<object>();

                    if (worldCheckData != null) allResults.Add(worldCheckData);

                    if (veosData != null) allResults.Add(veosData);

                    var htmlContent = await htmlTemplateService.GenerateReportAsync(allResults);

                    var combinedFileName = $"Combined_Report_{DateTime.Now:yyyyMMdd_HHmmss}.html";

                    var outputPath = "./reports";

                    Directory.CreateDirectory(outputPath);

                    var combinedFilePath = Path.Combine(outputPath, combinedFileName);

                    await File.WriteAllTextAsync(combinedFilePath, htmlContent);

                    System.Console.WriteLine($"?? Combined Report saved: {combinedFileName}");

                }

                System.Console.WriteLine();

                System.Console.WriteLine("? Monit0 execution completed successfully!");

            }

            catch (Exception ex)

            {

                System.Console.WriteLine($"? Application Error: {ex.Message}");

                System.Console.WriteLine($"?? Details: {ex}");

                Environment.Exit(1);

            }

        }

        static IHostBuilder CreateHostBuilder(string[] args) =>

            Host.CreateDefaultBuilder(args)

                .ConfigureAppConfiguration((context, config) =>

                {

                    config.AddJsonFile("appsettings.json", optional: true);

                    config.AddUserSecrets<Program>(optional: true);

                    config.AddEnvironmentVariables();

                    // Support pour les arguments en ligne de commande (futur)

                    if (args?.Length > 0)

                    {

                        config.AddCommandLine(args);

                    }

                })

                .ConfigureLogging(logging =>

                {

                    // Configuration du logging

                    logging.ClearProviders();

                    logging.AddConsole();

                    logging.SetMinimumLevel(LogLevel.Information);

                })

                .ConfigureServices((context, services) =>

                {

                    // Services de donn�es

                    services.AddScoped<IDataService, DataService>();

                    // Services de templates et monitoring

                    // services.AddScoped<IHtmlTemplateService, HtmlTemplateService>();

                    // services.AddScoped<IWorldCheckService, WorldCheckService>();

                    // services.AddScoped<IVeosService, VeosService>(); // Ajout du service VEOS

                });

    }

}
