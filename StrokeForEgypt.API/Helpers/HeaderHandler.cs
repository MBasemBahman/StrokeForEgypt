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
    }
    public class StatusHandler<T>
    {
        public static string GetPagination(PaginationMetaData<T> model)
        {
            return JsonSerializer.Serialize(model);
        }
    }
}
