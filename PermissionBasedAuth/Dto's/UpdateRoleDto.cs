namespace PermissionBasedAuth.Dto_s
{
	public class UpdateRoleDto
	{
		public string? Name { get; set; }
		public bool? IsActive { get; set; }
		public List<int>? PermissionIds { get; set; }
	}
}
