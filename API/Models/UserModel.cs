using System.ComponentModel.DataAnnotations;

namespace StudentHub.Models
{
	public class UserModel
	{
		[Key]
		public Guid Id { get; set; }
		public string Username { get; set; }
		public byte[] PasswordHash { get; set; }
		public byte[] PasswordSalt { get; set; }
		public UserRoles Role { get; set; } 
	}
}
