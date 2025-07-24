namespace PermissionBasedAuth.Dto_s
{
	public class GrantUserPermissionDto
	{
		public int UserId { get; set; }
		public string PermissionName { get; set; } = string.Empty;
	}
}
