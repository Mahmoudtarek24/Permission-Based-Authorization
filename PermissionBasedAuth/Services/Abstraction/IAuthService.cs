using PermissionBasedAuth.Domain;
using PermissionBasedAuth.Dto_s;

namespace PermissionBasedAuth.Services.Abstraction
{
	public interface IAuthService
	{
		Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
		string GenerateJwtToken(User user, List<string> roles, List<string> permissions);
		Task<User?> CreateUserAsync(CreateUserDto createUserDto);
	}
}
