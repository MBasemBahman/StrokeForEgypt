using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrokeForEgypt.AdminApp.Filters;
using StrokeForEgypt.AdminApp.Services;
using StrokeForEgypt.AdminApp.ViewModel;
using StrokeForEgypt.Common;
using StrokeForEgypt.Entity.MainDataEntity;
using StrokeForEgypt.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static StrokeForEgypt.Common.EnumData;


namespace StrokeForEgypt.AdminApp.Controllers.MainDataEntity
{
    public class AppAboutController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;
        private readonly CommonLocalizationService _CommonLocalizationService;

        public AppAboutController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor, CommonLocalizationService CommonLocalizationService)
        {
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
            _CommonLocalizationService = CommonLocalizationService;
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public IActionResult Details()
        {
            AppAbout AppAbout = _UnitOfWork.AppAbout.GetFirst().Result;

            if (AppAbout == null)
            {
                return NotFound();
            }

            return View("~/Views/MainDataEntity/AppAbout/Details.cshtml", AppAbout);
        }

        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id = 0)
        {
            AppAbout AppAbout = new AppAbout();

            if (id > 0)
            {
                AppAbout = await _UnitOfWork.AppAbout.GetByID(id);

                if (AppAbout == null)
                {
                    return NotFound();
                }
            }


            return View("~/Views/MainDataEntity/AppAbout/CreateOrEdit.cshtml", AppAbout);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id, AppAbout AppAbout)
        {
            if (id != AppAbout.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0)
                    {
                        AppAbout.CreatedBy = _Session.GetString("FullName");

                        _UnitOfWork.AppAbout.CreateEntity(AppAbout);
                        await _UnitOfWork.AppAbout.Save();
                    }
                    else
                    {
                        AppAbout Data = await _UnitOfWork.AppAbout.GetByID(id);

                        AppAbout.LastModifiedBy = _Session.GetString("FullName");

                        _Mapper.Map(AppAbout, Data);


                        _UnitOfWork.AppAbout.UpdateEntity(Data);
                        await _UnitOfWork.AppAbout.Save();

                    }
                 
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_UnitOfWork.AppAbout.Any(a => a.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details));
            }

            return View("~/Views/MainDataEntity/AppAbout/CreateOrEdit.cshtml", AppAbout);

        }
    }
}
