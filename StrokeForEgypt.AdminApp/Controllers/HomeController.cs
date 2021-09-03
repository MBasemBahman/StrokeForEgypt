using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using StrokeForEgypt.AdminApp.Models;
using StrokeForEgypt.Common;
using StrokeForEgypt.DAL;
using StrokeForEgypt.Repository;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

namespace StrokeForEgypt.AdminApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISession _Session;
        private readonly UnitOfWork _UnitOfWork;
        private readonly BaseDBContext _BaseDBContext;

        public HomeController(IHttpContextAccessor HttpContextAccessor, UnitOfWork UnitOfWork, BaseDBContext BaseDBContext)
        {
            _Session = HttpContextAccessor.HttpContext.Session;
            _UnitOfWork = UnitOfWork;
            _BaseDBContext = BaseDBContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SetCulture(string returnUrl, string Language)
        {
            string Culture = "en";

            if (!string.IsNullOrEmpty(Language))
            {
                Culture = Language;
            }
            else
            {
                // Retrieves the requested culture
                IRequestCultureFeature CultureFeature = Request.HttpContext.Features.Get<IRequestCultureFeature>();
                // Culture contains the information of the requested culture
                CultureInfo CurrentCulture = CultureFeature.RequestCulture.UICulture;
                if (CurrentCulture.Name == "en")
                {
                    Culture = "ar";
                }
            }

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture: "en", uiCulture: Culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            return LocalRedirect(returnUrl);
        }

        public IActionResult SetTheme(string returnUrl)
        {
            CookieOptions cookie = new()
            {
                Expires = DateTime.Now.AddMonths(1)
            };

            if (bool.Parse(Request.Cookies["IsDarkMode"]) == false)
            {
                Response.Cookies.Append("IsDarkMode", "true", cookie);
            }
            else
            {
                Response.Cookies.Append("IsDarkMode", "false", cookie);
            }

            return LocalRedirect(returnUrl);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
