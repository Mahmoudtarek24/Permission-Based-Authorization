using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PermissionBasedAuth.Dto_s;
using PermissionBasedAuth.Services.Abstraction;
using PermissionBasedAuth.Services.Implementation;
using System.Security.Claims;

namespace PermissionBasedAuth.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService authService;
		private readonly IPermissionService permissionService;
		public AuthController(IAuthService authService,IPermissionService permissionService)
		{
			this.authService = authService;		
			this.permissionService = permissionService;
		}
		[HttpPost("login")]
		public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
		{
			var result = await authService.LoginAsync(loginDto);

			if(result is null)
				return Unauthorized("Incorrect username or password");
			
			return Ok(result);
		}
		
		[HttpPost("register")]
		[Authorize(AuthenticationSchemes ="Bearer",Roles = "SuperAdmin")]
		public async Task<ActionResult<UserWithRolesDto>> Register([FromBody] CreateUserDto dto)
		{
			var user = await authService.CreateUserAsync(dto);

			if(user is null)
				return BadRequest("The user already exists");

			var userWithRoles = new UserWithRolesDto
			{
				Id = user.Id,
				Username = user.Username,
				Email = user.Email
			};
			return Ok(userWithRoles);	
		}
		[HttpGet("profile")]
		[Authorize]
		public async Task<ActionResult<UserWithRolesDto>> GetProfile()
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			var userProfile = await permissionService.GetUserWithRolesAndPermissionsAsync(int.Parse(userId));

			if (userProfile == null)
				return NotFound("User not exist");

			return Ok(userProfile);
		}

	}
}
