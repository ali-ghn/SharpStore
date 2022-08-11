namespace SharpStore.Entities
{
    public interface IDatabaseSettings
    {
        string DatabaseName { get; set; }
        string ConnectionString { get; set; }
    }
    

    public class DatabaseSettings : IDatabaseSettings
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
    }
}