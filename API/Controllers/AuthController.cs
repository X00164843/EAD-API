using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
		public static UserModel user  = new UserModel();
		private readonly IConfiguration _configuration;

		public AuthController(IConfiguration configuration) 
		{
			_configuration = configuration;
		}

		[HttpPost("register")]
		public async Task<ActionResult<UserModel>> Register(UserDTO request)
		{
			CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

			user.Username = request.Username;
			user.PasswordHash = passwordHash;
			user.PasswordSalt = passwordSalt;
			user.Role = request.Role;

			return Ok(user);
		}

		[HttpPost("login")]
		public async Task<ActionResult<string>> Login(UserDTO request)
		{
			if (user.Username != request.Username)
			{
				return BadRequest("User not found");
			}

			if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
			{
				return BadRequest("Wrong password");
			}

			string token = CreateToken(user);

			return Ok(token);
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

		private string CreateToken(UserModel user)
		{
			List<Claim> claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.Username),
				new Claim(ClaimTypes.Role, user.Role.ToString())
			};

			var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetConnectionString("Token")));

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

			var token = new JwtSecurityToken(
				claims: claims,
				expires: DateTime.Now.AddDays(1),
				signingCredentials: creds
			);

			var jwt = new JwtSecurityTokenHandler().WriteToken(token);

			return jwt;
		}
	}
}
