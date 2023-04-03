
using SwaggerSecurityTrimmingV1.Models.Base;

namespace SwaggerSecurityTrimmingV1.Models.Entities
{
    public class User : ModelBaseWithId
    {
        public User()
        {
            Roles = new List<string>();
        }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }

        public List<string> Roles { get; set; }

        public void AddRole(string roleName)
        {
            Roles.Add(roleName);
        }
    }
}
