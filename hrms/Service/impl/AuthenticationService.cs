using AutoMapper;
using hrms.CustomException;
using hrms.Dto.Request.Authentication;
using hrms.Dto.Response.Authentication;
using hrms.Dto.Response.User;
using hrms.Model;
using hrms.Repository;
using hrms.Utility;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace hrms.Service.impl
{
    public class AuthenticationService : IAuthenticationService
    {

        public readonly IUserRepository _repo;
        private readonly IConfiguration _config;
        private readonly IEmailService _email;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinary;
        private readonly IDepartmentRepository _departmentRepo;

        public AuthenticationService(IMapper mapper,IUserRepository repo, IConfiguration config,IEmailService email,ICloudinaryService cloudinary, IDepartmentRepository departmentRepo)
        {
            _mapper = mapper;
            _repo = repo;
            _config = config;
            _email = email;
            _cloudinary = cloudinary;
            _departmentRepo = departmentRepo;
        }

        public async Task ForgetPassword(ForgetPasswordRequestDto dto)
        {
            string newPassword = PasswordHelper.GenerateRandomPassword();
            User user = await _repo.GetByEmailAsync(dto.Email);
            if (user == null)
                throw new NotFoundCustomException($"user with email {dto.Email} not found !");
            user.HashPassword = PasswordHelper.HashPassword(newPassword);
            await _repo.UpdateAsync(user);
            await _email.SendEmailAsync(dto.Email, "Password Reset", $"<h1>Your New Password is : {newPassword} </h1>");
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto dto)
        {
            User user = await _repo.GetByEmailAsync(dto.Email);
            if (user == null)
                throw new NotFoundCustomException($"user with email {dto.Email} not found !");

            if (!PasswordHelper.Verify(dto.Password, user.HashPassword))
                throw new InvalidOperationCustomException("Invalid Password !");

            var claims = new[]
            {
                new Claim(ClaimTypes.PrimarySid,user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(

                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.TryParse(_config["Jwt:ExpiresInMinutes"], out var m) ? m : 60),
                signingCredentials: creds

            );

            string generatedToken = new JwtSecurityTokenHandler().WriteToken(token);
            LoginResponseDto response = new LoginResponseDto()
            {
                id = user.Id,
                email = user.Email,
                role = user.Role.ToString(),
                token = generatedToken,
                image = user.Image
            };
            return response;
        }

        
        public async Task<UserResponseDto> RegisterNewUser(RegisterRequestDto dto)
        {
            if (await _repo.ExistsByEmailAsync(dto.Email))
                throw new ExistsCustomException($"User Exists With Email {dto.Email}");

            User user = CreateUser(dto).Result;
            User response = await _repo.AddAsync(user);
            await _email.SendEmailAsync(dto.Email, "registration Confirmation", "<h1>Congratulations, Your have successfully complete Registration !</h1>");
            return _mapper.Map<UserResponseDto>(response);
        }

        private async Task<User> CreateUser(RegisterRequestDto dto)
        {
            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Image = dto.Image != null ? await _cloudinary.UploadAsync(dto.Image) : "",
                DateOfBirth= dto.DateOfBirth,
                DateOfJoin = dto.DateOfJoin,
                is_deleted = false,
                Designation = dto.Designation,
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };
            if(dto.Role != "ADMIN" && 
                dto.Role != "MANAGER" && 
                dto.Role != "EMPLOYEE" && 
                dto.Role != "HR")
            {
                throw new NotFoundCustomException($"User Role {dto.Role} Not Exists !");
            }

            switch (dto.Role)
            {
                case "ADMIN":
                    user.Role = UserRole.ADMIN;
                    break;
                case "MANAGER":
                    user.Role = UserRole.MANAGER;
                    break;
                case "EMPLOYEE":
                    user.Role = UserRole.EMPLOYEE;
                    break;
                case "HR":
                    user.Role = UserRole.HR;
                    break;
            }

            if (dto.ManagerId != null)
            {
                User manager = await _repo.GetManagerByIdAsync(dto.ManagerId);
                if (manager == null)
                    throw new NotFoundCustomException($"Manager With manager_id : {dto.ManagerId} Not Found !");
                user.Reported = manager;
                user.ReportTo = manager.Id;
            }

            if (dto.DepartmentId != null)
            {
                Department department = await _departmentRepo.GetDepartmentById((int)dto.DepartmentId);
                user.Department = department;
                user.DepartmentId = department.Id;
            }

            user.HashPassword = PasswordHelper.HashPassword(dto.Password);
            return user;
        }
    }
}
