using System.Data;

using Microsoft.Data.SqlClient;

using Oracle.ManagedDataAccess.Client;

using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.Logging;

using System.Diagnostics;

using Monit0.Core.Models.Database;

using Monit0.Core.Interfaces;

namespace Monit0.Infrastructure.Services

{

    public class DataService : IDataService

    {

        private readonly IConfiguration _configuration;

        private readonly ILogger<DataService> _logger;

        private readonly Dictionary<string, DatabaseConnection> _databases;

        public DataService(IConfiguration configuration, ILogger<DataService> logger)

        {

            _configuration = configuration;

            _logger = logger;

            _databases = LoadDatabaseConfigurations();

        }

        public async Task<QueryResult> ExecuteQueryAsync(string databaseName, string query)
         
        {

            var stopwatch = Stopwatch.StartNew();

            var result = new QueryResult();

            try

            {

                if (!_databases.TryGetValue(databaseName, out var dbConfig))

                {

                    result.ErrorMessage = $"Database '{databaseName}' not found";

                    return result;

                }

                _logger.LogInformation("Executing query on database {DatabaseName}", databaseName);

                using var connection = CreateConnection(dbConfig);

                // Cast vers le type sp�cifique pour les m�thodes async

                if (connection is SqlConnection sqlConn)

                {

                    await sqlConn.OpenAsync();

                    using var sqlCommand = sqlConn.CreateCommand();

                    sqlCommand.CommandText = query;

                    sqlCommand.CommandTimeout = dbConfig.TimeoutSeconds;

                    using var reader = await sqlCommand.ExecuteReaderAsync();

                    while (await reader.ReadAsync())

                    {

                        var row = new Dictionary<string, object>();

                        for (int i = 0; i < reader.FieldCount; i++)

                        {

                            row[reader.GetName(i)] = reader.GetValue(i);

                        }

                        result.Data.Add(row);

                    }

                }

                else if (connection is OracleConnection oracleConn)

                {

                    await oracleConn.OpenAsync();

                    using var oracleCommand = oracleConn.CreateCommand();

                    oracleCommand.CommandText = query;

                    oracleCommand.CommandTimeout = dbConfig.TimeoutSeconds;

                    using var reader = await oracleCommand.ExecuteReaderAsync();

                    while (await reader.ReadAsync())

                    {

                        var row = new Dictionary<string, object>();

                        for (int i = 0; i < reader.FieldCount; i++)

                        {

                            row[reader.GetName(i)] = reader.GetValue(i);

                        }

                        result.Data.Add(row);

                    }

                }

                else

                {

                    throw new NotSupportedException($"Connection type not supported: {connection.GetType()}");

                }

                result.IsSuccess = true;

                _logger.LogInformation("Query executed successfully. Rows returned: {RowCount}", result.RowCount);

            }

            catch (Exception ex)

            {

                _logger.LogError(ex, "Error executing query on database {DatabaseName}", databaseName);

                result.ErrorMessage = ex.Message;

            }

            finally

            {

                stopwatch.Stop();

                result.ExecutionTime = stopwatch.Elapsed;

            }

            return result;

        }

        public async Task<bool> TestConnectionAsync(string databaseName)

        {

            if (!_databases.TryGetValue(databaseName, out var dbConfig))

                return false;

            try

            {

                using var connection = CreateConnection(dbConfig);

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

                return connection.State == ConnectionState.Open;

            }

            catch (Exception ex)

            {

                _logger.LogError(ex, "Connection test failed for {DatabaseName}", databaseName);

                return false;

            }

        }

        public Task<List<string>> GetAvailableDatabasesAsync()

        {

            return Task.FromResult(_databases.Keys.ToList());

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

        private Dictionary<string, DatabaseConnection> LoadDatabaseConfigurations()

        {

            var databases = new Dictionary<string, DatabaseConnection>();

            var section = _configuration.GetSection("Databases");

            foreach (var child in section.GetChildren())

            {

                var dbConfig = new DatabaseConnection

                {

                    Name = child.Key,

                    Type = Enum.Parse<DatabaseType>(child["Type"] ?? "SqlServer"),

                    ConnectionString = child["ConnectionString"] ?? string.Empty,

                    IsActive = child.GetValue<bool>("IsActive", true),

                    TimeoutSeconds = child.GetValue<int>("TimeoutSeconds", 30),

                    Description = child["Description"] ?? string.Empty

                };

                databases[child.Key] = dbConfig;

                _logger.LogDebug("Loaded database config: {DatabaseName} ({Type})", child.Key, dbConfig.Type);

            }

            _logger.LogInformation("Loaded {Count} database configurations", databases.Count);

            return databases;

        }

    }

}
