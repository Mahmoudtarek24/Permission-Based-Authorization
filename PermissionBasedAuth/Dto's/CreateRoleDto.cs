namespace PermissionBasedAuth.Dto_s
{
	public class CreateRoleDto
	{
		public string Name { get; set; } = string.Empty;
		public List<int>? PermissionIds { get; set; }
	}
}
