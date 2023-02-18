#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

using UnityEngine;

namespace Ninsar
{
    public class Notifications : MonoBehaviour
    {
        public string Title = "Hey! Come back!";
        public string Text = "We missed you!";
        public int Hours = 23;
        
        private void Start()
        {
#if UNITY_ANDROID
            InitializeAndroid();
#endif
        }
        
#if UNITY_ANDROID
        private void InitializeAndroid()
        {
            AndroidNotificationCenter.CancelAllDisplayedNotifications();
            
            var c = new AndroidNotificationChannel()
            {
                Id = "channel_id",
                Name = "Notification Channel",
                Importance = Importance.High,
                Description = "Regular notifications",
            };
            AndroidNotificationCenter.RegisterNotificationChannel(c);
            
            var notification = new AndroidNotification
            {
                Title = Title,
                Text = Text,
                FireTime = System.DateTime.Now.AddHours(Hours),
                SmallIcon = "icon_0"
            };

            var id = AndroidNotificationCenter.SendNotification(notification, "channel_id");

            if (AndroidNotificationCenter.CheckScheduledNotificationStatus(id) == NotificationStatus.Scheduled)
            {
                AndroidNotificationCenter.CancelAllNotifications();
                AndroidNotificationCenter.SendNotification(notification, "channel_id");
            }
        }
#endif
    }
}