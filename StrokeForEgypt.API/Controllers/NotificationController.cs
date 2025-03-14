﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrokeForEgypt.API.Authorization;
using StrokeForEgypt.API.Helpers;
using StrokeForEgypt.API.Services;
using StrokeForEgypt.Common;
using StrokeForEgypt.DAL;
using StrokeForEgypt.Entity.AccountEntity;
using StrokeForEgypt.Entity.NotificationEntity;
using StrokeForEgypt.Repository;
using StrokeForEgypt.Service;
using StrokeForEgypt.Service.NotificationEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static StrokeForEgypt.Common.EnumData;

namespace StrokeForEgypt.API.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "Notification")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class NotificationController : ControllerBase
    {
        private readonly BaseDBContext _DBContext;
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly EntityLocalizationService _Localizer;

        public NotificationController(BaseDBContext dataContext, UnitOfWork unitOfWork, IMapper mapper, EntityLocalizationService Localizer)
        {
            _DBContext = dataContext;
            _UnitOfWork = unitOfWork;
            _Mapper = mapper;
            _Localizer = Localizer;
        }

        /// <summary>
        /// Get: Get Notification Types
        /// </summary>
        [HttpGet]
        [Route(nameof(GetNotificationTypes))]
        public async Task<List<NotificationTypeModel>> GetNotificationTypes(
            [FromQuery] Paging paging)
        {
            string ActionName = nameof(GetNotificationTypes);
            List<NotificationTypeModel> returnData = new();
            Status Status = new();

            try
            {
                List<NotificationType> Data = await _UnitOfWork.NotificationType.GetAll(a => a.IsActive);

                Data = OrderBy<NotificationType>.OrderData(Data, paging.OrderBy);

                PagedList<NotificationType> PagedData = PagedList<NotificationType>.Create(Data, paging.PageNumber, paging.PageSize);

                _Mapper.Map(PagedData, returnData);

                PaginationMetaData<NotificationType> PaginationMetaData = new(PagedData)
                {
                    PrevoisPageLink = (PagedData.HasPrevious) ? Url.Link(ActionName, new { paging.OrderBy, pageNumber = (paging.PageNumber - 1), paging.PageSize }) : null,
                    NextPageLink = (PagedData.HasNext) ? Url.Link(ActionName, new { paging.OrderBy, pageNumber = (paging.PageNumber + 1), paging.PageSize }) : null
                };

                Response.Headers.Add("X-Pagination", StatusHandler<NotificationType>.GetPagination(PaginationMetaData));

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
        /// Get: Get Notifications
        /// </summary>
        [HttpGet]
        [Route(nameof(GetNotifications))]
        public async Task<List<NotificationModel>> GetNotifications(
            [FromQuery] Paging paging,
            [FromQuery] int Fk_Event = 0,
            [FromQuery] int Fk_NotificationType = 0,
            [FromQuery] int Fk_OpenType = 0)
        {
            string ActionName = nameof(GetNotifications);
            List<NotificationModel> returnData = new();
            Status Status = new();

            try
            {
                Account account = (Account)Request.HttpContext.Items["Account"];

                List<Notification> Data = await _UnitOfWork.Notification.GetAll(a => a.IsActive && a.Fk_NotificationType != (int)NotificationTypeEnum.Verification &&
                                                                                     (!a.NotificationAccounts.Any() || a.NotificationAccounts.Any(b => b.Fk_Account == account.Id)) &&
                                                                                     (Fk_Event == 0 || (a.Fk_Event > 0 && a.Fk_Event == Fk_Event)) &&
                                                                                     (Fk_NotificationType == 0 || a.Fk_NotificationType == Fk_NotificationType));

                Data = OrderBy<Notification>.OrderData(Data, paging.OrderBy);

                PagedList<Notification> PagedData = PagedList<Notification>.Create(Data, paging.PageNumber, paging.PageSize);

                _Mapper.Map(PagedData, returnData);

                PaginationMetaData<Notification> PaginationMetaData = new(PagedData)
                {
                    PrevoisPageLink = (PagedData.HasPrevious) ? Url.Link(ActionName, new { paging.OrderBy, pageNumber = (paging.PageNumber - 1), paging.PageSize }) : null,
                    NextPageLink = (PagedData.HasNext) ? Url.Link(ActionName, new { paging.OrderBy, pageNumber = (paging.PageNumber + 1), paging.PageSize }) : null
                };

                Response.Headers.Add("X-Pagination", StatusHandler<Notification>.GetPagination(PaginationMetaData));

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
