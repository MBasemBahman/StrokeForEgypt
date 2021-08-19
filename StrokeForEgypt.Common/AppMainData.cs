﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace StrokeForEgypt.Common
{
    public static class AppMainData
    {
        public static string WebRootPath { get; set; }

        public static string DomainName { get; set; }

        public static string RefererUrl { get; set; } = null;

        public static string ApiServiceURL { get; set; }
    }

    public static class NotificationTopic
    {
        public static string All { get; set; } = "all";
        public static string Visitor { get; set; } = "visitor";
        public static string Client { get; set; } = "client";

    }

    public static class EnumData
    {
        public static Dictionary<int, string> GetDayEnum()
        {
            Dictionary<int, string> result = new Dictionary<int, string>();
            List<int> Days = ((DayOfWeek[])Enum.GetValues(typeof(DayOfWeek))).Select(c => (int)c).ToList();

            foreach (int item in Days)
            {
                result.Add(item, Enum.GetName(typeof(DayOfWeek), item));
            }

            return result;
        }

        public enum AccessLevelEnum
        {
            ReadAccess = 1,
            CreateAccess = 2,
            UpdateAccess = 3,
            DeleteAccess = 4,
            CreateOrUpdateAccess = 5,
            FullAccess = 6,
        }

        public enum SystemUserEnum
        {
            Developer = 1
        }

        public enum SystemViewEnum
        {
            Home = 1,
            SystemUser = 2,
            SystemView = 3,
            SystemRole = 4
        }

        public enum SystemRoleEnum
        {
            Developer = 1
        }
    }
}
