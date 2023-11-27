using Microsoft.AspNetCore.Authorization;

namespace PermissionBase.Common
{
    public class PermissionRequirement : IAuthorizationRequirement
    {

    }

    public class PermissionRequirementHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PermissionRequirementHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var routes = _httpContextAccessor.HttpContext.Request.RouteValues;
            var permissionName = $"{routes["controller"].ToString()}Controller.{routes["action"].ToString()}";

            if (context.User == null || !context.User.HasClaim(permissionName, permissionName + ".value"))
            {
                context.Fail();
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

    public class PermissionAttribute : AuthorizeAttribute
    {
        const string POLICY = "Permission";

        public PermissionAttribute()
        {
            Policy = POLICY;
        }
    }
}
