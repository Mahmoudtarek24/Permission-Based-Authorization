namespace PermissionBasedAuth.Dto_s
{
	public class RoleDto
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public bool IsActive { get; set; }
		public bool IsSystemRole { get; set; }
		public string Message { get; set; }	
	}
}
