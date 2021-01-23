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
    public static String NOTIFICATION = "notification";

    public void onReceive(Context context, Intent intent)
    {
        try 
        {
            com.igorgalimski.unitylocalnotification.NotificationManager.SetContext(context);
    
            if(!com.igorgalimski.unitylocalnotification.NotificationManager.AreNotificationEnabledInternal())
            {
                return;
            }
    
            if (Intent.ACTION_BOOT_COMPLETED.equals(intent.getAction()))
            {
                List<ILocalNotificationBridge> pendingNotifications = new ArrayList<>(NotificationProvider.GetPendingNotifications());
    
                for (ILocalNotificationBridge localNotification: pendingNotifications)
                {
                    com.igorgalimski.unitylocalnotification.NotificationManager.ScheduleLocalNotificationInternal(localNotification);
                }
    
                NotificationProvider.ClearPendingNotifications();
            }
            else
            {
                NotificationManager notificationManager = (NotificationManager)context.getSystemService(Context.NOTIFICATION_SERVICE);
    
                ILocalNotification localNotification = com.igorgalimski.unitylocalnotification.NotificationManager.GetLocalNotification(intent);
    
                if(localNotification != null)
                {
                    Notification notification = intent.getParcelableExtra(NOTIFICATION);
                    notificationManager.notify(localNotification.GetID().hashCode(), notification);
    
                    com.igorgalimski.unitylocalnotification.NotificationManager.NotifyNotificationReceived(localNotification);
                }
            }
        }
        catch (Exception exception)
        {
            Log.e(com.igorgalimski.unitylocalnotification.NotificationManager.LOG, "onReceive", exception);
        }
    }
}