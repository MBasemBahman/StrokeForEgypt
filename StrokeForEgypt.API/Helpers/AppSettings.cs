namespace StrokeForEgypt.API.Helpers
{
    public class AppSettings
    {
        public string Secret { get; set; }

        public string AndroidAPIKEY { get; set; }

        public string IOSAPIKEY { get; set; }

        public string WebAPIKEY { get; set; }

        // refresh token time to live (in days), inactive tokens are
        // automatically deleted from the database after this time
        public int RefreshTokenTTL { get; set; }
    }
}