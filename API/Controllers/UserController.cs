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

		[HttpGet("module/{moduleId}"), Authorize]
		public async Task<ActionResult> GetModule(string moduleId)
		{
			var module = _dataContext.Modules.FirstOrDefault(m => m.ModuleId.ToString() == moduleId);

			if (module == null)
			{
				return BadRequest("Module not found.");
			}

			var username = User.Identity.Name;
			var user = _dataContext.Users.FirstOrDefault(u => u.Username == username);

			bool userInModule = _dataContext.ModuleUser.Any(mu => mu.ModuleId.ToString() == moduleId && mu.User == user);

			if (!userInModule && !(module.Owner == user))
			{
				return BadRequest("You are not in this module");
			}

			List<SectionGetDTO> sections = new ();

			if (module.Sections != null)
			{
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
			}

			//TODO  Owner = module.Owner.Username giving null pointer exceptions
			ModuleGetOneDTO moduleDto = new()
			{
				ModuleId = Guid.NewGuid(),
				Name = module.Name,
				Owner = module.Owner.Username,
				Sections = sections
			};

			return Ok(moduleDto);
		}
	}
}
