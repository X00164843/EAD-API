using System.ComponentModel.DataAnnotations;

namespace StudentHub.Models
{
	public class Module
	{
		[Key]
		public Guid ModuleId { get; set; }
		public string Name { get; set; }
		public User Owner { get; set; }
		public ICollection<ModuleUser> ModuleUsers { get; set; }
		public ICollection<Section> Sections { get; set; }
	}
}
