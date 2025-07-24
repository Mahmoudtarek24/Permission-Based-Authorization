namespace PermissionBasedAuth.Dto_s
{
	public class UserSummaryDto
	{
		public int Id { get; set; }
		public string Username { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public bool IsActive { get; set; }
		public List<RoleDto> Roles { get; set; } = new();
	}
}
