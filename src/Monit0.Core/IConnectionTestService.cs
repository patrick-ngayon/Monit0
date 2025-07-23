using Monit0.Core.Models.Database;

namespace Monit0.Core.Interfaces
{
    public interface IConnectionTestService
    {
        /// <summary>
        /// Teste toutes les connexions configurées et retourne les résultats
        /// </summary>
        Task<Dictionary<string, bool>> TestAllConnectionsAsync();

        /// <summary>
        /// Teste une connexion spécifique
        /// </summary>
        Task<bool> TestConnectionAsync(string databaseName);

        /// <summary>
        /// Retourne la liste des bases de données disponibles
        /// </summary>
        List<string> GetAvailableDatabases();

        /// <summary>
        /// Retourne la configuration d'une base de données spécifique
        /// </summary>
        DatabaseConnection? GetDatabaseConfig(string databaseName);
    }
}