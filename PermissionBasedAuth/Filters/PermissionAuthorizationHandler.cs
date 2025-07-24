using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace PermissionBasedAuth.Filters
{
	public class PermissionAuthorizationHandler : AuthorizationHandler<HasPermissionAttribute>
	{
		protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, HasPermissionAttribute requirement)
		{
			// 1- check we have user
			if (context.User?.Identity?.IsAuthenticated != true)
			{
				context.Fail();
				return;
			}

			// 2- Get UserId
			var userId = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
			if (userId is null) 
			{
				context.Fail();
				return;
			}

			if (context.User.IsInRole("SuperAdmin"))
			{
				context.Succeed(requirement);
				return;
			}
			// 3- Check from her permission  
			else if(context.User.HasClaim("permission", requirement.Permission))
			{
				context.Succeed(requirement);
				return;	
			}
			else
				context.Fail();

			return ;	
		}
	}
}
