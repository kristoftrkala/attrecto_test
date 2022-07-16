﻿using Attrecto.Data;

namespace Attrecto.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUserAsync();
        Task<User> GetUserByIdAsync(int id);
        Task CreateUserAsync(User user);
        void DeleteUser(User user);
        Task SaveChangesAsync();
        bool CheckExistingEmail(string email);
    }
}