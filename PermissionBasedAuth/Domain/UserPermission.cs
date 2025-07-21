namespace PermissionBasedAuth.Domain
{
	public class UserPermission
	{
		public int UserId { get; set; }
		public int PermissionId { get; set; }
		public bool IsGranted { get; set; } = true; // true(mean we add new permission to it that her role dint have it)
		                                           // False (delete permission from it that her rolue have it )
		public Permission Permission { get; set; } = null!;
		public User User { get; set; }	
	}
}
