namespace Sessions
{
    public class HangfireOptions
    {
        public string StorageType { get; set; } = "Memory";
        public string ConnectionString { get; set; } = string.Empty;
    }
}
