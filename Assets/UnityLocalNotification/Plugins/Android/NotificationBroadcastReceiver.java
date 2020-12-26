package com.igorgalimski.unitylocalnotification;

import android.app.Notification;
import android.app.NotificationManager;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;

public class NotificationBroadcastReceiver extends BroadcastReceiver
{
    public static String NOTIFICATION = "notification";
    public static String LOCAL_NOTIFICATION = "local_notification";
    public static String ID = "id";
    public static String TITLE = "title";
    public static String BODY = "body";
    public static String DATA = "data";

    public void onReceive(Context context, Intent intent)
    {
        com.igorgalimski.unitylocalnotification.NotificationManager.SetContext(context);

        if(!com.igorgalimski.unitylocalnotification.NotificationManager.AreNotificationEnabledInternal())
        {
            return;
        }

        if (Intent.ACTION_BOOT_COMPLETED.equals(intent.getAction()))
        {
            for (ILocalNotification localNotification: NotificationProvider.GetPendingNotifications()) 
            {
                com.igorgalimski.unitylocalnotification.NotificationManager.ScheduleLocalNotificationInternal(localNotification);
            }

            NotificationProvider.ClearPendingNotifications();
        }
        else
        {
            NotificationManager notificationManager = (NotificationManager)context.getSystemService(Context.NOTIFICATION_SERVICE);

            Notification notification = intent.getParcelableExtra(NOTIFICATION);
            notificationManager.notify(0, notification);

            ILocalNotification localNotification = com.igorgalimski.unitylocalnotification.NotificationManager.GetLocalNotification(intent);

            com.igorgalimski.unitylocalnotification.NotificationManager.NotifyNotificationReceived(localNotification);
        }
    }
}
