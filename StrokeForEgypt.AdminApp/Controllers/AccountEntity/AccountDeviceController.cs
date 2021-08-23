using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StrokeForEgypt.AdminApp.Filters;
using StrokeForEgypt.AdminApp.ViewModel;
using StrokeForEgypt.Common;
using StrokeForEgypt.Entity.AccountEntity;
using StrokeForEgypt.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static StrokeForEgypt.Common.EnumData;

namespace StrokeForEgypt.AdminApp.Controllers.AccountEntity
{
    public class AccountDeviceController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;

        public AccountDeviceController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor)
        {
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public IActionResult Index(int Id, int Fk_Account, bool ProfileLayOut = false)
        {
            AccountDeviceFilter Filter = new AccountDeviceFilter
            {
                Id = Id,
                Fk_Account = Fk_Account,
            };
            ViewData["ProfileLayOut"] = ProfileLayOut;
            return View("~/Views/AccountEntity/AccountDevice/Index.cshtml", Filter);
        }

        [HttpPost]
        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public async Task<IActionResult> LoadTable([FromBody] AccountDeviceFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<AccountDevice> result = await _UnitOfWork.AccountDevice.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id)
                                                                                         && (dtParameters.Fk_Account == 0 || a.Fk_Account == dtParameters.Fk_Account));


            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.DeviceModel.ToLower().Contains(searchBy.ToLower())
                                        || a.DeviceType.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.DeviceVersion.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.AppVersion.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.IsActive.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.Id.ToString().ToLower().Contains(searchBy.ToLower()))
                               .ToList();
            }

            DataTableManager<AccountDevice> DataTableManager = new DataTableManager<AccountDevice>();

            DataTableResult<AccountDevice> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.AccountDevice.Count());

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
            AccountDevice AccountDevice = await _UnitOfWork.AccountDevice.GetByID(id);

            if (AccountDevice == null)
            {
                return NotFound();
            }

            return View("~/Views/AccountEntity/AccountDevice/Details.cshtml", AccountDevice);
        }
    }
}
