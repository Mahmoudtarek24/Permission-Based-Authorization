using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PermissionBasedAuth.Context;
using PermissionBasedAuth.Dto_s;
using PermissionBasedAuth.Services.Abstraction;
using PermissionBasedAuth.Services.Implementation;

namespace PermissionBasedAuth.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PermissionsController : ControllerBase
	{
		private readonly IPermissionService permissionService;

		public PermissionsController(IPermissionService permissionService )
		{
			this.permissionService = permissionService;
		}
		[HttpGet("all")]
		[Authorize(Policy ="permissions.read")]
		public async Task<ActionResult<List<PermissionGroupDto>>> GetPermissions()
		{
			var permissions = await permissionService.GetAllPermissionsGroupedAsync();

			var grouped = permissions
				.GroupBy(p => p.Module)
				.Select(g => new PermissionGroupDto
				{
					Module = g.Key,
					Permissions = g.Select(p => new PermissionDto
					{
						Id = p.Id,
						Name = p.Name,
						Module = p.Module,
						Action = p.Action,
						IsSystemPermission = p.IsSystemPermission,
					}).ToList()
				})
				.ToList();

			return Ok(grouped);
		}

		[HttpPost("create")]
		[Authorize(Policy = "permissions.create")]
		public async Task<ActionResult<PermissionDto>> CreatePermission([FromBody] CreatePermissionDto createDto)
		{
			var permission = await permissionService.CreatePermissionAsync(
				createDto.Name,
				createDto.Module,
				createDto.Action);

			if (permission == null)
				return BadRequest("Permission name is exectully found");

			var permissionDto = new PermissionDto
			{
				Id = permission.Id,
				Name = permission.Name,
				Module = permission.Module,
				Action = permission.Action,
				IsSystemPermission = permission.IsSystemPermission,
			};

			return Ok(permissionDto);
		}

	}
}
