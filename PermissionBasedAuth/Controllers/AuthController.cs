using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PermissionBasedAuth.Dto_s;
using PermissionBasedAuth.Services.Abstraction;
using PermissionBasedAuth.Services.Implementation;

namespace PermissionBasedAuth.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService authService;
		public AuthController(IAuthService authService)
		{
			this.authService = authService;		
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
		public async Task<ActionResult<UserWithRolesDto>> GetProfile()
		{

			return Ok();
		}

	}
}
