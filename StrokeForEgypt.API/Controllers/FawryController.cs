using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrokeForEgypt.API.Authorization;
using StrokeForEgypt.API.Services;
using StrokeForEgypt.Common;
using StrokeForEgypt.DAL;
using StrokeForEgypt.Entity.AccountEntity;
using StrokeForEgypt.Entity.BookingEntity;
using StrokeForEgypt.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StrokeForEgypt.API.Controllers
{
    [Authorize]
    public class FawryController : Controller
    {
        private readonly BaseDBContext _DBContext;
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly EntityLocalizationService _Localizer;
        private readonly IAccountService _AccountService;

        public FawryController(
            BaseDBContext dataContext,
            UnitOfWork unitOfWork,
            IMapper mapper,
            EntityLocalizationService Localizer,
            IAccountService AccountService)
        {
            _DBContext = dataContext;
            _UnitOfWork = unitOfWork;
            _Mapper = mapper;
            _Localizer = Localizer;
            _AccountService = AccountService;
        }

        public async Task<IActionResult> Index(
            int Fk_Booking)
        {
            Account account = (Account)Request.HttpContext.Items["Account"];

            Booking booking = await _UnitOfWork.Booking.GetFirst(a => a.Id == Fk_Booking, new List<string> { "EventPackage" });

            FawryManager fawryManager = new(false);
            ChargeRequest ChargeRequest = fawryManager.BuildChargeRequest(new PayRequest
            {
                CustomerProfileId = account.Id.ToString(),
                CustomerEmail = account.Email,
                CustomerMobile = account.Phone,
                CustomerName = account.FullName.ToLower().Trim().Replace(" ", ""),
                MerchantRefNum = Fk_Booking.ToString(),
                ChargeItem = new ChargeItem
                {
                    ItemId = booking.Id.ToString(),
                    Description = $"DaysCount: {booking.DaysCount}, PersonCount: {booking.PersonCount}",
                    ImageUrl = booking.EventPackage.ImageURL,
                    Price = (decimal)booking.TotalPrice,
                    Quantity = booking.PersonCount
                }
            });

            return View(ChargeRequest);
        }

        [AllowAll]
        public IActionResult ChargeResponse(ChargeResponse model)
        {
            return Ok();
        }
    }
}
