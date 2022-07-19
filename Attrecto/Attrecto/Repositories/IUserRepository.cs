using Attrecto.Data;

namespace Attrecto.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUserAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<User> CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        void DeleteUser(int id);
        Task SaveChangesAsync();
        bool CheckExistingEmail(string email);
    }
}
