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
        private readonly bool Production = true;

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

                FawryManager fawryManager = new(Production);
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
                        Quantity = 1
                    }
                });

                _UnitOfWork.BookingPayment.CreateEntity(new BookingPayment
                {
                    Fk_Booking = booking.Id,
                    MerchantRefNumber = returnData.MerchantRefNum,
                    CustomerMobile = returnData.CustomerMobile,
                    CustomerMail = returnData.CustomerEmail,
                    CustomerProfileId = returnData.CustomerProfileId,
                    Signature = returnData.Signature
                });
                await _UnitOfWork.Save();

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
                if (_UnitOfWork.BookingPayment.Any(a => a.Fk_Booking == int.Parse(model.MerchantRefNumber) &&
                                                        a.MerchantRefNumber == model.MerchantRefNumber &&
                                                        a.CustomerProfileId == model.CustomerProfileId))
                {
                    FawryManager fawryManager = new(Production);
                    ChargeResponse paymentStatus = fawryManager.GetPaymentStatus(model.MerchantRefNumber);

                    if (paymentStatus != null &&
                        paymentStatus.OrderStatus == model.OrderStatus &&
                        paymentStatus.PaymentAmount == model.PaymentAmount)
                    {
                        Booking booking = await _UnitOfWork.Booking.GetByID(int.Parse(model.MerchantRefNumber));

                        if (booking.TotalPrice == model.PaymentAmount &&
                            booking.Fk_BookingState != (int)BookingStateEnum.Success &&
                            model.OrderStatus == "PAID")
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

                        BookingPayment bookingPayment = await _UnitOfWork.BookingPayment.GetFirst(a => a.Fk_Booking == int.Parse(model.MerchantRefNumber) &&
                                                           a.MerchantRefNumber == model.MerchantRefNumber &&
                                                           a.CustomerProfileId == model.CustomerProfileId);

                        bookingPayment.ReferenceNumber = model.ReferenceNumber;
                        bookingPayment.OrderAmount = model.OrderAmount;
                        bookingPayment.PaymentAmount = model.PaymentAmount;
                        bookingPayment.FawryFees = model.FawryFees;
                        bookingPayment.PaymentMethod = model.PaymentMethod;
                        bookingPayment.OrderStatus = model.OrderStatus;
                        bookingPayment.CustomerMobile = model.CustomerMobile;
                        bookingPayment.PaymentTime = model.PaymentTime;
                        bookingPayment.CustomerMail = model.CustomerMail;
                        bookingPayment.StatusCode = model.StatusCode;
                        bookingPayment.StatusDescription = model.StatusDescription;
                        bookingPayment.Signature = model.Signature;
                        bookingPayment.LastModifiedAt = DateTime.UtcNow;

                        _UnitOfWork.BookingPayment.UpdateEntity(bookingPayment);
                        await _UnitOfWork.Save();
                    }
                }
            }

            return RedirectToAction(nameof(PaymentStatus), new { model.StatusCode, model.StatusDescription });
        }

        [AllowAll]
        public IActionResult PaymentStatus(int StatusCode, string StatusDescription)
        {
            ViewData["StatusCode"] = StatusCode;
            ViewData["StatusDescription"] = StatusDescription;

            return View();
        }
    }
}
