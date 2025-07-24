using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PermissionBasedAuth.Context;
using PermissionBasedAuth.Domain;
using PermissionBasedAuth.Dto_s;
using PermissionBasedAuth.Services.Abstraction;
using PermissionBasedAuth.Services.Implementation;
using System.Security.Claims;

namespace PermissionBasedAuth.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly ApplicationDbContext context;
		private readonly IPermissionService permissionService;
		private readonly IAuthService authService;

		public UsersController(ApplicationDbContext context, IPermissionService permissionService, 
			                   IAuthService authService)
		{
			this.context = context;
			this.permissionService = permissionService;
			this.authService = authService;
		}

		[HttpGet("all")]
		[Authorize(Policy ="users.read")]
		public async Task<ActionResult<List<UserSummaryDto>>> GetUsers()
		{
			var response =await context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
				.Select(e=>new UserSummaryDto {
					Id = e.Id,
					Username = e.Username,
					Email = e.Email,
					IsActive = e.IsActive,
					Roles = e.UserRoles.Select(e => new RoleDto
					{
						Id = e.Role.Id,
						Name = e.Role.Name,
					}).ToList()
				}).ToListAsync();	
			return Ok(response);
		}
		[HttpGet("{id:int}")]
		[Authorize(Policy = "users.read")]
		public async Task<ActionResult<UserWithRolesDto>> GetUser(int id)
		{
			var user = await permissionService.GetUserWithRolesAndPermissionsAsync(id);

			if (user == null)
				return NotFound("Not found User");

			return Ok(user);
		}
		[HttpPut("update/{id:int}")]
		[Authorize(Policy = "users.update")]
		public async Task<ActionResult<UserWithRolesDto>> UpdateUser(int id, [FromBody] UpdateUserDto updateDto)
		{
			var user = await context.Users.FindAsync(id);

			if (user == null)
				return NotFound("User is not found");

			if (!string.IsNullOrEmpty(updateDto.Username))
				user.Username = updateDto.Username;

			if (!string.IsNullOrEmpty(updateDto.Email))
				user.Email = updateDto.Email;
			
			if (!string.IsNullOrEmpty(updateDto.Password))
				user.PasswordHash = updateDto.Password;
			
			if (updateDto.IsActive.HasValue)
				user.IsActive = updateDto.IsActive.Value;

			if (updateDto.RoleIds != null)
			{
				var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

				var existingRoles = await context.UserRoles.Where(e=>e.UserId == id).ToListAsync();
				context.UserRoles.RemoveRange(existingRoles);

				var newRoles = updateDto.RoleIds.Select(roleId => new UserRole
				{
					UserId = id,
					RoleId = roleId,
				});

				context.UserRoles.AddRange(newRoles);
			}

			await context.SaveChangesAsync();

			var updatedUser = await permissionService.GetUserWithRolesAndPermissionsAsync(id);
			return Ok(updatedUser);
		}

		[HttpDelete("delete/{id:int}")]
		[Authorize(Policy = "users.delete")]
		public async Task<ActionResult<string>> DeleteUser(int id)
		{
			var user = await context.Users.FindAsync(id);

			if (user == null)
				return NotFound("User Not Found");

			context.Users.Remove(user);
			await context.SaveChangesAsync();

			return Ok("Delete user Successfully");
		}

		[HttpPost("{id:int}/grant-permission")]
		[Authorize(Policy = "users.manage")]
		public async Task<ActionResult<string>> GrantPermission( [FromBody] GrantUserPermissionDto request)
		{
			var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value );

			await permissionService.GrantUserPermissionAsync(request.UserId,request.PermissionName);

			return Ok("add Permission to Successfully");
		}

		[HttpPost("{id:int}/revoke-permission")]
		[Authorize(Policy = "users.manage")]
		public async Task<ActionResult<string>> RevokePermission([FromBody] RevokeUserPermissionDto request)
		{
			var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

			await permissionService.RevokeUserPermissionAsync(request.UserId, request.PermissionName);

			return Ok("Permission deleted from user Successfully");
		}
	}
}
