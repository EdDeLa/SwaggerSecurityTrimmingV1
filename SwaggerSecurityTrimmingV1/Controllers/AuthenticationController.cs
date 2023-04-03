using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwaggerSecurityTrimmingV1.Models.Entities;
using SwaggerSecurityTrimmingV1.Models.Requests;
using SwaggerSecurityTrimmingV1.Models.Responses;
using SwaggerSecurityTrimmingV1.Repositories.UserRepositories;
using SwaggerSecurityTrimmingV1.Services.Authenticators;

namespace SwaggerSecurityTrimmingV1.Controllers
{
    [ApiController]
    [Route("[controller]/")]
    public class AuthenticationController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly Authenticator _authenticator;

        public AuthenticationController(
            IUserRepository userRepository,
            Authenticator authenticator)
        {
            this._userRepository = userRepository;
            _authenticator = authenticator;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequestDto loginRequest)
        {
            IActionResult result = Unauthorized();

            User? user = await _userRepository.FindByNameAsync(loginRequest.Username);

            if (user != null && user.Active)
            {
                bool isCorrectPassword;

                if (!string.IsNullOrEmpty(user.Password))
                {
                    isCorrectPassword = await _userRepository.CheckPasswordAsync(user, loginRequest.Password);

                    if (isCorrectPassword)
                    {
                        AuthenticatedUserResponseDto? response = await _authenticator.Authenticate(user);
                        result = Ok(response);
                    }
                    else
                    {
                        throw new Exception("login attempt failed");
                    }
                }
            }
            else
            {
                throw new Exception("login attempt failed");
            }

            return result;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("onlyAdmin")]
        public async Task<IActionResult> OnlyAdmin()
        {
            bool val1 = User.Identity.IsAuthenticated;
            return Ok("Only Admin Access");
        }

        [Authorize(Roles = "User")]
        [HttpGet("onlyUser")]
        public async Task<IActionResult> OnlyUser()
        {
            return Ok("Only User Access");
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("adminAndUser")]
        public async Task<IActionResult> AdminAndUser()
        {
            return Ok("Admin AND User Access");
        }
    }
}
