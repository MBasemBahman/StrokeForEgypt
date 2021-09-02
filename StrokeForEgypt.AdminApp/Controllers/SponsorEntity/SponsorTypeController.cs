using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrokeForEgypt.AdminApp.Filters;
using StrokeForEgypt.AdminApp.Services;
using StrokeForEgypt.AdminApp.ViewModel;
using StrokeForEgypt.Common;
using StrokeForEgypt.Entity.SponsorEntity;
using StrokeForEgypt.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static StrokeForEgypt.Common.EnumData;
namespace StrokeForEgypt.AdminApp.Controllers.SponsorEntity
{
    public class SponsorTypeController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;
        private readonly CommonLocalizationService _CommonLocalizationService;

        public SponsorTypeController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor, CommonLocalizationService CommonLocalizationService)
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

            return View("~/Views/SponsorEntity/SponsorType/Index.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> LoadTable([FromBody] CommonFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<SponsorType> result = await _UnitOfWork.SponsorType.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id));



            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.Name.ToLower().Contains(searchBy.ToLower())
                                        || a.IsActive.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.Order.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.Id.ToString().ToLower().Contains(searchBy.ToLower()))
                               .ToList();
            }

            DataTableManager<SponsorType> DataTableManager = new();

            DataTableResult<SponsorType> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.SponsorType.Count());

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
            SponsorType SponsorType = await _UnitOfWork.SponsorType.GetByID(id);

            if (SponsorType == null)
            {
                return NotFound();
            }

            return View("~/Views/SponsorEntity/SponsorType/Details.cshtml", SponsorType);
        }


        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id = 0)
        {
            SponsorType SponsorType = new();

            if (id > 0)
            {
                SponsorType = await _UnitOfWork.SponsorType.GetByID(id);
                if (SponsorType == null)
                {
                    return NotFound();
                }
            }

            return View("~/Views/SponsorEntity/SponsorType/CreateOrEdit.cshtml", SponsorType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id, SponsorType SponsorType)
        {
            if (id != SponsorType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0)
                    {
                        SponsorType.CreatedBy = Request.Cookies["FullName"];
                        _UnitOfWork.SponsorType.CreateEntity(SponsorType);
                        await _UnitOfWork.SponsorType.Save();
                    }
                    else
                    {
                        SponsorType Data = await _UnitOfWork.SponsorType.GetByID(id);

                        SponsorType.LastModifiedBy = Request.Cookies["FullName"];

                        _Mapper.Map(SponsorType, Data);

                        _UnitOfWork.SponsorType.UpdateEntity(Data);

                        await _UnitOfWork.SponsorType.Save();

                    }



                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_UnitOfWork.SponsorType.Any(a => a.Id == id))
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

            return View("~/Views/SponsorEntity/SponsorType/CreateOrEdit.cshtml", SponsorType);
        }


        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> Delete(int id)
        {
            SponsorType SponsorType = await _UnitOfWork.SponsorType.GetByID(id);

            if (SponsorType == null)
            {
                return NotFound();
            }

            if ((_UnitOfWork.Sponsor.Any(a => a.Fk_SponsorType == id)))
            {
                ViewBag.CanDelete = false;
            }

            return View("~/Views/SponsorEntity/SponsorType/Delete.cshtml", SponsorType);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            SponsorType SponsorType = await _UnitOfWork.SponsorType.GetByID(id);

            if (!(_UnitOfWork.Sponsor.Any(a => a.Fk_SponsorType == id)))
            {
                _UnitOfWork.SponsorType.DeleteEntity(SponsorType);

                await _UnitOfWork.SponsorType.Save();

            }

            return RedirectToAction(nameof(Index));
        }
    }
}
