using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrokeForEgypt.API.Authorization;
using StrokeForEgypt.API.Services;
using StrokeForEgypt.Common;
using StrokeForEgypt.DAL;
using StrokeForEgypt.Entity.MainDataEntity;
using StrokeForEgypt.Repository;
using StrokeForEgypt.Service;
using StrokeForEgypt.Service.MainDataEntity;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace StrokeForEgypt.API.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "MainData")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class MainDataController : ControllerBase
    {
        private readonly BaseDBContext _DBContext;
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly EntityLocalizationService _Localizer;

        public MainDataController(BaseDBContext dataContext, UnitOfWork unitOfWork, IMapper mapper, EntityLocalizationService Localizer)
        {
            _DBContext = dataContext;
            _UnitOfWork = unitOfWork;
            _Mapper = mapper;
            _Localizer = Localizer;
        }

        /// <summary>
        /// Get: Get App About
        /// </summary>
        [HttpGet]
        [Route(nameof(GetAppAbout))]
        public async Task<AppAboutModel> GetAppAbout()
        {
            AppAboutModel returnData = new();
            Status Status = new();

            try
            {
                AppAbout Data = await _UnitOfWork.AppAbout.GetFirst();

                _Mapper.Map(Data, returnData);

                List<Gallery> Gallery = await _UnitOfWork.Gallery.GetAll(a => a.IsActive);
                returnData.Galleries = new List<GalleryModel>();
                _Mapper.Map(Gallery, returnData.Galleries);

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

        /// <summary>
        /// Get: Get Galleries
        /// </summary>
        [HttpGet]
        [Route(nameof(GetGalleries))]
        public async Task<List<GalleryModel>> GetGalleries(
            [FromQuery] Paging paging)
        {
            string ActionName = nameof(GetGalleries);
            List<GalleryModel> returnData = new();
            Status Status = new();

            try
            {
                List<Gallery> Data = await _UnitOfWork.Gallery.GetAll(a => a.IsActive);

                Data = OrderBy<Gallery>.OrderData(Data, paging.OrderBy);

                PagedList<Gallery> PagedData = PagedList<Gallery>.Create(Data, paging.PageNumber, paging.PageSize);

                _Mapper.Map(PagedData, returnData);

                PaginationMetaData<Gallery> PaginationMetaData = new(PagedData)
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

        /// <summary>
        /// Get: Get Genders
        /// </summary>
        [HttpGet]
        [Route(nameof(GetGenders))]
        public async Task<List<GenderModel>> GetGenders(
            [FromQuery] Paging paging)
        {
            string ActionName = nameof(GetGenders);
            List<GenderModel> returnData = new();
            Status Status = new();

            try
            {
                List<Gender> Data = await _UnitOfWork.Gender.GetAll(a => a.IsActive);

                Data = OrderBy<Gender>.OrderData(Data, paging.OrderBy);

                PagedList<Gender> PagedData = PagedList<Gender>.Create(Data, paging.PageNumber, paging.PageSize);

                _Mapper.Map(PagedData, returnData);

                PaginationMetaData<Gender> PaginationMetaData = new(PagedData)
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

        /// <summary>
        /// Get: Get Countries
        /// </summary>
        [HttpGet]
        [Route(nameof(GetCountries))]
        public async Task<List<CountryModel>> GetCountries(
            [FromQuery] Paging paging,
            [FromQuery] string Name)
        {
            string ActionName = nameof(GetCountries);
            List<CountryModel> returnData = new();
            Status Status = new();

            try
            {
                List<Country> Data = await _UnitOfWork.Country.GetAll(a => a.IsActive &&
                                                                           (string.IsNullOrEmpty(Name) || a.Name.ToLower().Trim() == Name.ToLower().Trim()));

                Data = OrderBy<Country>.OrderData(Data, paging.OrderBy);

                PagedList<Country> PagedData = PagedList<Country>.Create(Data, paging.PageNumber, paging.PageSize);

                _Mapper.Map(PagedData, returnData);

                PaginationMetaData<Country> PaginationMetaData = new(PagedData)
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

        /// <summary>
        /// Get: Get Cities
        /// </summary>
        [HttpGet]
        [Route(nameof(GetCities))]
        public async Task<List<CityModel>> GetCities(
            [FromQuery] Paging paging,
            [FromQuery] string Name,
            [FromQuery] int Fk_Country = 0)
        {
            string ActionName = nameof(GetCities);
            List<CityModel> returnData = new();
            Status Status = new();

            try
            {
                List<City> Data = await _UnitOfWork.City.GetAll(a => a.IsActive &&
                                                                    (string.IsNullOrEmpty(Name) || a.Name.ToLower().Trim() == Name.ToLower().Trim()) &&
                                                                    (Fk_Country == 0 || a.Fk_Country == Fk_Country));

                Data = OrderBy<City>.OrderData(Data, paging.OrderBy);

                PagedList<City> PagedData = PagedList<City>.Create(Data, paging.PageNumber, paging.PageSize);

                _Mapper.Map(PagedData, returnData);

                PaginationMetaData<City> PaginationMetaData = new(PagedData)
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
