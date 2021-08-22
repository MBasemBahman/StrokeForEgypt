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
            // skip authorization if action is decorated with [AllowAnonymous] attribute
            bool allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
            {
                string secret = context.HttpContext.Request.Cookies["Secret"];
                string AppSecret = context.HttpContext.Request.Cookies["App-Secret"];
                string UserAgent = context.HttpContext.Request.Headers["User-Agent"];

                IServiceProvider services = context.HttpContext.RequestServices;

                AppSettings _appSettings = services.GetService<IOptions<AppSettings>>().Value;

                if (string.IsNullOrEmpty(secret) ||
                    string.IsNullOrEmpty(AppSecret) ||
                    string.IsNullOrEmpty(UserAgent))
                {
                    context.Result = new JsonResult(new { message = "BadRequest" }) { StatusCode = StatusCodes.Status400BadRequest };
                }

                if (secret != _appSettings.Secret)
                {
                    context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                }
                else
                {
                    if (UserAgent == "Android")
                    {
                        if (AppSecret != _appSettings.AndroidSecret)
                        {
                            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                        }
                    }
                    else if (UserAgent == "IOS")
                    {
                        if (AppSecret != _appSettings.IOSSecret)
                        {
                            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                        }
                    }
                    else if (UserAgent == "Web")
                    {
                        if (AppSecret != _appSettings.WebSecret)
                        {
                            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                        }
                    }
                }

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