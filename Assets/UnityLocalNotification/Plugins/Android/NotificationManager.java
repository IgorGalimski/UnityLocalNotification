package com.igorgalimski.unitylocalnotification;

import android.app.AlarmManager;
import android.app.Notification;
import android.app.NotificationChannel;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.os.Build;
import android.os.SystemClock;

import androidx.core.app.NotificationCompat;

import com.igorgalimski.unitylocalnotification.R;

public class NotificationManager
{
    private static Context _context;
    private static Class _mainActivity;

    private static android.app.NotificationManager _systemNotificationManager;
    private static String _notificationChannelId;

    public static void Initialize(Context context, Class mainActivity)
    {
        _context = context;
        _mainActivity = mainActivity;
    }

    public static void CreateChannel(com.igorgalimski.unitylocalnotification.NotificationChannel notificationChannel)
    {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O)
        {
            _systemNotificationManager = (android.app.NotificationManager) _context.getSystemService(Context.NOTIFICATION_SERVICE);

            _notificationChannelId = notificationChannel.Id;

            NotificationChannel mChannel = new NotificationChannel(_notificationChannelId, notificationChannel.Name, notificationChannel.Importance);
            mChannel.setDescription(notificationChannel.Description);
            mChannel.setShowBadge(notificationChannel.ShowBadge);

            _systemNotificationManager.createNotificationChannel(mChannel);
        }
    }

    public static void ScheduleLocalNotificationInternal(LocalNotification localNotification)
    {
        int icon = _context.getApplicationInfo().icon;
        
        NotificationCompat.Builder builder = new NotificationCompat.Builder(_context, _notificationChannelId)
                .setContentTitle(localNotification.Title)
                .setContentText(localNotification.Body)
                .setSmallIcon(icon);

        Intent intent = new Intent(_context, _mainActivity);
        PendingIntent activity = PendingIntent.getActivity(_context, 0, intent, PendingIntent.FLAG_CANCEL_CURRENT);
        builder.setContentIntent(activity);

        Notification notification = builder.build();

        Intent notificationIntent = new Intent(_context, NotificationBroadcastReceiver.class);
        notificationIntent.putExtra(NotificationBroadcastReceiver.NOTIFICATION, notification);
        PendingIntent pendingIntent = PendingIntent.getBroadcast(_context, 0, notificationIntent, PendingIntent.FLAG_CANCEL_CURRENT);

        long futureInMillis = SystemClock.elapsedRealtime() + localNotification.FireInSeconds;
        AlarmManager alarmManager = (AlarmManager) _context.getSystemService(Context.ALARM_SERVICE);
        alarmManager.set(AlarmManager.ELAPSED_REALTIME_WAKEUP, futureInMillis, pendingIntent);
    }
}
