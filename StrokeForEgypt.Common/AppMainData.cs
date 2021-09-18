using System;
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
            CreateOrUpdateAccess = 2,
            FullAccess = 3,
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

        public enum GenderEnum
        {
            Male = 1,
            Female = 2
        }

        public enum BookingStateEnum
        {
            PendingOnPayment = 1,
            PendingOnMembersData = 2,
            Success = 3,
            PaymentFailed = 4,
            Refunded = 5
        }

        public enum EventProfileItems
        {
            EventActivity = 1,
            EventAgenda = 2,
            EventGallery = 3,
            EventPackage = 4,
            News = 5,
            Sponsor = 6
        }

        public enum AccountProfileItems
        {
            AccountDevice = 1,
            RefreshToken = 2,
            Booking = 3
        }

        public enum BookingProfileItems
        {
            BookingMember = 1,
            BookingStateHistory = 2,
            BookingPayment = 3
        }

        public enum NotificationTypeEnum
        {
            ExternalURL = 1,
            Verification = 2,
            EventProfile = 3,
            EventNews = 4,
            EventAgenda = 5,
            EventRegistration = 6,
            EventRegistrationMembers = 7,
            EventRegistrationPayment = 8,
            Other = 9
        }

        public enum NotificationProfileItem
        {
            Accounts = 1
        }

    }
}
