using Microsoft.EntityFrameworkCore;
using PermissionBasedAuth.Context;
using PermissionBasedAuth.Domain;
using PermissionBasedAuth.Dto_s;
using PermissionBasedAuth.Services.Abstraction;

namespace PermissionBasedAuth.Services.Implementation
{
	public class PermissionService : IPermissionService
	{
		private readonly ApplicationDbContext context;
		public PermissionService(ApplicationDbContext context)
		{ 
			this.context = context;	
		}

		public async Task<List<string>> GetUserEffectivePermissionsAsync(int userId)
		{
			///All permission for rule by user id
			var rolePermissions = await context.UserRoles.Include(e=>e.Role).ThenInclude(e => e.RolePermissions)
						    .ThenInclude(e=> e.Permission).Where(e=>e.UserId == userId)	
							.SelectMany(e=> e.Role.RolePermissions.Select(e =>e.Permission.Name))
							.ToListAsync();

			///Get user Permission to check have access to all permission of her rule
			///(by check IsGranted true mean have adtional permission rule , false dint have all her assential rule permission )
			var userPermissions = await context.UserPermissions.Include(e => e.Permission)
								 .Where(e => e.UserId == userId).ToListAsync();

			foreach (var userPerm in userPermissions) 
			{
				if (userPerm.IsGranted)
					rolePermissions.Add(userPerm.Permission.Name);
				else
					rolePermissions.Remove(userPerm.Permission.Name);		
			}

			return rolePermissions;		
		}

		public async Task<bool> HasPermissionAsync(int userId, string permission)
		{
			var effectivePermissions=await GetUserEffectivePermissionsAsync(userId);	

			return effectivePermissions.Contains(permission);		
		}

		public async Task<List<string>> GetRolePermissionsAsync(int roleId)
		{
			var rolePermissions =await context.RolePermissions.Include(e=>e.Permission)
				                       .Where(e=>e.RoleId == roleId).Select(e=>e.Permission.Name).ToListAsync();	
		
			return rolePermissions;	
		}

		/// this method i used to search if user have this "permission" on UserPermission Table 
		/// if dint have Permission add it to UserPermission 
		public async Task GrantUserPermissionAsync(int userId, string prmission)
		{

			var permission = await context.Permissions.FirstOrDefaultAsync(p => p.Name == prmission);

			if (permission is null)
				return;

			var existingUserPermission = await context.UserPermissions
			                  .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permission.Id);
			

			if(existingUserPermission is null)
			{
				var NewUserPermission = new UserPermission()
				{
					IsGranted = true,
					PermissionId = permission.Id,
					UserId = userId,
				};
				context.UserPermissions.Add(NewUserPermission);	
			}
			else
				existingUserPermission.IsGranted = true;	
			
			context.SaveChanges();
		}

		public async Task<UserWithRolesDto?> GetUserWithRolesAndPermissionsAsync(int userId)
		{
			var user = await context.Users.Include(e=> e.UserRoles).ThenInclude(e=>e.Role)
		                                .FirstOrDefaultAsync(e => e.Id == userId);

			var permissions = await GetUserEffectivePermissionsAsync(userId);

			return new UserWithRolesDto
			{
				Id = user.Id,
				Username = user.Username,
				Email = user.Email,
				Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
				Permissions = permissions
			};
		}

		public async Task<List<Permission>> GetAllPermissionsGroupedAsync() =>
			 await context.Permissions.OrderBy(e => e.Module).ThenBy(e => e.Action)
											  .ToListAsync();

		public async Task<Permission?> CreatePermissionAsync(string name, string module, string action)
		{
			var isPermissionExist=await context.Permissions.AnyAsync(e=>e.Name == name&&e.Module==module&&e.Action==action);

			if (isPermissionExist)
				return null;

			var permisssion = new Permission
			{
				IsSystemPermission = false,
				Action = action,
				Module = module,
				Name = name,
			};
			context.Permissions.Add(permisssion);	
			context.SaveChanges();
			return permisssion;	
		}

		public async Task RevokeUserPermissionAsync(int userId, string prmission)
		{
			var permission = await context.Permissions.FirstOrDefaultAsync(p => p.Name == prmission);

			if (permission == null) return;

			var existingUserPermission = await context.UserPermissions
				.FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permission.Id);

			if (existingUserPermission != null)
				existingUserPermission.IsGranted = false;
			else
			{
				context.UserPermissions.Add(new UserPermission
				{
					UserId = userId,
					PermissionId = permission.Id,
					IsGranted = false,
				});
			}

			await context.SaveChangesAsync();
		}
	}
}
