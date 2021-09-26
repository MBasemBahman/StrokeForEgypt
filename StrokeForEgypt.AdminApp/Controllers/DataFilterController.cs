using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StrokeForEgypt.AdminApp.Services;
using StrokeForEgypt.Repository;
using System.Linq;

namespace StrokeForEgypt.AdminApp.Controllers
{
    public class DataFilterController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;
        private readonly CommonLocalizationService _CommonLocalizationService;

        public DataFilterController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor, CommonLocalizationService CommonLocalizationService)
        {
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
            _CommonLocalizationService = CommonLocalizationService;
        }

        [HttpGet]
        public JsonResult GetEventPackages(int Fk_Event)
        {
            var result = _UnitOfWork.EventPackage.GetAll(a => a.Fk_Event == Fk_Event).Result.Select(a => new { a.Id, Name = a.Title }).ToList();

            return Json(result);
        }

        [HttpGet]
        public JsonResult GetEventPackagePrice(int Fk_EventPackage)
        {
            double result = _UnitOfWork.EventPackage.GetByID(Fk_EventPackage).Result.Price;

            return Json(result);
        }

        [HttpGet]
        public JsonResult GetCities(int Fk_Country)
        {
            var result = _UnitOfWork.City.GetAll(a => a.Fk_Country == Fk_Country).Result.Select(a => new { Id = a.Id, Name = a.Name });

            return Json(result);
        }


        [HttpGet]
        public JsonResult GetAccounts(bool haveNotificationToken = false)
        {

            var result = _UnitOfWork.Account.GetAll(a => haveNotificationToken == false ||
                                                         a.AccountDevices.Any(b => !string.IsNullOrEmpty(b.NotificationToken)))
                                            .Result.Select(a => new { a.Id, Name = a.FullName });

            return Json(result);
        }
    }
}
