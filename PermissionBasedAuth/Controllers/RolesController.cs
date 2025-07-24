using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PermissionBasedAuth.Context;
using PermissionBasedAuth.Domain;
using PermissionBasedAuth.Dto_s;

namespace PermissionBasedAuth.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RolesController : ControllerBase
	{
		private readonly ApplicationDbContext context;

		public RolesController(ApplicationDbContext context)
		{
			this.context = context;
		}

		[HttpGet("all")]
		[Authorize(Policy = "roles.read")]
		public async Task<ActionResult<List<RoleDto>>> GetRoles()
		{
			var roles = await context.Roles
				.OrderBy(e => e.Name)
				.Select(e=> new RoleDto
				{
					Id = e.Id,
					Name = e.Name,
					IsActive = e.IsActive,
					IsSystemRole = e.IsSystemRole,
				})
				.ToListAsync();

			return Ok(roles);
		}

		[HttpGet("{id:int}")]
		[Authorize(Policy = "roles.read")]
		public async Task<ActionResult<RoleWithPermissionsDto>> GetRole(int id)
		{
			var role = await context.Roles
				.Include(r => r.RolePermissions)
				.ThenInclude(rp => rp.Permission)
				.FirstOrDefaultAsync(r => r.Id == id);

			if (role == null)
				return NotFound("Role not found");

			var roleDto = new RoleWithPermissionsDto
			{
				Id = role.Id,
				Name = role.Name,
				IsActive = role.IsActive,
				IsSystemRole = role.IsSystemRole,
				Permissions = role.RolePermissions.Select(rp => new PermissionDto
				{
					Id = rp.Permission.Id,
					Name = rp.Permission.Name,
					Module = rp.Permission.Module,
					Action = rp.Permission.Action,
					IsSystemPermission = rp.Permission.IsSystemPermission
				}).ToList()
			};
			return Ok(roleDto);
		}

		[HttpPost("create")]
		[Authorize("SuperAdmin")]
		public async Task<ActionResult<RoleDto>> CreateRole([FromBody] CreateRoleDto dto)
		{
			var IsExistRole=await context.Roles.AnyAsync(e=>e.Name==dto.Name);
			if (IsExistRole)
				return BadRequest("Role Is Exist");

			var role = new Role
			{
				Name = dto.Name,
				IsActive = true,
				IsSystemRole = false
			};

			context.Roles.Add(role);
			await context.SaveChangesAsync();

			var NotValidPermissionId = new List<int>();

			if (dto.PermissionIds?.Any() == true)
			{
				var permissionIds=await context.Permissions.Select(p => p.Id).ToListAsync();	
				var validpermissionId= permissionIds.Intersect(dto.PermissionIds);
				NotValidPermissionId=dto.PermissionIds.Except(permissionIds).ToList();		

				var rolePermissions = validpermissionId.Select(e => new RolePermission
				{
					RoleId = role.Id,
					PermissionId = e
				});

				context.RolePermissions.AddRange(rolePermissions);
				await context.SaveChangesAsync();
			}

			var roleDto = new RoleDto
			{
				Id = role.Id,
				Name = role.Name,
				IsActive = role.IsActive,
				IsSystemRole = role.IsSystemRole,
			};

			if (NotValidPermissionId.Count == dto.PermissionIds.Count)
			{
				roleDto.Message = "Role created, and not create any Permission all not valid permission";
				return Ok(roleDto);
			}
			if (NotValidPermissionId.Any())
			{
				roleDto.Message = $"this Permission not assign with id  {string.Join(" ,", NotValidPermissionId)}not valid";
				return Ok(roleDto);	
			}

			return Ok(roleDto);	
		}
		[HttpPut("update/{id:int}")]
		[Authorize("SuperAdmin")]

		public async Task<ActionResult<RoleDto>> UpdateRole(int id, [FromBody] UpdateRoleDto dto)
		{
			var role = await context.Roles.FindAsync(id);

			if (role is null)
				return NotFound("Role not found");

			if (role.IsSystemRole)
				return BadRequest($"role with id :{id} is a System Role can not update it");


			if (!string.IsNullOrEmpty(dto.Name))
				role.Name = dto.Name;
			if (dto.IsActive.HasValue)
				role.IsActive = dto.IsActive.Value;

			var roleDto = new RoleDto
			{
				Id = role.Id,
				Name = role.Name,
				IsActive = role.IsActive,
				IsSystemRole = role.IsSystemRole,
			};

			if (dto.PermissionIds != null)
			{
				var validPermissionIdsInDb = await context.Permissions.Select(p => p.Id).ToListAsync();

				var validPermissionIds = dto.PermissionIds.Intersect(validPermissionIdsInDb).ToList();
				var notValidPermissionIds = dto.PermissionIds.Except(validPermissionIdsInDb).ToList();

				var warning = new List<string>();

				if (validPermissionIds.Any())
				{
					var existingPermissions = await context.RolePermissions.Where(e => e.RoleId == id).ToListAsync();

					context.RolePermissions.RemoveRange(existingPermissions);

					var newPermissions = validPermissionIds.Select(pid => new RolePermission
					{
						RoleId = id,
						PermissionId = pid
					});

					context.RolePermissions.AddRange(newPermissions);
				}
				else
					warning.Add("No permissions assigned. All provided PermissionIds are invalid.");

				if (notValidPermissionIds.Any() && validPermissionIds.Any())
					warning.Add($"Some permissions not assigned. Invalid PermissionIds: {string.Join(", ", notValidPermissionIds)}");

				await context.SaveChangesAsync();

				roleDto.Message = string.Join(" | ", warning);
				return Ok(roleDto);
			}
			return Ok(roleDto);	      
		}
		[HttpDelete("delete/{id:int}")]
		[Authorize("SuperAdmin")]

		public async Task<ActionResult<string>> DeleteRole(int id)
		{
			var role = await context.Roles.FindAsync(id);

			if (role == null)
				return NotFound("Role not found");

			if (role.IsSystemRole)
				return BadRequest("Can not delete System role");

			context.Roles.Remove(role);
			await context.SaveChangesAsync();

			return Ok("role Deleted Successfully");
		}
	}
}

