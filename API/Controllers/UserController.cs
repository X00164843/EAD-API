using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentHub.Data;
using StudentHub.DTOs;

namespace StudentHub.Controllers
{
	[Route("api/user/")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly DataContext _dataContext;

		public UserController(IConfiguration configuration, DataContext dataContext)
		{
			_configuration = configuration;
			_dataContext = dataContext;
		}

		[HttpGet("joined-modules"), Authorize(Roles = "Student")]
		public async Task<ActionResult> GetJoinedModules()
		{
			var username = User.Identity.Name;
			var user = _dataContext.Users.FirstOrDefault(u => u.Username == username);

			var moduleIds = _dataContext.ModuleUser
												.Where(mu => mu.UserId == user.UserId)
												.Select(mu => mu.ModuleId);

			IEnumerable<ModuleGetAllDTO> modules = _dataContext.Modules
																	.Where(m => moduleIds.Contains(m.ModuleId))
																	.Select(m => new ModuleGetAllDTO { ModuleId = m.ModuleId, Name = m.Name });

			return Ok(modules);
		}

		[HttpGet("owned-modules"), Authorize(Roles = "Teacher")]
		public async Task<ActionResult> GetOwnedModules()
		{
			var username = User.Identity.Name;
			var user = _dataContext.Users.FirstOrDefault(u => u.Username == username);

			IEnumerable<ModuleGetAllDTO> modules = _dataContext.Modules
																	.Where(m => m.Owner == user)
																	.Select(m => new ModuleGetAllDTO { ModuleId = m.ModuleId, Name = m.Name });

			return Ok(modules);
		}
	}
}
