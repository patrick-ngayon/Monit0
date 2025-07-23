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
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            try
            {
                using var scope = host.Services.CreateScope();
                var connectionTestService = scope.ServiceProvider.GetRequiredService<IConnectionTestService>();
                System.Console.WriteLine("🔍 Testing database connections...\n");

                // Tester toutes les connexions
                var results = await connectionTestService.TestAllConnectionsAsync();

                System.Console.WriteLine("\n📋 Summary:");
                var successful = results.Count(r => r.Value);
                var failed = results.Count(r => !r.Value);

                System.Console.WriteLine($"✅ Successful: {successful}");
                System.Console.WriteLine($"❌ Failed: {failed}");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"💥 Application error: {ex.Message}");
                Environment.Exit(1);
            }
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddScoped<IConnectionTestService, ConnectionTestService>();
                });
    }
}