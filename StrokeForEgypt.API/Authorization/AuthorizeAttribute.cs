using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StrokeForEgypt.API.Helpers;
using StrokeForEgypt.Entity.AccountEntity;
using System;
using System.Linq;

namespace StrokeForEgypt.API.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool allowAll = context.ActionDescriptor.EndpointMetadata.OfType<AllowAllAttribute>().Any();
            if (allowAll)
            {
                return;
            }

            string secret = context.HttpContext.Request.Headers["Secret"];
            string ApiKey = context.HttpContext.Request.Headers["Api-Key"];
            string UserAgent = context.HttpContext.Request.Headers["User-Platform"];

            IServiceProvider services = context.HttpContext.RequestServices;

            AppSettings _appSettings = services.GetService<IOptions<AppSettings>>().Value;

            if (string.IsNullOrEmpty(secret) ||
                string.IsNullOrEmpty(ApiKey) ||
                string.IsNullOrEmpty(UserAgent))
            {
                context.Result = new JsonResult(new { message = "BadRequest" }) { StatusCode = StatusCodes.Status400BadRequest };
            }
            else if (secret != _appSettings.Secret)
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
            else
            {
                if (UserAgent == "Android")
                {
                    if (ApiKey != _appSettings.AndroidAPIKEY)
                    {
                        context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                    }
                }
                else if (UserAgent == "IOS")
                {
                    if (ApiKey != _appSettings.IOSAPIKEY)
                    {
                        context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                    }
                }
                else if (UserAgent == "Web")
                {
                    if (ApiKey != _appSettings.WebAPIKEY)
                    {
                        context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                    }
                }
                else
                {
                    context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                }
            }

            // skip authorization if action is decorated with [AllowAnonymous] attribute
            bool allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
            {
                return;
            }

            // authorization
            Account account = (Account)context.HttpContext.Items["Account"];
            if (account == null)
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}