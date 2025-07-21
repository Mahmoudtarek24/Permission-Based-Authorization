namespace PermissionBasedAuth.Dto_s
{
	public class LoginResponseDto
	{
		public string Token { get; set; } = string.Empty;
		public string Username { get; set; } = string.Empty;
		public List<string> Roles { get; set; } = new();
		public List<string> Permissions { get; set; } = new();
		public DateTime ExpiresAt { get; set; }
	}
}
