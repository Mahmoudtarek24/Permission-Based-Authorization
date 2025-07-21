namespace PermissionBasedAuth.Domain
{
	public class UserPermission
	{
		public int UserId { get; set; }
		public int PermissionId { get; set; }
		public bool IsGranted { get; set; } = true; /// is active permission for user or no 
		public User User { get; set; } = null!;
		public Permission Permission { get; set; } = null!;
	}
}
