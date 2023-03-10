using Microsoft.EntityFrameworkCore;
using StudentHub.Models;

namespace StudentHub.Data
{
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions<DataContext> options) : base(options) { }

		public DbSet<UserModel> Users { get; set; }
	}
}
