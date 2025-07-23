using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Monit0.Core.Interfaces;
using Monit0.Infrastructure.Services;

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
                System.Console.WriteLine("?? Starting Monit0 with Real WorldCheck Data...");
                var worldCheckService = scope.ServiceProvider.GetRequiredService<IWorldCheckService>();
                var data = await worldCheckService.GetWorldCheckMonitoringAsync();
                if (data != null)
                {
                    System.Console.WriteLine($"? WorldCheck Data Retrieved:");
                    System.Console.WriteLine($"   ?? Total Records: {data.TotalCount:N0}");
                    System.Console.WriteLine($"   ? Errors: {data.ErrorCount:N0}");
                    System.Console.WriteLine($"   ?? Error Rate: {data.ErrorPercentage:F1}%");
                    System.Console.WriteLine($"   ?? Last Update: {data.LastDate:dd/MM/yyyy HH:mm}");
                    System.Console.WriteLine($"   ?? Status: {data.GlobalStatus}");
                    // Générer et sauvegarder le rapport HTML
                    await worldCheckService.SaveReportAsync(data, "./reports");
                    System.Console.WriteLine("? Professional HTML Report saved in ./reports/");
                    // Afficher le chemin du fichier généré
                    var fileName = $"WorldCheck_Report_{DateTime.Now:yyyyMMdd_HHmmss}.html";
                    System.Console.WriteLine($"?? Report File: {fileName}");
                }
                else
                {
                    System.Console.WriteLine("?? No WorldCheck data found");
                }
                System.Console.WriteLine();
                System.Console.WriteLine("?? Monit0 execution completed successfully!");
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
                // Services de données
                services.AddScoped<IDataService, DataService>();
                // Services de templates et monitoring
                services.AddScoped<IHtmlTemplateService, HtmlTemplateService>();
                services.AddScoped<IWorldCheckService, WorldCheckService>();
            });
    }
}