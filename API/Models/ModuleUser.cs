namespace StudentHub.Models
{
	public class ModuleUser
	{
		public Guid ModuleId { get; set; }
		public Module Module { get; set; }
		public Guid UserId { get; set; }
		public User User { get; set; }
	}
}
