using SwaggerSecurityTrimmingV1.Models;
using SwaggerSecurityTrimmingV1.Models.Entities;
using SwaggerSecurityTrimmingV1.Repositories.RoleRepository;
using SwaggerSecurityTrimmingV1.Repositories.UserRepositories;
using System.Security.Claims;

namespace SwaggerSecurityTrimmingV1.Services.TokenGenerators
{
    public class AccessTokenGenerator
    {
        private readonly AuthenticationConfiguration _configuration;
        private readonly TokenGenerator _tokenGenerator;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public AccessTokenGenerator(
            AuthenticationConfiguration configuration,
            TokenGenerator tokenGenerator,
            IUserRepository userRepository,
            IRoleRepository roleRepository)
        {
            _configuration = configuration;
            _tokenGenerator = tokenGenerator;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task<string> GenerateToken(User user)
        {
            IList<String> userRoles = await _userRepository.GetRolesAsync(user);

            List<Claim> claims = new()
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            claims.AddRange(userRoles.Select(roleName => new Claim(ClaimTypes.Role, roleName)));

            string token = await _tokenGenerator.GenerateToken(
                _configuration.Issuer,
                _configuration.Audience,
                _configuration.AccessTokenExpirationMinutes,
                claims);

            return token;
        }
    }
}
