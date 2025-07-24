namespace PermissionBasedAuth.Dto_s
{
	public class UpdateUserDto
	{   public string? Username { get; set; }
		public string? Email { get; set; }
		public string? Password { get; set; }
		public bool? IsActive { get; set; }
		public List<int>? RoleIds { get; set; }
	}
}
