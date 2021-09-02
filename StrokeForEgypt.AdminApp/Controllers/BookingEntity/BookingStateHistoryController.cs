using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StrokeForEgypt.AdminApp.Filters;
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
    public class BookingStateHistoryController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;

        public BookingStateHistoryController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor)
        {
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public IActionResult Index(int Id, int Fk_Booking, bool ProfileLayOut = false)
        {
            BookingStateHistoryFilter Filter = new BookingStateHistoryFilter
            {
                Id = Id,
                Fk_Booking = Fk_Booking
            };

            ViewData["ProfileLayOut"] = ProfileLayOut;

            return View("~/Views/BookingEntity/BookingStateHistory/Index.cshtml", Filter);

        }

        [HttpPost]
        public async Task<IActionResult> LoadTable([FromBody] BookingStateHistoryFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<BookingStateHistory> result = await _UnitOfWork.BookingStateHistory.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id)
                                                                                            && (dtParameters.Fk_Booking == 0 || a.Fk_Booking == dtParameters.Fk_Booking)
                                                                                            ,new List<string> { "BookingState"});

            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.Id.ToString().Contains(searchBy.ToLower())
                                        || a.BookingState.Name.ToLower().Contains(searchBy.ToLower())
                                        || a.CreatedBy.ToLower().Contains(searchBy.ToLower())
                                        || a.CreatedAt.ToString().Contains(searchBy.ToLower()))
                               .ToList();
            }

            result.ForEach(a => {a.BookingState.BookingStateHistories = null; a.BookingState.Bookings = null; });

            DataTableManager<BookingStateHistory> DataTableManager = new DataTableManager<BookingStateHistory>();

            DataTableResult<BookingStateHistory> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.BookingStateHistory.Count());

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
            BookingStateHistory BookingStateHistory = await _UnitOfWork.BookingStateHistory.GetByID(id);

            if (BookingStateHistory == null)
            {
                return NotFound();
            }

            return View("~/Views/BookingEntity/BookingStateHistory/Details.cshtml", BookingStateHistory);
        }


    }
}
