using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrokeForEgypt.AdminApp.Filters;
using StrokeForEgypt.AdminApp.Services;
using StrokeForEgypt.AdminApp.ViewModel;
using StrokeForEgypt.Common;
using StrokeForEgypt.Entity.NotificationEntity;
using StrokeForEgypt.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static StrokeForEgypt.Common.EnumData;

namespace StrokeForEgypt.AdminApp.Controllers.NotificationEntity
{
    public class NotificationTypeController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;
        private readonly CommonLocalizationService _CommonLocalizationService;

        public NotificationTypeController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor, CommonLocalizationService CommonLocalizationService)
        {
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
            _CommonLocalizationService = CommonLocalizationService;
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public IActionResult Index(int Id)
        {
            ViewData["Id"] = Id;

            return View("~/Views/NotificationEntity/NotificationType/Index.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> LoadTable([FromBody] CommonFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<NotificationType> result = await _UnitOfWork.NotificationType.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id));



            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.Name.ToLower().Contains(searchBy.ToLower())
                                        || a.IsActive.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.Order.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.Id.ToString().ToLower().Contains(searchBy.ToLower()))
                               .ToList();
            }

            DataTableManager<NotificationType> DataTableManager = new();

            DataTableResult<NotificationType> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.NotificationType.Count());

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
            NotificationType NotificationType = await _UnitOfWork.NotificationType.GetByID(id);

            if (NotificationType == null)
            {
                return NotFound();
            }

            return View("~/Views/NotificationEntity/NotificationType/Details.cshtml", NotificationType);
        }


        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id = 0)
        {
            NotificationType NotificationType = new();

            if (id > 0)
            {
                NotificationType = await _UnitOfWork.NotificationType.GetByID(id);
                if (NotificationType == null)
                {
                    return NotFound();
                }
            }

            return View("~/Views/NotificationEntity/NotificationType/CreateOrEdit.cshtml", NotificationType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id, NotificationType NotificationType)
        {
            if (id != NotificationType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0)
                    {
                        NotificationType.CreatedBy = Request.Cookies["FullName"];
                        _UnitOfWork.NotificationType.CreateEntity(NotificationType);
                        await _UnitOfWork.NotificationType.Save();
                    }
                    else
                    {
                        NotificationType Data = await _UnitOfWork.NotificationType.GetByID(id);

                        NotificationType.LastModifiedBy = Request.Cookies["FullName"];

                        _Mapper.Map(NotificationType, Data);

                        _UnitOfWork.NotificationType.UpdateEntity(Data);

                        await _UnitOfWork.NotificationType.Save();

                    }



                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_UnitOfWork.NotificationType.Any(a => a.Id == id))
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

            return View("~/Views/NotificationEntity/NotificationType/CreateOrEdit.cshtml", NotificationType);
        }


        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> Delete(int id)
        {
            NotificationType NotificationType = await _UnitOfWork.NotificationType.GetByID(id);

            if (NotificationType == null)
            {
                return NotFound();
            }

            if ((_UnitOfWork.Notification.Any(a => a.Fk_NotificationType == id)))
            {
                ViewBag.CanDelete = false;
            }

            return View("~/Views/NotificationEntity/NotificationType/Delete.cshtml", NotificationType);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            NotificationType NotificationType = await _UnitOfWork.NotificationType.GetByID(id);

            if (!(_UnitOfWork.Notification.Any(a => a.Fk_NotificationType == id)))
            {
                _UnitOfWork.NotificationType.DeleteEntity(NotificationType);

                await _UnitOfWork.NotificationType.Save();

            }

            return RedirectToAction(nameof(Index));
        }
    }
}
