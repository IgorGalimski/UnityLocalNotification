package com.igorgalimski.unitylocalnotification;

import android.app.Activity;
import android.app.AlarmManager;
import android.app.Notification;
import android.app.NotificationChannel;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Build;
import android.os.Bundle;
import android.os.SystemClock;

import androidx.core.app.NotificationCompat;

import com.unity3d.player.UnityPlayer;

import java.util.HashSet;

public class NotificationManager
{
    private static final String NOTIFICATION_IDS_SHARED_PREFS = "INTENTS";
    private static Context _context;
    private static Class _mainActivity;

    private static android.app.NotificationManager _systemNotificationManager;
    private static AlarmManager _alarmManager;
    private static String _notificationChannelId;
    private static INotificationReceivedCallback _notificationReceivedCallback;

    private static SharedPreferences _prefs;
    private static SharedPreferences.Editor _prefsEditor;
    
    public static ILocalNotification LastReceivedNotification;

    public static void InitializeInternal(Context context, Class mainActivity, INotificationReceivedCallback notificationReceivedCallback)
    {
        _context = context;
        _mainActivity = mainActivity;
        _notificationReceivedCallback = notificationReceivedCallback;
        _alarmManager = (AlarmManager) _context.getSystemService(Context.ALARM_SERVICE);
        _systemNotificationManager = (android.app.NotificationManager) _context.getSystemService(Context.NOTIFICATION_SERVICE);

        _prefs = context.getSharedPreferences(NOTIFICATION_IDS_SHARED_PREFS, Context.MODE_PRIVATE);
        _prefsEditor = _prefs.edit();
    }

    public static void CreateChannelInternal(INotificationChannel notificationChannel)
    {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O)
        {
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
        notificationIntent.addFlags(Intent.FLAG_ACTIVITY_SINGLE_TOP);

        long futureInMillis = SystemClock.elapsedRealtime() + localNotification.GetFireInSeconds()*1000;
        int id = (int) futureInMillis;
        PendingIntent pendingIntent = PendingIntent.getBroadcast(_context, id, notificationIntent, PendingIntent.FLAG_CANCEL_CURRENT);

        _alarmManager.set(AlarmManager.ELAPSED_REALTIME_WAKEUP, futureInMillis, pendingIntent);

        AddPendingNotificationId(id);
    }

    public static void RemoveScheduledNotificationsInternal()
    {
        for (String id : GetPendingIntents())
        {
            //try
            Intent intent = new Intent(_context, NotificationBroadcastReceiver.class);
            PendingIntent broadcast = PendingIntent.getBroadcast(_context, Integer.valueOf(id), intent, PendingIntent.FLAG_NO_CREATE);

            if (broadcast != null)
            {
                _alarmManager.cancel(broadcast);
                broadcast.cancel();
            }
        }

        UpdatePendingIntents(new HashSet<>());
    }
    
    public static ILocalNotification GetOpenedNotificationInternal()
    {
        Activity unityPlayerActivity = UnityPlayer.currentActivity;
        Intent activityIntent = unityPlayerActivity.getIntent();
        
        return GetLocalNotification(activityIntent);
    }

    private static HashSet<String> GetPendingIntents()
    {
        return new HashSet<>(_prefs.getStringSet(NOTIFICATION_IDS_SHARED_PREFS, new HashSet<String>()));
    }

    private static void AddPendingNotificationId(int id)
    {
        HashSet<String> pendingIntents = GetPendingIntents();
        pendingIntents.add(String.valueOf(id));

        UpdatePendingIntents(pendingIntents);
    }

    private static void RemotePendingIntentId(int id)
    {
        HashSet<String> pendingIntents = GetPendingIntents();
        pendingIntents.remove(String.valueOf(id));

        UpdatePendingIntents(pendingIntents);
    }

    private static void UpdatePendingIntents(HashSet<String> set)
    {
        _prefsEditor.putStringSet(NOTIFICATION_IDS_SHARED_PREFS, set);
        _prefsEditor.apply();
    }

    public static void RemoveReceivedNotificationsInternal()
    {
        _systemNotificationManager.cancelAll();
    }
    
    public static ILocalNotification GetLocalNotification(Intent intent)
    {
        Bundle localNotificationBundle = intent.getBundleExtra(NotificationBroadcastReceiver.LOCAL_NOTIFICATION);
        
        if(localNotificationBundle == null)
        {
            return null;
        }
        
        String title = localNotificationBundle.getString(NotificationBroadcastReceiver.TITLE);
        String body = localNotificationBundle.getString(NotificationBroadcastReceiver.BODY);
        String data = localNotificationBundle.getString(NotificationBroadcastReceiver.DATA);
        ILocalNotification localNotification = new LocalNotification(title, body, data, 0);
        
        return localNotification;
    }

    public static void NotifyNotificationReceived(ILocalNotification localNotification)
    {
        LastReceivedNotification = localNotification;
        
        _notificationReceivedCallback.OnNotificationReceived();
    }
}
