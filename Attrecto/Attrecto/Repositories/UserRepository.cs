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

        public async Task<User> CreateUserAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentException("User cannot be null");
            }
            var basicRole = _context.Roles.FirstOrDefault(x => x.Name.Equals("Basic"));
            user.FkRoleNavigation = basicRole;
            var result = await _context.Users.AddAsync(user);
            return result.Entity;
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await SaveChangesAsync();
        }

        public void DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.IdUser == id);
            if (user == null)
                throw new ArgumentException("User not found");
            else
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
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
