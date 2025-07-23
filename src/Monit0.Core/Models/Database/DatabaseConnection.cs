namespace Monit0.Core.Models.Database
{
    public class DatabaseConnection
    {
        public string Name { get; set; } = string.Empty;
        public DatabaseType Type { get; set; }
        public string ConnectionString { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int TimeoutSeconds { get; set; } = 30;
        public string Description { get; set; } = string.Empty;
    }

    public enum DatabaseType
    {
        SqlServer,
        Oracle,
        MySQL,
        PostgreSQL
    }
}