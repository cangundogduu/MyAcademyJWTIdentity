﻿using JWTIdentity.Entities;
using jwt = JWTIdentity.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace JWTIdentity.Services
{
    public class JwtService : IJwtService
    {
        private readonly jwt.TokenOptions _tokenOptions;
        private readonly UserManager<AppUser> _userManager;
        public JwtService(IOptions<jwt.TokenOptions> tokenOptions, UserManager<AppUser> userManager)
        {
            _tokenOptions = tokenOptions.Value;
            _userManager = userManager;
        }
        public async Task<string> CreateTokenAsync(AppUser user)
        {
            SymmetricSecurityKey symmetricSecurityKey = new(Encoding.UTF8.GetBytes(_tokenOptions.Key));

            var userRoles = await _userManager.GetRolesAsync(user);


            List<Claim> claims = new()
           {
               new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
               new Claim(ClaimTypes.Name, user.UserName),
               new Claim(ClaimTypes.Role, userRoles.FirstOrDefault()),
               new Claim(ClaimTypes.Email, user.Email),
               new Claim("fullName", String.Join("",user.Name,user.Surname))
           };

            JwtSecurityToken jwtSecurityToken = new(
                issuer: _tokenOptions.Issuer,
                audience: _tokenOptions.Audience,
                expires: DateTime.Now.AddMinutes(_tokenOptions.ExpireInMinutes),
                claims: claims,
                notBefore: DateTime.Now,
                signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256)
            );

            JwtSecurityTokenHandler handler = new();

            var token= handler.WriteToken(jwtSecurityToken);

            return token;
        }
    }
}
