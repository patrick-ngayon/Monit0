using Monit0.Core.Models.Database;

namespace Monit0.Core.Interfaces
{
    public interface IDataService
    {
        Task<QueryResult> ExecuteQueryAsync(string databaseName, string query);
        Task<bool> TestConnectionAsync(string databaseName);
        Task<List<string>> GetAvailableDatabasesAsync();
    }
}