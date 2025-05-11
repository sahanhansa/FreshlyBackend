using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FreshlyBackendNew.Data;
using FreshlyBackendNew.Models;
using System;
using Microsoft.AspNetCore.Identity;

namespace FreshlyBackendNew.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        


        public async Task<AuthResponse> LoginAsync(LoginData loginData)
        {
            try
            {
                var user = await _context.Auth.FirstOrDefaultAsync(u => u.username == loginData.Username);
                if (user == null || user.password != loginData.Password)
                {
                    return null;
                }

                string token = GenerateJwtToken(user.id.ToString(), user.username);

                return new AuthResponse
                {
                    Token = token,
                    Username = user.username,
                    UserId = user.id.ToString(),
                    Role = user.role

                };
                }
                catch(Exception ex)
                {
                return new AuthResponse
                {
                    Token = null,
                   Username = ex.ToString(),
                    UserId = "Error",
                    Role = "Error"
                };
            }
        }

        private string GenerateJwtToken(string userId, string username)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
