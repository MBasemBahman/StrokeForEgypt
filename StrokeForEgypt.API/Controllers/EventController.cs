using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrokeForEgypt.API.Authorization;
using StrokeForEgypt.API.Helpers;
using StrokeForEgypt.API.Services;
using StrokeForEgypt.DAL;
using StrokeForEgypt.Entity.EventEntity;
using StrokeForEgypt.Repository;
using StrokeForEgypt.Service;
using StrokeForEgypt.Service.EventEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StrokeForEgypt.API.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "Event")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class EventController : ControllerBase
    {
        private readonly BaseDBContext _DBContext;
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly EntityLocalizationService _Localizer;

        public EventController(BaseDBContext dataContext, UnitOfWork unitOfWork, IMapper mapper, EntityLocalizationService Localizer)
        {
            _DBContext = dataContext;
            _UnitOfWork = unitOfWork;
            _Mapper = mapper;
            _Localizer = Localizer;
        }

        /// <summary>
        /// Get: Get Events
        /// </summary>
        [HttpGet]
        [Route(nameof(GetEvents))]
        public async Task<List<EventModel>> GetEvents(
            [FromQuery] Paging paging,
            [FromQuery] string Title,
            [FromQuery] DateTime? FromDate,
            [FromQuery] DateTime? ToDate,
            [FromQuery] DateTime? RegistrationFrom,
            [FromQuery] DateTime? RegistrationTo)
        {
            string ActionName = nameof(GetEvents);
            List<EventModel> returnData = new();
            Status Status = new();

            try
            {
                List<Event> Data = await _UnitOfWork.Event.GetAll(a => a.IsActive &&
                                                                      (string.IsNullOrEmpty(Title) || a.Title.ToLower().Trim() == Title.ToLower().Trim()) &&
                                                                      (FromDate == null || a.FromDate >= FromDate) &&
                                                                      (ToDate == null || a.ToDate <= ToDate) &&
                                                                      (RegistrationFrom == null || a.RegistrationFrom >= RegistrationFrom) &&
                                                                      (RegistrationTo == null || a.RegistrationTo <= RegistrationTo));

                Data = OrderBy<Event>.OrderData(Data, paging.OrderBy);

                PagedList<Event> PagedData = PagedList<Event>.Create(Data, paging.PageNumber, paging.PageSize);

                _Mapper.Map(PagedData, returnData);

                PaginationMetaData<Event> PaginationMetaData = new(PagedData)
                {
                    PrevoisPageLink = (PagedData.HasPrevious) ? Url.Link(ActionName, new { paging.OrderBy, pageNumber = (paging.PageNumber - 1), paging.PageSize }) : null,
                    NextPageLink = (PagedData.HasNext) ? Url.Link(ActionName, new { paging.OrderBy, pageNumber = (paging.PageNumber + 1), paging.PageSize }) : null
                };

                Response.Headers.Add("X-Pagination", StatusHandler<Event>.GetPagination(PaginationMetaData));

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
        /// Get: Get Event
        /// </summary>
        [HttpGet]
        [Route(nameof(GetEvent))]
        public EventModel GetEvent(
            [FromQuery] int Id)
        {
            EventModel returnData = new();
            Status Status = new();

            try
            {
                if (_UnitOfWork.Event.Any(a => a.IsActive && a.Id == Id))
                {
                    Event Data = _DBContext.Event
                                           .Include(a => a.EventAgendas.Where(a => a.IsActive))
                                           .ThenInclude(a => a.EventAgendaGalleries.Where(a => a.IsActive))
                                           .Include(a => a.EventPackages.Where(a => a.IsActive))
                                           .Include(a => a.EventGalleries.Where(a => a.IsActive))
                                           .Include(a => a.EventActivities.Where(a => a.IsActive))
                                           .Single(a => a.Id == Id);

                    _Mapper.Map(Data, returnData);

                    returnData.EventAgendas = new List<EventAgendaModel>();
                    if (Data.EventAgendas.Any())
                    {
                        foreach (EventAgenda EventAgenda in Data.EventAgendas)
                        {
                            EventAgendaModel eventAgendaModel = new();
                            _Mapper.Map(EventAgenda, eventAgendaModel);

                            eventAgendaModel.EventAgendaGalleries = new List<EventAgendaGalleryModel>();
                            _Mapper.Map(EventAgenda.EventAgendaGalleries, eventAgendaModel.EventAgendaGalleries);

                            returnData.EventAgendas.Add(eventAgendaModel);
                        }
                    }

                    returnData.EventPackages = new List<EventPackageModel>();
                    _Mapper.Map(Data.EventPackages, returnData.EventPackages);

                    returnData.EventGalleries = new List<EventGalleryModel>();
                    _Mapper.Map(Data.EventGalleries, returnData.EventGalleries);

                    returnData.EventActivities = new List<EventActivityModel>();
                    _Mapper.Map(Data.EventActivities, returnData.EventActivities);

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

        /// <summary>
        /// Get: Get Event Packages
        /// </summary>
        [HttpGet]
        [Route(nameof(GetEventPackages))]
        public async Task<List<EventPackageModel>> GetEventPackages(
            [FromQuery] Paging paging,
            [FromQuery] int Id)
        {
            string ActionName = nameof(GetEventPackages);
            List<EventPackageModel> returnData = new();
            Status Status = new();

            try
            {
                List<EventPackage> Data = await _UnitOfWork.EventPackage.GetAll(a => a.IsActive &&
                                                                                    a.Fk_Event == Id);

                Data = OrderBy<EventPackage>.OrderData(Data, paging.OrderBy);

                PagedList<EventPackage> PagedData = PagedList<EventPackage>.Create(Data, paging.PageNumber, paging.PageSize);

                _Mapper.Map(PagedData, returnData);

                PaginationMetaData<EventPackage> PaginationMetaData = new(PagedData)
                {
                    PrevoisPageLink = (PagedData.HasPrevious) ? Url.Link(ActionName, new { paging.OrderBy, pageNumber = (paging.PageNumber - 1), paging.PageSize }) : null,
                    NextPageLink = (PagedData.HasNext) ? Url.Link(ActionName, new { paging.OrderBy, pageNumber = (paging.PageNumber + 1), paging.PageSize }) : null
                };

                Response.Headers.Add("X-Pagination", StatusHandler<EventPackage>.GetPagination(PaginationMetaData));

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
        /// Get: Get Event Activities
        /// </summary>
        [HttpGet]
        [Route(nameof(GetEventActivities))]
        public async Task<List<EventActivityModel>> GetEventActivities(
            [FromQuery] Paging paging,
            [FromQuery] int Id = 0,
            [FromQuery] int Fk_Booking = 0)
        {
            string ActionName = nameof(GetEventActivities);
            List<EventActivityModel> returnData = new();
            Status Status = new();

            try
            {
                List<EventActivity> Data = await _UnitOfWork.EventActivity.GetAll(a => a.IsActive &&
                                                                                       (Id == 0 && a.Fk_Event == Id) &&
                                                                                       (Fk_Booking == 0 || a.BookingMemberActivities.Any(a => a.BookingMember.Fk_Booking == Fk_Booking)));

                Data = OrderBy<EventActivity>.OrderData(Data, paging.OrderBy);

                PagedList<EventActivity> PagedData = PagedList<EventActivity>.Create(Data, paging.PageNumber, paging.PageSize);

                _Mapper.Map(PagedData, returnData);

                PaginationMetaData<EventActivity> PaginationMetaData = new(PagedData)
                {
                    PrevoisPageLink = (PagedData.HasPrevious) ? Url.Link(ActionName, new { paging.OrderBy, pageNumber = (paging.PageNumber - 1), paging.PageSize }) : null,
                    NextPageLink = (PagedData.HasNext) ? Url.Link(ActionName, new { paging.OrderBy, pageNumber = (paging.PageNumber + 1), paging.PageSize }) : null
                };

                Response.Headers.Add("X-Pagination", StatusHandler<EventActivity>.GetPagination(PaginationMetaData));

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
        /// Get: Get Event Agendas
        /// </summary>
        [HttpGet]
        [Route(nameof(GetEventAgendas))]
        public List<EventAgendaModel> GetEventAgendas(
            [FromQuery] Paging paging,
            [FromQuery] int Id)
        {
            string ActionName = nameof(GetEventAgendas);
            List<EventAgendaModel> returnData = new();
            Status Status = new();

            try
            {
                List<EventAgenda> Data = _DBContext.EventAgenda
                                                     .Where(a => a.IsActive &&
                                                                 a.Fk_Event == Id)
                                                     .Include(a => a.EventAgendaGalleries.Where(a => a.IsActive))
                                                     .ToList();

                Data = OrderBy<EventAgenda>.OrderData(Data, paging.OrderBy);

                PagedList<EventAgenda> PagedData = PagedList<EventAgenda>.Create(Data, paging.PageNumber, paging.PageSize);


                foreach (EventAgenda EventAgenda in PagedData)
                {
                    EventAgendaModel eventAgendaModel = new();
                    _Mapper.Map(EventAgenda, eventAgendaModel);

                    //eventAgendaModel.EventAgendaGalleries = new List<EventAgendaGalleryModel>();
                    //_Mapper.Map(EventAgenda.EventAgendaGalleries, eventAgendaModel.EventAgendaGalleries);

                    returnData.Add(eventAgendaModel);
                }

                PaginationMetaData<EventAgenda> PaginationMetaData = new(PagedData)
                {
                    PrevoisPageLink = (PagedData.HasPrevious) ? Url.Link(ActionName, new { paging.OrderBy, pageNumber = (paging.PageNumber - 1), paging.PageSize }) : null,
                    NextPageLink = (PagedData.HasNext) ? Url.Link(ActionName, new { paging.OrderBy, pageNumber = (paging.PageNumber + 1), paging.PageSize }) : null
                };

                Response.Headers.Add("X-Pagination", StatusHandler<EventAgenda>.GetPagination(PaginationMetaData));

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
        /// Get: Get Event Agenda Profile
        /// </summary>
        [HttpGet]
        [Route(nameof(GetEventAgendaProfile))]
        public async Task<EventAgendaModel> GetEventAgendaProfile(
            [FromQuery] int Id)
        {
            EventAgendaModel returnData = new();
            Status Status = new();

            try
            {
                EventAgenda eventAgenda = await _UnitOfWork.EventAgenda.GetFirst(a => a.Id == Id, new List<string> { "EventAgendaGalleries" });

                _Mapper.Map(eventAgenda, returnData);

                returnData.EventAgendaGalleries = new List<EventAgendaGalleryModel>();
                _Mapper.Map(eventAgenda.EventAgendaGalleries, returnData.EventAgendaGalleries);

                Status = new Status(true);
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
