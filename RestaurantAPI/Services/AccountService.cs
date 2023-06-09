﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace RestaurantAPI.Services
{
    public interface IAccountService
    {
        void RegisterUser(RegisterUserDto dto);
        public string GenerateJwt(LoginDto dto);
    }

    public class AccountService : IAccountService
    {
        private readonly AuthenticationSettings _authenticationSetting;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly RestaurantDbContext _context;

        public AccountService(RestaurantDbContext context, IPasswordHasher<User> passwordHasher,AuthenticationSettings authenticationSettings)
        {
            _authenticationSetting=authenticationSettings;
            _passwordHasher = passwordHasher;
            _context = context;

        }
        public void RegisterUser(RegisterUserDto dto)
        {
            var newUser = new User()
            {
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                Nationality = dto.Nationality,
                RoleId = dto.RoleId,
            };

            var hashPassword= _passwordHasher.HashPassword(newUser, dto.Password);
            newUser.PasswordHash = hashPassword;
            _context.Users.Add(newUser);
            _context.SaveChanges();

        }

        public string GenerateJwt(LoginDto dto)
        {
            var user = _context.Users
                .Include(u=> u.Role)
                .FirstOrDefault(u => u.Email == dto.Email);

            if (user is null)
                throw new BadRequestException("Invalid username or password");
            

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if(result==PasswordVerificationResult.Failed)
                throw new BadRequestException("Invalid username or password");

            var claim = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.Role.Name),
                new Claim("dateOfBirth", user.DateOfBirth.Value.ToString("yyyy-MM-dd")),
                new Claim("nationality", user.Nationality)
            };

            var key =new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSetting.JwtKey));
            var cred= new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_authenticationSetting.JwtExpireDays);

            var token = new JwtSecurityToken(_authenticationSetting.JwtIssuer,
                _authenticationSetting.JwtIssuer,
                claim,
                expires: expires,
                signingCredentials:cred);

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);

        }
    }
}
