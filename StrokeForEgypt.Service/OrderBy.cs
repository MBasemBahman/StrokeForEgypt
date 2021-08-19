using System.Collections.Generic;
using System.Linq;

namespace StrokeForEgypt.Service
{
    public static class OrderBy<T>
    {
        public static List<T> OrderData(List<T> items, string OrderString)
        {
            if (!string.IsNullOrEmpty(OrderString))
            {
                string[] OrderByProp = OrderString.Split(",");

                foreach (string item in OrderByProp)
                {
                    string Prop = item;
                    bool Desc = (Prop.Contains("desc")) ? true : false;

                    Prop = Prop.Replace("desc", "");
                    Prop = Prop.Replace(",", "");
                    Prop = Prop.Trim();

                    System.Reflection.PropertyInfo propertyInfo = typeof(T).GetProperty(Prop);

                    if (propertyInfo != null)
                    {
                        items = (Desc == true) ?
                            items.OrderByDescending(x => propertyInfo.GetValue(x, null)).ToList() :
                            items.OrderBy(x => propertyInfo.GetValue(x, null)).ToList();
                    }
                }
            }

            return items;
        }
    }
}
