using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class NewsController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;

        public NewsController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor)
        {
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public IActionResult Index(int Id, int Fk_Event, bool ProfileLayOut = false)
        {
            NewsFilter Filter = new NewsFilter
            {
                Id = Id,
                Fk_Event = Fk_Event,
            };
            ViewData["ProfileLayOut"] = ProfileLayOut;
            return View("~/Views/NewsEntity/News/Index.cshtml", Filter);
        }

        [HttpPost]
        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public async Task<IActionResult> LoadTable([FromBody] NewsFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<News> result = await _UnitOfWork.News.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id)
                                                                                         && (dtParameters.Fk_Event == 0 || a.Fk_Event == dtParameters.Fk_Event));


            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.Title.ToLower().Contains(searchBy.ToLower())
                                        || a.IsActive.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.Id.ToString().ToLower().Contains(searchBy.ToLower()))
                               .ToList();
            }

            DataTableManager<News> DataTableManager = new DataTableManager<News>();

            DataTableResult<News> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.News.Count());

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
            News News = await _UnitOfWork.News.GetByID(id);

            if (News == null)
            {
                return NotFound();
            }

            return View("~/Views/NewsEntity/News/Details.cshtml", News);
        }

        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id = 0, int Fk_Event = 0)
        {
            News News = new News();

           
            if (id > 0)
            {
                News = await _UnitOfWork.News.GetByID(id);
                if (News == null)
                {
                    return NotFound();
                }
            }
           

            return View("~/Views/NewsEntity/News/CreateOrEdit.cshtml", News);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id, News News)
        {
            if (id != News.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0)
                    {
                        News.CreatedBy = _Session.GetString("FullName");
                        _UnitOfWork.News.CreateEntity(News);
                        await _UnitOfWork.News.Save();
                    }
                    else
                    {
                        News Data = await _UnitOfWork.News.GetByID(id);

                        News.LastModifiedBy = _Session.GetString("FullName");

                        _Mapper.Map(News, Data);


                        _UnitOfWork.News.UpdateEntity(Data);
                        await _UnitOfWork.News.Save();
                    }
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!_UnitOfWork.EventAgenda.Any(a => a.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/NewsEntity/News/CreateOrEdit.cshtml", News);

        }

        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> Delete(int id)
        {
            News News = await _UnitOfWork.News.GetByID(id);

            if (News == null)
            {
                return NotFound();
            }

            return View("~/Views/NewsEntity/News/Delete.cshtml", News);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            News News = await _UnitOfWork.News.GetByID(id);

            _UnitOfWork.News.DeleteEntity(News);
            await _UnitOfWork.News.Save();

            return RedirectToAction(nameof(Index));
        }
    }
}
