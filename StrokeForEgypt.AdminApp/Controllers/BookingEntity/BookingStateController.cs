using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrokeForEgypt.AdminApp.Filters;
using StrokeForEgypt.AdminApp.Services;
using StrokeForEgypt.AdminApp.ViewModel;
using StrokeForEgypt.Common;
using StrokeForEgypt.Entity.BookingEntity;
using StrokeForEgypt.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static StrokeForEgypt.Common.EnumData;

namespace StrokeForEgypt.AdminApp.Controllers.BookingEntity
{
    public class BookingStateController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;
        private readonly CommonLocalizationService _CommonLocalizationService;

        public BookingStateController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor, CommonLocalizationService CommonLocalizationService)
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

            return View("~/Views/BookingEntity/BookingState/Index.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> LoadTable([FromBody] CommonFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<BookingState> result = await _UnitOfWork.BookingState.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id));



            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.Name.ToLower().Contains(searchBy.ToLower())
                                        || a.IsActive.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.Order.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.Id.ToString().ToLower().Contains(searchBy.ToLower()))
                               .ToList();
            }

            DataTableManager<BookingState> DataTableManager = new();

            DataTableResult<BookingState> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.BookingState.Count());

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
            BookingState BookingState = await _UnitOfWork.BookingState.GetByID(id);

            if (BookingState == null)
            {
                return NotFound();
            }

            return View("~/Views/BookingEntity/BookingState/Details.cshtml", BookingState);
        }


        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id = 0)
        {
            BookingState BookingState = new();

            if (id > 0)
            {
                BookingState = await _UnitOfWork.BookingState.GetByID(id);
                if (BookingState == null)
                {
                    return NotFound();
                }
            }

            return View("~/Views/BookingEntity/Country/CreateOrEdit.cshtml", BookingState);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id, BookingState BookingState)
        {
            if (id != BookingState.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0)
                    {
                        BookingState.CreatedBy = Request.Cookies["FullName"];
                        _UnitOfWork.BookingState.CreateEntity(BookingState);
                        await _UnitOfWork.BookingState.Save();
                    }
                    else
                    {
                        BookingState Data = await _UnitOfWork.BookingState.GetByID(id);

                        BookingState.LastModifiedBy = Request.Cookies["FullName"];

                        _Mapper.Map(BookingState, Data);

                        _UnitOfWork.BookingState.UpdateEntity(Data);

                        await _UnitOfWork.BookingState.Save();

                    }



                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_UnitOfWork.BookingState.Any(a => a.Id == id))
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

            return View("~/Views/BookingEntity/BookingState/CreateOrEdit.cshtml", BookingState);
        }


        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> Delete(int id)
        {
            BookingState BookingState = await _UnitOfWork.BookingState.GetByID(id);

            if (BookingState == null)
            {
                return NotFound();
            }

            if ((_UnitOfWork.Booking.Any(a => a.Fk_BookingState == id) ||
                _UnitOfWork.BookingStateHistory.Any(a => a.Fk_BookingState == id)))
            {
                ViewBag.CanDelete = false;
            }

            return View("~/Views/BookingEntity/BookingState/Delete.cshtml", BookingState);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            BookingState BookingState = await _UnitOfWork.BookingState.GetByID(id);

            if (!(_UnitOfWork.Booking.Any(a => a.Fk_BookingState == id) ||
                _UnitOfWork.BookingStateHistory.Any(a => a.Fk_BookingState == id)))
            {
                _UnitOfWork.BookingState.DeleteEntity(BookingState);

                await _UnitOfWork.BookingState.Save();

            }

            return RedirectToAction(nameof(Index));
        }
    }
}
