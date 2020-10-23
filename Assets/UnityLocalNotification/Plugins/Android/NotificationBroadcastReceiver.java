package com.igorgalimski.unitylocalnotification;

import android.app.Notification;
import android.app.NotificationManager;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;

public class NotificationBroadcastReceiver extends BroadcastReceiver
{
    public static String NOTIFICATION = "notification";
    public static String LOCAL_NOTIFICATION = "local_notification";
    public static String TITLE = "title";
    public static String BODY = "body";
    public static String DATA = "data";

    public void onReceive(Context context, Intent intent)
    {
        NotificationManager notificationManager = (NotificationManager)context.getSystemService(Context.NOTIFICATION_SERVICE);

        Notification notification = intent.getParcelableExtra(NOTIFICATION);
        notificationManager.notify(0, notification);

        Bundle localNotificationBundle = intent.getBundleExtra(LOCAL_NOTIFICATION);
        String title = localNotificationBundle.getString(TITLE);
        String body = localNotificationBundle.getString(BODY);
        String data = localNotificationBundle.getString(DATA);
        ILocalNotification localNotification = new LocalNotification(title, body, data, 0);

        com.igorgalimski.unitylocalnotification.NotificationManager.NotifyNotificationReceived(localNotification);
    }
}
