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
    public class EventActivityController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;

        public EventActivityController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor)
        {
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public IActionResult Index(int Id, int Fk_Event, bool ProfileLayOut = false)
        {
            EventActivityFilter Filter = new EventActivityFilter
            {
                Id = Id,
                Fk_Event = Fk_Event,
            };
            ViewData["ProfileLayOut"] = ProfileLayOut;
            return View("~/Views/EventEntity/EventActivity/Index.cshtml", Filter);
        }

        [HttpPost]
        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public async Task<IActionResult> LoadTable([FromBody] EventActivityFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<EventActivity> result = await _UnitOfWork.EventActivity.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id)
                                                                                         && (dtParameters.Fk_Event == 0 || a.Fk_Event == dtParameters.Fk_Event));


            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.Name.ToLower().Contains(searchBy.ToLower())
                                        || a.IsActive.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.Id.ToString().ToLower().Contains(searchBy.ToLower()))
                               .ToList();
            }

            DataTableManager<EventActivity> DataTableManager = new DataTableManager<EventActivity>();

            DataTableResult<EventActivity> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.EventActivity.Count());

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
            EventActivity EventActivity = await _UnitOfWork.EventActivity.GetByID(id);

            if (EventActivity == null)
            {
                return NotFound();
            }

            return View("~/Views/EventEntity/EventActivity/Details.cshtml", EventActivity);
        }

        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id = 0, int Fk_Event = 0)
        {
            EventActivity EventActivity = new EventActivity();

            if (id == 0 && !_UnitOfWork.Event.Any(a => a.Id == Fk_Event))
            {
                return NotFound();
            }

            if (id > 0)
            {
                EventActivity = await _UnitOfWork.EventActivity.GetByID(id);
                if (EventActivity == null)
                {
                    return NotFound();
                }
            }
            else
            {
                EventActivity.Fk_Event = Fk_Event;
            }

            return View("~/Views/EventEntity/EventActivity/CreateOrEdit.cshtml", EventActivity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id, EventActivity EventActivity)
        {
            if (id != EventActivity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0)
                    {
                        EventActivity.CreatedBy = _Session.GetString("FullName");
                        _UnitOfWork.EventActivity.CreateEntity(EventActivity);
                        await _UnitOfWork.EventActivity.Save();
                    }
                    else
                    {
                        EventActivity Data = await _UnitOfWork.EventActivity.GetByID(id);

                        EventActivity.LastModifiedBy = _Session.GetString("FullName");

                        _Mapper.Map(EventActivity, Data);


                        _UnitOfWork.EventActivity.UpdateEntity(Data);
                        await _UnitOfWork.EventActivity.Save();

                        EventActivity = Data;
                    }

                    IFormFile ImageFile = HttpContext.Request.Form.Files["ImageFile"];

                    if (ImageFile != null)
                    {
                        ImgManager ImgManager = new ImgManager(AppMainData.WebRootPath);

                        string FileURL = await ImgManager.UploudImage(AppMainData.DomainName, EventActivity.Id.ToString(), ImageFile, "Uploud/EventActivity");

                        if (!string.IsNullOrEmpty(FileURL))
                        {
                            if (!string.IsNullOrEmpty(EventActivity.ImageURL))
                            {
                                ImgManager.DeleteImage(EventActivity.ImageURL, AppMainData.DomainName);
                            }
                            EventActivity.ImageURL = FileURL;
                            _UnitOfWork.EventActivity.UpdateEntity(EventActivity);
                            await _UnitOfWork.EventActivity.Save();
                        }
                    }
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!_UnitOfWork.EventActivity.Any(a => a.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("Profile", "Event", new { id = EventActivity.Fk_Event, returnItem = (int)EventProfileItems.EventActivity });
            }

            return View("~/Views/EventEntity/EventActivity/CreateOrEdit.cshtml", EventActivity);

        }

        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> Delete(int id)
        {
            EventActivity EventActivity = await _UnitOfWork.EventActivity.GetByID(id);

            if (EventActivity == null)
            {
                return NotFound();
            }
            if (_UnitOfWork.BookingMemberActivity.Any(a => a.Fk_EventActivity == id))
            {
                ViewBag.CanDelete = false;
            }

            return View("~/Views/EventEntity/EventActivity/Delete.cshtml", EventActivity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            EventActivity EventActivity = await _UnitOfWork.EventActivity.GetByID(id);
            if (!_UnitOfWork.BookingMemberActivity.Any(a => a.Fk_EventActivity == id))
            {
                if (!string.IsNullOrEmpty(EventActivity.ImageURL))
                {
                    ImgManager ImgManager = new ImgManager(AppMainData.WebRootPath);
                    ImgManager.DeleteImage(EventActivity.ImageURL, AppMainData.DomainName);
                }

                _UnitOfWork.EventActivity.DeleteEntity(EventActivity);
                await _UnitOfWork.EventActivity.Save();
            }

            return RedirectToAction("Profile", "Event", new { id = EventActivity.Fk_Event, returnItem = (int)EventProfileItems.EventActivity });
        }
    }
}
