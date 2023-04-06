using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentHub.Data;
using StudentHub.DTOs;
using StudentHub.Models;
using System.Reflection;

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
		public async Task<ActionResult<Models.Module>> CreateModule(ModuleCreateDTO request)
		{
			var username = User.Identity.Name;
			var user = _dataContext.Users.FirstOrDefault(u => u.Username == username);

			Models.Module module = new Models.Module();

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
				DueDate = DateTime.UtcNow.AddDays(7),
				Module = module
			};

			_dataContext.Add(section);
			await _dataContext.SaveChangesAsync();

			return Ok();
		}

		[HttpGet("{moduleId}"), Authorize]
		public async Task<IActionResult> GetModule(string moduleId)
		{
			var module = _dataContext.Modules.Include(m => m.Owner)
											 .Include(m => m.Sections).ToList()
											 .FirstOrDefault(m => m.ModuleId.ToString() == moduleId);

			if (module == null)
			{
				Response.StatusCode = StatusCodes.Status400BadRequest;
				return new JsonResult("Module not found.");

			}

			var username = User.Identity.Name;
			var user = _dataContext.Users.FirstOrDefault(u => u.Username == username);

			bool userInModule = _dataContext.ModuleUser.Any(mu => mu.ModuleId.ToString() == moduleId && mu.User == user);

			if (!userInModule && !(module.Owner == user))
			{
				Response.StatusCode = StatusCodes.Status400BadRequest;
				return new JsonResult("You are not in this module");
			}

			List<SectionGetDTO> sections = new();

			foreach (var section in module.Sections)
			{
				sections.Add(new SectionGetDTO()
				{
					Title = section.Title,
					Body = section.Body,
					DueDate = section.DueDate,
					DateCreated = section.DateCreated
				});
			}

			ModuleGetOneDTO moduleDto = new()
			{
				ModuleId = Guid.NewGuid(),
				Name = module.Name,
				Owner = module.Owner.Username,
				Sections = sections
			};

			Response.StatusCode = StatusCodes.Status200OK;
			return new JsonResult(moduleDto);
		}
	}
}
