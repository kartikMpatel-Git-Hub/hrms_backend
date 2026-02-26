using hrms.CustomException;
using hrms.Data;
using hrms.Dto.Response.Other;
using hrms.Model;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace hrms.Repository.impl
{
    public class UserRepository(
        ApplicationDbContext _context,
        ILogger<UserRepository> _logger
        ) : IUserRepository
    {
        public async Task<User> AddAsync(User user)
        {
            var AddedEntity = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            var AddedUser = AddedEntity.Entity;
            _logger.LogInformation("Created User with Id {Id}", AddedUser.Id);
            foreach(var game in await _context.Games.ToListAsync())
            {
                var userGameState = new UserGameState()
                {
                    UserId = AddedUser.Id,
                    GameId = game.Id,
                    GamePlayed = 0,
                    LastPlayedAt = DateTime.Now
                };
                await _context.UserGameStates.AddAsync(userGameState);
                await _context.SaveChangesAsync();
                var userGameInterest = new UserGameInterest()
                {
                    UserId = AddedUser.Id,
                    GameId = game.Id,
                    Status = InterestStatus.NOTINTERESTED
                };
                await _context.UserGameInterests.AddAsync(userGameInterest);
                await _context.SaveChangesAsync();
            }
            return AddedUser;

        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync((u) => u.Email == email);
        }

        public async Task<PagedReponseOffSet<User>> GetAll(int PageSize, int PageNumber)
        {
            var TotalRecords = await _context.Users.Where(u => !u.is_deleted).CountAsync();
            _logger.LogInformation("Fetching all users, total: {Total}, page {Page}", TotalRecords, PageNumber);
            List<User> users = await _context.Users
                .OrderBy(u => u.Id)
                .Where(u => !u.is_deleted)
                .OrderBy(u => u.created_at)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .Include(u => u.Department)
                .ToListAsync();
            PagedReponseOffSet<User> Response = new PagedReponseOffSet<User>(users, PageNumber, PageSize, TotalRecords);
            return Response;
        }

        public async Task<List<User>> GetAllEmployee(int pageSize, int pageNumber)
        {
            var TotalRecords = await _context.Users.Where(u => !u.is_deleted && u.Role == UserRole.EMPLOYEE).CountAsync();
            _logger.LogInformation("Fetching all employees, total: {Total}, page {Page}", TotalRecords, pageNumber);
            List<User> users = await _context.Users
                .OrderBy(u => u.Id)
                .Where(u => !u.is_deleted && u.Role == UserRole.EMPLOYEE)
                .OrderByDescending(u => u.created_at)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return users;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            User user = await _context.Users.FirstOrDefaultAsync((u) => u.Email == email);
            if (user == null)
                throw new NotFoundCustomException($"User With Email : {email} not found");
            _logger.LogInformation("Fetched User by Email {Email}", email);
            return user;
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
            User user = await _context.Users
                .FirstOrDefaultAsync(
                (u) => u.Id == employeeId && u.Role == UserRole.EMPLOYEE);
            if (user == null)
                throw new NotFoundCustomException($"Employee With id : {employeeId} not found");
            return user;
        }

        public async Task<List<User>> GetEmployeesByName(string s)
        {
            List<User> employees
                = await _context.Users
                .Where(u => u.FullName.Contains(s)
                && u.Role == UserRole.EMPLOYEE
                && u.is_deleted == false)
                .Skip(0)
                .Take(10)
                .ToListAsync();
            return employees;
        }
        public async Task<List<User>> GetUserByKey(string s)
        {
            List<User> employees
                = await _context.Users
                .Where(u => (u.FullName.Contains(s) || u.Email.Contains(s))
                && u.is_deleted == false)
                .OrderBy(u => u.created_at)
                .Skip(0)
                .Take(10)
                .Include(u => u.Department)
                .ToListAsync();
            return employees;
        }

        public async Task<User> GetHrById(int hrId)
        {
            User user = await _context.Users
                .FirstOrDefaultAsync(
                (u) => u.Id == hrId && u.Role == UserRole.HR);
            if (user == null)
                throw new NotFoundCustomException($"HR With id : {hrId} not found");
            return user;
        }

        public async Task<User> GetManagerByIdAsync(int? managerId)
        {
            if (managerId == null)
                throw new NotFoundCustomException($"manager id not found");
            User user = await _context.Users
                .FirstOrDefaultAsync(
                (u) => u.Id == managerId && (u.Role == UserRole.MANAGER || u.Role == UserRole.ADMIN));
            if (user == null)
                throw new NotFoundCustomException($"Manager With id : {managerId} not found");
            return user;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<PagedReponseOffSet<User>> GetAllHrs(int pageSize, int pageNumber)
        {
            var TotalRecords = await _context.Users
                .Where(u => !u.is_deleted && u.Role == UserRole.HR).CountAsync();
            List<User> hrs = await _context.Users
                .Where(u => !u.is_deleted && u.Role == UserRole.HR)
                .OrderByDescending(u => u.created_at)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            PagedReponseOffSet<User> Response = new PagedReponseOffSet<User>(hrs, pageNumber, pageSize, TotalRecords);
            return Response;
        }

        public async Task<List<User>> GetHrByKey(string s)
        {
            List<User> employees
                = await _context.Users
                .Where(u => (u.FullName.Contains(s) || u.Email.Contains(s))
                && u.is_deleted == false && u.Role == UserRole.HR)
                .Skip(0)
                .Take(10)
                .ToListAsync();
            return employees;
        }

        public async Task<User> GetById(int id)
        {
            User user = await _context.Users
                .FirstOrDefaultAsync(
                (u) => u.Id == id);
            if (user == null)
                throw new NotFoundCustomException($"User With id : {id} not found");
            return user;
        }

        public async Task ToggleGameInterestStatus(int userId, int gameId)
        {
            UserGameInterest? interest = _context.UserGameInterests
                .FirstOrDefault(ugi => ugi.UserId == userId && ugi.GameId == gameId);

            if (interest == null)
            {
                interest = new UserGameInterest
                {
                    UserId = userId,
                    GameId = gameId,
                    Status = InterestStatus.INTERESTED
                };
                await _context.UserGameInterests.AddAsync(interest);
            }
            else
            {
                interest.Status = interest.Status == InterestStatus.INTERESTED ? InterestStatus.NOTINTERESTED : InterestStatus.INTERESTED;
                _context.UserGameInterests.Update(interest);
            }

            await _context.SaveChangesAsync();
        }

        public Task<PagedReponseOffSet<User>> GetAllUserForHr(int pageSize, int pageNumber)
        {
            var TotalRecords = _context.Users.Where(u => !u.is_deleted && (u.Role == UserRole.EMPLOYEE || u.Role == UserRole.MANAGER)).Count();
            _logger.LogInformation("Fetching users for HR, total: {Total}, page {Page}", TotalRecords, pageNumber);
            List<User> users = _context.Users
                .OrderByDescending(u => u.created_at)
                .Where(u => !u.is_deleted && (u.Role == UserRole.EMPLOYEE || u.Role == UserRole.MANAGER))
                .Include(u => u.Department)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            PagedReponseOffSet<User> Response = new PagedReponseOffSet<User>(users, pageNumber, pageSize, TotalRecords);
            return Task.FromResult(Response);
        }

        public Task<PagedReponseOffSet<User>> GetAllManagers(int pageSize, int pageNumber)
        {
            var TotalRecords = _context.Users.Where(u => !u.is_deleted && (u.Role == UserRole.MANAGER || u.Role == UserRole.ADMIN)).Count();
            _logger.LogInformation("Fetching all managers, total: {Total}, page {Page}", TotalRecords, pageNumber);
            List<User> users = _context.Users
                .OrderByDescending(u => u.created_at)
                .Where(u => !u.is_deleted && (u.Role == UserRole.MANAGER || u.Role == UserRole.ADMIN))
                .Include(u => u.Department)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            PagedReponseOffSet<User> Response = new PagedReponseOffSet<User>(users, pageNumber, pageSize, TotalRecords);
            return Task.FromResult(Response);
        }

        public Task<PagedReponseOffSet<User>> GetEmployeeUnderManager(int userId, int pageSize, int pageNumber)
        {
            var TotalRecords = _context.Users.Where(u => !u.is_deleted && u.ReportTo == userId).Count();
            _logger.LogInformation("Fetching employees under ManagerId {ManagerId}, total: {Total}", userId, TotalRecords);
            List<User> users = _context.Users
                .OrderByDescending(u => u.created_at)
                .Where(u => !u.is_deleted && u.ReportTo == userId)
                .Include(u => u.Department)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            PagedReponseOffSet<User> Response = new PagedReponseOffSet<User>(users, pageNumber, pageSize, TotalRecords);
            return Task.FromResult(Response);
        }

        public async Task<User> GetUserProfile(int userId)
        {
            User user = await _context.Users
                .Where(u => u.Id == userId && !u.is_deleted)
                .Include(u => u.Department)
                .Include(u => u.Reported)
                .Include(u => u.Employees)
                .FirstOrDefaultAsync();
            if(user == null)
            {
                throw new NotFoundCustomException($"User with id {userId} not found");
            }
            return user;
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated User with Id {Id}", user.Id);
        }
    }
}
