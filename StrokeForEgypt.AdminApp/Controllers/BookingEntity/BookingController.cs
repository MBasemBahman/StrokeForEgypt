using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class BookingController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;

        public BookingController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor)
        {
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public IActionResult Index(int Id, int Fk_Account,int Fk_BookingState,int Fk_EventPackage, bool ProfileLayOut = false)
        {
            BookingFilter Filter = new BookingFilter
            {
                Id = Id,
                Fk_Account = Fk_Account,
                Fk_BookingState = Fk_BookingState,
                Fk_EventPackage  = Fk_EventPackage
            };
            ViewData["ProfileLayOut"] = ProfileLayOut;
            return View("~/Views/BookingEntity/Booking/Index.cshtml", Filter);
        }

        [HttpPost]
        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public async Task<IActionResult> LoadTable([FromBody] BookingFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<Booking> result = await _UnitOfWork.Booking.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id)
                                                                                         && (dtParameters.Fk_EventPackage == 0 || a.Fk_EventPackage == dtParameters.Fk_EventPackage)
                                                                                         && (dtParameters.Fk_BookingState == 0 || a.Fk_BookingState == dtParameters.Fk_BookingState)
                                                                                         && (dtParameters.Fk_Account == 0 || a.Fk_Account == dtParameters.Fk_Account)
                                                                                         , new List<string> { "EventPackage","BookingState","Account" });


            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.BookingState.Name.ToLower().Contains(searchBy.ToLower())
                                        || a.Account.FullName.ToLower().Contains(searchBy.ToLower())
                                        || a.DaysCount.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.PersonCount.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.TotalPrice.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.IsActive.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.Id.ToString().ToLower().Contains(searchBy.ToLower()))
                               .ToList();
            }

            result.ForEach(a => { a.Account.Bookings = null; a.EventPackage.Bookings = null;a.BookingState.Bookings = null;a.BookingState.BookingStateHistories = null; });

            DataTableManager<Booking> DataTableManager = new DataTableManager<Booking>();

            DataTableResult<Booking> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.Booking.Count());

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
            Booking Booking = await _UnitOfWork.Booking.GetByID(id);

            if (Booking == null)
            {
                return NotFound();
            }

            return View("~/Views/BookingEntity/Booking/Details.cshtml", Booking);
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public async Task<IActionResult> Profile(int id,int returnItem = (int)BookingProfileItems.BookingMember)
        {
            Booking Booking = await _UnitOfWork.Booking.GetByID(id);

            if (Booking == null)
            {
                return NotFound();
            }

            ViewData["returnItem"] = returnItem;

            return View("~/Views/BookingEntity/Booking/Profile.cshtml", Booking);
        }

        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id = 0, int Fk_Account = 0)
        {
            Booking Booking = new Booking();

            if (id == 0 && !_UnitOfWork.Account.Any(a => a.Id == Fk_Account))
            {
                return NotFound();
            }

            if (id > 0)
            {
                Booking = await _UnitOfWork.Booking.GetByID(id);
                if (Booking == null)
                {
                    return NotFound();
                }
            }
            else
            {
                Booking.Fk_Account = Fk_Account;
            }

            return View("~/Views/BookingEntity/Booking/CreateOrEdit.cshtml", Booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id, Booking Booking)
        {
            if (id != Booking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0)
                    {
                        Booking.CreatedBy = _Session.GetString("FullName");

                        Booking.BookingStateHistories = new List<BookingStateHistory>();
                        Booking = await _UnitOfWork.Booking.UpdateStateHistory(Booking);

                        _UnitOfWork.Booking.CreateEntity(Booking);
                        await _UnitOfWork.Booking.Save();
                    }
                    else
                    {
                        Booking Data = await _UnitOfWork.Booking.GetByID(id);

                        int OldState = Data.Fk_BookingState;


                        Booking.LastModifiedBy = _Session.GetString("FullName");

                        _Mapper.Map(Booking, Data);

                        Data.BookingStateHistories = await _UnitOfWork.BookingStateHistory.GetAll(a => a.Fk_Booking == id);

                        Data = await _UnitOfWork.Booking.UpdateStateHistory(Data, OldState);

                        _UnitOfWork.Booking.UpdateEntity(Data);
                        await _UnitOfWork.Booking.Save();
                    }
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!_UnitOfWork.Booking.Any(a => a.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Profile", "Booking", new { id = Booking.Id, returnItem = (int)BookingProfileItems.BookingMember });
            }

            return View("~/Views/BookingEntity/Booking/CreateOrEdit.cshtml", Booking);

        }

        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> Delete(int id)
        {
            Booking Booking = await _UnitOfWork.Booking.GetByID(id);

            if (Booking == null)
            {
                return NotFound();
            }
            if(_UnitOfWork.BookingMember.Any(a => a.Fk_Booking == id))
            {
                ViewBag.CanDelete = false;
            }
            return View("~/Views/BookingEntity/Booking/Delete.cshtml", Booking);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Booking Booking = await _UnitOfWork.Booking.GetByID(id);

           if(!_UnitOfWork.BookingMember.Any(a => a.Fk_Booking == id))
            {
                _UnitOfWork.Booking.DeleteEntity(Booking);
                await _UnitOfWork.Booking.Save();
            }

            return RedirectToAction("Profile", "Account", new { id = Booking.Fk_Account, returnItem = (int)AccountProfileItems.Booking });
        }
    }
}
