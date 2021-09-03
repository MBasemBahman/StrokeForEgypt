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
    public class SystemRoleController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;

        public SystemRoleController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor)
        {
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public IActionResult Index(int Id)
        {
            ViewData["Id"] = Id;
            return View("~/Views/AuthEntity/SystemRole/Index.cshtml");
        }

        [HttpPost]
        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public async Task<IActionResult> LoadTable([FromBody] CommonFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<SystemRole> result = await _UnitOfWork.SystemRole.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id));



            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.Name.ToLower().Contains(searchBy.ToLower())
                                        || a.Id.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.CreatedAt.ToString().ToLower().Contains(searchBy.ToLower()))
                               .ToList();
            }

            result.ForEach(a => { a.SystemRolePremissions = null; a.SystemUsers = null; });
            DataTableManager<SystemRole> DataTableManager = new();

            DataTableResult<SystemRole> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.SystemRole.Count());

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
            SystemRole SystemRole = await _UnitOfWork.SystemRole.GetByID(id);

            if (SystemRole == null)
            {
                return NotFound();
            }

            return View("~/Views/AuthEntity/SystemRole/Details.cshtml", await GetViewModel(SystemRole));
        }

        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id = 0)
        {
            SystemRole SystemRole = new();

            if (id > 0)
            {
                SystemRole = await _UnitOfWork.SystemRole.GetByID(id);
                if (SystemRole == null)
                {
                    return NotFound();
                }
            }

            return View("~/Views/AuthEntity/SystemRole/CreateOrEdit.cshtml", await GetViewModel(SystemRole));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id, SystemRole SystemRole, List<int> FullAccessViews,
                                                   List<int> ControlAccessViews, List<int> ViewAccessViews)
        {
            if (id != SystemRole.Id)
            {
                return NotFound();
            }

            if (FullAccessViews != null && ControlAccessViews != null && ViewAccessViews != null)
            {
                ControlAccessViews = ControlAccessViews.Except(FullAccessViews).ToList();

                ViewAccessViews = ViewAccessViews.Except(FullAccessViews).ToList();
                ViewAccessViews = ViewAccessViews.Except(ControlAccessViews).ToList();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0)
                    {
                        SystemRole.CreatedBy = Request.Cookies["FullName"];

                        SystemRole.SystemRolePremissions = new List<SystemRolePremission>();

                        SystemRole = _UnitOfWork.SystemRolePremission.CreateEntity(SystemRole, FullAccessViews, AccessLevelEnum.FullAccess);
                        SystemRole = _UnitOfWork.SystemRolePremission.CreateEntity(SystemRole, ControlAccessViews, AccessLevelEnum.CreateOrUpdateAccess);
                        SystemRole = _UnitOfWork.SystemRolePremission.CreateEntity(SystemRole, ViewAccessViews, AccessLevelEnum.ReadAccess);

                        _UnitOfWork.SystemRole.CreateEntity(SystemRole);
                        await _UnitOfWork.SystemRole.Save();
                    }
                    else
                    {
                        SystemRole Data = await _UnitOfWork.SystemRole.GetByID(id);

                        SystemRole.LastModifiedBy = Request.Cookies["FullName"];

                        _Mapper.Map(SystemRole, Data);

                        Data.SystemRolePremissions = await _UnitOfWork.SystemRolePremission.GetAll(a => a.Fk_SystemRole == SystemRole.Id);


                        Data = _UnitOfWork.SystemRolePremission.UpdateEntity(Data, Data.SystemRolePremissions.Where(a => a.Fk_AccessLevel == (int)AccessLevelEnum.FullAccess).Select(a => a.Fk_SystemView).ToList(), FullAccessViews, AccessLevelEnum.FullAccess);
                        Data = _UnitOfWork.SystemRolePremission.UpdateEntity(Data, Data.SystemRolePremissions.Where(a => a.Fk_AccessLevel == (int)AccessLevelEnum.CreateOrUpdateAccess).Select(a => a.Fk_SystemView).ToList(), ControlAccessViews, AccessLevelEnum.CreateOrUpdateAccess);
                        Data = _UnitOfWork.SystemRolePremission.UpdateEntity(Data, Data.SystemRolePremissions.Where(a => a.Fk_AccessLevel == (int)AccessLevelEnum.ReadAccess).Select(a => a.Fk_SystemView).ToList(), ViewAccessViews, AccessLevelEnum.ReadAccess);



                        _UnitOfWork.SystemRole.UpdateEntity(Data);

                        await _UnitOfWork.SystemRole.Save();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_UnitOfWork.SystemRole.Any(a => a.Id == id))
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

            return View("~/Views/AuthEntity/SystemRole/CreateOrEdit.cshtml", await GetViewModel(SystemRole, FullAccessViews, ControlAccessViews, ViewAccessViews));
        }

        public async Task<SystemRoleViewModel> GetViewModel(SystemRole SystemRole
          , List<int> FullAccessViews = null, List<int> ControlAccessViews = null, List<int> ViewAccessViews = null)
        {
            SystemRoleViewModel Data = new SystemRoleViewModel()
            {
                SystemRole = SystemRole,
                SystemViews = _UnitOfWork.SystemView.GetAll(a => a.Id != (int)SystemViewEnum.Home).Result.ToDictionary(a => a.Id.ToString(), a => a.DisplayName)
            };

            if (SystemRole.Id > 0 && FullAccessViews == null && ControlAccessViews == null && ViewAccessViews == null)
            {
                List<SystemRolePremission> SystemRolePremissions = await _UnitOfWork.SystemRolePremission.GetAll(a => a.Fk_SystemRole == SystemRole.Id);

                if (SystemRolePremissions != null && SystemRolePremissions.Any())
                {
                    Data.FullAccessViews = SystemRolePremissions.Where(a => a.Fk_AccessLevel == (int)AccessLevelEnum.FullAccess).Select(a => a.Fk_SystemView).ToList();
                    Data.ControlAccessViews = SystemRolePremissions.Where(a => a.Fk_AccessLevel == (int)AccessLevelEnum.CreateOrUpdateAccess).Select(a => a.Fk_SystemView).ToList();
                    Data.ViewAccessViews = SystemRolePremissions.Where(a => a.Fk_AccessLevel == (int)AccessLevelEnum.ReadAccess).Select(a => a.Fk_SystemView).ToList();
                }
            }
            else
            {
                Data.FullAccessViews = FullAccessViews ?? Data.FullAccessViews;
                Data.ControlAccessViews = ControlAccessViews ?? Data.ControlAccessViews;
                Data.ViewAccessViews = ViewAccessViews ?? Data.ViewAccessViews;
            }

            return Data;
        }

        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> Delete(int id)
        {
            SystemRole SystemRole = await _UnitOfWork.SystemRole.GetByID(id);

            if (SystemRole == null)
            {
                return NotFound();
            }
            if (_UnitOfWork.SystemUser.Any(a => a.Fk_SystemRole == id))
            {
                ViewBag.CanDelete = false;
            }
            return View("~/Views/AuthEntity/SystemRole/Delete.cshtml", SystemRole);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            SystemRole SystemRole = await _UnitOfWork.SystemRole.GetByID(id);
            if (!_UnitOfWork.SystemUser.Any(a => a.Fk_SystemRole == id))
            {
                _UnitOfWork.SystemRole.DeleteEntity(SystemRole);
                await _UnitOfWork.SystemRole.Save();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
