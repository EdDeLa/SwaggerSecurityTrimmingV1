using SwaggerSecurityTrimmingV1.Models.Entities;

namespace SwaggerSecurityTrimmingV1.Repositories.UserRepositories
{
    public interface IUserRepository
    {
        Task<User> Create(User user);
        Task<User?> FindByNameAsync(string name);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<IList<string>> GetRolesAsync(User user);
    }
}
