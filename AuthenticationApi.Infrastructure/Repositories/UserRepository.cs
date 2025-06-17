using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace AuthenticationApi.Infrastructure.Repositories
{
    internal class UserRepository(AuthenticationDbContext context, IConfiguration config) : IUser
    {
        private async Task<AppUser?> GetAppUserByEmailAsync(string email)
        {
            var user = await context.AppUsers.FirstOrDefaultAsync(u => u.Email == email);
            return user is null ? null! : user;
        }
        public async Task<GetUserDTO?> GetAppUserAsync(int id)
        {
            var user = await context.AppUsers.FindAsync(id);
            return user is not null ? new GetUserDTO(
                id,
                user.UserName!,
                user.Email!,
                user.Address!,
                user.City!,
                user.Province!,
                user.ZipCode!,
                user.Country!,
                user.TelephoneNumber!
            ) : null;
        }

        public async Task<Response> LoginAsync(LoginDTO loginDTO)
        {
            var getUser = await GetAppUserByEmailAsync(loginDTO.Email);
            if (getUser is null)
            {
                return new Response(false,$"User does not exist with email {loginDTO.Email}");
            }
            if (!BCrypt.Net.BCrypt.Verify(loginDTO.Password, getUser.Password))
            {
                return new Response(false,"Invalid password");
            }
            string token = GenerateToken(getUser);
            return new Response(true, token);
        }

        private string GenerateToken(AppUser user)
        {
            var key = Encoding.UTF8.GetBytes(config.GetSection("Authentication:Key").Value!);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
            };
            
            if(!string.IsNullOrEmpty(user.Role) || !Equals(user.Role, "string"))
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role!));
            }
            var token = new JwtSecurityToken(
                issuer: config["Authentication:Issuer"],
                audience: config["Authentication:Audience"],
                claims: claims,
                expires: null,
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Response> RegisterAsync(AppUserDTO userDTO)
        {
            var getUser = await GetAppUserByEmailAsync(userDTO.Email);
            if (getUser is not null)
            {
                return new Response(false, $"User already exists with email {userDTO.Email}");
            }
            var user = new AppUser
            {
                UserName = userDTO.UserName,
                Email = userDTO.Email,
                Address = userDTO.Address,
                City = userDTO.City,
                Province = userDTO.Province,
                ZipCode = userDTO.ZipCode,
                Country = userDTO.Country,
                TelephoneNumber = userDTO.TelephoneNumber,
                Password = BCrypt.Net.BCrypt.HashPassword(userDTO.Password),
                Role = userDTO.Role
            };
            var result = context.AppUsers.Add(user);
            await context.SaveChangesAsync();
            return result.Entity.Id > 0 ? new Response(true, "User registered successfully") : new Response(false, "User registration failed");
        }
    }
}
