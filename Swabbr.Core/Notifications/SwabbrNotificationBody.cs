using Newtonsoft.Json.Linq;
using System;

namespace Swabbr.Core.Notifications
{
    public class SwabbrNotificationBody
    {
        /// <summary>
        /// Title of the notification
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Message/description of the notification
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// The name of the protocol
        /// </summary>
        /// Const
        public string Protocol => "swabbr";

        /// <summary>
        /// The version of the protocol
        /// </summary>
        /// Const
        public string ProtocolVersion => "1.0";

        /// <summary>
        /// The client sided action to perform upon clicking the notification alert
        /// </summary>
        public string ClickAction { get; set; }

        /// <summary>
        /// An object of type <see cref="SwabbrNotificationBody.ObjectType"/>
        /// </summary>
        public JObject Object { get; set; }

        /// <summary>
        /// The type of the included data
        /// </summary>
        /// TODO THOMAS Very bug sensitive, make enum
        public string ObjectType { get; set; }

        /// <summary>
        /// The version of the type of data
        /// </summary>
        /// TODO THOMAS Very bug sensitive, make enum
        public string ObjectTypeVersion { get; set; }

        /// <summary>
        /// Indicates the media type of the content
        /// </summary>
        /// TODO THOMAS Very bug sensitive, make enum
        public string ContentType { get; set; }

        /// <summary>
        /// Timestamp of when the message was sent
        /// </summary>
        public DateTime TimeStamp { get; set; } = DateTime.Now;

        /// <summary>
        /// Contains information about the senders’ operating system, device, etc.
        /// </summary>
        /// TODO THOMAS Very bug sensitive, specify
        public string UserAgent { get; set; }


        /* TODO THOMAS These json object formatters seem VERY bug sensitive. */
        /* TODO THOMAS They shouldn't be placed in this class either */

        public JObject GetAppleContentJSON()
        {
            // Get base content
            var json = GetContentJSON();

            // Add APNS specific data object
            json.Add(new JProperty("aps",
                new JObject(
                    new JProperty("alert",
                        new JObject(
                            new JProperty("title", Title),
                            new JProperty("body", Body),
                            new JProperty("content-available", 1)
                        )
                    )
                )
            ));

            return json;
        }

        public JObject GetFcmContentJSON()
        {
            // Get base content
            var json = GetContentJSON();

            // Add FCM specific data object
            json.Add("notification", new JObject()
            {
               new JProperty("title", Title),
               new JProperty("body", Body)
            });

            return json;
        }

        private JObject GetContentJSON()
        {
            // Add nested values in data
            var dataObj = new JObject();

            dataObj.Add("click_action", ClickAction);

            dataObj.Add("object", Object);
            dataObj.Add("object_type", ObjectType);
            dataObj.Add("object_type_version", ObjectTypeVersion);

            return new JObject(
                new JProperty("protocol", Protocol),
                new JProperty("protocol_version", ProtocolVersion),
                new JProperty("data", dataObj),
                new JProperty("content_type", ContentType),
                new JProperty("timestamp", TimeStamp),
                new JProperty("user_agent", UserAgent)
            );
        }
    }
}