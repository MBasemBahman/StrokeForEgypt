using StrokeForEgypt.Entity.AccountEntity;
using System.Text.Json.Serialization;

namespace StrokeForEgypt.API.Models.Accounts
{
    public class AuthenticateResponse
    {
        public string FullName { get; set; }

        public string ImageURL { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string JwtToken { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public AuthenticateResponse()
        {

        }

        public AuthenticateResponse(Account account, string jwtToken, string refreshToken)
        {
            FullName = account.FullName;
            ImageURL = account.ImageURL;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}