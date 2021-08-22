using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrokeForEgypt.API.Services;
using StrokeForEgypt.Common;
using StrokeForEgypt.DAL;
using StrokeForEgypt.Entity.NewsEntity;
using StrokeForEgypt.Repository;
using StrokeForEgypt.Service;
using StrokeForEgypt.Service.NewsEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace StrokeForEgypt.API.Controllers
{
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
            [FromQuery] int Fk_Event = 0,
            [FromQuery] bool IncludeGallery = false)
        {
            string ActionName = nameof(GetNews);
            List<NewsModel> returnData = new();
            Status Status = new();

            try
            {
                List<News> Data = await _UnitOfWork.News.GetAll(a => a.IsActive &&
                                                                     (Fk_Event == 0 || (a.Fk_Event > 0 && a.Fk_Event == Fk_Event)));

                Data = OrderBy<News>.OrderData(Data, paging.OrderBy);

                PagedList<News> PagedData = PagedList<News>.Create(Data, paging.PageNumber, paging.PageSize);

                _Mapper.Map(PagedData, returnData);

                foreach (News News in PagedData)
                {
                    NewsModel newsModel = new();
                    _Mapper.Map(News, newsModel);

                    if (IncludeGallery)
                    {
                        News.NewsGalleries = await _UnitOfWork.NewsGallery.GetAll(a => a.IsActive &&
                                                                            a.Fk_News == News.Id);
                        News.NewsGalleries = OrderBy<NewsGallery>.OrderData(News.NewsGalleries.ToList(), paging.OrderBy);

                        newsModel.NewsGalleries = new List<NewsGalleryModel>();
                        _Mapper.Map(News.NewsGalleries, newsModel.NewsGalleries);
                    }

                    returnData.Add(newsModel);
                }

                PaginationMetaData<News> PaginationMetaData = new(PagedData)
                {
                    PrevoisPageLink = (PagedData.HasPrevious) ? Url.Link(ActionName, new { paging.OrderBy, pageNumber = (paging.PageNumber - 1), paging.PageSize }) : null,
                    NextPageLink = (PagedData.HasNext) ? Url.Link(ActionName, new { paging.OrderBy, pageNumber = (paging.PageNumber + 1), paging.PageSize }) : null
                };

                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(PaginationMetaData).Replace(@"\u0026", "&"));

                Status = new Status(true);
            }
            catch (Exception ex)
            {
                Status.ExceptionMessage = ex.Message;
            }

            Status.ErrorMessage = EncodeManager.Base64Encode(Status.ErrorMessage);
            Response.Headers.Add("X-Status", JsonSerializer.Serialize(Status));

            return returnData;
        }
    }
}
