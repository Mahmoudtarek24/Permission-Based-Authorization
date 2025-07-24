namespace PermissionBasedAuth.Dto_s
{
	public class RoleWithPermissionsDto
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public bool IsActive { get; set; }
		public bool IsSystemRole { get; set; }
		public List<PermissionDto> Permissions { get; set; } = new();
	}
}
