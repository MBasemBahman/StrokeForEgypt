using StrokeForEgypt.Common;
using StrokeForEgypt.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace StrokeForEgypt.API.Helpers
{
    public class StatusHandler
    {
        public static string GetStatus(Status model)
        {
            model.ErrorMessage = EncodeManager.Base64Encode(model.ErrorMessage);
            return JsonSerializer.Serialize(model).Replace(",", @"\002C");
        }

        public static string GetRefresh(string token)
        {
            Set_Refresh model = new()
            {
                RefreshToken = token,
                Expires = DateTime.UtcNow.AddDays(7).ToString("ddd, dd MMM yyy HH:mm:ss 'GMT'")
            };
            return JsonSerializer.Serialize(model).Replace(",", @"\002C");
        }

        public static string GetExpires()
        {
            return DateTime.UtcNow.AddMinutes(60).ToString("ddd, dd MMM yyy HH:mm:ss 'GMT'").Replace(",", @"\002C");
        }
    }
    public class StatusHandler<T>
    {
        public static string GetPagination(PaginationMetaData<T> model)
        {
            return JsonSerializer.Serialize(model).Replace(",", @"\002C");
        }
    }
}
