using Azure;
using hrms.Dto.Request.Authentication;
using hrms.Model;
using hrms.Repository;
using hrms.Utility;
using hrms.CustomException;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Net;

namespace hrms.Service.impl
{
    public class AuthenticationService : IAuthenticationService
    {

        public readonly IUserRepository _repo;
        private readonly IConfiguration _config;

        public AuthenticationService(IUserRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }
        public async Task<string> Login(LoginRequestDto dto)
        {
            User user = await _repo.GetByEmailAsync(dto.email);
            if (user == null)
                throw new NotFoundException($"user with email {dto.email} not found !",HttpStatusCode.NotFound);

            if (!PasswordHelper.Verify(dto.password, user.hash_password))
                throw new InvalidOperationCustomException("Invalid Password !");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.email),
                new Claim(ClaimTypes.Role, user.user_role.ToString())
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

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task RegisterNewUser(RegisterRequestDto dto)
        {
            if (await _repo.ExistsByEmailAsync(dto.email))
                throw new BadHttpRequestException($"User Exists With Email {dto.email}");

            User user = CreateUser(dto).Result;
            await _repo.AddAsync(user);
            await _repo.SaveChangesAsync();
        }

        private async Task<User> CreateUser(RegisterRequestDto dto)
        {
            var user = new User
            {
                full_name = dto.full_name,
                image_url = dto.image_url,
                date_of_birth = dto.date_of_birth,
                date_of_join = dto.date_of_join,
                is_deleted = false,
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };

            switch (dto.user_role)
            {
                case "ADMIN":
                    user.user_role = UserRole.ADMIN;
                    break;
                case "MANAGER":
                    user.user_role = UserRole.MANAGER;
                    break;
                case "EMPLOYEE":
                    user.user_role = UserRole.EMPLOYEE;
                    break;
                case "HR":
                    user.user_role = UserRole.HR;
                    break;
            }

            if (dto.manage_id != null)
            {
                User manager = await _repo.GetByIdAsync(dto.manage_id);
                user.manager = manager;
                user.manager_id = manager.Id;
            }

            user.hash_password = PasswordHelper.HashPassword(dto.password);
            return user;
        }
    }
}
