using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace PermissionBasedAuth.Filters
{
	public class PermissionPolicyProvider : IAuthorizationPolicyProvider
	{
		public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }
		public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
		{
			FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
		}

		public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
			FallbackPolicyProvider.GetDefaultPolicyAsync();

		public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
			FallbackPolicyProvider.GetFallbackPolicyAsync();

		public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
		{
			if (!string.IsNullOrEmpty(policyName))
			{
				var policy=new AuthorizationPolicyBuilder()
					        .AddRequirements(new HasPermissionAttribute(policyName)).Build();
				return Task.FromResult<AuthorizationPolicy?>(policy);
			}
			return FallbackPolicyProvider.GetPolicyAsync(policyName);
		}
	}
}
