using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Web.Http.Controllers;

namespace GabinetePsicologia.Server.Models
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class AuthAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context != null)
            {

                var hasClaim = context.HttpContext.User.Claims.Any();
                if (!hasClaim)
                {
                    context.Result = new ForbidResult();
                }
            }
        }
    }
}
