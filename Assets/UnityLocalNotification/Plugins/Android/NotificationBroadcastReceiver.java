package com.igorgalimski.unitylocalnotification;

import android.app.Notification;
import android.app.NotificationManager;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.util.Log;

import java.util.ArrayList;
import java.util.List;

public class NotificationBroadcastReceiver extends BroadcastReceiver
{
    private static int NOTIFICATION_DELAY = 5;

    public static String NOTIFICATION = "notification";
    public static String LOCAL_NOTIFICATION = "local_notification";
    public static String ID = "id";
    public static String TITLE = "title";
    public static String BODY = "body";
    public static String DATA = "data";
    public static String FIRED_SECONDS = "firedSeconds";

    public void onReceive(Context context, Intent intent)
    {
        com.igorgalimski.unitylocalnotification.NotificationManager.SetContext(context);

        if(!com.igorgalimski.unitylocalnotification.NotificationManager.AreNotificationEnabledInternal())
        {
            return;
        }

        if (Intent.ACTION_BOOT_COMPLETED.equals(intent.getAction()))
        {
            List<ILocalNotification> pendingNotifications = new ArrayList<>(NotificationProvider.GetPendingNotifications());

            for (ILocalNotification localNotification: pendingNotifications)
            {
                com.igorgalimski.unitylocalnotification.NotificationManager.ScheduleLocalNotificationInternal(localNotification);
            }

            NotificationProvider.ClearPendingNotifications();
        }
        else
        {
            NotificationManager notificationManager = (NotificationManager)context.getSystemService(Context.NOTIFICATION_SERVICE);

            Notification notification = intent.getParcelableExtra(NOTIFICATION);
            ILocalNotification localNotification = com.igorgalimski.unitylocalnotification.NotificationManager.GetLocalNotification(intent);

            if(localNotification != null)
            {
                long realtime = System.currentTimeMillis()/1000;

                if((realtime - localNotification.GetFiredSeconds()) > NOTIFICATION_DELAY)
                {
                    Log.e(com.igorgalimski.unitylocalnotification.NotificationManager.LOG, "Notification time is expired");

                    return;
                }

                notificationManager.notify(localNotification.GetID().hashCode(), notification);

                com.igorgalimski.unitylocalnotification.NotificationManager.NotifyNotificationReceived(localNotification);
            }
        }
    }
}