namespace PermissionBasedAuth.Domain
{
	public class Role
	{
		public int Id { get; set; } 
		public string Name { get; set; } = string.Empty; 
		public bool IsActive { get; set; } = true;
		public bool IsSystemRole { get; set; } = false; ///this role will not deleted or updated 

		public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
		public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
	}
}
