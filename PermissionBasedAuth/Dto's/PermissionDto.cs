namespace PermissionBasedAuth.Dto_s
{
	public class PermissionDto
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Module { get; set; } = string.Empty;
		public string Action { get; set; } = string.Empty;
		public bool IsSystemPermission { get; set; }
	}
}
