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
    public class BookingMemberController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;

        public BookingMemberController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor)
        {
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public IActionResult Index(int Id, int Fk_Booking, bool ProfileLayOut = false)
        {
            BookingMemberFilter Filter = new BookingMemberFilter
            {
                Id = Id,
                Fk_Booking = Fk_Booking,
            };
            ViewData["ProfileLayOut"] = ProfileLayOut;
            return View("~/Views/BookingEntity/BookingMember/Index.cshtml", Filter);
        }

        [HttpPost]
        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public async Task<IActionResult> LoadTable([FromBody] BookingMemberFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<BookingMember> result = await _UnitOfWork.BookingMember.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id)
                                                                                         && (dtParameters.Fk_Booking == 0 || a.Fk_Booking == dtParameters.Fk_Booking)
                                                                                         , new List<string> { "Gender", "City" });


            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.Gender.Name.ToLower().Contains(searchBy.ToLower())
                                        || a.City.Name.ToLower().Contains(searchBy.ToLower())
                                        || a.FullName.ToLower().Contains(searchBy.ToLower())
                                        || a.DateOfBirth.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.Phone.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.Email.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.IsActive.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.Id.ToString().ToLower().Contains(searchBy.ToLower()))
                               .ToList();
            }

            result.ForEach(a => { a.Gender.BookingMembers = null; a.City.BookingMembers = null; });

            DataTableManager<BookingMember> DataTableManager = new DataTableManager<BookingMember>();

            DataTableResult<BookingMember> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.BookingMember.Count());

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
            BookingMember BookingMember = await _UnitOfWork.BookingMember.GetByID(id);

            if (BookingMember == null)
            {
                return NotFound();
            }

            return View("~/Views/BookingEntity/BookingMember/Details.cshtml", BookingMember);
        }

        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id = 0, int Fk_Booking = 0)
        {
            BookingMember BookingMember = new BookingMember();

            if (id == 0 && !_UnitOfWork.Booking.Any(a => a.Id == Fk_Booking))
            {
                return NotFound();
            }

            if (id > 0)
            {
                BookingMember = await _UnitOfWork.BookingMember.GetByID(id);
                if (BookingMember == null)
                {
                    return NotFound();
                }
            }
            else
            {
                BookingMember.Fk_Booking = Fk_Booking;
            }

            return View("~/Views/BookingEntity/BookingMember/CreateOrEdit.cshtml", GetViewModel(BookingMember));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id, BookingMember BookingMember, List<int> Fk_Activities = null)
        {
            if (id != BookingMember.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0)
                    {
                        BookingMember.CreatedBy = _Session.GetString("FullName");

                        BookingMember.BookingMemberActivities = new List<BookingMemberActivity>();

                        BookingMember =  _UnitOfWork.BookingMemberActivity.CreateEntity(BookingMember, Fk_Activities);

                        _UnitOfWork.BookingMember.CreateEntity(BookingMember);
                        await _UnitOfWork.BookingMember.Save();
                    }
                    else
                    {
                        BookingMember Data = await _UnitOfWork.BookingMember.GetByID(id);


                        BookingMember.LastModifiedBy = _Session.GetString("FullName");

                        _Mapper.Map(BookingMember, Data);

                        Data.BookingMemberActivities = await _UnitOfWork.BookingMemberActivity.GetAll(a => a.Fk_BookingMember == id);

                        Data = _UnitOfWork.BookingMemberActivity.UpdateEntity(BookingMember, Data.BookingMemberActivities.Select(a => a.Fk_EventActivity).ToList(), Fk_Activities);
                       
                        _UnitOfWork.BookingMember.UpdateEntity(Data);
                        await _UnitOfWork.BookingMember.Save();
                    }
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!_UnitOfWork.BookingMember.Any(a => a.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Profile", "Booking", new { id = BookingMember.Fk_Booking, returnItem = (int)BookingProfileItems.BookingMember });
            }

            return View("~/Views/BookingEntity/BookingMember/CreateOrEdit.cshtml", GetViewModel(BookingMember));

        }

        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> Delete(int id)
        {
            BookingMember BookingMember = await _UnitOfWork.BookingMember.GetByID(id);

            if (BookingMember == null)
            {
                return NotFound();
            }

            return View("~/Views/BookingEntity/BookingMember/Delete.cshtml", BookingMember);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            BookingMember BookingMember = await _UnitOfWork.BookingMember.GetByID(id);

            _UnitOfWork.BookingMember.DeleteEntity(BookingMember);
            await _UnitOfWork.BookingMember.Save();


            return RedirectToAction("Profile", "Booking", new { id = BookingMember.Fk_Booking, returnItem = (int)BookingProfileItems.BookingMember });
        }

        public BookingMemberViewModel GetViewModel(BookingMember BookingMember, List<int> Fk_Activities = null)
        {
            BookingMemberViewModel Model = new BookingMemberViewModel()
            {
                BookingMember = BookingMember,

            };

            if (BookingMember.Id > 0)
            {
                if (Fk_Activities == null)
                {
                    List<int> Activities = _UnitOfWork.BookingMemberActivity.GetAll(a => a.Fk_BookingMember == BookingMember.Id).Result.Select(a => a.Fk_EventActivity).ToList();

                    if (Activities.Any())
                    {
                        Model.Fk_Activities = Activities;
                    }
                }

            }
            else
            {
                Model.Fk_Activities = Fk_Activities ?? Model.Fk_Activities;
            }

            return Model;
        }

    }
}
