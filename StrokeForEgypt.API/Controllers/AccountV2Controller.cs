using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StrokeForEgypt.API.Authorization;
using StrokeForEgypt.API.Helpers;
using StrokeForEgypt.API.Models.Accounts;
using StrokeForEgypt.API.Services;
using StrokeForEgypt.Common;
using StrokeForEgypt.DAL;
using StrokeForEgypt.Entity.AccountEntity;
using StrokeForEgypt.Repository;
using StrokeForEgypt.Service;
using StrokeForEgypt.Service.AccountEntity;
using System;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace StrokeForEgypt.API.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "AccountV2")]
    [Route("api/v{version:apiVersion}/Account")]
    [ApiVersion("2.0")]
    public class AccountV2Controller : ControllerBase
    {
        private readonly BaseDBContext _DBContext;
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly EntityLocalizationService _Localizer;
        private readonly IAccountService _AccountService;

        public AccountV2Controller(
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

        /// <summary>
        /// Post: Login
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route(nameof(Login))]
        public AuthenticateResponse Login([FromBody] AuthenticateRequest model)
        {
            AuthenticateResponse returnData = new();

            Status Status = new();

            try
            {
                returnData = _AccountService.Authenticate(model, IpAddress());

                if (returnData.IsActive)
                {
                    if (returnData.IsVerified)
                    {
                        SetJwtTokenHeader(returnData.JwtToken);
                        SetTokenCookie(returnData.RefreshToken);
                    }

                    Status = new Status(true);
                }
                else
                {
                    Status.ErrorMessage = _Localizer.Get("Account is inactive contact support!");
                }

            }
            catch (Exception ex)
            {
                Status.ErrorMessage = _Localizer.Get(ex.Message);
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
        /// Post: Create Account
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route(nameof(Register))]
        public async Task<AuthenticateResponse> Register([FromBody] RegisterModel model)
        {
            AuthenticateResponse returnData = new();

            Status Status = new();

            try
            {
                if (string.IsNullOrEmpty(model.LoginToken) && string.IsNullOrEmpty(model.Email))
                {
                    Status.ErrorMessage = _Localizer.Get("Complete your profile!");
                }
                else if (!string.IsNullOrEmpty(model.Email) && _UnitOfWork.Account.Any(a => a.Email == model.Email))
                {
                    Status.ErrorMessage = _Localizer.Get("Email already registered!");
                }
                else if (!string.IsNullOrEmpty(model.Phone) && _UnitOfWork.Account.Any(a => a.Phone == model.Phone))
                {
                    Status.ErrorMessage = _Localizer.Get("Phone already registered!");
                }
                else if (!string.IsNullOrEmpty(model.LoginToken) && _UnitOfWork.Account.Any(a => a.LoginTokenHash == BC.HashPassword(model.LoginToken)))
                {
                    Status.ErrorMessage = _Localizer.Get("You already registered!");
                }
                else
                {
                    Account account = new();
                    _Mapper.Map(model, account);

                    string code = "";
                    if (!string.IsNullOrEmpty(model.Email))
                    {
                        code = RandomGenerator.RandomString(3, true) + RandomGenerator.RandomNumber(100, 999);
                        account.VerificationCodeHash = code;
                    }

                    account = _UnitOfWork.Account.Register(account);

                    _UnitOfWork.Account.CreateEntity(account);

                    await _UnitOfWork.Save();

                    returnData = _AccountService.Authenticate(new AuthenticateRequest
                    {
                        Email = model.Email,
                        Password = model.Password,
                        LoginToken = model.LoginToken
                    }, IpAddress());

                    if (returnData.IsVerified)
                    {
                        SetJwtTokenHeader(returnData.JwtToken);
                        SetTokenCookie(returnData.RefreshToken);
                    }

                    Status = new Status(true);

                    if (!string.IsNullOrEmpty(code))
                    {
                        // Send Email
                        string Title = "'Stroke For Egypt' verification code";
                        string Message = code;

                        await EmailManager.SendMailWithTemplate(account.Email, Title, Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Status.ErrorMessage = _Localizer.Get(ex.Message);
                Status.ExceptionMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    Status.ExceptionMessage = ex.InnerException.Message;
                }
            }

            Response.Headers.Add("X-Status", StatusHandler.GetStatus(Status));

            return returnData;
        }

        private void SetTokenCookie(string token)
        {
            // append cookie with refresh token to the http response
            //CookieOptions cookieOptions = new()
            //{
            //    HttpOnly = true,
            //    Expires = DateTime.UtcNow.AddDays(7)
            //};
            //Response.Cookies.Append("refreshToken", token, cookieOptions);

            Response.Headers.Add("Set-Refresh", StatusHandler.GetRefresh(token));
        }

        private void SetJwtTokenHeader(string token)
        {
            Response.Headers.Add("Expires", StatusHandler.GetExpires());
            Response.Headers.Add("Authorization", token);
        }

        private string IpAddress()
        {
            // get source ip address for the current request
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return Request.Headers["X-Forwarded-For"];
            }
            else
            {
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }
        }
    }
}
