namespace StudentHub.DTOs
{
	public class ModuleCreateDTO
	{
		public string Name { get; set; }
	}

	public class ModuleGetAllDTO
	{
		public Guid ModuleId { get; set; }
		public string Name { get; set; }
	}

	public class ModuleGetOneDTO
	{
		public Guid ModuleId { get; set; }
		public string Name { get; set; }
		public string Owner { get; set; }
		public IEnumerable<SectionGetDTO> Sections { get; set; }
	}

	public class ModuleJoinDTO
	{
		public Guid ModuleId { get; set; }
	}
}
