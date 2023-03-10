using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentHub.Data;
using StudentHub.DTOs;
using StudentHub.Models;

namespace StudentHub.Controllers
{
	[Route("api/module/")]
	[ApiController]
	public class ModuleController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly DataContext _dataContext;

		public ModuleController(IConfiguration configuration, DataContext dataContext)
		{
			_configuration = configuration;
			_dataContext = dataContext;
		}

		[HttpPost("create"), Authorize(Roles = "Teacher")]
		public async Task<ActionResult<Module>> CreateModule(ModuleCreateDTO request)
		{
			var username = User.Identity.Name;
			var user = _dataContext.Users.FirstOrDefault(u => u.Username == username);

			Module module = new Module();

			module.ModuleId = Guid.NewGuid();
			module.Name = request.Name;
			module.Owner = user;

			_dataContext.Add(module);
			await _dataContext.SaveChangesAsync();

			return Ok(module);
		}

		[HttpPost("join"), Authorize(Roles = "Student")]
		public async Task<ActionResult> JoinModule(ModuleJoinDTO request)
		{
			var username = User.Identity.Name;
			var user = _dataContext.Users.FirstOrDefault(u => u.Username == username);

			var module = _dataContext.Modules.FirstOrDefault(m => m.ModuleId == request.ModuleId);

			if (module == null)
			{
				return BadRequest("Couldn't find module.");
			}

			var userInModule = _dataContext.ModuleUser.FirstOrDefault(mu => mu.Module == module && mu.User == user);

			if (userInModule != null)
			{
				return BadRequest("User already in module.");
			}

			ModuleUser moduleUser = new ModuleUser()
			{
				User = user,
				UserId = user.UserId,
				Module = module,
				ModuleId = module.ModuleId
			};

			_dataContext.Add(moduleUser);
			await _dataContext.SaveChangesAsync();

			return Ok();
		}

		[HttpPost("create-section"), Authorize(Roles = "Teacher")]
		public async Task<ActionResult> CreateSection(SectionCreateDTO request)
		{
			var module = _dataContext.Modules.FirstOrDefault(m => m.ModuleId == request.ModuleId);

			if (module == null)
			{
				return BadRequest("Couldn't find module.");
			}

			var username = User.Identity.Name;
			var user = _dataContext.Users.FirstOrDefault(u => u.Username == username);

			if (module.Owner != user)
			{
				return BadRequest("User does not own module.");
			}

			Section section = new Section()
			{
				SectionId = Guid.NewGuid(),
				Title = request.Title,
				Body = request.Body,
				DateCreated = DateTime.UtcNow,
				DueDate = request.DueDate,
				Module = module
			};

			_dataContext.Add(section);
			await _dataContext.SaveChangesAsync();

			return Ok();
		}
	}
}
