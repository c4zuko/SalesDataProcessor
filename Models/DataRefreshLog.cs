namespace SalesDataProcessor.Models
{
    public class DataRefreshLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Details { get; set; }
        public bool Success { get; set; }
    }
}
