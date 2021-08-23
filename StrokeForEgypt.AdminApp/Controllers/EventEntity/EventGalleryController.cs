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
    public class EventGalleryController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;

        public EventGalleryController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor)
        {
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public IActionResult Index(int Id, int Fk_Event, bool ProfileLayOut = false)
        {
            EventGalleryFilter Filter = new EventGalleryFilter
            {
                Id = Id,
                Fk_Event = Fk_Event
            };

            ViewData["ProfileLayOut"] = ProfileLayOut;

            return View("~/Views/EventEntity/EventGallery/Index.cshtml", Filter);

        }

        [HttpPost]
        public async Task<IActionResult> LoadTable([FromBody] EventGalleryFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<EventGallery> result = await _UnitOfWork.EventGallery.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id)
                                                                                            && (dtParameters.Fk_Event == 0 || a.Fk_Event == dtParameters.Fk_Event));

            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.Id.ToString().Contains(searchBy.ToLower())
                                        || a.FileName.ToLower().Contains(searchBy.ToLower())
                                        || a.CreatedAt.ToString().Contains(searchBy.ToLower()))
                               .ToList();
            }

            DataTableManager<EventGallery> DataTableManager = new DataTableManager<EventGallery>();

            DataTableResult<EventGallery> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.EventGallery.Count());

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
            EventGallery EventGallery = await _UnitOfWork.EventGallery.GetByID(id);

            if (EventGallery == null)
            {
                return NotFound();
            }

            return View("~/Views/EventEntity/EventGallery/Details.cshtml", EventGallery);
        }


        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> Uploud(int Id)
        {
            ImgManager ImgManager = new ImgManager(AppMainData.WebRootPath);

            IFormFile Images = HttpContext.Request.Form.Files["file"];
            if (Images != null)
            {
                EventGallery EventGallery = new EventGallery
                {
                    Fk_Event = Id,
                    FileType = Images.ContentType,
                    FileName = Images.FileName,
                    FileLength = Images.Length,
                    FileURL = AppMainData.DomainName

                };
                EventGallery.CreatedBy = _Session.GetString("FullName");
                _UnitOfWork.EventGallery.CreateEntity(EventGallery);
                await _UnitOfWork.EventGallery.Save();

                string ImageURL = await ImgManager.UploudImage(AppMainData.DomainName, EventGallery.Id.ToString(), Images, "Uploud/EventGallery");

                if (!string.IsNullOrEmpty(ImageURL))
                {
                    EventGallery.FileURL = ImageURL;
                    _UnitOfWork.EventGallery.UpdateEntity(EventGallery);
                    await _UnitOfWork.EventGallery.Save();
                }
            }
            return Ok();
        }



        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> Delete(int id)
        {
            EventGallery EventGallery = await _UnitOfWork.EventGallery.GetByID(id);

            if (EventGallery == null)
            {
                return NotFound();
            }

            return View("~/Views/EventEntity/EventGallery/Delete.cshtml", EventGallery);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            EventGallery EventGallery = await _UnitOfWork.EventGallery.GetByID(id);

            if (!string.IsNullOrEmpty(EventGallery.FileURL))
            {
                ImgManager ImgManager = new ImgManager(AppMainData.WebRootPath);
                ImgManager.DeleteImage(EventGallery.FileURL, AppMainData.DomainName);
            }

            _UnitOfWork.EventGallery.DeleteEntity(EventGallery);
            await _UnitOfWork.EventGallery.Save();


            return RedirectToAction("Profile", "Event", new { id = EventGallery.Fk_Event,returnItem = (int)EventProfileItems.EventGallery });
        }
    }
}
