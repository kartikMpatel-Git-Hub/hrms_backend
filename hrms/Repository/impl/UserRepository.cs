using hrms.Data;
using hrms.Model;
using hrms.CustomException;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace hrms.Repository.impl
{
    public class UserRepository : IUserRepository
    {

        public readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            this._context = context;
        }


        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync((u) => u.Email == email);
        }

        public async Task<List<User>> GetAll()
        {
            return await _context.Users
                .Where(u => !u.is_deleted)
                .ToListAsync();
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync((u) => u.Email == email);
        }

        public async Task<User> GetByIdAsync(int? id)
        {
            if (id == null)
                throw new NotFoundCustomException($"id not found");
            User user = await _context.Users.FirstOrDefaultAsync((u) => u.Id == id);
            if (user == null)
                throw new NotFoundCustomException($"Manager With id : {id} not found");
            return user;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
