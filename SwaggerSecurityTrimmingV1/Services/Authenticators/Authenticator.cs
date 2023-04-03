using SwaggerSecurityTrimmingV1.Models;
using SwaggerSecurityTrimmingV1.Models.Entities;
using SwaggerSecurityTrimmingV1.Models.Responses;
using SwaggerSecurityTrimmingV1.Services.TokenGenerators;

namespace SwaggerSecurityTrimmingV1.Services.Authenticators
{
    public class Authenticator
    {
        private readonly AccessTokenGenerator _accessTokenGenerator;
        private readonly AuthenticationConfiguration _authenticationConfig;
        public Authenticator(AccessTokenGenerator accessTokenGenerator,
            AuthenticationConfiguration authenticationConfig)
        {
            _accessTokenGenerator = accessTokenGenerator;
            _authenticationConfig = authenticationConfig;
        }
        public async Task<AuthenticatedUserResponseDto?> Authenticate(User user)
        {
            AuthenticatedUserResponseDto? result = null;

            string accessToken = await _accessTokenGenerator.GenerateToken(user);

            result = new AuthenticatedUserResponseDto()
            {
                AccessToken = accessToken
            };

            return result;
        }
    }
}
