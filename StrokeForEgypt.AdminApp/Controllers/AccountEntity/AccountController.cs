using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrokeForEgypt.AdminApp.Filters;
using StrokeForEgypt.AdminApp.Services;
using StrokeForEgypt.AdminApp.ViewModel;
using StrokeForEgypt.Common;
using StrokeForEgypt.Entity.AccountEntity;
using StrokeForEgypt.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static StrokeForEgypt.Common.EnumData;


namespace StrokeForEgypt.AdminApp.Controllers.AccountEntity
{
    public class AccountController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;
        private readonly CommonLocalizationService _CommonLocalizationService;

        public AccountController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor, CommonLocalizationService CommonLocalizationService)
        {
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
            _CommonLocalizationService = CommonLocalizationService;
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public IActionResult Index(int Id)
        {
            AccountFilter Filter = new AccountFilter
            {
                Id = Id
            };

            return View("~/Views/AccountEntity/Account/Index.cshtml", Filter);
        }

        [HttpPost]
        public async Task<IActionResult> LoadTable([FromBody] AccountFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<Account> result = await _UnitOfWork.Account.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id));



            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.FullName.ToLower().Contains(searchBy.ToLower())
                                        || a.Phone.ToLower().Contains(searchBy.ToLower())
                                        || a.Email.ToLower().Contains(searchBy.ToLower())
                                        || a.IsActive.ToString().ToLower().Contains(searchBy.ToLower())
                                        || (!string.IsNullOrEmpty(a.ImageURL) && a.ImageURL.ToLower().Contains(searchBy.ToLower()))
                                        || a.Id.ToString().ToLower().Contains(searchBy.ToLower()))
                               .ToList();
            }

            DataTableManager<Account> DataTableManager = new();

            DataTableResult<Account> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.Account.Count());

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
            Account Account = await _UnitOfWork.Account.GetByID(id);

            if (Account == null)
            {
                return NotFound();
            }

            return View("~/Views/AccountEntity/Account/Details.cshtml", Account);
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public async Task<IActionResult> Profile(int id, int returnItem = (int)AccountProfileItems.AccountDevice)
        {
            Account Account = await _UnitOfWork.Account.GetByID(id);

            if (Account == null)
            {
                return NotFound();
            }
            ViewData["returnItem"] = returnItem;
            return View("~/Views/AccountEntity/Account/Profile.cshtml", Account);
        }


        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id = 0, bool IsProfile = false)
        {
            Account Account = new();

            if (id > 0)
            {
                Account = await _UnitOfWork.Account.GetByID(id);
                if (Account == null)
                {
                    return NotFound();
                }
            }

            ViewData["IsProfile"] = IsProfile;
            return View("~/Views/AccountEntity/Account/CreateOrEdit.cshtml", Account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id, Account Account, bool IsProfile)
        {
            if (id != Account.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (_UnitOfWork.Account.Any(a => a.Email == Account.Email && a.Id != id))
                    {
                        ViewData["EmailError"] = _CommonLocalizationService.Get("Email Duplicated");
                        return View("~/Views/AccountEntity/Account/CreateOrEdit.cshtml", Account);
                    }
                    if (id == 0)
                    {
                        Account.CreatedBy = Request.Cookies["FullName"];

                        _UnitOfWork.Account.CreateEntity(Account);
                        await _UnitOfWork.Account.Save();
                    }
                    else
                    {
                        Account Data = await _UnitOfWork.Account.GetByID(id);

                        Account.LastModifiedBy = Request.Cookies["FullName"];

                        _Mapper.Map(Account, Data);

                        _UnitOfWork.Account.UpdateEntity(Data);

                        await _UnitOfWork.Event.Save();

                        Account = Data;
                    }


                    IFormFile ImageFile = HttpContext.Request.Form.Files["ImageFile"];

                    if (ImageFile != null)
                    {
                        ImgManager ImgManager = new ImgManager(AppMainData.WebRootPath);

                        string FileURL = await ImgManager.UploudImage(AppMainData.DomainName, Account.Id.ToString(), ImageFile, "Uploud/Account");

                        if (!string.IsNullOrEmpty(FileURL))
                        {
                            if (!string.IsNullOrEmpty(Account.ImageURL))
                            {
                                ImgManager.DeleteImage(Account.ImageURL, AppMainData.DomainName);
                            }
                            Account.ImageURL = FileURL;
                            _UnitOfWork.Account.UpdateEntity(Account);
                            await _UnitOfWork.Account.Save();
                        }
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_UnitOfWork.Account.Any(a => a.Id == id))
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
                    return RedirectToAction(nameof(Profile), new { id = Account.Id });

                }
                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/AccountEntity/Account/CreateOrEdit.cshtml", Account);
        }


        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> Delete(int id)
        {
            Account Account = await _UnitOfWork.Account.GetByID(id);

            if (Account == null)
            {
                return NotFound();
            }

            return View("~/Views/AccountEntity/Account/Delete.cshtml", Account);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Account Account = await _UnitOfWork.Account.GetByID(id);
            if (!string.IsNullOrEmpty(Account.ImageURL))
            {
                ImgManager ImgManager = new ImgManager(AppMainData.WebRootPath);
                ImgManager.DeleteImage(Account.ImageURL, AppMainData.DomainName);
            }
            _UnitOfWork.Account.DeleteEntity(Account);
            await _UnitOfWork.Account.Save();

            return RedirectToAction(nameof(Index));
        }
    }
}
