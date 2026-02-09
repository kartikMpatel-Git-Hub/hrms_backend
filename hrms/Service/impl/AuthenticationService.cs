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
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using AutoMapper;
using hrms.Dto.Response.User;

namespace hrms.Service.impl
{
    public class AuthenticationService : IAuthenticationService
    {

        public readonly IUserRepository _repo;
        private readonly IConfiguration _config;
        private readonly IEmailService _email;
        private readonly IMapper _mapper;

        public AuthenticationService(IMapper mapper,IUserRepository repo, IConfiguration config,IEmailService email)
        {
            _mapper = mapper;
            _repo = repo;
            _config = config;
            _email = email;
        }
        public async Task<string> Login(LoginRequestDto dto)
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
                new Claim(ClaimTypes.Role, user.Role.ToString())
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
                Image = dto.Image,
                DateOfBirth= dto.DateOfBirth,
                DateOfJoin = dto.DateOfJoin,
                is_deleted = false,
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
                user.Manager = manager;
                user.ManagerId = manager.Id;
            }

            user.HashPassword = PasswordHelper.HashPassword(dto.Password);
            return user;
        }
    }
}
