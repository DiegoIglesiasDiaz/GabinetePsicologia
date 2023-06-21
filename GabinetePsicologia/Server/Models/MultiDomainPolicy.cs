namespace GabinetePsicologia.Server.Models
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class MultiDomainPolicy : IAuthorizationRequirement
    {
    }

    public class MultiDomainAuthorizationHandler : AuthorizationHandler<MultiDomainPolicy>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MultiDomainPolicy requirement)
        {
            if (context.Resource is AuthorizationFilterContext filterContext)
            {
                var validIssuers = new[] { "https://domain1.com", "https://domain2.com" };
                var token = filterContext.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                // Validate the token's issuer
                //var issuer = TokenValidation.ValidateIssuer(token); // Implement your token validation logic here

                //if (validIssuers.Contains(issuer))
                //{
                //    context.Succeed(requirement);
                //}
            }

            return Task.CompletedTask;
        }
    }
}
