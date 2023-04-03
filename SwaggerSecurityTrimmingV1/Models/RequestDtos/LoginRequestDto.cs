using System.ComponentModel.DataAnnotations;

namespace SwaggerSecurityTrimmingV1.Models.Requests
{
    public class LoginRequestDto
    {
        public LoginRequestDto()
        {
            Username = string.Empty;
            Password = string.Empty;
        }
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
