using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrokeForEgypt.AdminApp.Filters;
using StrokeForEgypt.AdminApp.Services;
using StrokeForEgypt.AdminApp.ViewModel;
using StrokeForEgypt.Common;
using StrokeForEgypt.Entity.MainDataEntity;
using StrokeForEgypt.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static StrokeForEgypt.Common.EnumData;


namespace StrokeForEgypt.AdminApp.Controllers.MainDataEntity
{
    public class GalleryController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;

        public GalleryController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor)
        {
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public IActionResult Index(int Id, bool ProfileLayOut = false)
        {
            CommonFilter Filter = new CommonFilter
            {
                Id = Id,
            };

            ViewData["ProfileLayOut"] = ProfileLayOut;

            return View("~/Views/MainDataEntity/Gallery/Index.cshtml", Filter);

        }

        [HttpPost]
        public async Task<IActionResult> LoadTable([FromBody] CommonFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<Gallery> result = await _UnitOfWork.Gallery.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id));

            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.Id.ToString().Contains(searchBy.ToLower())
                                        || a.FileName.ToLower().Contains(searchBy.ToLower())
                                        || a.FileURL.ToLower().Contains(searchBy.ToLower())
                                        || a.CreatedAt.ToString().Contains(searchBy.ToLower()))
                               .ToList();
            }

            DataTableManager<Gallery> DataTableManager = new DataTableManager<Gallery>();

            DataTableResult<Gallery> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.Gallery.Count());

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
            Gallery Gallery = await _UnitOfWork.Gallery.GetByID(id);

            if (Gallery == null)
            {
                return NotFound();
            }

            return View("~/Views/MainDataEntity/Gallery/Details.cshtml", Gallery);
        }


        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> Uploud()
        {
            ImgManager ImgManager = new ImgManager(AppMainData.WebRootPath);

            IFormFile Images = HttpContext.Request.Form.Files["file"];
            if (Images != null)
            {
                Gallery Gallery = new Gallery
                {
                    FileType = Images.ContentType,
                    FileName = Images.FileName,
                    FileLength = Images.Length,
                    FileURL = AppMainData.DomainName

                };
                Gallery.CreatedBy = _Session.GetString("FullName");
                _UnitOfWork.Gallery.CreateEntity(Gallery);
                await _UnitOfWork.Gallery.Save();

                string ImageURL = await ImgManager.UploudImage(AppMainData.DomainName, Gallery.Id.ToString(), Images, "Uploud/Gallery");

                if (!string.IsNullOrEmpty(ImageURL))
                {
                    Gallery.FileURL = ImageURL;
                    _UnitOfWork.Gallery.UpdateEntity(Gallery);
                    await _UnitOfWork.Gallery.Save();
                }
            }
            return Ok();
        }



        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> Delete(int id)
        {
            Gallery Gallery = await _UnitOfWork.Gallery.GetByID(id);

            if (Gallery == null)
            {
                return NotFound();
            }

            return View("~/Views/MainDataEntity/Gallery/Delete.cshtml", Gallery);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Gallery Gallery = await _UnitOfWork.Gallery.GetByID(id);

            if (!string.IsNullOrEmpty(Gallery.FileURL))
            {
                ImgManager ImgManager = new ImgManager(AppMainData.WebRootPath);
                ImgManager.DeleteImage(Gallery.FileURL, AppMainData.DomainName);
            }

            _UnitOfWork.Gallery.DeleteEntity(Gallery);
            await _UnitOfWork.Gallery.Save();


            return RedirectToAction(nameof(Index));
        }
    }
}
