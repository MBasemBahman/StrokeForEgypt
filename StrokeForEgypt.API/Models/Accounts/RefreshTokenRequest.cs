using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StrokeForEgypt.API.Models.Accounts
{
    public class RefreshTokenRequest
    {
        public string Token { get; set; }
    }
}
