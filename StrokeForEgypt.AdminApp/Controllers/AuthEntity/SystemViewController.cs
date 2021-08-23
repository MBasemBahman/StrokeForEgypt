using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrokeForEgypt.AdminApp.Filters;
using StrokeForEgypt.AdminApp.ViewModel;
using StrokeForEgypt.Common;
using StrokeForEgypt.Entity.AuthEntity;
using StrokeForEgypt.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static StrokeForEgypt.Common.EnumData;

namespace StrokeForEgypt.AdminApp.Controllers.AuthEntity
{
    public class SystemViewController : Controller
    {

        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;

        public SystemViewController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor)
        {
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public IActionResult Index(int Id)
        {
            ViewData["Id"] = Id;

            return View("~/Views/AuthEntity/SystemView/Index.cshtml");
        }

        [HttpPost]
        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public async Task<IActionResult> LoadTable([FromBody] CommonFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<SystemView> result = await _UnitOfWork.SystemView.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id));

            result.ForEach(a => a.SystemRolePremissions = null);

            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.Name.ToLower().Contains(searchBy.ToLower())
                                        || a.DisplayName.ToLower().Contains(searchBy.ToLower())
                                        || a.Id.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.CreatedAt.ToString().ToLower().Contains(searchBy.ToLower()))
                               .ToList();
            }

            DataTableManager<SystemView> DataTableManager = new();

            DataTableResult<SystemView> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.SystemView.Count());

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
            SystemView SystemView = await _UnitOfWork.SystemView.GetByID(id);

            if (SystemView == null)
            {
                return NotFound();
            }

            return View("~/Views/AuthEntity/SystemView/Details.cshtml", SystemView);
        }

        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id = 0)
        {
            SystemView SystemView = new();

            if (id > 0)
            {
                SystemView = await _UnitOfWork.SystemView.GetByID(id);
                if (SystemView == null)
                {
                    return NotFound();
                }
            }

            return View("~/Views/AuthEntity/SystemView/CreateOrEdit.cshtml", SystemView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id, SystemView SystemView)
        {
            if (id != SystemView.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0)
                    {
                        SystemView.CreatedBy = Request.Cookies["FullName"];

                        _UnitOfWork.SystemView.CreateEntity(SystemView);
                        await _UnitOfWork.SystemView.Save();
                    }
                    else
                    {
                        SystemView Data = await _UnitOfWork.SystemView.GetByID(id);

                        SystemView.LastModifiedBy = Request.Cookies["FullName"];

                        _Mapper.Map(SystemView, Data);

                        _UnitOfWork.SystemView.UpdateEntity(Data);
                        await _UnitOfWork.SystemView.Save();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_UnitOfWork.SystemView.Any(a => a.Id == id))
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

            return View("~/Views/AuthEntity/SystemView/CreateOrEdit.cshtml", SystemView);
        }

        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> Delete(int id)
        {
            SystemView SystemView = await _UnitOfWork.SystemView.GetByID(id);

            if (SystemView == null)
            {
                return NotFound();
            }

            if (_UnitOfWork.SystemRolePremission.Any(a => a.Fk_SystemView == id))
            {
                ViewBag.CanDelete = false;
            }

            return View("~/Views/AuthEntity/SystemView/Delete.cshtml", SystemView);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            SystemView SystemView = await _UnitOfWork.SystemView.GetByID(id);

            if (!_UnitOfWork.SystemRolePremission.Any(a => a.Fk_SystemView == id))
            {
                _UnitOfWork.SystemView.DeleteEntity(SystemView);

                await _UnitOfWork.SystemView.Save();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
