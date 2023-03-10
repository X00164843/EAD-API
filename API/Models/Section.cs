using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace StudentHub.Models
{
	public class Section
	{
		[Key]
		public Guid SectionId { get; set; }
		public string Title { get; set; }
		public string Body { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime DueDate { get; set; }
		public Module Module { get; set; }
	}
}
