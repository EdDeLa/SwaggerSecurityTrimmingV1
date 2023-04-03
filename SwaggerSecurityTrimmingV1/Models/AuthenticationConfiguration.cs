namespace SwaggerSecurityTrimmingV1.Models
{
    public class AuthenticationConfiguration
    {
        public AuthenticationConfiguration()
        {
            Issuer = String.Empty;
            Audience = String.Empty;
            Secret = String.Empty;
        }
        public double AccessTokenExpirationMinutes { get; set; }
        public double RefreshTokenExpirationMinutes { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }
    }
}
