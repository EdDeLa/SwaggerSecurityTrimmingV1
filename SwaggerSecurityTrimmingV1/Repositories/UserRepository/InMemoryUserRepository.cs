using SwaggerSecurityTrimmingV1.Models.Entities;

namespace SwaggerSecurityTrimmingV1.Repositories.UserRepositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly List<User> _users = new();

        public Task<User> Create(User user)
        {
            user.Id = Guid.NewGuid();

            _users.Add(user);

            return Task.FromResult(user);
        }

        public Task<User?> GetById(Guid id)
        {
            Task<User?> foundUser = Task.FromResult(_users.FirstOrDefault(user => user.Id != Guid.Empty && user.Id.Equals(id)));
            return foundUser;
        }

        public Task<User?> GetByEmail(string email)
        {
            Task<User?> foundUser = Task.FromResult(_users.FirstOrDefault(user => !string.IsNullOrEmpty(user.Email) && user.Email.Equals(email)));
            return foundUser;
        }

        public Task<User?> FindByNameAsync(string username)
        {
            Task<User?> foundUser = Task.FromResult(_users.FirstOrDefault(user => !string.IsNullOrEmpty(user.UserName) && user.UserName.Equals(username)));
            return foundUser;
        }

        public Task<bool> CheckPasswordAsync(User user, string password)
        {
            Task<bool> passwordCorrent = Task.FromResult(false);

            passwordCorrent = Task.FromResult(user.Password.Equals(password));

            return passwordCorrent;
        }

        public Task<IList<string>> GetRolesAsync(User user)
        {
         return Task.FromResult((IList<string>)user.Roles);
        }

    }
}
