using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrokeForEgypt.API.Authorization;
using StrokeForEgypt.API.Helpers;
using StrokeForEgypt.API.Services;
using StrokeForEgypt.Common;
using StrokeForEgypt.DAL;
using StrokeForEgypt.Entity.AccountEntity;
using StrokeForEgypt.Entity.BookingEntity;
using StrokeForEgypt.Repository;
using StrokeForEgypt.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static StrokeForEgypt.Common.EnumData;

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

        [AllowAll]
        public async Task<IActionResult> Index(
            int Fk_Booking)
        {
            ChargeRequest returnData = new();
            Status Status = new();

            try
            {
                Account account = (Account)Request.HttpContext.Items["Account"];

                Booking booking = await _UnitOfWork.Booking.GetFirst(a => a.Id == Fk_Booking,
                    new List<string> { "EventPackage" });

                if (account == null)
                {
                    account = await _UnitOfWork.Account.GetByID(booking.Fk_Account);
                }

                string TotalPrice = string.Format("{0:N2}", Convert.ToDecimal(booking.TotalPrice));

                FawryManager fawryManager = new(true);
                returnData = fawryManager.BuildChargeRequest(new PayRequest
                {
                    CustomerProfileId = account.Id.ToString(),
                    CustomerEmail = account.Email,
                    CustomerMobile = account.Phone,
                    CustomerName = account.FullName.ToLower().Trim().Replace(" ", ""),
                    MerchantRefNum = booking.Id.ToString(),
                    ChargeItem = new ChargeItem
                    {
                        ItemId = booking.EventPackage.Id.ToString(),
                        Description = $"DaysCount: {booking.DaysCount}, PersonCount: {booking.PersonCount}",
                        ImageUrl = booking.EventPackage.ImageURL,
                        Price = decimal.Parse(TotalPrice),
                        Quantity = booking.PersonCount
                    }
                });

                Status = new Status(true);
            }
            catch (Exception ex)
            {
                Status.ExceptionMessage = ex.Message;
            }

            Response.Headers.Add("X-Status", StatusHandler.GetStatus(Status));

            return View(returnData);
        }

        [AllowAll]
        public async Task<IActionResult> ChargeResponse(ChargeResponse model)
        {
            if (model.StatusCode == 200)
            {
                Booking booking = await _UnitOfWork.Booking.GetByID(int.Parse(model.MerchantRefNumber));

                if (booking.TotalPrice == model.PaymentAmount)
                {
                    booking.Fk_BookingState = (int)BookingStateEnum.Success;
                    _UnitOfWork.Booking.UpdateEntity(booking);

                    _UnitOfWork.BookingStateHistory.CreateEntity(new BookingStateHistory
                    {
                        Fk_Booking = booking.Id,
                        Fk_BookingState = booking.Fk_BookingState,
                        CreatedBy = !string.IsNullOrEmpty(booking.LastModifiedBy) ? booking.LastModifiedBy : booking.CreatedBy,
                    });
                }
                _UnitOfWork.BookingPayment.CreateEntity(new BookingPayment
                {
                    Fk_Booking = booking.Id,
                    Type = model.Type,
                    ReferenceNumber = model.ReferenceNumber,
                    MerchantRefNumber = model.MerchantRefNumber,
                    OrderAmount = model.OrderAmount,
                    PaymentAmount = model.PaymentAmount,
                    FawryFees = model.FawryFees,
                    PaymentMethod = model.PaymentMethod,
                    OrderStatus = model.OrderStatus,
                    PaymentTime = model.PaymentTime,
                    CustomerMobile = model.CustomerMobile,
                    CustomerMail = model.CustomerMail,
                    CustomerProfileId = model.CustomerProfileId,
                    Signature = model.Signature,
                    StatusCode = model.StatusCode,
                    StatusDescription = model.StatusDescription
                });
                await _UnitOfWork.Save();
            }

            return Ok();
        }
    }
}
