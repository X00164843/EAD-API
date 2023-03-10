using StudentHub.Models;

namespace StudentHub.DTOs
{
	public class SectionCreateDTO
	{
		public string Title { get; set; }
		public string Body { get; set; }
		public DateTime DueDate { get; set; }
		public Guid ModuleId { get; set; }
	}
}
