using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrokeForEgypt.API.Authorization;
using StrokeForEgypt.API.Helpers;
using StrokeForEgypt.API.Services;
using StrokeForEgypt.Common;
using StrokeForEgypt.DAL;
using StrokeForEgypt.Entity.SponsorEntity;
using StrokeForEgypt.Repository;
using StrokeForEgypt.Service;
using StrokeForEgypt.Service.SponsorEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace StrokeForEgypt.API.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "Sponsor")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class SponsorController : ControllerBase
    {
        private readonly BaseDBContext _DBContext;
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly EntityLocalizationService _Localizer;

        public SponsorController(BaseDBContext dataContext, UnitOfWork unitOfWork, IMapper mapper, EntityLocalizationService Localizer)
        {
            _DBContext = dataContext;
            _UnitOfWork = unitOfWork;
            _Mapper = mapper;
            _Localizer = Localizer;
        }

        /// <summary>
        /// Get: Get Sponsor Types
        /// </summary>
        [HttpGet]
        [Route(nameof(GetSponsorTypes))]
        public async Task<List<SponsorTypeModel>> GetSponsorTypes(
            [FromQuery] Paging paging,
            [FromQuery] int Id = 0,
            [FromQuery] bool IncludeSponsor = false,
            [FromQuery] int Fk_Event = 0)
        {
            string ActionName = nameof(GetSponsorTypes);
            List<SponsorTypeModel> returnData = new();
            Status Status = new();

            try
            {
                List<SponsorType> Data = await _UnitOfWork.SponsorType.GetAll(a => a.IsActive &&
                                                                                   (Id == 0 || a.Id == Id) &&
                                                                                   (IncludeSponsor == false || a.Sponsors.Any(a => a.IsActive)) &&
                                                                                   (Fk_Event == 0 || a.Sponsors.Any(a => a.IsActive && a.Fk_Event > 0 && a.Fk_Event == Fk_Event)));

                Data = OrderBy<SponsorType>.OrderData(Data, paging.OrderBy);

                PagedList<SponsorType> PagedData = PagedList<SponsorType>.Create(Data, paging.PageNumber, paging.PageSize);

                if (IncludeSponsor)
                {
                    foreach (SponsorType SponsorType in PagedData)
                    {
                        SponsorTypeModel sponsorTypeModel = new();

                        _Mapper.Map(SponsorType, sponsorTypeModel);

                        SponsorType.Sponsors = await _UnitOfWork.Sponsor.GetAll(a => a.IsActive &&
                                                                                     a.Fk_SponsorType == SponsorType.Id &&
                                                                                     (Fk_Event == 0 || (a.Fk_Event > 0 && a.Fk_Event == Fk_Event)));

                        SponsorType.Sponsors = OrderBy<Sponsor>.OrderData(SponsorType.Sponsors.ToList(), paging.OrderBy);

                        sponsorTypeModel.Sponsors = new List<SponsorModel>();
                        _Mapper.Map(SponsorType.Sponsors, sponsorTypeModel.Sponsors);

                        returnData.Add(sponsorTypeModel);
                    }
                }
                else
                {
                    _Mapper.Map(PagedData, returnData);
                }

                PaginationMetaData<SponsorType> PaginationMetaData = new(PagedData)
                {
                    PrevoisPageLink = (PagedData.HasPrevious) ? Url.Link(ActionName, new { paging.OrderBy, pageNumber = (paging.PageNumber - 1), paging.PageSize }) : null,
                    NextPageLink = (PagedData.HasNext) ? Url.Link(ActionName, new { paging.OrderBy, pageNumber = (paging.PageNumber + 1), paging.PageSize }) : null
                };

                Response.Headers.Add("X-Pagination", StatusHandler<SponsorType>.GetPagination(PaginationMetaData));

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
        /// Get: Get Sponsors
        /// </summary>
        [HttpGet]
        [Route(nameof(GetSponsors))]
        public async Task<List<SponsorModel>> GetSponsors(
            [FromQuery] Paging paging,
            [FromQuery] int Fk_Event = 0,
            [FromQuery] int Fk_SponsorType = 0)
        {
            string ActionName = nameof(GetSponsors);
            List<SponsorModel> returnData = new();
            Status Status = new();

            try
            {
                List<Sponsor> Data = await _UnitOfWork.Sponsor.GetAll(a => a.IsActive &&
                                                                    (Fk_Event == 0 || (a.Fk_Event > 0 && a.Fk_Event == Fk_Event)) &&
                                                                    (Fk_SponsorType == 0 || a.Fk_SponsorType == Fk_SponsorType));

                Data = OrderBy<Sponsor>.OrderData(Data, paging.OrderBy);

                PagedList<Sponsor> PagedData = PagedList<Sponsor>.Create(Data, paging.PageNumber, paging.PageSize);

                _Mapper.Map(PagedData, returnData);

                PaginationMetaData<Sponsor> PaginationMetaData = new(PagedData)
                {
                    PrevoisPageLink = (PagedData.HasPrevious) ? Url.Link(ActionName, new { paging.OrderBy, pageNumber = (paging.PageNumber - 1), paging.PageSize }) : null,
                    NextPageLink = (PagedData.HasNext) ? Url.Link(ActionName, new { paging.OrderBy, pageNumber = (paging.PageNumber + 1), paging.PageSize }) : null
                };

                Response.Headers.Add("X-Pagination", StatusHandler<Sponsor>.GetPagination(PaginationMetaData));

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
