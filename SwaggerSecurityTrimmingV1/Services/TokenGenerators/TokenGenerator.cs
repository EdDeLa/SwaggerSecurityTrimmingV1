using Microsoft.IdentityModel.Tokens;
using SwaggerSecurityTrimmingV1.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SwaggerSecurityTrimmingV1.Services.TokenGenerators
{
    public class TokenGenerator
    {
        private readonly AuthenticationConfiguration _authenticationConfiguration;
        private SigningCredentials _signingCredentials;

        public TokenGenerator(AuthenticationConfiguration authenticationConfiguration)
        {
            this._authenticationConfiguration = authenticationConfiguration;
        }

        public async Task<string> GenerateToken(string issuer, string audience, double accessTokenExpirationMinutes, IEnumerable<Claim>? claims = null)
        {
            _signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationConfiguration.Secret)), SecurityAlgorithms.HmacSha256);

            JwtHeader header = new(_signingCredentials);
            JwtPayload payload = new(issuer,
                audience,
                claims,
                DateTime.Now,
                DateTime.Now.AddMinutes(accessTokenExpirationMinutes)
            );

            JwtSecurityToken token = new(header, payload);

            string tokenTemp = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenTemp;
        }
    }
}
