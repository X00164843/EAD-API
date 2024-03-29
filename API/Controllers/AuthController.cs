﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentHub.Data;
using StudentHub.DTOs;
using StudentHub.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace StudentHub.Controllers
{
	[Route("api/auth/")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly DataContext _dataContext;

		public AuthController(IConfiguration configuration, DataContext dataContext) 
		{
			_configuration = configuration;
			_dataContext = dataContext;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register(UserRegisterDTO request)
		{
			var userExists = await _dataContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

			if (userExists != null)
			{
				Response.StatusCode = StatusCodes.Status400BadRequest;
				return new JsonResult("User already exists.");
			}

			User user = new User();

			CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

			user.UserId = Guid.NewGuid();
			user.Username = request.Username;
			user.PasswordHash = passwordHash;
			user.PasswordSalt = passwordSalt;
			user.Role = request.Role;

			_dataContext.Users.Add(user);
			await _dataContext.SaveChangesAsync();

			Response.StatusCode = StatusCodes.Status201Created;
			return new JsonResult("Account created successfully.");
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(UserSigninDTO request)
		{
			var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

			if (user == null)
			{
				Response.StatusCode = StatusCodes.Status400BadRequest;
				return new JsonResult("User not found.");
			}

			if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
			{
				Response.StatusCode = StatusCodes.Status400BadRequest;
				return new JsonResult("Wrong password.");
			}

			string token = CreateToken(user);

			UserSigninResponseDTO response = new UserSigninResponseDTO()
			{
				accessToken = token,
				role = user.Role
			};

			Response.StatusCode = StatusCodes.Status200OK;
			return new JsonResult(response);
		}

		private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
		{
			var hmac = new HMACSHA512();

			passwordSalt = hmac.Key;
			passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
		}

		private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
		{
			var hmac = new HMACSHA512(passwordSalt);
			var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

			return computedHash.SequenceEqual(passwordHash);
		}

		private string CreateToken(User user)
		{
			string role;

			if (user.Role == 0)
			{
				role = "Student";
			}
			else
			{
				role = "Teacher";
			}

			List<Claim> claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.Username),
				new Claim(ClaimTypes.Role, role)
			};

			var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetConnectionString("Token")));

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

			var token = new JwtSecurityToken(
				claims: claims,
				expires: DateTime.Now.AddDays(365),
				signingCredentials: creds
			);

			var jwt = new JwtSecurityTokenHandler().WriteToken(token);

			return jwt;
		}
	}
}
