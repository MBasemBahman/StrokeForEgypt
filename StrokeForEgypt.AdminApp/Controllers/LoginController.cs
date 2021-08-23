using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StrokeForEgypt.AdminApp.Services;
using StrokeForEgypt.Entity.AuthEntity;
using StrokeForEgypt.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrokeForEgypt.AdminApp.Controllers
{
    public class LoginController : Controller
    {

        private readonly ILogger<LoginController> _logger;
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly CommonLocalizationService _CommonLocalizationService;

        public LoginController(ILogger<LoginController> logger, UnitOfWork UnitOfWork, IMapper Mapper
            , CommonLocalizationService CommonLocalizationService)
        {
            _logger = logger;
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _CommonLocalizationService = CommonLocalizationService;
        }

        public IActionResult Index()
        {
            string cookie = Request.Cookies["Email"];
            if ((!string.IsNullOrEmpty(cookie)) && (_UnitOfWork.SystemUser.UserExists(cookie)))
            {
                return RedirectToAction(nameof(SetViews), new { Email = cookie });
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(SystemUser systemUser)
        {
            try
            {
                if (_UnitOfWork.SystemUser.UserExists(systemUser.Email, systemUser.Password))
                {
                    return RedirectToAction(nameof(SetViews), new { systemUser.Email });
                }

                ViewData["Error"] = _CommonLocalizationService.Get("Login Validation");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_UnitOfWork.SystemUser.UserExists(systemUser.Email))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return View(systemUser);
        }

        public IActionResult SetViews(string Email)
        {
            SystemUser SystemUser = _UnitOfWork.SystemUser.GetByEmail(Email);

            foreach (string Key in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(Key);
            }

            CookieOptions cookie = new()
            {
                Expires = DateTime.Now.AddMonths(1)
            };

            Response.Cookies.Append("Email", SystemUser.Email, cookie);
            Response.Cookies.Append("Fk_SystemRole", SystemUser.Fk_SystemRole.ToString(), cookie);
            Response.Cookies.Append("FullName", SystemUser.FullName, cookie);
            Response.Cookies.Append("JopTitle", SystemUser.JobTitle, cookie);
            Response.Cookies.Append("IsDarkMode", "false", cookie);

            Dictionary<string, string> Views = _UnitOfWork.SystemUser.GetViews(SystemUser.Fk_SystemRole);

            foreach (KeyValuePair<string, string> View in Views)
            {
                Response.Cookies.Append(View.Key, View.Value, cookie);
            }

            return RedirectToAction("SetCulture", "Home", new { returnUrl = "/Home/Index", Language = "en" });
        }

        public IActionResult Edit()
        {
            string Email = Request.Cookies["Email"];
            if (Email == null)
            {
                return NotFound();
            }
            return View(_UnitOfWork.SystemUser.GetByEmail(Email));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SystemUser systemUser)
        {
            try
            {
                string Email = Request.Cookies["Email"];

                if (Email == null)
                {
                    return NotFound();
                }

                systemUser.LastModifiedBy = Request.Cookies["Email"];
                systemUser.IsActive = true;

                SystemUser Data = _UnitOfWork.SystemUser.GetByEmail(Email);

                _Mapper.Map(systemUser, Data);

                _UnitOfWork.SystemUser.UpdateEntity(Data);
                await _UnitOfWork.SystemUser.Save();

                return RedirectToAction(nameof(SetViews), new { Email = Data.Email });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_UnitOfWork.SystemUser.UserExists(systemUser.Email))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        public IActionResult Logout()
        {
            foreach (string Key in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(Key);
            }

            CookieOptions cookie = new()
            {
                Expires = DateTime.Now.AddDays(-1)
            };

            return RedirectToAction(nameof(Index));
        }
    }
}
