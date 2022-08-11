namespace SharpStore.Entities
{
    public interface IJwtAuthSettings
    {
        string Issuer { get; set; }
        string Audience { get; set; }
        string Key { get; set; }
        int ExpirationMinutes { get; set; }
    }
    public class JwtAuthSettings : IJwtAuthSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
        public int ExpirationMinutes { get; set; }
    }
}