using PermissionBasedAuth.Domain;
using PermissionBasedAuth.Dto_s;

namespace PermissionBasedAuth.Services.Abstraction
{
	public interface IPermissionService
	{
		Task<List<string>> GetUserEffectivePermissionsAsync(int userId);
		Task<bool> HasPermissionAsync(int userId, string permission);
		Task<List<string>> GetRolePermissionsAsync(int roleId);
		Task GrantUserPermissionAsync(int userId, string permission);
		Task RevokeUserPermissionAsync(int userId, string permission);
		Task<UserWithRolesDto?> GetUserWithRolesAndPermissionsAsync(int userId);
		Task<List<Permission>> GetAllPermissionsGroupedAsync();
		Task<Permission?> CreatePermissionAsync(string name, string module, string action);
	}
}
