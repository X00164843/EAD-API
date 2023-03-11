namespace StudentHub.DTOs
{
	public class ModuleGetOneDTO
	{
		public Guid ModuleId { get; set; }
		public string Name { get; set; }
		public string Owner { get; set; }
		public IEnumerable<SectionGetDTO> Sections { get; set; }
	}
}
