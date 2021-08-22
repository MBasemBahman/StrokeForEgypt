using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StrokeForEgypt.API.Authorization;
using StrokeForEgypt.API.Models.Accounts;
using StrokeForEgypt.API.Services;
using StrokeForEgypt.Common;
using StrokeForEgypt.DAL;
using StrokeForEgypt.Entity.AccountEntity;
using StrokeForEgypt.Repository;
using StrokeForEgypt.Service;
using StrokeForEgypt.Service.AccountEntity;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace StrokeForEgypt.API.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "Account")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AccountController : ControllerBase
    {
        private readonly BaseDBContext _DBContext;
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly EntityLocalizationService _Localizer;
        private readonly IAccountService _AccountService;

        public AccountController(
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
        /// Post: Send Verification Code
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route(nameof(SendVerificationCode))]
        public async Task<string> SendVerificationCode([FromBody] EmailCode model)
        {
            string returnData = "";

            Status Status = new();

            try
            {
                if (!_UnitOfWork.Account.Any(a => a.Email == model.Email))
                {
                    returnData = RandomGenerator.RandomString(3, true) + RandomGenerator.RandomNumber(100, 999);

                    // Send Email
                    string Title = "'Stroke For Egypt' password code";
                    string Message = "Your verification code is: " + returnData;

                    await EmailManager.SendMail(model.Email, Title, Message);

                    Status = new Status(true);
                }
                else
                {
                    Status.ErrorMessage = _Localizer.Get("Email already registered!");
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

            Response.Headers.Add("X-Status", JsonSerializer.Serialize(Status));

            return returnData;
        }

        /// <summary>
        /// Post: Forget Password
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route(nameof(ForgetPassword))]
        public async Task<string> ForgetPassword([FromBody] EmailCode model)
        {
            string returnData = "";

            Status Status = new();

            try
            {
                if (_UnitOfWork.Account.Any(a => a.Email == model.Email))
                {
                    returnData = RandomGenerator.RandomString(3, true) + RandomGenerator.RandomNumber(100, 999);

                    // Send Email
                    string Title = "'Stroke For Egypt' password code";
                    string Message = "Your verification code is: " + returnData;

                    await EmailManager.SendMail(model.Email, Title, Message);

                    Status = new Status(true);
                }
                else
                {
                    Status.ErrorMessage = _Localizer.Get("Email not registered register now!");
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

            Response.Headers.Add("X-Status", JsonSerializer.Serialize(Status));

            return returnData;
        }

        /// <summary>
        /// Post: Authenticate
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route(nameof(Authenticate))]
        public AuthenticateResponse Authenticate([FromBody] AuthenticateRequest model)
        {
            AuthenticateResponse returnData = new();

            Status Status = new();

            try
            {
                returnData = _AccountService.Authenticate(model, IpAddress());
                SetTokenCookie(returnData.RefreshToken);

                Status = new Status(true);
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

            Response.Headers.Add("X-Status", JsonSerializer.Serialize(Status));

            return returnData;
        }

        /// <summary>
        /// Post: Add Account Device
        /// </summary>
        [HttpPost]
        [Route(nameof(AddAccountDevice))]
        public async void AddAccountDevice([FromBody] AccountDeviceModel model)
        {
            Status Status = new();

            try
            {
                Account account = (Account)Request.HttpContext.Items["Account"];

                AccountDevice accountDevice = new();

                _Mapper.Map(model, accountDevice);

                accountDevice.Fk_Account = account.Id;

                _UnitOfWork.AccountDevice.CreateEntity(accountDevice);
                await _UnitOfWork.Save();

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

            Response.Headers.Add("X-Status", JsonSerializer.Serialize(Status));
        }

        /// <summary>
        /// Post: RefreshToken
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route(nameof(RefreshToken))]
        public AuthenticateResponse RefreshToken()
        {
            AuthenticateResponse returnData = new();

            Status Status = new();

            try
            {
                string refreshToken = Request.Cookies["refreshToken"];
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    AuthenticateResponse response = _AccountService.RefreshToken(refreshToken, IpAddress());
                    SetTokenCookie(response.RefreshToken);

                    Status = new Status(true);
                }
                else
                {
                    Status.ErrorMessage = _Localizer.Get("Token is required!");
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

            Response.Headers.Add("X-Status", JsonSerializer.Serialize(Status));

            return returnData;
        }

        /// <summary>
        /// Post: Revoke Token
        /// </summary>
        [HttpPost]
        [Route(nameof(RevokeToken))]
        public void RevokeToken([FromBody] RevokeTokenRequest model)
        {
            Status Status = new();

            try
            {
                // accept refresh token in request body or cookie
                string token = model.Token ?? Request.Cookies["refreshToken"];

                if (!string.IsNullOrEmpty(token))
                {
                    _AccountService.RevokeToken(token, IpAddress());

                    Status = new Status(true);
                }
                else
                {
                    Status.ErrorMessage = _Localizer.Get("Token is required!");
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

            Response.Headers.Add("X-Status", JsonSerializer.Serialize(Status));
        }

        // helper methods

        private void SetTokenCookie(string token)
        {
            // append cookie with refresh token to the http response
            CookieOptions cookieOptions = new()
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
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
