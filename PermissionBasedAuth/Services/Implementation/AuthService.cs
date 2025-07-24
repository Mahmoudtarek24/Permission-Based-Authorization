using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PermissionBasedAuth.Context;
using PermissionBasedAuth.Domain;
using PermissionBasedAuth.Dto_s;
using PermissionBasedAuth.Services.Abstraction;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Text;

namespace PermissionBasedAuth.Services.Implementation
{
	public class AuthService : IAuthService
	{
		private readonly ApplicationDbContext context;
		private readonly IPermissionService permissionService;
		private readonly IConfiguration configuration;
		public AuthService(ApplicationDbContext context, IPermissionService permissionService, IConfiguration configuration)
		{
			this.context = context;
			this.permissionService = permissionService;
			this.configuration = configuration;
		}
		public async Task<User?> CreateUserAsync(CreateUserDto dto)
		{
			var existsUsername = await context.Users.AnyAsync(u => u.Username == dto.Username);
			var existsEmail = await context.Users.AnyAsync(u => u.Email == dto.Email);

			if (existsUsername || existsEmail)
				return null;

			var user = new User
			{
				Username = dto.Username,
				Email = dto.Email,
				PasswordHash = dto.Password
			};
			
			context.Users.Add(user);
			await context.SaveChangesAsync();

			if (dto.RoleIds?.Any() == true)
			{
				var userRoles = dto.RoleIds.Select(roleId => new UserRole
				{
					UserId = user.Id,
					RoleId = roleId
				});

				context.UserRoles.AddRange(userRoles);
			}

			await context.SaveChangesAsync();

			return user;
		}

		public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
		{
			var user = await context.Users.Include(e => e.UserRoles).ThenInclude(e => e.Role)
		                       .FirstOrDefaultAsync(e => e.Username == dto.Username && e.PasswordHash == dto.Password);

			if (user is null)
				return null;

			var roles = user.UserRoles.Select(e => e.Role.Name).ToList();
			var permissions = await permissionService.GetUserEffectivePermissionsAsync(user.Id);
			var token = GenerateJwtToken(user, roles, permissions);

			return new LoginResponseDto
			{
				Token = token,
				Username = user.Username,
				Roles = roles,
				Permissions = permissions,
				ExpiresAt = DateTime.Now.AddHours(24)
			};
		}
		public string GenerateJwtToken(User user, List<string> roles, List<string> permissions)
		{
			var claims = new List<Claim>
			{
				new(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new(ClaimTypes.Name, user.Username),
				new(ClaimTypes.Email, user.Email)
			};
			claims.AddRange(permissions.Select(permission => new Claim("permission", permission)));
			claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSetting:Key"]!));
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
			issuer: configuration["JWTSetting:Issuer"],
			audience: configuration["JWTSetting:Audience"],
			claims: claims,
			expires: DateTime.Now.AddHours(24),
			signingCredentials: credentials );

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

	}
}