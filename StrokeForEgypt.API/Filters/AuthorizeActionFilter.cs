using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StrokeForEgypt.Repository;

namespace StrokeForEgypt.API.Filters
{
    public class AuthorizeActionFilter : IActionFilter
    {
        private readonly UnitOfWork _UnitOfWork;

        public AuthorizeActionFilter(UnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork;
        }


        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            HttpRequest Request = context.HttpContext.Request;

            string token = Request.Query["Token"];
            if (string.IsNullOrEmpty(token) || token != "2f97dc0d-f405-46c6-9219-d3bbb515a638")
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }

    public class AuthorizeAttribute : TypeFilterAttribute
    {
        public AuthorizeAttribute()
            : base(typeof(AuthorizeActionFilter))
        {
            Arguments = new object[] { };
        }
    }
}
