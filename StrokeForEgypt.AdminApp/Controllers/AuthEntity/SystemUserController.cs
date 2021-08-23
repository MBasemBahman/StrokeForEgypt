using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrokeForEgypt.AdminApp.Filters;
using StrokeForEgypt.AdminApp.Services;
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
    public class SystemUserController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;
        private readonly CommonLocalizationService _CommonLocalizationService;

        public SystemUserController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor, CommonLocalizationService CommonLocalizationService)
        {
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
            _CommonLocalizationService = CommonLocalizationService;
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public IActionResult Index(int Id, int Fk_SystemRole)
        {
            ViewData["Id"] = Id;
            ViewData["Fk_SystemRole"] = Fk_SystemRole;
            return View("~/Views/AuthEntity/SystemUser/Index.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> LoadTable([FromBody] SystemUserFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<SystemUser> result = await _UnitOfWork.SystemUser.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id)
                                                                               && (dtParameters.Fk_SystemRole == 0 || a.Fk_SystemRole == dtParameters.Fk_SystemRole), new List<string> { "SystemRole" });



            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.FullName.ToLower().Contains(searchBy.ToLower())
                                        || a.Phone.ToLower().Contains(searchBy.ToLower())
                                        || a.Email.ToLower().Contains(searchBy.ToLower())
                                        || a.SystemRole.Name.ToLower().Contains(searchBy.ToLower())
                                        || a.JobTitle.ToLower().Contains(searchBy.ToLower())
                                        || a.IsActive.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.Id.ToString().ToLower().Contains(searchBy.ToLower()))
                               .ToList();
            }

            result.ForEach(a => { a.SystemRole.SystemUsers = null; });

            DataTableManager<SystemUser> DataTableManager = new();

            DataTableResult<SystemUser> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.SystemUser.Count());

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
            SystemUser SystemUser = await _UnitOfWork.SystemUser.GetFirst(a => a.Id == id, new List<string> { "SystemRole" });

            if (SystemUser == null)
            {
                return NotFound();
            }

            return View("~/Views/AuthEntity/SystemUser/Details.cshtml", SystemUser);
        }


        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id = 0)
        {
            SystemUser SystemUser = new();

            if (id > 0)
            {
                SystemUser = await _UnitOfWork.SystemUser.GetByID(id);
                if (SystemUser == null)
                {
                    return NotFound();
                }
            }

            return View("~/Views/AuthEntity/SystemUser/CreateOrEdit.cshtml", SystemUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id, SystemUser SystemUser)
        {
            if (id != SystemUser.Id)
            {
                return NotFound();
            }

            if (_UnitOfWork.SystemUser.Any(a => a.Id != SystemUser.Id && a.Email == SystemUser.Email))
            {
                ViewData["Error"] = _CommonLocalizationService.Get("Email is already registered");

                return View(SystemUser);
            }
            if (_UnitOfWork.SystemUser.Any(a => a.Id != SystemUser.Id && a.Phone == SystemUser.Phone))
            {
                ViewData["Error"] = _CommonLocalizationService.Get("The phone number is already registered");

                return View(SystemUser);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0)
                    {
                        SystemUser.CreatedBy = Request.Cookies["FullName"];
                        _UnitOfWork.SystemUser.CreateEntity(SystemUser);
                        await _UnitOfWork.SystemUser.Save();
                    }
                    else
                    {
                        SystemUser Data = await _UnitOfWork.SystemUser.GetByID(id);

                        SystemUser.LastModifiedBy = Request.Cookies["FullName"];

                        if (string.IsNullOrEmpty(SystemUser.Password))
                        {
                            SystemUser.Password = Data.Password;
                        }

                        _Mapper.Map(SystemUser, Data);

                        _UnitOfWork.SystemUser.UpdateEntity(Data);

                        await _UnitOfWork.SystemUser.Save();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_UnitOfWork.SystemUser.Any(a => a.Id == id))
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

            return View("~/Views/AuthEntity/SystemUser/CreateOrEdit.cshtml", SystemUser);
        }


        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> Delete(int id)
        {
            SystemUser SystemUser = await _UnitOfWork.SystemUser.GetByID(id);

            if (SystemUser == null)
            {
                return NotFound();
            }

            return View("~/Views/AuthEntity/SystemUser/Delete.cshtml", SystemUser);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            SystemUser SystemUser = await _UnitOfWork.SystemUser.GetByID(id);

            _UnitOfWork.SystemUser.DeleteEntity(SystemUser);

            await _UnitOfWork.SystemUser.Save();

            return RedirectToAction(nameof(Index));
        }
    }
}
