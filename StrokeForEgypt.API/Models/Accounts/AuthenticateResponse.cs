using StrokeForEgypt.Entity.AccountEntity;
using System.Text.Json.Serialization;

namespace StrokeForEgypt.API.Models.Accounts
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string JwtToken { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public AuthenticateResponse(Account account, string jwtToken, string refreshToken)
        {
            Id = account.Id;
            FullName = account.FullName;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}