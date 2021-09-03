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
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

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
                    SetJwtTokenHeader(returnData.JwtToken);
                    SetTokenCookie(returnData.RefreshToken);

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

                    SetJwtTokenHeader(returnData.JwtToken);
                    SetTokenCookie(returnData.RefreshToken);

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

        /// <summary>
        /// Post: Verify Account
        /// </summary>
        [HttpPost]
        [Route(nameof(VerifyAccount))]
        public async Task VerifyAccount([FromBody] VerifyAccountModel model)
        {
            Status Status = new();

            try
            {
                Account account = (Account)Request.HttpContext.Items["Account"];
                if (!string.IsNullOrEmpty(account.VerificationCodeHash) &&
                    BC.Verify(model.Code, account.VerificationCodeHash))
                {
                    Account data = await _UnitOfWork.Account.GetByID(account.Id);

                    data.VerificationCodeHash = null;
                    data.IsVerified = true;
                    data.LastModifiedAt = DateTime.UtcNow;

                    _UnitOfWork.Account.UpdateEntity(data);
                    await _UnitOfWork.Save();

                    Status = new Status(true);
                }
                else
                {
                    Status.ErrorMessage = _Localizer.Get("Verification code is wrong!");
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
        }

        /// <summary>
        /// Get: Get Profile
        /// </summary>
        [HttpGet]
        [Route(nameof(GetProfile))]
        public AccountFullModel GetProfile()
        {
            AccountFullModel returnData = new();

            Status Status = new();

            try
            {
                Account account = (Account)Request.HttpContext.Items["Account"];

                _Mapper.Map(account, returnData);

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
        /// Post: Edit Profile
        /// </summary>
        [HttpPost]
        [Route(nameof(EditProfile))]
        public async Task<AccountFullModel> EditProfile([FromBody] EditProfileModel model)
        {
            AccountFullModel returnData = new();

            Status Status = new();

            try
            {
                Account account = (Account)Request.HttpContext.Items["Account"];

                if (!string.IsNullOrEmpty(model.Phone) && _UnitOfWork.Account.Any(a => a.Id != account.Id && a.Phone == model.Phone))
                {
                    Status.ErrorMessage = _Localizer.Get("Phone already registered!");
                }
                else if (!string.IsNullOrEmpty(model.Email) && _UnitOfWork.Account.Any(a => a.Id != account.Id && a.Email == model.Email))
                {
                    Status.ErrorMessage = _Localizer.Get("Email already registered!");
                }
                else
                {
                    Account data = await _UnitOfWork.Account.GetByID(account.Id);

                    string code = "";
                    if (model.Email != data.Email)
                    {
                        code = RandomGenerator.RandomString(3, true) + RandomGenerator.RandomNumber(100, 999);
                        account.VerificationCodeHash = code;
                        account.IsVerified = false;
                    }

                    _Mapper.Map(model, data);

                    data.LastModifiedAt = DateTime.UtcNow;

                    _UnitOfWork.Account.UpdateEntity(data);
                    await _UnitOfWork.Save();

                    _Mapper.Map(data, returnData);

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
        /// Post: Change Profile Image
        /// </summary>
        [HttpPost]
        [Route(nameof(ChangeProfileImage))]
        public async Task<string> ChangeProfileImage([FromForm] ProfileImageModel model)
        {
            string returnData = "";

            Status Status = new();

            try
            {
                Account account = (Account)Request.HttpContext.Items["Account"];

                Account data = await _UnitOfWork.Account.GetByID(account.Id);

                if (model.ProfileImage != null)
                {
                    ImgManager ImgManager = new(AppMainData.WebRootPath);

                    string FileURL = await ImgManager.UploudImage(AppMainData.DomainName, account.Id.ToString(), model.ProfileImage, "wwwroot/Uploud/Account");

                    if (!string.IsNullOrEmpty(FileURL))
                    {
                        if (FileURL.Contains("wwwroot"))
                        {
                            FileURL = FileURL.Replace("wwwroot/", "");
                        }
                        data.ImageURL = FileURL;
                    }
                }

                data.LastModifiedAt = DateTime.UtcNow;

                _UnitOfWork.Account.UpdateEntity(data);
                await _UnitOfWork.Save();

                returnData = data.ImageURL;

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
        /// Post: Change Notification Token
        /// </summary>
        [HttpPost]
        [Route(nameof(ChangeNotificationToken))]
        public async Task<string> ChangeNotificationToken([FromBody] NotificationTokenModel model)
        {
            string returnData = "";

            Status Status = new();

            try
            {
                Account account = (Account)Request.HttpContext.Items["Account"];

                Account data = await _UnitOfWork.Account.GetByID(account.Id);

                data.NotificationToken = model.NotificationToken;
                data.LastModifiedAt = DateTime.UtcNow;

                _UnitOfWork.Account.UpdateEntity(data);
                await _UnitOfWork.Save();

                returnData = data.ImageURL;

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
        /// Post: Forget Password
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route(nameof(ForgetPassword))]
        public async Task ForgetPassword([FromBody] ForgetPasswordModel model)
        {
            Status Status = new();

            try
            {
                if (_UnitOfWork.Account.Any(a => a.Email == model.Email))
                {
                    Account data = await _UnitOfWork.Account.GetFirst(a => a.Email == model.Email);

                    if (data.IsActive)
                    {
                        string code = RandomGenerator.RandomString(3, true) + RandomGenerator.RandomNumber(100, 999);

                        data.VerificationCodeHash = BC.HashPassword(code);
                        data.LastModifiedAt = DateTime.UtcNow;

                        _UnitOfWork.Account.UpdateEntity(data);
                        await _UnitOfWork.Save();

                        // Send Email
                        string Title = "'Stroke For Egypt' verification code";
                        string Message = code;

                        await EmailManager.SendMailWithTemplate(model.Email, Title, Message);

                        Status = new Status(true);
                    }
                    else
                    {
                        Status.ErrorMessage = _Localizer.Get("Account is inactive contact support!");
                    }
                }
                else
                {
                    Status.ErrorMessage = _Localizer.Get("Email not registered, register now!");
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
        }

        /// <summary>
        /// Post: Reset Password
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route(nameof(ResetPassword))]
        public async Task<AuthenticateResponse> ResetPassword([FromBody] ResetPasswordModel model)
        {
            AuthenticateResponse returnData = new();

            Status Status = new();

            try
            {
                if (_UnitOfWork.Account.Any(a => a.Email == model.Email))
                {
                    Account account = await _UnitOfWork.Account.GetFirst(a => a.Email == model.Email);

                    if (!string.IsNullOrEmpty(account.VerificationCodeHash) &&
                    BC.Verify(model.Code, account.VerificationCodeHash))
                    {
                        account.VerificationCodeHash = null;
                        account.PasswordHash = BC.HashPassword(model.NewPassword);
                        account.LastModifiedAt = DateTime.UtcNow;

                        _UnitOfWork.Account.UpdateEntity(account);
                        await _UnitOfWork.Save();

                        returnData = _AccountService.Authenticate(new AuthenticateRequest
                        {
                            Email = account.Email,
                            Password = account.PasswordHash,
                            LoginToken = account.LoginTokenHash
                        }, IpAddress());

                        SetJwtTokenHeader(returnData.JwtToken);
                        SetTokenCookie(returnData.RefreshToken);

                        Status = new Status(true);
                    }
                    else
                    {
                        Status.ErrorMessage = _Localizer.Get("Verification code is wrong!");
                    }
                }
                else
                {
                    Status.ErrorMessage = _Localizer.Get("Email not registered, register now!");
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
        /// Post: Change Password
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route(nameof(ChangePassword))]
        public async void ChangePassword([FromBody] ChangePasswordModel model)
        {
            Status Status = new();

            try
            {
                Account account = (Account)Request.HttpContext.Items["Account"];

                if (!BC.Verify(model.OldPassword, account.PasswordHash))
                {
                    Status.ErrorMessage = _Localizer.Get("Old password is incorrect!");
                }
                else
                {
                    Account data = await _UnitOfWork.Account.GetByID(account.Id);

                    data.PasswordHash = BC.HashPassword(account.PasswordHash);
                    data.LastModifiedAt = DateTime.UtcNow;

                    _UnitOfWork.Account.UpdateEntity(data);
                    await _UnitOfWork.Save();

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
        }

        /// <summary>
        /// Post: Add Device
        /// </summary>
        [HttpPost]
        [Route(nameof(AddDevice))]
        public async void AddDevice([FromBody] AccountDeviceModel model)
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

            Response.Headers.Add("X-Status", StatusHandler.GetStatus(Status));
        }

        /// <summary>
        /// Post: RefreshToken
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route(nameof(RefreshToken))]
        public AuthenticateResponse RefreshToken([FromBody] RefreshTokenRequest model)
        {
            AuthenticateResponse returnData = new();

            Status Status = new();

            try
            {
                string token = model.Token ?? Request.Cookies["refreshToken"];
                token = WebUtility.UrlDecode(token);
                if (!string.IsNullOrEmpty(token))
                {
                    AuthenticateResponse response = _AccountService.RefreshToken(token, IpAddress());

                    SetJwtTokenHeader(response.JwtToken);
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

            Response.Headers.Add("X-Status", StatusHandler.GetStatus(Status));

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

            Response.Headers.Add("X-Status", StatusHandler.GetStatus(Status));
        }

        // helper methods

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
