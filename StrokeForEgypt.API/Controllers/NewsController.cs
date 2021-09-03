using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrokeForEgypt.API.Authorization;
using StrokeForEgypt.API.Helpers;
using StrokeForEgypt.API.Services;
using StrokeForEgypt.DAL;
using StrokeForEgypt.Entity.NewsEntity;
using StrokeForEgypt.Repository;
using StrokeForEgypt.Service;
using StrokeForEgypt.Service.NewsEntity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrokeForEgypt.API.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "News")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class NewsController : ControllerBase
    {
        private readonly BaseDBContext _DBContext;
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly EntityLocalizationService _Localizer;

        public NewsController(BaseDBContext dataContext, UnitOfWork unitOfWork, IMapper mapper, EntityLocalizationService Localizer)
        {
            _DBContext = dataContext;
            _UnitOfWork = unitOfWork;
            _Mapper = mapper;
            _Localizer = Localizer;
        }

        /// <summary>
        /// Get: Get News
        /// </summary>
        [HttpGet]
        [Route(nameof(GetNews))]
        public async Task<List<NewsModel>> GetNews(
            [FromQuery] Paging paging,
            [FromQuery] int Fk_Event)
        {
            string ActionName = nameof(GetNews);
            List<NewsModel> returnData = new();
            Status Status = new();

            try
            {
                List<News> Data = await _UnitOfWork.News.GetAll(a => a.IsActive && a.Fk_Event == Fk_Event);

                Data = OrderBy<News>.OrderData(Data, paging.OrderBy);

                PagedList<News> PagedData = PagedList<News>.Create(Data, paging.PageNumber, paging.PageSize);

                _Mapper.Map(PagedData, returnData);

                PaginationMetaData<News> PaginationMetaData = new(PagedData)
                {
                    PrevoisPageLink = (PagedData.HasPrevious) ? Url.Link(ActionName, new { paging.OrderBy, pageNumber = (paging.PageNumber - 1), paging.PageSize }) : null,
                    NextPageLink = (PagedData.HasNext) ? Url.Link(ActionName, new { paging.OrderBy, pageNumber = (paging.PageNumber + 1), paging.PageSize }) : null
                };

                Response.Headers.Add("X-Pagination", StatusHandler<News>.GetPagination(PaginationMetaData));

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
        /// Get: Get News Profile
        /// </summary>
        [HttpGet]
        [Route(nameof(GetNewsProfile))]
        public async Task<NewsModel> GetNewsProfile(
            [FromQuery] int Id)
        {
            NewsModel returnData = new();
            Status Status = new();

            try
            {
                News news = await _UnitOfWork.News.GetFirst(a => a.Id == Id, new List<string> { "NewsGalleries" });

                _Mapper.Map(news, returnData);

                returnData.NewsGalleries = new List<NewsGalleryModel>();
                _Mapper.Map(news.NewsGalleries, returnData.NewsGalleries);

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
