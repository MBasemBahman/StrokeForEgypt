using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrokeForEgypt.AdminApp.Filters;
using StrokeForEgypt.AdminApp.Services;
using StrokeForEgypt.AdminApp.ViewModel;
using StrokeForEgypt.Common;
using StrokeForEgypt.DAL;
using StrokeForEgypt.Entity.NotificationEntity;
using StrokeForEgypt.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static StrokeForEgypt.Common.EnumData;

namespace StrokeForEgypt.AdminApp.Controllers.NotificationEntity
{
    public class NotificationController : Controller
    {
        private readonly BaseDBContext _DBContext;
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;
        private readonly CommonLocalizationService _CommonLocalizationService;

        public NotificationController(BaseDBContext BaseDBContext, UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor, CommonLocalizationService CommonLocalizationService)
        {
            _DBContext = BaseDBContext;
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
            _CommonLocalizationService = CommonLocalizationService;
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public IActionResult Index(int Id, int Fk_NotificationType, int Fk_Account, bool ProfileLayOut = false)
        {
            NotificationFilter Filter = new NotificationFilter
            {
                Id = Id,
                Fk_Account = Fk_Account,
                Fk_NotificationType = Fk_NotificationType
            };
            ViewData["ProfileLayOut"] = ProfileLayOut;
            return View("~/Views/NotificationEntity/Notification/Index.cshtml", Filter);
        }

        [HttpPost]
        public async Task<IActionResult> LoadTable([FromBody] NotificationFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<Notification> result = await _UnitOfWork.Notification.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id)
                                                                                    && (dtParameters.Fk_NotificationType == 0 || a.Fk_NotificationType == dtParameters.Fk_NotificationType)
                                                                                    && (dtParameters.Fk_Account == 0 || a.NotificationAccounts.Any(b => b.Fk_Account == dtParameters.Fk_Account)), new List<string> { "NotificationType" });



            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.Heading.ToLower().Contains(searchBy.ToLower())
                                        || a.NotificationType.Name.ToLower().Contains(searchBy.ToLower())
                                        || a.IsActive.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.Order.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.Id.ToString().ToLower().Contains(searchBy.ToLower()))
                               .ToList();
            }
            result.ForEach(a => a.NotificationType.Notifications = null);

            DataTableManager<Notification> DataTableManager = new();

            DataTableResult<Notification> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.Notification.Count());

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
            Notification Notification = await _UnitOfWork.Notification.GetByID(id);

            if (Notification == null)
            {
                return NotFound();
            }

            return View("~/Views/NotificationEntity/Notification/Details.cshtml", Notification);
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public async Task<IActionResult> Profile(int id, int returnItem = (int)NotificationProfileItem.Accounts)
        {
            Notification Notification = await _UnitOfWork.Notification.GetByID(id);

            if (Notification == null)
            {
                return NotFound();
            }
            ViewData["returnItem"] = returnItem;
            return View("~/Views/NotificationEntity/Notification/Profile.cshtml", Notification);
        }


        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id = 0, int Fk_Account = 0, bool IsProfile = false)
        {
            Notification Notification = new();

            if (id > 0)
            {
                Notification = await _UnitOfWork.Notification.GetByID(id);
                if (Notification == null)
                {
                    return NotFound();
                }
            }

            ViewData["IsProfile"] = IsProfile;
            ViewData["Fk_Account"] = Fk_Account;
            return View("~/Views/NotificationEntity/Notification/CreateOrEdit.cshtml", Notification);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(
            int id,
            Notification Notification,
            bool IsProfile,
            bool IsPrivate = false,
            bool SendNotification = false,
            List<int> Fk_Accounts = null)
        {
            if (id != Notification.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (Notification.Fk_NotificationType == (int)NotificationTypeEnum.ExternalURL &&
                        string.IsNullOrEmpty(Notification.Target))
                    {
                        ViewData["Error"] = "Please add Url in target value";
                        return View("~/Views/NotificationEntity/Notification/CreateOrEdit.cshtml", Notification);
                    }
                    else if ((Notification.Fk_NotificationType == (int)NotificationTypeEnum.EventAgenda ||
                              Notification.Fk_NotificationType == (int)NotificationTypeEnum.EventNews ||
                              Notification.Fk_NotificationType == (int)NotificationTypeEnum.EventProfile ||
                              Notification.Fk_NotificationType == (int)NotificationTypeEnum.EventRegistration) &&
                              Notification.Fk_Event == null)
                    {
                        ViewData["Error"] = "Please select event";
                        return View("~/Views/NotificationEntity/Notification/CreateOrEdit.cshtml", Notification);
                    }
                    else if ((Notification.Fk_NotificationType == (int)NotificationTypeEnum.EventRegistrationMembers ||
                              Notification.Fk_NotificationType == (int)NotificationTypeEnum.EventRegistrationPayment) &&
                              string.IsNullOrEmpty(Notification.Target))
                    {
                        ViewData["Error"] = "Please add booking value in target value";
                        return View("~/Views/NotificationEntity/Notification/CreateOrEdit.cshtml", Notification);
                    }
                    else if ((Notification.Fk_NotificationType == (int)NotificationTypeEnum.Verification ||
                              IsPrivate) &&
                             (Fk_Accounts == null || !Fk_Accounts.Any()))
                    {
                        ViewData["Error"] = "Please activate private option and select account";
                        return View("~/Views/NotificationEntity/Notification/CreateOrEdit.cshtml", Notification);
                    }

                    if ((Notification.Fk_NotificationType == (int)NotificationTypeEnum.EventAgenda ||
                         Notification.Fk_NotificationType == (int)NotificationTypeEnum.EventNews ||
                         Notification.Fk_NotificationType == (int)NotificationTypeEnum.EventProfile ||
                         Notification.Fk_NotificationType == (int)NotificationTypeEnum.EventRegistration))
                    {
                        Notification.Target = Notification.Fk_Event.ToString();
                    }
                    else if (Notification.Fk_NotificationType == (int)NotificationTypeEnum.Verification)
                    {
                        Notification.Target = Fk_Accounts.First().ToString();
                    }
                    else if (Notification.Fk_NotificationType == (int)NotificationTypeEnum.EventRegistrationMembers ||
                             Notification.Fk_NotificationType == (int)NotificationTypeEnum.EventRegistrationPayment)
                    {
                        Fk_Accounts = _DBContext.Booking.Where(a => a.Id.ToString() == Notification.Target)
                                                .Select(a => a.Fk_Account)
                                                .ToList();
                    }

                    if (id == 0)
                    {
                        Notification.CreatedBy = Request.Cookies["FullName"];
                        if (IsPrivate)
                        {
                            Notification.NotificationAccounts = new List<NotificationAccount>();
                            Notification = _UnitOfWork.NotificationAccount.CreateEntity(Notification, Fk_Accounts);
                        }
                        _UnitOfWork.Notification.CreateEntity(Notification);
                        await _UnitOfWork.Notification.Save();

                    }
                    else
                    {
                        Notification Data = await _UnitOfWork.Notification.GetByID(id);

                        Data.LastModifiedBy = Request.Cookies["FullName"];
                        Data.Heading = Notification.Heading;
                        Data.Content = Notification.Content;
                        Data.Order = Notification.Order;
                        Data.IsActive = Notification.IsActive;

                        if (_UnitOfWork.NotificationAccount.Any(a => a.Fk_Notification == Data.Id))
                        {
                            IsPrivate = true;
                        }

                        _UnitOfWork.Notification.UpdateEntity(Data);

                        await _UnitOfWork.Event.Save();

                        Notification = Data;
                    }

                    if (SendNotification)
                    {
                        if (IsPrivate)
                        {
                            var NotificationData = _DBContext.Notification
                                                             .Where(a => a.Id == Notification.Id)
                                                             .Select(a => new
                                                             {
                                                                 a.Heading,
                                                                 a.Content,
                                                                 a.Target,
                                                                 NotificationType = new
                                                                 {
                                                                     a.NotificationType.Id,
                                                                     a.NotificationType.Name
                                                                 }
                                                             })
                                                             .First();

                            var Accounts = _DBContext.AccountDevice
                                                     .Where(a => a.Account.NotificationAccounts.Any(b => b.Fk_Notification == Notification.Id) &&
                                                                 !string.IsNullOrEmpty(a.NotificationToken))
                                                     .Select(a => a.NotificationToken)
                                                     .Distinct()
                                                     .ToList();

                            NotificationManager.Notification = new FirebaseNotificationModel
                            {
                                NotificationType = new KeyValuePair<int, string>(NotificationData.NotificationType.Id, NotificationData.NotificationType.Name),
                                MessageHeading = NotificationData.Heading,
                                MessageContent = NotificationData.Content,
                                Target = NotificationData.Target,
                                RegistrationTokens = Accounts,
                            };
                            await NotificationManager.SendMulticast(NotificationManager.CreateMulticastMessage(NotificationManager.Notification));
                        }
                        else
                        {
                            var NotificationData = _DBContext.Notification
                                                             .Where(a => a.Id == Notification.Id)
                                                             .Select(a => new
                                                             {
                                                                 a.Heading,
                                                                 a.Content,
                                                                 a.Target,
                                                                 NotificationType = new
                                                                 {
                                                                     a.NotificationType.Id,
                                                                     a.NotificationType.Name
                                                                 }
                                                             })
                                                             .First();

                            NotificationManager.Notification = new FirebaseNotificationModel
                            {
                                NotificationType = new KeyValuePair<int, string>(NotificationData.NotificationType.Id, NotificationData.NotificationType.Name),
                                MessageHeading = NotificationData.Heading,
                                MessageContent = NotificationData.Content,
                                Target = NotificationData.Target
                            };
                            await NotificationManager.SendToTopic(NotificationManager.CreateMessage(NotificationManager.Notification));
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_UnitOfWork.Notification.Any(a => a.Id == id))
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
                    return RedirectToAction(nameof(Profile), new { id = Notification.Id });

                }
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/NotificationEntity/Notification/CreateOrEdit.cshtml", Notification);
        }


        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> Delete(int id)
        {
            Notification Notification = await _UnitOfWork.Notification.GetByID(id);

            if (Notification == null)
            {
                return NotFound();
            }

            return View("~/Views/NotificationEntity/Notification/Delete.cshtml", Notification);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Notification Notification = await _UnitOfWork.Notification.GetByID(id);
            _UnitOfWork.Notification.DeleteEntity(Notification);
            await _UnitOfWork.Notification.Save();

            return RedirectToAction(nameof(Index));
        }
    }
}
