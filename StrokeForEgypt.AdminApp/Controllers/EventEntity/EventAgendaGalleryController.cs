using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StrokeForEgypt.AdminApp.Filters;
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
    public class EventAgendaGalleryController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;

        public EventAgendaGalleryController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor)
        {
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public IActionResult Index(int Id, int Fk_EventAgenda, bool ProfileLayOut = false)
        {
            EventAgendaGalleryFilter Filter = new EventAgendaGalleryFilter
            {
                Id = Id,
                Fk_EventAgenda = Fk_EventAgenda
            };

            ViewData["ProfileLayOut"] = ProfileLayOut;

            return View("~/Views/EventEntity/EventAgendaGallery/Index.cshtml", Filter);

        }

        [HttpPost]
        public async Task<IActionResult> LoadTable([FromBody] EventAgendaGalleryFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<EventAgendaGallery> result = await _UnitOfWork.EventAgendaGallery.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id)
                                                                                            && (dtParameters.Fk_EventAgenda == 0 || a.Fk_EventAgenda == dtParameters.Fk_EventAgenda));

            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.Id.ToString().Contains(searchBy.ToLower())
                                        || a.FileURL == null || a.FileURL.ToLower().Contains(searchBy.ToLower())
                                        || a.CreatedAt.ToString().Contains(searchBy.ToLower())
                                        || a.Id.ToString().ToLower().Contains(searchBy.ToLower()))
                               .ToList();
            }

            DataTableManager<EventAgendaGallery> DataTableManager = new DataTableManager<EventAgendaGallery>();

            DataTableResult<EventAgendaGallery> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.EventAgendaGallery.Count());

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
            EventAgendaGallery EventAgendaGallery = await _UnitOfWork.EventAgendaGallery.GetByID(id);

            if (EventAgendaGallery == null)
            {
                return NotFound();
            }

            return View("~/Views/EventEntity/EventAgendaGallery/Details.cshtml", EventAgendaGallery);
        }


        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> Uploud(int Id)
        {
            ImgManager ImgManager = new ImgManager(AppMainData.WebRootPath);

            IFormFile Images = HttpContext.Request.Form.Files["file"];
            if (Images != null)
            {
                EventAgendaGallery EventAgendaGallery = new EventAgendaGallery
                {
                    Fk_EventAgenda = Id,
                    FileType = Images.ContentType,
                    FileName = Images.FileName,
                    FileLength = Images.Length,
                    FileURL = AppMainData.DomainName

                };
                EventAgendaGallery.CreatedBy = _Session.GetString("FullName");
                _UnitOfWork.EventAgendaGallery.CreateEntity(EventAgendaGallery);
                await _UnitOfWork.EventGallery.Save();

                string ImageURL = await ImgManager.UploudImage(AppMainData.DomainName, EventAgendaGallery.Id.ToString(), Images, "Uploud/EventAgendaGallery");

                if (!string.IsNullOrEmpty(ImageURL))
                {
                    EventAgendaGallery.FileURL = ImageURL;
                    _UnitOfWork.EventAgendaGallery.UpdateEntity(EventAgendaGallery);
                    await _UnitOfWork.EventAgendaGallery.Save();
                }
            }
            return Ok();
        }



        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> Delete(int id)
        {
            EventAgendaGallery EventAgendaGallery = await _UnitOfWork.EventAgendaGallery.GetByID(id);

            if (EventAgendaGallery == null)
            {
                return NotFound();
            }

            return View("~/Views/EventEntity/EventAgendaGallery/Delete.cshtml", EventAgendaGallery);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            EventAgendaGallery EventAgendaGallery = await _UnitOfWork.EventAgendaGallery.GetByID(id);

            if (!string.IsNullOrEmpty(EventAgendaGallery.FileURL))
            {
                ImgManager ImgManager = new ImgManager(AppMainData.WebRootPath);
                ImgManager.DeleteImage(EventAgendaGallery.FileURL, AppMainData.DomainName);
            }

            _UnitOfWork.EventAgendaGallery.DeleteEntity(EventAgendaGallery);
            await _UnitOfWork.EventGallery.Save();


            return RedirectToAction("Index", "EventAgendaGallery", new { Fk_EventAgenda = EventAgendaGallery.Fk_EventAgenda });
        }
    }
}
