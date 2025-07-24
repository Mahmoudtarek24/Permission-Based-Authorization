namespace PermissionBasedAuth.Dto_s
{
	public class CreatePermissionDto
	{
		public string Name { get; set; } = string.Empty;
		public string Module { get; set; } = string.Empty;
		public string Action { get; set; } = string.Empty;
	}
}
