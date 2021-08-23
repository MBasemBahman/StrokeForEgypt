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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static StrokeForEgypt.Common.EnumData;

namespace StrokeForEgypt.AdminApp.Controllers.EventEntity
{
    public class EventController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;
        private readonly CommonLocalizationService _CommonLocalizationService;

        public EventController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor, CommonLocalizationService CommonLocalizationService)
        {
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
            _CommonLocalizationService = CommonLocalizationService;
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public IActionResult Index(int Id)
        {
            EventFilter Filter = new EventFilter
            {
                Id = Id
            };

            return View("~/Views/EventEntity/Event/Index.cshtml", Filter);
        }

        [HttpPost]
        public async Task<IActionResult> LoadTable([FromBody] EventFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<Event> result = await _UnitOfWork.Event.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id));



            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.Title.ToLower().Contains(searchBy.ToLower())
                                        || a.ShortDescription.ToLower().Contains(searchBy.ToLower())
                                        || a.Address.ToLower().Contains(searchBy.ToLower())
                                        || a.FromDate.ToString().Contains(searchBy.ToLower())
                                        || a.ToDate.ToString().Contains(searchBy.ToLower())
                                        || a.IsActive.ToString().ToLower().Contains(searchBy.ToLower())
                                        || (!string.IsNullOrEmpty(a.ImageURL) && a.ImageURL.ToLower().Contains(searchBy.ToLower()))
                                        || a.Id.ToString().ToLower().Contains(searchBy.ToLower()))
                               .ToList();
            }

            DataTableManager<Event> DataTableManager = new();

            DataTableResult<Event> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.Event.Count());

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
            Event Event = await _UnitOfWork.Event.GetByID(id);

            if (Event == null)
            {
                return NotFound();
            }

            return View("~/Views/EventEntity/Event/Details.cshtml", Event);
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public async Task<IActionResult> Profile(int id, int returnItem = (int)EventProfileItems.EventActivity)
        {
            Event Event = await _UnitOfWork.Event.GetByID(id);

            if (Event == null)
            {
                return NotFound();
            }
            ViewData["returnItem"] = returnItem;
            return View("~/Views/EventEntity/Event/Profile.cshtml", Event);
        }


        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id = 0, bool IsProfile = false)
        {
            Event Event = new();

            if (id > 0)
            {
                Event = await _UnitOfWork.Event.GetByID(id);
                if (Event == null)
                {
                    return NotFound();
                }
            }

            ViewData["IsProfile"] = IsProfile;
            return View("~/Views/EventEntity/Event/CreateOrEdit.cshtml", Event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id, Event Event, bool IsProfile)
        {
            if (id != Event.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0)
                    {
                        Event.CreatedBy = Request.Cookies["FullName"];

                        _UnitOfWork.Event.CreateEntity(Event);
                        await _UnitOfWork.Event.Save();
                    }
                    else
                    {
                        Event Data = await _UnitOfWork.Event.GetByID(id);

                        Event.LastModifiedBy = Request.Cookies["FullName"];

                        _Mapper.Map(Event, Data);

                        _UnitOfWork.Event.UpdateEntity(Data);

                        await _UnitOfWork.Event.Save();

                        Event = Data;
                    }


                    IFormFile ImageFile = HttpContext.Request.Form.Files["ImageFile"];

                    if (ImageFile != null)
                    {
                        ImgManager ImgManager = new ImgManager(AppMainData.WebRootPath);

                        string FileURL = await ImgManager.UploudImage(AppMainData.DomainName, Event.Id.ToString(), ImageFile, "Uploud/Event");

                        if (!string.IsNullOrEmpty(FileURL))
                        {
                            if (!string.IsNullOrEmpty(Event.ImageURL))
                            {
                                ImgManager.DeleteImage(Event.ImageURL, AppMainData.DomainName);
                            }
                            Event.ImageURL = FileURL;
                            _UnitOfWork.Event.UpdateEntity(Event);
                            await _UnitOfWork.Event.Save();
                        }
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_UnitOfWork.Event.Any(a => a.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                if (IsProfile)
                {
                    return RedirectToAction(nameof(Profile), new { id = Event.Id });

                }
                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/EventEntity/Event/CreateOrEdit.cshtml", Event);
        }


        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> Delete(int id)
        {
            Event Event = await _UnitOfWork.Event.GetByID(id);

            if (Event == null)
            {
                return NotFound();
            }

            return View("~/Views/EventEntity/Event/Delete.cshtml", Event);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Event Event = await _UnitOfWork.Event.GetByID(id);
            if (!string.IsNullOrEmpty(Event.ImageURL))
            {
                ImgManager ImgManager = new ImgManager(AppMainData.WebRootPath);
                ImgManager.DeleteImage(Event.ImageURL, AppMainData.DomainName);
            }
            _UnitOfWork.Event.DeleteEntity(Event);
            await _UnitOfWork.Event.Save();

            return RedirectToAction(nameof(Index));
        }
    }
}
