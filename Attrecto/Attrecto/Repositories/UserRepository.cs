using Attrecto.Data;
using Microsoft.EntityFrameworkCore;

namespace Attrecto.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AttrectoTestDbContext _context;

        public UserRepository(AttrectoTestDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUserAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.IdUser == id);
        }

        public async Task CreateUserAsync(User user)
        {
            if(user == null)
            {
                throw new ArgumentException("User cannot be null");
            }
            var basicRole = _context.Roles.FirstOrDefault(x => x.Name.Equals("Basic"));
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.FkRoleNavigation = basicRole;
            await _context.Users.AddAsync(user);
        }

        public void DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.IdUser == id);
            if (user != null)
                _context.Users.Remove(user);
            else
                throw new ArgumentException("User not found");
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public bool CheckExistingEmail(string email)
        {
            return _context.Users.FirstOrDefault(x => x.Email.Equals(email)) != null;
        }
    }
}
