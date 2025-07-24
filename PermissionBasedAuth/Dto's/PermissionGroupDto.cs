namespace PermissionBasedAuth.Dto_s
{
	public class PermissionGroupDto
	{
		public string Module { get; set; } = string.Empty;
		public List<PermissionDto> Permissions { get; set; } = new();
	}
}
