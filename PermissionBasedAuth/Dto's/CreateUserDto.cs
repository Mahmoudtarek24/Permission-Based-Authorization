namespace PermissionBasedAuth.Dto_s
{
	public class CreateUserDto
	{
		public string Username { get; set; } = string.Empty;//""
		public string Email { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;    
		public List<int>? RoleIds { get; set; }
	}
}
