namespace PermissionBasedAuth.Domain
{
	public class Permission 
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty; //  "users.create"
		public string Module { get; set; } = string.Empty; // "Users", "Categories"
	    public string Action {  get; set; } = string.Empty;
		public bool IsSystemPermission { get; set; } = false;

		public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
		public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
	}
}
