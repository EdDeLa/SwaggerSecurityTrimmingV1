namespace SwaggerSecurityTrimmingV1.Models.Responses
{
    public class AuthenticatedUserResponseDto
    {
        public AuthenticatedUserResponseDto()
        {
            AccessToken = String.Empty;
        }
        public string AccessToken { get; set; }
    }
}
