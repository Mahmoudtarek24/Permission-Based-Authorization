using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PermissionBasedAuth.Filters
{
	public class HasPermissionAttribute : IAuthorizationRequirement
	{
		public string Permission {  get; }	
		public HasPermissionAttribute(string permission) 
		{
			Permission = permission;	
		}	
	}
}
