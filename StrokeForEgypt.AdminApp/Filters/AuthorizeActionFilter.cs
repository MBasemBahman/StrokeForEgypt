using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using StrokeForEgypt.Common;
using StrokeForEgypt.Repository;

namespace StrokeForEgypt.AdminApp.Filters
{
    public class AuthorizeActionFilter : IActionFilter
    {
        private readonly UnitOfWork _UnitOfWork;

        private string _ControlerName;
        private readonly bool Referer;
        private readonly int _Fk_AccessLevel;


        public AuthorizeActionFilter(int Fk_AccessLevel, bool Referer, UnitOfWork UnitOfWork)
        {
            _Fk_AccessLevel = Fk_AccessLevel;
            _UnitOfWork = UnitOfWork;
            this.Referer = Referer;
        }


        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            HttpRequest Request = context.HttpContext.Request;

            _ControlerName = context.RouteData.Values["controller"].ToString();

            if (Referer)
            {
                AppMainData.RefererUrl = context.HttpContext.Request.Headers["Referer"].ToString();
            }

            if (string.IsNullOrEmpty(Request.Cookies["Email"]) || !_UnitOfWork.SystemUser.CheckAuthorization(int.Parse(Request.Cookies["Fk_SystemRole"]), _ControlerName, _Fk_AccessLevel))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Logout" }));
            }

        }
    }


    public class AuthorizeAttribute : TypeFilterAttribute
    {
        public AuthorizeAttribute(int Fk_AccessLevel, bool Referer = false)
            : base(typeof(AuthorizeActionFilter))
        {
            Arguments = new object[] { Fk_AccessLevel, Referer };
        }
    }
}
