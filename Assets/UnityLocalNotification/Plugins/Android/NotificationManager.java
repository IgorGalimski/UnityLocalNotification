package com.igorgalimski.unitylocalnotification;

import android.app.AlarmManager;
import android.app.Notification;
import android.app.NotificationChannel;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.os.Build;
import android.os.Bundle;
import android.os.SystemClock;

import androidx.core.app.NotificationCompat;

public class NotificationManager
{
    private static Context _context;
    private static Class _mainActivity;

    private static android.app.NotificationManager _systemNotificationManager;
    private static String _notificationChannelId;
    private static INotificationReceivedCallback _notificationReceivedCallback;

    public static void InitializeInternal(Context context, Class mainActivity, INotificationReceivedCallback notificationReceivedCallback)
    {
        _context = context;
        _mainActivity = mainActivity;
        _notificationReceivedCallback = notificationReceivedCallback;
    }

    public static void CreateChannelInternal(INotificationChannel notificationChannel)
    {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O)
        {
            _systemNotificationManager = (android.app.NotificationManager) _context.getSystemService(Context.NOTIFICATION_SERVICE);

            _notificationChannelId = notificationChannel.GetId();

            NotificationChannel mChannel = new NotificationChannel(_notificationChannelId, notificationChannel.GetName(), notificationChannel.GetImportance());
            mChannel.setDescription(notificationChannel.GetDescription());
            mChannel.setShowBadge(notificationChannel.GetShowBadge());

            _systemNotificationManager.createNotificationChannel(mChannel);
        }
    }

    public static void ScheduleLocalNotificationInternal(ILocalNotification localNotification)
    {
        int icon = _context.getApplicationInfo().icon;
        
        NotificationCompat.Builder builder = new NotificationCompat.Builder(_context, _notificationChannelId)
                .setContentTitle(localNotification.GetTitle())
                .setContentText(localNotification.GetBody())
                .setSmallIcon(icon);

        Intent intent = new Intent(_context, _mainActivity);
        PendingIntent activity = PendingIntent.getActivity(_context, 0, intent, PendingIntent.FLAG_CANCEL_CURRENT);
        builder.setContentIntent(activity);

        Notification notification = builder.build();

        Intent notificationIntent = new Intent(_context, NotificationBroadcastReceiver.class);

        Bundle bundle = new Bundle();
        bundle.putString(NotificationBroadcastReceiver.TITLE, localNotification.GetTitle());
        bundle.putString(NotificationBroadcastReceiver.BODY, localNotification.GetBody());
        bundle.putString(NotificationBroadcastReceiver.DATA, localNotification.GetData());

        notificationIntent.putExtra(NotificationBroadcastReceiver.LOCAL_NOTIFICATION, bundle);

        notificationIntent.putExtra(NotificationBroadcastReceiver.NOTIFICATION, notification);

        PendingIntent pendingIntent = PendingIntent.getBroadcast(_context, 0, notificationIntent, PendingIntent.FLAG_CANCEL_CURRENT);

        long futureInMillis = SystemClock.elapsedRealtime() + localNotification.GetFireInSeconds();
        AlarmManager alarmManager = (AlarmManager) _context.getSystemService(Context.ALARM_SERVICE);
        alarmManager.set(AlarmManager.ELAPSED_REALTIME_WAKEUP, futureInMillis, pendingIntent);
    }

    public static void NotifyNotificationReceived(ILocalNotification localNotification)
    {
        _notificationReceivedCallback.OnNotificationReceived(localNotification);
    }
}
