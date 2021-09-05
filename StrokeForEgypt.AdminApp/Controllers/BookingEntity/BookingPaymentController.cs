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
    public class BookingPaymentController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;

        public BookingPaymentController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor)
        {
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public IActionResult Index(int Id, int Fk_Booking, int Fk_Account, bool ProfileLayOut = false)
        {
            BookingPaymentFilter Filter = new BookingPaymentFilter
            {
                Id = Id,
                Fk_Booking = Fk_Booking,
                Fk_Account = Fk_Account
            };

            ViewData["ProfileLayOut"] = ProfileLayOut;

            return View("~/Views/BookingEntity/BookingPayment/Index.cshtml", Filter);

        }

        [HttpPost]
        public async Task<IActionResult> LoadTable([FromBody] BookingPaymentFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<BookingPayment> result = await _UnitOfWork.BookingPayment.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id) &&
                                                                                       (dtParameters.Fk_Booking == 0 || a.Fk_Booking == dtParameters.Fk_Booking) &&
                                                                                       (dtParameters.Fk_Account == 0 || a.Booking.Fk_Account == dtParameters.Fk_Account));

            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.Id.ToString().Contains(searchBy.ToLower())
                                        || a.Type.ToLower().Contains(searchBy.ToLower())
                                        || a.OrderAmount.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.PaymentAmount.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.FawryFees.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.PaymentMethod.ToLower().Contains(searchBy.ToLower())
                                        || a.OrderStatus.ToLower().Contains(searchBy.ToLower())
                                        || a.CustomerMobile.ToLower().Contains(searchBy.ToLower())
                                        || a.CustomerMail.ToLower().Contains(searchBy.ToLower())
                                        || a.StatusCode.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.StatusDescription.ToLower().Contains(searchBy.ToLower())
                                        || a.CreatedBy.ToLower().Contains(searchBy.ToLower())
                                        || a.CreatedAt.ToString().Contains(searchBy.ToLower()))
                               .ToList();
            }


            DataTableManager<BookingPayment> DataTableManager = new DataTableManager<BookingPayment>();

            DataTableResult<BookingPayment> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.BookingPayment.Count());

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
    }
}
