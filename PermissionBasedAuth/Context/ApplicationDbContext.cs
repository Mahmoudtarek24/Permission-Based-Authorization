using Microsoft.EntityFrameworkCore;
using PermissionBasedAuth.Domain;

namespace PermissionBasedAuth.Context
{
	public class ApplicationDbContext :DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			
			modelBuilder.Entity<User>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.HasIndex(e => e.Username).IsUnique();
				entity.HasIndex(e => e.Email).IsUnique();
				entity.Property(e => e.Email).HasMaxLength(100);
				entity.Property(e => e.Username).HasMaxLength(100);
			});

			modelBuilder.Entity<Role>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.HasIndex(e => e.Name).IsUnique();
				entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
			});

			modelBuilder.Entity<Permission>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.HasIndex(e => e.Name).IsUnique();
				entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
				entity.Property(e => e.Module).HasMaxLength(50).IsRequired();
				entity.Property(e => e.Action).HasMaxLength(50).IsRequired();
			});

			modelBuilder.Entity<UserRole>(entity =>
			{
				entity.HasKey(e=>new {e.RoleId,e.UserId});
				entity.HasOne(e => e.User).WithMany(e => e.UserRoles).HasForeignKey(e => e.UserId);
				entity.HasOne(e => e.Role).WithMany(e => e.UserRoles).HasForeignKey(e => e.RoleId);
			});
			modelBuilder.Entity<RolePermission>(entity =>
			{
				entity.HasKey(e => new { e.RoleId, e.PermissionId });
				entity.HasOne(e => e.Role).WithMany(r => r.RolePermissions).HasForeignKey(e => e.RoleId);
				entity.HasOne(e => e.Permission).WithMany(p => p.RolePermissions).HasForeignKey(e => e.PermissionId);
			});
			modelBuilder.Entity<UserPermission>(entity =>
			{
				entity.HasKey(e => new { e.UserId, e.PermissionId });
				entity.HasOne(e => e.User).WithMany(u => u.UserPermissions).HasForeignKey(e => e.UserId);
				entity.HasOne(e => e.Permission).WithMany(p => p.UserPermissions).HasForeignKey(e => e.PermissionId);
			});

		}
		public DbSet<User> Users { get; set; }	
		public DbSet<Role> Roles { get; set; }	
		public DbSet<UserRole> UserRoles { get; set; }
		public DbSet<Permission> Permissions { get; set; }	
		public DbSet<RolePermission> RolePermissions { get; set; }

		public void SeedPermission(ModelBuilder modelBuilder)
		{
			var Permissions=new List<Permission>();
			var Id = 1;
			var systemPermissions = new []
			{
				("permissions.create","System","create"),
				("permissions.read","System","read"),
				("permissions.update","System","update"),
				("permissions.delete","System","delete"),
				("roles.create","System","create"),
				("roles.read","System","read"),
				("roles.update","System","update"),
				("roles.delete","System","delete"),
				("users.create","System","create"),
				("users.read","System","read"),
				("users.update","System","update"),
				("users.delete","System","delete"),
				("users.manage","System","manage")
			};

			foreach(var (name, module, action) in systemPermissions)
			{
				Permissions.Add(new Permission
				{
					Id = Id++,
					Action = action,
					Module = module,
					Name = name,
					IsSystemPermission = true

				});
			}

			var businessModules = new[] { "Categories", "Products", "Orders", "Suppliers", "Reports", "Settings" };
			var actions = new[] { "create", "read", "update", "delete" };


		}
		private void SeedRoles(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Role>().HasData(
				new Role { Id = 1, Name = "SuperAdmin",  IsSystemRole = true },
				new Role { Id = 2, Name = "Admin", IsSystemRole = false },
				new Role { Id = 3, Name = "Manager",IsSystemRole = false },
				new Role { Id = 4, Name = "Employee", IsSystemRole = false }
			);
		}
		private void SeedUsers(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>().HasData(
				new User
				{
					Id = 1,
					Username = "superadmin",
					Email = "superadmin@system.com",
					PasswordHash = "1234Super"
				}
			);
		}
		private void SeedUserRoles(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<UserRole>().HasData(new UserRole { UserId = 1, RoleId = 1 });
		}

	}
}
