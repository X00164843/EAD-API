using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using StudentHub.Models;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace StudentHub.Data
{
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions<DataContext> options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>()
				.HasMany(u => u.OwnedModules)
				.WithOne(m => m.Owner);

			modelBuilder.Entity<Module>()
				.HasMany(m => m.Sections)
				.WithOne(s => s.Module);

			modelBuilder.Entity<ModuleUser>()
				.HasKey(mu => new { mu.UserId, mu.ModuleId });

			modelBuilder.Entity<ModuleUser>()
				.HasOne(mu => mu.User)
				.WithMany(u => u.ModuleUsers)
				.HasForeignKey(mu => mu.UserId)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<ModuleUser>()
				.HasOne(mu => mu.Module)
				.WithMany(m => m.ModuleUsers)
				.HasForeignKey(mu => mu.ModuleId)
				.OnDelete(DeleteBehavior.NoAction);
		}

		public DbSet<User> Users { get; set; }
		public DbSet<Module> Modules { get; set; }
		public DbSet<Section> Sections { get; set; }
		public DbSet<ModuleUser> ModuleUser { get; set; }
	}
}
