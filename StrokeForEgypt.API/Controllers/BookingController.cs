using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StrokeForEgypt.API.Authorization;
using StrokeForEgypt.API.Helpers;
using StrokeForEgypt.API.Services;
using StrokeForEgypt.Common;
using StrokeForEgypt.DAL;
using StrokeForEgypt.Entity.AccountEntity;
using StrokeForEgypt.Entity.BookingEntity;
using StrokeForEgypt.Entity.EventEntity;
using StrokeForEgypt.Repository;
using StrokeForEgypt.Service;
using StrokeForEgypt.Service.BookingEntity;
using StrokeForEgypt.Service.EventEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static StrokeForEgypt.Common.EnumData;

namespace StrokeForEgypt.API.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "Booking")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class BookingController : ControllerBase
    {
        private readonly BaseDBContext _DBContext;
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly EntityLocalizationService _Localizer;

        public BookingController(BaseDBContext dataContext, UnitOfWork unitOfWork, IMapper mapper, EntityLocalizationService Localizer)
        {
            _DBContext = dataContext;
            _UnitOfWork = unitOfWork;
            _Mapper = mapper;
            _Localizer = Localizer;
        }

        /// <summary>
        /// Post: Create
        /// </summary>
        [HttpPost]
        [Route(nameof(Create))]
        public async Task<BookingModel> Create([FromBody] BookingCreateModel model)
        {
            BookingModel returnData = new();

            Status Status = new();

            try
            {
                Account account = (Account)Request.HttpContext.Items["Account"];

                Booking booking = new();

                _Mapper.Map(model, booking);

                booking.Fk_Account = account.Id;
                booking.Fk_BookingState = (int)BookingStateEnum.PendingOnPayment;
                booking.BookingStateHistories = new List<BookingStateHistory> { new BookingStateHistory
                {
                    Fk_BookingState = booking.Fk_BookingState
                } };

                _UnitOfWork.Booking.CreateEntity(booking);
                await _UnitOfWork.Save();

                booking = await _UnitOfWork.Booking.GetFirst(a => a.Id == booking.Id, new List<string>
                {
                    "BookingState",
                    "EventPackage"
                });

                _Mapper.Map(booking, returnData);

                returnData.BookingState = new BookingStateModel();
                _Mapper.Map(booking.BookingState, returnData.BookingState);

                returnData.EventPackage = new EventPackageModel();
                _Mapper.Map(booking.EventPackage, returnData.EventPackage);

                Status = new Status(true);
            }
            catch (Exception ex)
            {
                Status.ExceptionMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    Status.ExceptionMessage = ex.InnerException.Message;
                }
            }

            Response.Headers.Add("X-Status", StatusHandler.GetStatus(Status));

            return returnData;
        }

        /// <summary>
        /// Post: Add Member
        /// </summary>
        [HttpPost]
        [Route(nameof(AddMember))]
        public async Task<BookingMemberModel> AddMember([FromForm] BookingMemberCreateModel model)
        {
            BookingMemberModel returnData = new();

            Status Status = new();

            try
            {
                Account account = (Account)Request.HttpContext.Items["Account"];

                if (_UnitOfWork.Booking.Any(a => a.Id == model.Fk_Booking && a.Fk_Account == account.Id))
                {
                    BookingMember bookingMember = new();

                    _Mapper.Map(model, bookingMember);

                    if (model.Activities != null && model.Activities.Any())
                    {
                        bookingMember.BookingMemberActivities = new List<BookingMemberActivity>();

                        model.Activities
                             .ToList()
                             .ForEach(Fk_EventActivity => bookingMember.BookingMemberActivities.Add(new BookingMemberActivity
                             {
                                 Fk_EventActivity = Fk_EventActivity
                             }));
                    }

                    _UnitOfWork.BookingMember.CreateEntity(bookingMember);
                    await _UnitOfWork.Save();

                    if (model.Attachments != null && model.Attachments.Any())
                    {
                        foreach (IFormFile File in model.Attachments)
                        {
                            await _UnitOfWork.BookingMemberAttachment.UploudFile(bookingMember.Id, File, "wwwroot/Uploud/BookingMemberAttachment");
                        }
                    }

                    bookingMember = await _UnitOfWork.BookingMember.GetFirst(a => a.Id == bookingMember.Id, new List<string>
                {
                    "BookingMemberAttachments"
                });

                    _Mapper.Map(bookingMember, returnData);

                    returnData.BookingMemberAttachments = new List<BookingMemberAttachmentModel>();

                    _Mapper.Map(bookingMember.BookingMemberAttachments, returnData.BookingMemberAttachments);

                    returnData.BookingMemberActivities = new List<EventActivityModel>();

                    List<EventActivity> BookingMemberActivities = await _UnitOfWork.EventActivity.GetAll(a => a.BookingMemberActivities.Any(b => b.Fk_BookingMember == bookingMember.Id));
                    _Mapper.Map(BookingMemberActivities, returnData.BookingMemberAttachments);

                    Status = new Status(true);
                }
            }
            catch (Exception ex)
            {
                Status.ExceptionMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    Status.ExceptionMessage = ex.InnerException.Message;
                }
            }

            Response.Headers.Add("X-Status", StatusHandler.GetStatus(Status));

            return returnData;
        }

        /// <summary>
        /// Get: Get Bookings
        /// </summary>
        [HttpGet]
        [Route(nameof(GetBookings))]
        public async Task<List<BookingModel>> GetBookings(
            [FromQuery] Paging paging)
        {
            string ActionName = nameof(GetBookings);
            List<BookingModel> returnData = new();
            Status Status = new();

            try
            {
                Account account = (Account)Request.HttpContext.Items["Account"];

                List<Booking> Data = await _UnitOfWork.Booking.GetAll(a => a.IsActive && a.Fk_Account == account.Id, new List<string>
                {
                    "BookingState",
                    "EventPackage"
                });

                Data = OrderBy<Booking>.OrderData(Data, paging.OrderBy);

                PagedList<Booking> PagedData = PagedList<Booking>.Create(Data, paging.PageNumber, paging.PageSize);

                foreach (Booking booking in PagedData)
                {
                    BookingModel bookingModel = new();
                    _Mapper.Map(booking, bookingModel);

                    bookingModel.BookingState = new BookingStateModel();
                    _Mapper.Map(booking.BookingState, bookingModel.BookingState);

                    bookingModel.EventPackage = new EventPackageModel();
                    _Mapper.Map(booking.EventPackage, bookingModel.EventPackage);

                    returnData.Add(bookingModel);
                }

                PaginationMetaData<Booking> PaginationMetaData = new(PagedData)
                {
                    PrevoisPageLink = (PagedData.HasPrevious) ? Url.Link(ActionName, new { paging.OrderBy, pageNumber = (paging.PageNumber - 1), paging.PageSize }) : null,
                    NextPageLink = (PagedData.HasNext) ? Url.Link(ActionName, new { paging.OrderBy, pageNumber = (paging.PageNumber + 1), paging.PageSize }) : null
                };

                Response.Headers.Add("X-Pagination", StatusHandler<Booking>.GetPagination(PaginationMetaData));

                Status = new Status(true);
            }
            catch (Exception ex)
            {
                Status.ExceptionMessage = ex.Message;
            }

            
            Response.Headers.Add("X-Status", StatusHandler.GetStatus(Status));

            return returnData;
        }

        /// <summary>
        /// Get: Get Booking Profile
        /// </summary>
        [HttpGet]
        [Route(nameof(GetBookingProfile))]
        public async Task<BookingModel> GetBookingProfile(int id)
        {
            BookingModel returnData = new();

            Status Status = new();

            try
            {
                Account account = (Account)Request.HttpContext.Items["Account"];

                if (_UnitOfWork.Booking.Any(a => a.Id == id && a.Fk_Account == account.Id))
                {
                    Booking booking = await _UnitOfWork.Booking.GetFirst(a => a.Id == id, new List<string>
                {
                    "BookingState",
                    "EventPackage"
                });

                    _Mapper.Map(booking, returnData);

                    returnData.BookingState = new BookingStateModel();
                    _Mapper.Map(booking.BookingState, returnData.BookingState);

                    returnData.EventPackage = new EventPackageModel();
                    _Mapper.Map(booking.EventPackage, returnData.EventPackage);

                    Status = new Status(true);
                }
            }
            catch (Exception ex)
            {
                Status.ExceptionMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    Status.ExceptionMessage = ex.InnerException.Message;
                }
            }

            Response.Headers.Add("X-Status", StatusHandler.GetStatus(Status));

            return returnData;
        }

        /// <summary>
        /// Get: Get Booking Members
        /// </summary>
        [HttpGet]
        [Route(nameof(GetBookingMembers))]
        public async Task<List<BookingMemberModel>> GetBookingMembers(
            [FromQuery] Paging paging,
            [FromQuery] int Fk_Booking)
        {
            string ActionName = nameof(GetBookingMembers);
            List<BookingMemberModel> returnData = new();
            Status Status = new();

            try
            {
                Account account = (Account)Request.HttpContext.Items["Account"];

                if (_UnitOfWork.Booking.Any(a => a.Id == Fk_Booking && a.Fk_Account == account.Id))
                {
                    List<BookingMember> Data = await _UnitOfWork.BookingMember.GetAll(a => a.IsActive && a.Fk_Booking == Fk_Booking &&
                                                                             a.Booking.Fk_Account == account.Id, new List<string>
                    {
                            "BookingMemberAttachments"
                    });

                    Data = OrderBy<BookingMember>.OrderData(Data, paging.OrderBy);

                    PagedList<BookingMember> PagedData = PagedList<BookingMember>.Create(Data, paging.PageNumber, paging.PageSize);

                    foreach (BookingMember bookingMember in PagedData)
                    {
                        BookingMemberModel bookingMemberModel = new();

                        _Mapper.Map(bookingMember, returnData);

                        bookingMemberModel.BookingMemberAttachments = new List<BookingMemberAttachmentModel>();

                        _Mapper.Map(bookingMember.BookingMemberAttachments, bookingMemberModel.BookingMemberAttachments);

                        bookingMemberModel.BookingMemberActivities = new List<EventActivityModel>();

                        List<EventActivity> BookingMemberActivities = await _UnitOfWork.EventActivity.GetAll(a => a.BookingMemberActivities.Any(b => b.Fk_BookingMember == bookingMember.Id));
                        _Mapper.Map(BookingMemberActivities, bookingMemberModel.BookingMemberAttachments);

                        returnData.Add(bookingMemberModel);
                    }

                    PaginationMetaData<BookingMember> PaginationMetaData = new(PagedData)
                    {
                        PrevoisPageLink = (PagedData.HasPrevious) ? Url.Link(ActionName, new { paging.OrderBy, pageNumber = (paging.PageNumber - 1), paging.PageSize }) : null,
                        NextPageLink = (PagedData.HasNext) ? Url.Link(ActionName, new { paging.OrderBy, pageNumber = (paging.PageNumber + 1), paging.PageSize }) : null
                    };

                    Response.Headers.Add("X-Pagination", StatusHandler<BookingMember>.GetPagination(PaginationMetaData));

                    Status = new Status(true);
                }

            }
            catch (Exception ex)
            {
                Status.ExceptionMessage = ex.Message;
            }

            
            Response.Headers.Add("X-Status", StatusHandler.GetStatus(Status));

            return returnData;
        }
    }
}
