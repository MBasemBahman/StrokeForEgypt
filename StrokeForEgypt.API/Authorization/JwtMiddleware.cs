using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using StrokeForEgypt.API.Helpers;
using StrokeForEgypt.API.Services;
using System.Linq;
using System.Threading.Tasks;

namespace StrokeForEgypt.API.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context, IAccountService accountService, IJwtUtils jwtUtils)
        {
            string token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            int? accountId = jwtUtils.ValidateJwtToken(token);
            if (accountId != null)
            {
                // attach account to context on successful jwt validation
                context.Items["Account"] = accountService.GetById(accountId.Value);
            }

            await _next(context);
        }
    }
}