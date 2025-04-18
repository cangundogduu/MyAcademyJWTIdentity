﻿using JWTIdentity.Entities;
using JWTIdentity.Models;
using JWTIdentity.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JWTIdentity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(UserManager<AppUser> _userManager,
                                 IJwtService _jwtService,
                                 RoleManager<AppRole> _roleManager) : ControllerBase
    {

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            var user = new AppUser
            {
                Name = model.Name,
                Surname = model.Surname,
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var roleExists = await _roleManager.RoleExistsAsync("Admin");
            if (!roleExists)
            {
                var role = new AppRole
                {
                    Name = "Admin"
                };
                await _roleManager.CreateAsync(role);
            }

            await _userManager.AddToRoleAsync(user, "Admin");
            return Ok("Kullanıcı kaydı başarılı");
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return BadRequest("Kullanıcı bulunamadı");
            }
            var result = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!result)
            {
                return BadRequest("Şifre hatalı");
            }

            var token = await _jwtService.CreateTokenAsync(user);
            return Ok(token);

        }
    }
}
