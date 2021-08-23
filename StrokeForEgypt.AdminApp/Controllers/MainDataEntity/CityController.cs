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
    public class CityController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;
        private readonly CommonLocalizationService _CommonLocalizationService;

        public CityController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor, CommonLocalizationService CommonLocalizationService)
        {
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
            _CommonLocalizationService = CommonLocalizationService;
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public IActionResult Index(int Id, int Fk_Country)
        {
            ViewData["Id"] = Id;
            ViewData["Fk_Country"] = Fk_Country;

            return View("~/Views/MainDataEntity/City/Index.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> LoadTable([FromBody] CityFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<City> result = await _UnitOfWork.City.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id) &&
                                                                                 (dtParameters.Fk_Country == 0 || a.Fk_Country == dtParameters.Fk_Country), new List<string> { "Country" });



            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.Name.ToLower().Contains(searchBy.ToLower())
                                        || a.Country.Name.Contains(searchBy.ToLower())
                                        || a.IsActive.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.Order.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.Id.ToString().ToLower().Contains(searchBy.ToLower()))
                               .ToList();
            }

            result.ForEach(a => { a.Country.Cities = null; });

            DataTableManager<City> DataTableManager = new();

            DataTableResult<City> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.City.Count());

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
            City City = await _UnitOfWork.City.GetFirst(a => a.Id == id, new List<string> { "Country" });

            if (City == null)
            {
                return NotFound();
            }

            return View("~/Views/MainDataEntity/City/Details.cshtml", City);
        }


        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id = 0)
        {
            City City = new();

            if (id > 0)
            {
                City = await _UnitOfWork.City.GetByID(id);
                if (City == null)
                {
                    return NotFound();
                }
            }

            return View("~/Views/MainDataEntity/City/CreateOrEdit.cshtml", City);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id, City City)
        {
            if (id != City.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0)
                    {
                        City.CreatedBy = Request.Cookies["FullName"];
                        _UnitOfWork.City.CreateEntity(City);
                        await _UnitOfWork.City.Save();
                    }
                    else
                    {
                        City Data = await _UnitOfWork.City.GetByID(id);

                        City.LastModifiedBy = Request.Cookies["FullName"];

                        _Mapper.Map(City, Data);

                        _UnitOfWork.City.UpdateEntity(Data);

                        await _UnitOfWork.City.Save();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_UnitOfWork.City.Any(a => a.Id == id))
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

            return View("~/Views/MainDataEntity/City/CreateOrEdit.cshtml", City);
        }


        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> Delete(int id)
        {
            City City = await _UnitOfWork.City.GetByID(id);

            if (City == null)
            {
                return NotFound();
            }

            if (_UnitOfWork.BookingMember.Any(a => a.Fk_City == id))
            {
                ViewBag.CanDelete = false;
            }

            return View("~/Views/MainDataEntity/City/Delete.cshtml", City);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            City City = await _UnitOfWork.City.GetByID(id);

            if ((!_UnitOfWork.BookingMember.Any(a => a.Fk_City == id)))
            {
                _UnitOfWork.City.DeleteEntity(City);

                await _UnitOfWork.City.Save();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
