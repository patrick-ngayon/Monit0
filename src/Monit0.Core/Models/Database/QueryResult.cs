namespace Monit0.Core.Models.Database
{
    public class QueryResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public List<Dictionary<string, object>> Data { get; set; } = new();
        public TimeSpan ExecutionTime { get; set; }
        public int RowCount => Data?.Count ?? 0;
    }
}