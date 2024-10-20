namespace chatApi.Models
{
    public class ChatDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string ChatCollectionName { get; set; } = null!;
    }
}
