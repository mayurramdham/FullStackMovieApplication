using App.Core.Interface;
using Domain.Entity;
using Microsoft.Extensions.Configuration;

using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;



namespace Infrastructure.Service
{
    internal class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> Authenticate(int userId, string userEmail,string userName, string Roles,string apiKey)
        {
            var issuer = _configuration["JwtConfig:Issuer"];
            var audience = _configuration["JwtConfig:Audience"];
            var key = _configuration["JwtConfig:Key"];
            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(120); // Token is valid for 30 minutes

            var tokenDescripter = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Email,userEmail),
                    new Claim("UserId",userId.ToString()),
                     new Claim("Name",userName),
                     new Claim("Email",userEmail),
                     new Claim("apiKey",apiKey),
                     new Claim(ClaimTypes.Role,Roles),

                }),
                Expires = tokenExpiryTimeStamp,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                    SecurityAlgorithms.HmacSha256Signature),


            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescripter);
            var accessToken = tokenHandler.WriteToken(securityToken);

            return accessToken;
        }

        public async Task<string> GenerateToken(int userId, string userEmail, string userName, string Roles, string apiKey)
        {
          
    
            var claims = new[]
           {
            new Claim(JwtRegisteredClaimNames.Email,userEmail),
            new Claim("UserId", userId.ToString()),
            new Claim("Name", userName),
            new Claim("Email", userEmail),
            new Claim("apiKey", apiKey),           
            new Claim(ClaimTypes.Role,Roles)
             };
    

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfig:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["JwtConfig:Issuer"],
                _configuration["JwtConfig:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
