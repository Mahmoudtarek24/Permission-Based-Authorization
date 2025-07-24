namespace PermissionBasedAuth.Dto_s
{
	public class RevokeUserPermissionDto
	{
		public int UserId { get; set; }
		public string PermissionName { get; set; } = string.Empty;
	}
}
