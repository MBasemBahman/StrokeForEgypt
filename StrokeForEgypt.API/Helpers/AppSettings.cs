namespace StrokeForEgypt.API.Helpers
{
    public class AppSettings
    {
        public string Secret { get; set; }

        public string AndroidSecret { get; set; }

        public string IOSSecret { get; set; }

        public string WebSecret { get; set; }

        // refresh token time to live (in days), inactive tokens are
        // automatically deleted from the database after this time
        public int RefreshTokenTTL { get; set; }
    }
}