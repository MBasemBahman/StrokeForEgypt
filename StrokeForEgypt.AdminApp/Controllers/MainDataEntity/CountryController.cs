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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static StrokeForEgypt.Common.EnumData;

namespace StrokeForEgypt.AdminApp.Controllers.MainDataEntity
{
    public class CountryController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;
        private readonly CommonLocalizationService _CommonLocalizationService;

        public CountryController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor, CommonLocalizationService CommonLocalizationService)
        {
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
            _CommonLocalizationService = CommonLocalizationService;
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public IActionResult Index(int Id)
        {
            ViewData["Id"] = Id;

            return View("~/Views/MainDataEntity/Country/Index.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> LoadTable([FromBody] CommonFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<Country> result = await _UnitOfWork.Country.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id));



            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.Name.ToLower().Contains(searchBy.ToLower())
                                        || a.IsActive.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.PhoneCode.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.Order.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.Id.ToString().ToLower().Contains(searchBy.ToLower()))
                               .ToList();
            }

            DataTableManager<Country> DataTableManager = new();

            DataTableResult<Country> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.Country.Count());

            return Json(new
            {
                draw = DataTableResult.DtParameters.Draw,
                recordsTotal = DataTableResult.TotalResultsCount,
                recordsFiltered = DataTableResult.FilteredResultsCount,
                data = DataTableResult.Data
                                      .Skip(DataTableResult.DtParameters.Start)
                                      .Take(DataTableResult.DtParameters.Length)
                                      .ToList()
            });
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public async Task<IActionResult> Details(int id)
        {
            Country Country = await _UnitOfWork.Country.GetByID(id);

            if (Country == null)
            {
                return NotFound();
            }

            return View("~/Views/MainDataEntity/Country/Details.cshtml", Country);
        }


        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id = 0)
        {
            Country Country = new();

            if (id > 0)
            {
                Country = await _UnitOfWork.Country.GetByID(id);
                if (Country == null)
                {
                    return NotFound();
                }
            }

            return View("~/Views/MainDataEntity/Country/CreateOrEdit.cshtml", Country);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id, Country Country)
        {
            if (id != Country.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0)
                    {
                        Country.CreatedBy = Request.Cookies["FullName"];
                        _UnitOfWork.Country.CreateEntity(Country);
                        await _UnitOfWork.Country.Save();
                    }
                    else
                    {
                        Country Data = await _UnitOfWork.Country.GetByID(id);

                        Country.LastModifiedBy = Request.Cookies["FullName"];

                        _Mapper.Map(Country, Data);

                        _UnitOfWork.Country.UpdateEntity(Data);

                        await _UnitOfWork.Country.Save();

                        Country = Data;
                    }

                    IFormFile files = HttpContext.Request.Form.Files["ImageFile"];

                    if (files != null)
                    {
                        ImgManager ImgManager = new(AppMainData.WebRootPath);

                        string ImgURl = await ImgManager.UploudImage(AppMainData.DomainName, Country.Id.ToString(), files, "Uploud/Country");

                        if (!string.IsNullOrEmpty(ImgURl))
                        {
                            if (!string.IsNullOrEmpty(Country.ImageURL))
                            {
                                ImgManager.DeleteImage(Country.ImageURL, AppMainData.DomainName);
                            }
                            Country.ImageURL = ImgURl;

                            _UnitOfWork.Country.UpdateEntity(Country);

                            await _UnitOfWork.Country.Save();
                        }
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_UnitOfWork.Country.Any(a => a.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/MainDataEntity/Country/CreateOrEdit.cshtml", Country);
        }


        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> Delete(int id)
        {
            Country Country = await _UnitOfWork.Country.GetByID(id);

            if (Country == null)
            {
                return NotFound();
            }

            if (_UnitOfWork.City.Any(a => a.Fk_Country == id))
            {
                ViewBag.CanDelete = false;
            }

            return View("~/Views/MainDataEntity/Country/Delete.cshtml", Country);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Country Country = await _UnitOfWork.Country.GetByID(id);

            if ((!_UnitOfWork.City.Any(a => a.Fk_Country == id)))
            {
                _UnitOfWork.Country.DeleteEntity(Country);

                await _UnitOfWork.Country.Save();

                ImgManager ImgManager = new(AppMainData.WebRootPath);

                if (!string.IsNullOrEmpty(Country.ImageURL))
                {
                    ImgManager.DeleteImage(Country.ImageURL, AppMainData.DomainName);
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
