using System.Data;
using Microsoft.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Monit0.Core.Models.Database;
using Monit0.Core.Interfaces;

namespace Monit0.Infrastructure.Services
{
    public class ConnectionTestService : IConnectionTestService
    {
        private readonly ILogger<ConnectionTestService> _logger;
        private readonly Dictionary<string, DatabaseConnection> _databases;

        public ConnectionTestService(IConfiguration configuration, ILogger<ConnectionTestService> logger)
        {
            _logger = logger;
            _databases = LoadDatabaseConfigurations(configuration);
        }

        public async Task<Dictionary<string, bool>> TestAllConnectionsAsync()
        {
            var results = new Dictionary<string, bool>();

            foreach (var database in _databases)
            {
                if (!database.Value.IsActive)
                {
                    _logger.LogInformation("Skipping inactive database: {DatabaseName}", database.Key);
                    continue;
                }

                var isConnected = await TestConnectionAsync(database.Key);
                results[database.Key] = isConnected;

                _logger.LogInformation("Database {DatabaseName}: {Status}",
                    database.Key,
                    isConnected ? "✅ CONNECTED" : "❌ FAILED");
            }

            return results;
        }

        public async Task<bool> TestConnectionAsync(string databaseName)
        {
            if (!_databases.TryGetValue(databaseName, out var dbConfig))
            {
                _logger.LogError("Database configuration not found: {DatabaseName}", databaseName);
                return false;
            }

            try
            {
                using var connection = CreateConnection(dbConfig);
                // Cast vers le type spécifique pour OpenAsync
                if (connection is SqlConnection sqlConn)
                {
                    await sqlConn.OpenAsync();
                }
                else if (connection is OracleConnection oracleConn)
                {
                    await oracleConn.OpenAsync();
                }
                else
                {
                    connection.Open(); // Fallback synchrone
                }
                var isOpen = connection.State == ConnectionState.Open;
                if (isOpen)
                {
                    _logger.LogDebug("Successfully connected to {DatabaseName} ({Type})",
                        databaseName, dbConfig.Type);
                }
                return isOpen;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to database {DatabaseName}: {Error}",
                    databaseName, ex.Message);
                return false;
            }
        }

        public List<string> GetAvailableDatabases()
        {
            return _databases.Keys.ToList();
        }

        public DatabaseConnection? GetDatabaseConfig(string databaseName)
        {
            return _databases.TryGetValue(databaseName, out var config) ? config : null;
        }

        private IDbConnection CreateConnection(DatabaseConnection config)
        {
            return config.Type switch
            {
                DatabaseType.SqlServer => new SqlConnection(config.ConnectionString),
                DatabaseType.Oracle => new OracleConnection(config.ConnectionString),
                _ => throw new NotSupportedException($"Database type {config.Type} not supported")
            };
        }

        private Dictionary<string, DatabaseConnection> LoadDatabaseConfigurations(IConfiguration configuration)
        {
            var databases = new Dictionary<string, DatabaseConnection>();
            var section = configuration.GetSection("Databases");

            foreach (var child in section.GetChildren())
            {
                var dbConfig = new DatabaseConnection
                {
                    Name = child.Key,
                    Type = Enum.Parse<DatabaseType>(child.GetValue<string>("Type") ?? "SqlServer"),
                    ConnectionString = child.GetValue<string>("ConnectionString") ?? string.Empty,
                    IsActive = child.GetValue<bool>("IsActive"),
                    TimeoutSeconds = child.GetValue<int>("TimeoutSeconds"),
                    Description = child.GetValue<string>("Description") ?? string.Empty
                };

                databases[child.Key] = dbConfig;
                _logger.LogDebug("Loaded database config: {DatabaseName} ({Type})",
                    child.Key, dbConfig.Type);
            }

            _logger.LogInformation("Loaded {Count} database configurations", databases.Count);
            return databases;
        }
    }
}