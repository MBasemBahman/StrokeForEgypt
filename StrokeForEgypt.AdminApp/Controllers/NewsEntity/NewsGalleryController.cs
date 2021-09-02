using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StrokeForEgypt.AdminApp.Filters;
using StrokeForEgypt.AdminApp.ViewModel;
using StrokeForEgypt.Common;
using StrokeForEgypt.Entity.NewsEntity;
using StrokeForEgypt.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static StrokeForEgypt.Common.EnumData;


namespace StrokeForEgypt.AdminApp.Controllers.NewsEntity
{
    public class NewsGalleryController : Controller
    {

        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;

        public NewsGalleryController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor)
        {
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
        }
        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public IActionResult Index(int Id, int Fk_News, bool ProfileLayOut = false)
        {
            NewsGalleryFilter Filter = new NewsGalleryFilter
            {
                Id = Id,
                Fk_News = Fk_News
            };

            ViewData["ProfileLayOut"] = ProfileLayOut;

            return View("~/Views/NewsEntity/NewsGallery/Index.cshtml", Filter);

        }

        [HttpPost]
        public async Task<IActionResult> LoadTable([FromBody] NewsGalleryFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<NewsGallery> result = await _UnitOfWork.NewsGallery.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id)
                                                                                            && (dtParameters.Fk_News == 0 || a.Fk_News == dtParameters.Fk_News));

            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.Id.ToString().Contains(searchBy.ToLower())
                                        || a.FileURL == null || a.FileURL.ToLower().Contains(searchBy.ToLower())
                                        || a.CreatedAt.ToString().Contains(searchBy.ToLower())
                                        || a.Id.ToString().ToLower().Contains(searchBy.ToLower()))
                               .ToList();
            }

            DataTableManager<NewsGallery> DataTableManager = new DataTableManager<NewsGallery>();

            DataTableResult<NewsGallery> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.NewsGallery.Count());

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
            NewsGallery NewsGallery = await _UnitOfWork.NewsGallery.GetByID(id);

            if (NewsGallery == null)
            {
                return NotFound();
            }

            return View("~/Views/NewsEntity/NewsGallery/Details.cshtml", NewsGallery);
        }


        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> Uploud(int Id)
        {
            ImgManager ImgManager = new ImgManager(AppMainData.WebRootPath);

            IFormFile Images = HttpContext.Request.Form.Files["file"];
            if (Images != null)
            {
                NewsGallery NewsGallery = new NewsGallery
                {
                    Fk_News = Id,
                    FileType = Images.ContentType,
                    FileName = Images.FileName,
                    FileLength = Images.Length,
                    FileURL = AppMainData.DomainName

                };
                NewsGallery.CreatedBy = _Session.GetString("FullName");
                _UnitOfWork.NewsGallery.CreateEntity(NewsGallery);
                await _UnitOfWork.NewsGallery.Save();

                string ImageURL = await ImgManager.UploudImage(AppMainData.DomainName, NewsGallery.Id.ToString(), Images, "Uploud/NewsGallery");

                if (!string.IsNullOrEmpty(ImageURL))
                {
                    NewsGallery.FileURL = ImageURL;
                    _UnitOfWork.NewsGallery.UpdateEntity(NewsGallery);
                    await _UnitOfWork.NewsGallery.Save();
                }
            }
            return Ok();
        }



        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> Delete(int id)
        {
            NewsGallery NewsGallery = await _UnitOfWork.NewsGallery.GetByID(id);

            if (NewsGallery == null)
            {
                return NotFound();
            }

            return View("~/Views/NewsEntity/NewsGallery/Delete.cshtml", NewsGallery);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            NewsGallery NewsGallery = await _UnitOfWork.NewsGallery.GetByID(id);

            if (!string.IsNullOrEmpty(NewsGallery.FileURL))
            {
                ImgManager ImgManager = new ImgManager(AppMainData.WebRootPath);
                ImgManager.DeleteImage(NewsGallery.FileURL, AppMainData.DomainName);
            }

            _UnitOfWork.NewsGallery.DeleteEntity(NewsGallery);
            await _UnitOfWork.EventGallery.Save();


            return RedirectToAction("Index", "NewsGallery", new { Fk_News = NewsGallery.Fk_News });
        }
    }
}
