namespace PermissionBasedAuth.Dto_s
{
	public class UserWithRolesDto
	{
		public int Id { get; set; }
		public string Username { get; set; }
		public string Email { get; set; } 
		public List<string> Roles { get; set; } = new();
		public List<string> Permissions { get; set; } = new();
	}
}
