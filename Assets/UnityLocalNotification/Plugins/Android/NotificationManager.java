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

import org.json.JSONArray;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;

public class NotificationManager
{
    private static final String NOTIFICATION_IDS_SHARED_PREFS = "INTENTS";
    private static final String RECEIVED_NOTIFICATION_IDS_SHARED_PREFS = "RECEIVED_INTENTS";

    private static final String PLAYER_ACTIVITY_POSTFIX = ".UnityPlayerActivity";

    private static Context _context;
    private static Class _mainActivity;

    private static android.app.NotificationManager _systemNotificationManager;
    private static AlarmManager _alarmManager;
    private static String _notificationChannelId;
    private static INotificationReceivedCallback _notificationReceivedCallback;
    private static List<ILocalNotification> _receivedNotifications;

    private static JSONArray _receivedNotificationsArray;

    private static SharedPreferences _prefs;
    private static SharedPreferences.Editor _prefsEditor;
    
    public static ILocalNotification LastReceivedNotification;

    private static Context GetContext()
    {
        if(_context == null)
        {
            UnityPlayerActivity unityPlayerActivity = (UnityPlayerActivity)UnityPlayer.currentActivity;
            _context = unityPlayerActivity.getApplicationContext();
        }

        return _context;
    }

    private static Class GetMainActivity()
    {
        try
        {
            _mainActivity = Class.forName(BuildConfig.APPLICATION_ID + PLAYER_ACTIVITY_POSTFIX);
        }
        catch (ClassNotFoundException ignored)
        {
            UnityPlayerActivity unityPlayerActivity = (UnityPlayerActivity)UnityPlayer.currentActivity;
            _mainActivity = unityPlayerActivity.getClass();
        }

        return _mainActivity;
    }

    public static void InitializeInternal(INotificationReceivedCallback notificationReceivedCallback)
    {
        _notificationReceivedCallback = notificationReceivedCallback;
        _alarmManager = (AlarmManager) GetContext().getSystemService(Context.ALARM_SERVICE);
        _systemNotificationManager = (android.app.NotificationManager) GetContext().getSystemService(Context.NOTIFICATION_SERVICE);

        _prefs = GetContext().getSharedPreferences(NOTIFICATION_IDS_SHARED_PREFS, Context.MODE_PRIVATE);
        _prefsEditor = _prefs.edit();

        //method
        try
        {
            _receivedNotificationsArray = new JSONArray();

            _receivedNotifications = new ArrayList<>();
            String receivedNotificationsList = _prefs.getString(RECEIVED_NOTIFICATION_IDS_SHARED_PREFS, "[{}]");

            JSONArray jsonArray = new JSONArray(receivedNotificationsList);
            for (int i=0; i < jsonArray.length(); i++)
            {
                JSONObject jsonObject = jsonArray.getJSONObject(i);
                ILocalNotification localNotification = LocalNotification.FromJSONObject(jsonObject);

                if(localNotification != null)
                {
                    _receivedNotifications.add(localNotification);
                }
            }
        }
        catch (Exception exception)
        {

        }
    }

    public static void CreateChannelInternal(INotificationChannel notificationChannel)
    {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O)
        {
            _notificationChannelId = notificationChannel.GetId();

            NotificationChannel channel = new NotificationChannel(_notificationChannelId, notificationChannel.GetName(), notificationChannel.GetImportance());
            channel.setDescription(notificationChannel.GetDescription());
            channel.setShowBadge(notificationChannel.GetShowBadge());
            channel.enableLights(notificationChannel.GetLight());
            channel.enableVibration(notificationChannel.GetVibration());

            _systemNotificationManager.createNotificationChannel(channel);
        }
    }

    public static void ScheduleLocalNotificationInternal(ILocalNotification localNotification)
    {
        int icon = GetContext().getApplicationInfo().icon;

        NotificationCompat.Builder builder = new NotificationCompat.Builder(GetContext(), _notificationChannelId)
                .setContentTitle(localNotification.GetTitle())
                .setContentText(localNotification.GetBody())
                .setSmallIcon(icon);

        Bundle notificationBundle = GetNotificationBundle(localNotification);

        Intent intent = new Intent(GetContext(), GetMainActivity());
        intent.putExtra(NotificationBroadcastReceiver.LOCAL_NOTIFICATION, notificationBundle);
        PendingIntent activity = PendingIntent.getActivity(GetContext(), 0, intent, PendingIntent.FLAG_CANCEL_CURRENT);
        builder.setContentIntent(activity);

        Notification notification = builder.build();

        Intent notificationIntent = new Intent(GetContext(), NotificationBroadcastReceiver.class);

        notificationIntent.putExtra(NotificationBroadcastReceiver.LOCAL_NOTIFICATION, notificationBundle);

        notificationIntent.putExtra(NotificationBroadcastReceiver.NOTIFICATION, notification);
        notificationIntent.addFlags(Intent.FLAG_ACTIVITY_SINGLE_TOP);

        long futureInMillis = SystemClock.elapsedRealtime() + localNotification.GetFireInSeconds()*1000;
        int id = localNotification.GetID() == null ? (int) futureInMillis : localNotification.GetID().hashCode();
        PendingIntent pendingIntent = PendingIntent.getBroadcast(GetContext(), id, notificationIntent, PendingIntent.FLAG_CANCEL_CURRENT);

        _alarmManager.set(AlarmManager.ELAPSED_REALTIME_WAKEUP, futureInMillis, pendingIntent);

        AddPendingNotificationId(id);
    }

    private static Bundle GetNotificationBundle(ILocalNotification localNotification)
    {
        Bundle bundle = new Bundle();
        bundle.putString(NotificationBroadcastReceiver.TITLE, localNotification.GetTitle());
        bundle.putString(NotificationBroadcastReceiver.BODY, localNotification.GetBody());
        bundle.putString(NotificationBroadcastReceiver.DATA, localNotification.GetData());

        return bundle;
    }

    public static void RemoveScheduledNotificationsInternal()
    {
        for (String id : GetPendingIntents())
        {
            //try
            Intent intent = new Intent(GetContext(), NotificationBroadcastReceiver.class);
            PendingIntent broadcast = PendingIntent.getBroadcast(GetContext(), Integer.valueOf(id), intent, PendingIntent.FLAG_NO_CREATE);

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

        String id = localNotificationBundle.getString(NotificationBroadcastReceiver.ID);
        String title = localNotificationBundle.getString(NotificationBroadcastReceiver.TITLE);
        String body = localNotificationBundle.getString(NotificationBroadcastReceiver.BODY);
        String data = localNotificationBundle.getString(NotificationBroadcastReceiver.DATA);
        ILocalNotification localNotification = new LocalNotification(id, title, body, data, 0);
        
        return localNotification;
    }

    public static void NotifyNotificationReceived(ILocalNotification localNotification)
    {
        LastReceivedNotification = localNotification;
        
        if(_notificationReceivedCallback != null)
        {
            _notificationReceivedCallback.OnNotificationReceived();
        }

        _receivedNotificationsArray.put(localNotification.GetAsObject());
        _receivedNotifications.add(localNotification);

        String notificationArrayString = _receivedNotificationsArray.toString();

        _prefsEditor.putString(RECEIVED_NOTIFICATION_IDS_SHARED_PREFS, notificationArrayString);
        _prefsEditor.apply();
    }

    public static List<ILocalNotification> GetReceivedNotificationsListInternal()
    {
        return _receivedNotifications;
    }

    public static void ClearReceivedNotificationsListInternal()
    {
         _receivedNotifications.clear();

        _prefsEditor.remove(RECEIVED_NOTIFICATION_IDS_SHARED_PREFS);
        _prefsEditor.apply();
    }
    
    public static boolean AreNotificationEnabledInternal()
    {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) 
        {
            return _systemNotificationManager.areNotificationsEnabled();
        }
        
        return false;
    }
}
