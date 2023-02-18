using System;
using System.Collections.Generic;
using UnityEngine;
using Yurowm.GameCore;

#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

#if UNITY_IOS
using NotificationServices = UnityEngine.iOS.NotificationServices;
using LocalNotification = UnityEngine.iOS.LocalNotification;
#endif

public class Notifications : MonoBehaviour 
{
    const string AndroidNotificationChanel = "notif1";

    void Awake() 
    {
        #if UNITY_ANDROID && ANDROID_NOTIFICATIONS
        if (Debug.isDebugBuild)
            DebugPanel.AddDelegate("Test Notification (10 sec.)", () => {
                ScheduleNotification("Test Notification", new TimeSpan(0, 0, 10),
                    "Test Notification", "Hello World!");
            });

        AndroidNotificationCenter.Initialize();

        var c = new AndroidNotificationChannel()
        {
            Id = AndroidNotificationChanel,
            Name = "Notification",
            Importance = Importance.High,
            Description = "Regular notifications", 
            CanShowBadge = true, 
            EnableVibration = true
        };
        AndroidNotificationCenter.RegisterNotificationChannel(c);
        
        #endif
    }
    
    
    public enum NotificationExecuteMode 
    {
        Inexact = 0,
        Exact = 1,
        ExactAndAllowWhileIdle = 2
    }

    public static void ScheduleNotificationUTC(string name, DateTime when, string title, string message) 
    {
        if (!TrueTime.IsKnown || when < TrueTime.Now) return;
        DebugPanel.Log("N@" + name, "Notifications", "({0})\n\t{1}\n\t{2}".FormatText(when, title, message));
        Notification(name, when - TrueTime.Now, title, message);
    }

    public static void ScheduleNotification(string name, TimeSpan span, string title, string message) 
    {
        Notification(name, span, title, message);
    }

    public static void ScheduleNotification(string name, DateTime when, string title, string message) 
    {
        if (!TrueTime.IsKnown) return;
        ScheduleNotificationUTC(name, when - TrueTime.Zone, title, message);
    }

    public static void CancelNotification(string name)
    {
        DebugPanel.Log("N@" + name, "Notifications", "REMOVED");
        int id = (int) name.CheckSum();
        
        #if UNITY_ANDROID && !UNITY_EDITOR && ANDROID_NOTIFICATIONS
        AndroidNotificationCenter.CancelNotification(id);
        #endif

        #if UNITY_IOS
        for (int i = 0; i < NotificationServices.localNotificationCount; ++i)
        {
            LocalNotification notif = NotificationServices.GetLocalNotification(i);
            if (notif.userInfo["id"] == (object) id.ToString())
                NotificationServices.CancelLocalNotification(notif);
        }
        #endif
    }

    static void Notification(string name, TimeSpan span, string title, string message) 
    {
        int id = (int) name.CheckSum();

        #if UNITY_ANDROID && !UNITY_EDITOR && ANDROID_NOTIFICATIONS
        DebugPanel.Log("N@" + name, "Notifications", "+({0})\n\t{1}\n\t{2}".FormatText(span, title, message));

        var notification = new AndroidNotification
        {
            Title = title,
            Text = message,
            FireTime = DateTime.Now + span,
            LargeIcon = "icon_1"
        };
        
        AndroidNotificationCenter.SendNotificationWithExplicitID(notification, AndroidNotificationChanel, id);

        if (AndroidNotificationCenter.CheckScheduledNotificationStatus(id) == NotificationStatus.Scheduled)
        {
            AndroidNotificationCenter.UpdateScheduledNotification(id, notification, AndroidNotificationChanel);
        }
        else if (AndroidNotificationCenter.CheckScheduledNotificationStatus(id) == NotificationStatus.Delivered)
        {
            AndroidNotificationCenter.CancelNotification(id);
        }
        else if (AndroidNotificationCenter.CheckScheduledNotificationStatus(id) == NotificationStatus.Unknown)
        {
            AndroidNotificationCenter.SendNotification(notification, "channel_id");
        }

        #endif

        #if UNITY_IOS
        LocalNotification notification = new LocalNotification();
        notification.fireDate = DateTime.Now + span;
        notification.userInfo = new Dictionary<string, string>() { { "id", id.ToString() } };
        notification.alertBody = message;
        notification.alertAction = title;
        notification.hasAction = false;
        notification.soundName = LocalNotification.defaultSoundName;    
        NotificationServices.ScheduleLocalNotification(notification);
        #endif
    }
}
