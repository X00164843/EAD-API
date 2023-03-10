using System.ComponentModel.DataAnnotations;

namespace StudentHub.Models
{
	public class User
	{
		[Key]
		public Guid UserId { get; set; }
		public string Username { get; set; }
		public byte[] PasswordHash { get; set; }
		public byte[] PasswordSalt { get; set; }
		public UserRoles Role { get; set; } 
		public ICollection<Module> OwnedModules { get; set; }
		public ICollection<ModuleUser> ModuleUsers { get; set; }
	}
}
