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
            return await _context.Users.AnyAsync((u) => u.email == email);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync((u) => u.email == email);
        }

        public async Task<User> GetByIdAsync(int? id)
        {
            if (id == null)
                throw new NotFoundException($"id not found", HttpStatusCode.NotFound);
            User user = await _context.Users.FirstOrDefaultAsync((u) => u.Id == id);
            if (user == null)
                throw new NotFoundException($"Manager With id : {id} not found",HttpStatusCode.NotFound);
            return user;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
