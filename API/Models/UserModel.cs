namespace StudentHub.Models
{
	public class UserModel
	{
		public string Username { get; set; }
		public byte[] PasswordHash { get; set; }
		public byte[] PasswordSalt { get; set; }
		public UserRoles Role { get; set; } 
	}
}
