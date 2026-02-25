using hrms.Data;
using hrms.Model;
using Microsoft.EntityFrameworkCore;

namespace hrms.Utility
{
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;

        public DataSeeder(ApplicationDbContext context, ILogger<DataSeeder> logger)
        {
            _context = context;
        }

        public async Task SeedDataAsync()
        {
            try
            {
                await SeedDepartmentsAsync();
                await SeedExpenseCategoriesAsync();
                await SeedUsersAsync();
            }
            catch (Exception ex)
            {
            }
        }

        private async Task SeedDepartmentsAsync()
        {
            if (_context.Departments.Any())
            {
                return;
            }

            var departments = new List<Department>
            {
                new Department { DepartmentName = "Administration", is_deleted = false, created_at = DateTime.Now, updated_at = DateTime.Now },
                new Department { DepartmentName = "Human Resources", is_deleted = false, created_at = DateTime.Now, updated_at = DateTime.Now },
                new Department { DepartmentName = "Information Technology", is_deleted = false, created_at = DateTime.Now, updated_at = DateTime.Now },
                new Department { DepartmentName = "Finance", is_deleted = false, created_at = DateTime.Now, updated_at = DateTime.Now },
                new Department { DepartmentName = "Operations", is_deleted = false, created_at = DateTime.Now, updated_at = DateTime.Now },
                new Department { DepartmentName = "Marketing", is_deleted = false, created_at = DateTime.Now, updated_at = DateTime.Now },
                new Department { DepartmentName = "Sales", is_deleted = false, created_at = DateTime.Now, updated_at = DateTime.Now }
            };

            await _context.Departments.AddRangeAsync(departments);
            await _context.SaveChangesAsync();
        }

        private async Task SeedExpenseCategoriesAsync()
        {
            if (_context.ExpenseCategories.Any())
            {
                return;
            }

            var categories = new List<ExpenseCategory>
            {
                new ExpenseCategory { Category = "Travel", is_deleted = false },
                new ExpenseCategory { Category = "Food & Dining", is_deleted = false },
                new ExpenseCategory { Category = "Accommodation", is_deleted = false },
                new ExpenseCategory { Category = "Transportation", is_deleted = false },
                new ExpenseCategory { Category = "Office Supplies", is_deleted = false },
                new ExpenseCategory { Category = "Equipment", is_deleted = false },
                new ExpenseCategory { Category = "Training & Development", is_deleted = false },
                new ExpenseCategory { Category = "Utilities", is_deleted = false }
            };

            await _context.ExpenseCategories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();
        }

        private async Task SeedUsersAsync()
        {
            var defaultPassword = "P@ssw0rd.018";
            
            var ceoExists = await _context.Users.AnyAsync(u => u.Email == "ceo@gmail.com");
            User? ceo = null;
            if (!ceoExists)
            {
                ceo = new User
                {
                    FullName = "CEO Admin",
                    Email = "ceo@gmail.com",
                    HashPassword = PasswordHelper.HashPassword(defaultPassword),
                    Role = UserRole.ADMIN,
                    DateOfJoin = DateTime.Now,
                    Image = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRCS2wb0ixNEu-qFWrF9k1ml03x2jJ6Fc_eKA&s",
                    Designation = "Chief Executive Officer",
                    ReportTo = null,
                    is_deleted = false,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };
                await _context.Users.AddAsync(ceo);
                await _context.SaveChangesAsync();
            }
            else
            {
                ceo = await _context.Users.FirstOrDefaultAsync(u => u.Email == "ceo@gmail.com");
            }

            var vpExists = await _context.Users.AnyAsync(u => u.Email == "vp@gmail.com");
            User? vp = null;
            if (!vpExists && ceo != null)
            {
                vp = new User
                {
                    FullName = "VP Admin",
                    Email = "vp@gmail.com",
                    HashPassword = PasswordHelper.HashPassword(defaultPassword),
                    Role = UserRole.ADMIN,
                    DateOfJoin = DateTime.Now,
                    Image = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRCS2wb0ixNEu-qFWrF9k1ml03x2jJ6Fc_eKA&s",
                    Designation = "Vice President",
                    ReportTo = ceo.Id,
                    is_deleted = false,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };
                await _context.Users.AddAsync(vp);
                await _context.SaveChangesAsync();
            }
            else if (vpExists)
            {
                vp = await _context.Users.FirstOrDefaultAsync(u => u.Email == "vp@gmail.com");
            }

            var hrExists = await _context.Users.AnyAsync(u => u.Email == "hr@gmail.com");
            if (!hrExists && vp != null)
            {
                var hr = new User
                {
                    FullName = "HR Manager",
                    Email = "hr@gmail.com",
                    HashPassword = PasswordHelper.HashPassword(defaultPassword),
                    Role = UserRole.HR,
                    DateOfJoin = DateTime.Now,
                    Image = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRCS2wb0ixNEu-qFWrF9k1ml03x2jJ6Fc_eKA&s",
                    Designation = "Human Resources Manager",
                    ReportTo = vp.Id,
                    is_deleted = false,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };
                await _context.Users.AddAsync(hr);
                await _context.SaveChangesAsync();
            }
            else if (hrExists)
            {
            }

            var systemExists = await _context.Users.AnyAsync(u => u.Email == "system@gmail.com");
            if (!systemExists)
            {
                var system = new User
                {
                    FullName = "System Admin",
                    Email = "system@gmail.com",
                    HashPassword = PasswordHelper.HashPassword(defaultPassword),
                    Role = UserRole.ADMIN,
                    DateOfJoin = DateTime.Now,
                    Image = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRCS2wb0ixNEu-qFWrF9k1ml03x2jJ6Fc_eKA&s",
                    Designation = "System Administrator",
                    ReportTo = null,
                    is_deleted = false,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };
                await _context.Users.AddAsync(system);
                await _context.SaveChangesAsync();
            }
            else
            {
            }
        }
    }
}
