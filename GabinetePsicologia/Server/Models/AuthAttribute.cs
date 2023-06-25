﻿using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace GabinetePsicologia.Server.Models
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class AuthAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext context)
        {
            context.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            //if (context != null)
            //{
            //    if (!context.RequestContext.Principal.Identity.IsAuthenticated)
            //    {
            //        context.Result = new ForbidResult();

            //    }

            //}
            //else
            //{
            //    context.Result = new ForbidResult();
            //}
        }
    }
}
