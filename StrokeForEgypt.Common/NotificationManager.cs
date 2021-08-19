using FirebaseAdmin.Messaging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StrokeForEgypt.Common
{
    public static class NotificationManager
    {
        public static FirebaseNotificationModel Notification { get; set; }

        public static async Task<string> SendToTopic(Message message)
        {
            // Send a message to the device corresponding to the provided
            // registration token.
            string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            // Response is a message ID string.
            return response;
            // [END send_to_token]
        }

        public static async Task<int> SendMulticast(MulticastMessage message)
        {
            BatchResponse response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
            // See the BatchResponse reference documentation
            // for the contents of response.
            return response.SuccessCount;
            // [END send_multicast]
        }

        public static async Task<int> SubscribeToTopic(List<string> registrationTokens, string topic)
        {
            // Subscribe the devices corresponding to the registration tokens to the
            // topic
            TopicManagementResponse response = await FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync(
                registrationTokens, topic);
            // See the TopicManagementResponse reference documentation
            // for the contents of response.
            return response.SuccessCount;
            // [END subscribe_to_topic]
        }

        public static Message CreateMessage(FirebaseNotificationModel Model)
        {
            // [START apns_message]
            Message message = new()
            {
                Apns = new ApnsConfig()
                {
                    Aps = new Aps()
                    {
                        Alert = new ApsAlert()
                        {
                            Title = Model.MessageHeading,
                            Body = Model.MessageContent,
                        },
                        ContentAvailable = true
                    },
                    Headers = new Dictionary<string, string>
                    {
                        {"apns-priority", "5" },
                        {"apns-push-type", "background" }
                    }
                },
                Android = new AndroidConfig
                {
                    Priority = Priority.High
                },
                Data = CreateData(Model),
                Topic = Model.Topic
            };
            // [END apns_message]
            return message;
        }

        public static MulticastMessage CreateMulticastMessage(FirebaseNotificationModel Model)
        {
            // [START apns_message]
            MulticastMessage message = new()
            {
                Tokens = Model.RegistrationTokens.Where(a => !string.IsNullOrEmpty(a)).ToList(),
                Apns = new ApnsConfig()
                {
                    Aps = new Aps()
                    {
                        Alert = new ApsAlert()
                        {
                            Title = Model.MessageHeading,
                            Body = Model.MessageContent,
                        },
                        ContentAvailable = true
                    },
                    Headers = new Dictionary<string, string>
                    {
                        {"apns-priority", "5" },
                        {"apns-push-type", "background" }
                    }
                },
                Android = new AndroidConfig
                {
                    Priority = Priority.High
                },
                Data = CreateData(Model),
            };
            // [END apns_message]
            return message;
        }

        public static Dictionary<string, string> CreateData(FirebaseNotificationModel Model)
        {
            Dictionary<string, object> NotificationType = new()
            {
                { "Id", Model.NotificationType.Key },
                { "Name", Model.NotificationType.Value }
            };

            Dictionary<string, object> Extra = new()
            {
                { "Fk_OpenType", Model.Fk_OpenType.ToString() },
                { "ExcludeToken", Model.ExcludeToken },
                { "Target_Id", Model.Target_Id.ToString() },
                { "Fk_NotificationType", Model.NotificationType.Key.ToString() },
                { "URL", Model.URL }
            };

            Dictionary<string, string> data = new()
            {
                { "NotificationType", JsonConvert.SerializeObject(NotificationType) },
                { "Extra", JsonConvert.SerializeObject(Extra) },
                { "Title", Model.MessageHeading },
                { "Body", Model.MessageContent },
                { "click_action", Model.Click_Action }
            };

            return data;
        }
    }

    public class FirebaseNotificationModel
    {
        public FirebaseNotificationModel()
        {
            RegistrationTokens = new List<string>();
        }

        public KeyValuePair<int, string> NotificationType { get; set; }

        public string MessageHeading { get; set; }

        public string MessageContent { get; set; }

        public List<string> RegistrationTokens { get; set; }

        public string Topic { get; set; } = "all";

        public string URL { get; set; } = "";

        public string ImgUrl { get; set; } = "";

        public int Target_Id { get; set; } = 0;

        public int Fk_OpenType { get; set; }

        public string ExcludeToken { get; set; } = "";

        public string Click_Action { get; set; } = "FLUTTER_NOTIFICATION_CLICK";
    }
}
