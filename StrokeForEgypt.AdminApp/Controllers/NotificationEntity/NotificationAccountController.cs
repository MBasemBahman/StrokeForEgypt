using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StrokeForEgypt.AdminApp.Filters;
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
    public class NotificationAccountController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;

        public NotificationAccountController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor)
        {
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public IActionResult Index(int Id, int Fk_Notification, bool ProfileLayOut = false)
        {
            NotificationAccountFilter Filter = new NotificationAccountFilter
            {
                Id = Id,
                Fk_Notification = Fk_Notification,
            };
            ViewData["ProfileLayOut"] = ProfileLayOut;
            return View("~/Views/NotificationEntity/NotificationAccount/Index.cshtml", Filter);
        }

        [HttpPost]
        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public async Task<IActionResult> LoadTable([FromBody] NotificationAccountFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<NotificationAccount> result = await _UnitOfWork.NotificationAccount.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id)
                                                                                         && (dtParameters.Fk_Notification == 0 || a.Fk_Notification == dtParameters.Fk_Notification)
                                                                                         , new List<string> { "Account" });


            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.Account.FullName.ToLower().Contains(searchBy.ToLower())
                                        || a.Account.Email.ToLower().Contains(searchBy.ToLower())
                                        || a.Account.Phone.ToLower().Contains(searchBy.ToLower())
                                        || (!string.IsNullOrEmpty(a.Account.ImageURL) && a.Account.ImageURL.ToLower().Contains(searchBy.ToLower()))
                                        || a.Id.ToString().ToLower().Contains(searchBy.ToLower()))
                               .ToList();
            }

            result.ForEach(a => { a.Account.NotificationAccounts = null; });

            DataTableManager<NotificationAccount> DataTableManager = new DataTableManager<NotificationAccount>();

            DataTableResult<NotificationAccount> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.NotificationAccount.Count());

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
    }
}
