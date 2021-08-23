using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrokeForEgypt.AdminApp.Filters;
using StrokeForEgypt.AdminApp.Services;
using StrokeForEgypt.AdminApp.ViewModel;
using StrokeForEgypt.Common;
using StrokeForEgypt.Entity.EventEntity;
using StrokeForEgypt.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static StrokeForEgypt.Common.EnumData;

namespace StrokeForEgypt.AdminApp.Controllers.EventEntity
{
    public class EventPackageController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;

        public EventPackageController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor)
        {
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public IActionResult Index(int Id, int Fk_Event, bool ProfileLayOut = false)
        {
            EventPackageFilter Filter = new EventPackageFilter
            {
                Id = Id,
                Fk_Event = Fk_Event,
            };
            ViewData["ProfileLayOut"] = ProfileLayOut;
            return View("~/Views/EventEntity/EventPackage/Index.cshtml", Filter);
        }

        [HttpPost]
        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public async Task<IActionResult> LoadTable([FromBody] EventPackageFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<EventPackage> result = await _UnitOfWork.EventPackage.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id)
                                                                                         && (dtParameters.Fk_Event == 0 || a.Fk_Event == dtParameters.Fk_Event));


            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.Title.ToLower().Contains(searchBy.ToLower())
                                        || a.OriginalPrice.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.Price.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.ExtraFees.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.IsActive.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.Id.ToString().ToLower().Contains(searchBy.ToLower()))
                               .ToList();
            }

            DataTableManager<EventPackage> DataTableManager = new DataTableManager<EventPackage>();

            DataTableResult<EventPackage> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.EventPackage.Count());

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
            EventPackage EventPackage = await _UnitOfWork.EventPackage.GetByID(id);

            if (EventPackage == null)
            {
                return NotFound();
            }

            return View("~/Views/EventEntity/EventPackage/Details.cshtml", EventPackage);
        }

        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id = 0, int Fk_Event = 0)
        {
            EventPackage EventPackage = new EventPackage();

            if (id == 0 && !_UnitOfWork.Event.Any(a => a.Id == Fk_Event))
            {
                return NotFound();
            }

            if (id > 0)
            {
                EventPackage = await _UnitOfWork.EventPackage.GetByID(id);
                if (EventPackage == null)
                {
                    return NotFound();
                }
            }
            else
            {
                EventPackage.Fk_Event = Fk_Event;
            }

            return View("~/Views/EventEntity/EventPackage/CreateOrEdit.cshtml", EventPackage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id, EventPackage EventPackage)
        {
            if (id != EventPackage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0)
                    {
                        EventPackage.CreatedBy = _Session.GetString("FullName");
                        _UnitOfWork.EventPackage.CreateEntity(EventPackage);
                        await _UnitOfWork.EventPackage.Save();
                    }
                    else
                    {
                        EventPackage Data = await _UnitOfWork.EventPackage.GetByID(id);

                        EventPackage.LastModifiedBy = _Session.GetString("FullName");

                        _Mapper.Map(EventPackage, Data);


                        _UnitOfWork.EventPackage.UpdateEntity(Data);
                        await _UnitOfWork.EventPackage.Save();
                    }
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!_UnitOfWork.EventPackage.Any(a => a.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Profile", "Event", new { id = EventPackage.Fk_Event, returnItem = (int)EventProfileItems.EventPackage });
            }

            return View("~/Views/EventEntity/EventPackage/CreateOrEdit.cshtml", EventPackage);

        }

        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> Delete(int id)
        {
            EventPackage EventPackage = await _UnitOfWork.EventPackage.GetByID(id);

            if (EventPackage == null)
            {
                return NotFound();
            }
            if(_UnitOfWork.Booking.Any(a => a.Fk_EventPackage == id))
            {
                ViewBag.CanDelete = false;
            }

            return View("~/Views/EventEntity/EventPackage/Delete.cshtml", EventPackage);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            EventPackage EventPackage = await _UnitOfWork.EventPackage.GetByID(id);
            if (!_UnitOfWork.Booking.Any(a => a.Fk_EventPackage == id))
            {

                _UnitOfWork.EventPackage.DeleteEntity(EventPackage);
                await _UnitOfWork.EventPackage.Save();
            }

            return RedirectToAction("Profile", "Event", new { id = EventPackage.Fk_Event, returnItem = (int)EventProfileItems.EventPackage });
        }
    }
}
