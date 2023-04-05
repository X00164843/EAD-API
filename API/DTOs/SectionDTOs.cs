namespace StudentHub.DTOs
{
	public class SectionGetDTO
	{
		public string Title { get; set; }
		public string Body { get; set; }
		public DateTime DueDate { get; set; }
		public DateTime DateCreated { get; set; }
	}

	public class SectionCreateDTO
	{
		public string Title { get; set; }
		public string Body { get; set; }
		public DateTime DueDate { get; set; }
		public Guid ModuleId { get; set; }
	}
}
