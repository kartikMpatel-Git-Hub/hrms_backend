using hrms.CustomException;
using hrms.Data;
using hrms.Dto.Response.Other;
using hrms.Model;
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


        public async Task<User> AddAsync(User user)
        {
            var AddedEntity = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            var AddedUser = AddedEntity.Entity;
            return AddedUser;

        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync((u) => u.Email == email);
        }

        public async Task<PagedReponseOffSet<User>> GetAll(int PageSize,int PageNumber)
        {
            var TotalRecords = await _context.Users.Where(u => !u.is_deleted).CountAsync();
            Console.WriteLine($"Total User : {TotalRecords}");
            List<User> users = await _context.Users
                .OrderBy(u => u.Id)
                .Where(u => !u.is_deleted)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
            PagedReponseOffSet<User> Response = new PagedReponseOffSet<User>(users, PageNumber, PageSize, TotalRecords);
            return Response;
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

        public async Task<User> GetEmployeeById(int employeeId)
        {
            if (employeeId == null)
                throw new NotFoundCustomException($"employee id not found");
            User user = await _context.Users
                .FirstOrDefaultAsync(
                (u) => u.Id == employeeId && u.Role == UserRole.EMPLOYEE);
            if (user == null)
                throw new NotFoundCustomException($"Employee With id : {employeeId} not found");
            return user;
        }

        public async Task<User> GetManagerByIdAsync(int? managerId)
        {
            if (managerId == null)
                throw new NotFoundCustomException($"manager id not found");
            User user = await _context.Users
                .FirstOrDefaultAsync(
                (u) => u.Id == managerId && u.Role == UserRole.MANAGER);
            if (user == null)
                throw new NotFoundCustomException($"Manager With id : {managerId} not found");
            return user;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
