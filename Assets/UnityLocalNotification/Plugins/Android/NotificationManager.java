package com.igorgalimski.unitylocalnotification;

import android.app.Activity;
import android.app.AlarmManager;
import android.app.Notification;
import android.app.NotificationChannel;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.content.res.Resources;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.Build;
import android.os.Bundle;
import android.util.Log;

import androidx.core.app.NotificationCompat;

import com.unity3d.player.UnityPlayer;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.List;

public class NotificationManager
{
    public static final String LOG = "NotificationManager";

    private static final String PLAYER_ACTIVITY_POSTFIX = ".UnityPlayerActivity";

    private static Context _context;
    private static Class _mainActivity;

    private static android.app.NotificationManager _systemNotificationManager;
    private static AlarmManager _alarmManager;

    private static INotificationReceivedCallback _notificationReceivedCallback;

    public static ILocalNotification LastReceivedNotification;

    public static Context GetContext()
    {
        if(_context == null)
        {
            UnityPlayerActivity unityPlayerActivity = (UnityPlayerActivity) UnityPlayer.currentActivity;
            _context = unityPlayerActivity.getApplicationContext();
        }

        return _context;
    }

    public static void SetContext(Context context)
    {
        _context = context;
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

    private static AlarmManager GetAlarmManager()
    {
        if(_alarmManager == null)
        {
            _alarmManager = (AlarmManager) GetContext().getSystemService(Context.ALARM_SERVICE);
        }

        return _alarmManager;
    }

    private static android.app.NotificationManager GetNotificationManager()
    {
        if(_systemNotificationManager == null)
        {
            _systemNotificationManager = (android.app.NotificationManager) GetContext().getSystemService(Context.NOTIFICATION_SERVICE);
        }

        return _systemNotificationManager;
    }

    public static void InitializeInternal(INotificationReceivedCallback notificationReceivedCallback)
    {
        _notificationReceivedCallback = notificationReceivedCallback;
    }

    public static void CreateChannelInternal(INotificationChannel notificationChannel)
    {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O)
        {
            String notificationChannelId = notificationChannel.GetId();
            NotificationProvider.SetNotificationChannelID(notificationChannelId);

            NotificationChannel channel = new NotificationChannel(notificationChannelId, notificationChannel.GetName(), notificationChannel.GetImportance());
            channel.setDescription(notificationChannel.GetDescription());
            channel.setShowBadge(notificationChannel.GetShowBadge());
            channel.enableLights(notificationChannel.GetLight());
            channel.enableVibration(notificationChannel.GetVibration());

            GetNotificationManager().createNotificationChannel(channel);
        }
    }

    public static void ScheduleLocalNotificationInternal(ILocalNotification localNotification)
    {
        NotificationCompat.Builder notificationBuilder;
        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.O)
        {
            notificationBuilder = new NotificationCompat.Builder(GetContext());
        }
        else
        {
            notificationBuilder = new NotificationCompat.Builder(GetContext(), NotificationProvider.GetNotificationChannelID());
        }

        notificationBuilder.setContentTitle(localNotification.GetTitle())
                .setContentText(localNotification.GetBody())
                .setAutoCancel(localNotification.GetAutoCancel());

        int smallIconId = GetDrawableId(localNotification.GetSmallIconId());
        if(smallIconId != 0)
        {
            notificationBuilder.setSmallIcon(smallIconId);
        }
        else
        {
            notificationBuilder.setSmallIcon(GetContext().getApplicationInfo().icon);
        }

        Bitmap largeIcon = GetDrawable(localNotification.GetLargeIconId());
        if(largeIcon != null)
        {
            notificationBuilder.setLargeIcon(largeIcon);
        }

        long futureInMillis = System.currentTimeMillis() + localNotification.GetFireInSeconds()*1000;

        int id;
        if(localNotification.GetID() != null)
        {
            id = localNotification.GetID().hashCode();
        }
        else
        {
            id = (int) futureInMillis;
        }
        localNotification.SetID(id);

        long fireSecondsUTC = System.currentTimeMillis()/1000 + localNotification.GetFireInSeconds();
        localNotification.SetFiredSeconds(fireSecondsUTC);

        Bundle notificationBundle = GetNotificationBundle(localNotification);

        Intent intent = new Intent(GetContext(), GetMainActivity());
        intent.putExtra(LocalNotification.LOCAL_NOTIFICATION, notificationBundle);
        PendingIntent activity = PendingIntent.getActivity(GetContext(), id, intent, PendingIntent.FLAG_CANCEL_CURRENT);
        notificationBuilder.setContentIntent(activity);

        Notification notification = notificationBuilder.build();

        Intent notificationIntent = new Intent(GetContext(), NotificationBroadcastReceiver.class);

        //notificationIntent.putExtra(LocalNotification.LOCAL_NOTIFICATION, notificationBundle);

        notificationIntent.putExtra(LocalNotification.NOTIFICATION, notification);
        notificationIntent.addFlags(Intent.FLAG_ACTIVITY_SINGLE_TOP | Intent.FLAG_ACTIVITY_CLEAR_TOP);

        PendingIntent pendingIntent = PendingIntent.getBroadcast(GetContext(), id, notificationIntent, PendingIntent.FLAG_CANCEL_CURRENT);

        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.M)
        {
            GetAlarmManager().set(AlarmManager.RTC_WAKEUP, futureInMillis, pendingIntent);
        }
        else
        {
            GetAlarmManager().setExactAndAllowWhileIdle(AlarmManager.RTC_WAKEUP, futureInMillis, pendingIntent);
        }

        AddPendingNotification(GetLocalNotification(notificationIntent));
    }

    private static Integer GetDrawableId(String resourceId)
    {
        int id = 0;

        Resources res = GetContext().getResources();
        if (res != null)
        {
            id = res.getIdentifier(resourceId, "mipmap", GetContext().getPackageName());
            if (id == 0)
            {
                id = res.getIdentifier(resourceId, "drawable", GetContext().getPackageName());
            }
        }

        return id;
    }

    private static Bitmap GetDrawable(String resourceId)
    {
        try
        {
            int id = GetDrawableId(resourceId);
            if(id != 0)
            {
                return BitmapFactory.decodeResource(GetContext().getResources(), id);
            }
        }
        catch (Exception exception)
        {
            Log.e(LOG, "GetDrawable error: " + exception.getMessage());
        }

        return null;
    }

    private static Bundle GetNotificationBundle(ILocalNotification localNotification)
    {
        Bundle bundle = new Bundle();

        bundle.putString(LocalNotification.NOTIFICATION, localNotification.GetAsObject().toString());

        return bundle;
    }

    public static void RemoveScheduledNotificationsInternal()
    {
        for (ILocalNotification localNotification : GetPendingNotifications())
        {
            try
            {
                Intent intent = new Intent(GetContext(), NotificationBroadcastReceiver.class);
                PendingIntent broadcast = PendingIntent.getBroadcast(GetContext(), Integer.valueOf(localNotification.GetID()), intent, PendingIntent.FLAG_NO_CREATE);

                if (broadcast != null)
                {
                    GetAlarmManager().cancel(broadcast);
                    broadcast.cancel();
                }
            }
            catch (Exception exception)
            {
                Log.e(LOG, "RemoveScheduledNotificationsInternal: " + exception.getMessage());
            }
        }

        NotificationProvider.SetPendingNotifications(new ArrayList<>());
    }

    public static ILocalNotification GetOpenedNotificationInternal()
    {
        Activity unityPlayerActivity = UnityPlayer.currentActivity;
        Intent activityIntent = unityPlayerActivity.getIntent();

        return GetLocalNotification(activityIntent);
    }

    private static List<ILocalNotification> GetPendingNotifications()
    {
        return NotificationProvider.GetPendingNotifications();
    }

    private static void AddPendingNotification(ILocalNotification localNotification)
    {
        List<ILocalNotification> pendingIntents = GetPendingNotifications();
        pendingIntents.add(localNotification);

        NotificationProvider.SetPendingNotifications(pendingIntents);
    }

    private static void RemovePendingNotification(ILocalNotification localNotification)
    {
        List<ILocalNotification> pendingNotifications = GetPendingNotifications();
        pendingNotifications.remove(localNotification);

        NotificationProvider.SetPendingNotifications(pendingNotifications);
    }

    public static void RemoveReceivedNotificationsInternal()
    {
        GetNotificationManager().cancelAll();
    }

    public static ILocalNotification GetLocalNotification(Intent intent)
    {
        try
        {
            Bundle localNotificationBundle = intent.getBundleExtra(LocalNotification.LOCAL_NOTIFICATION);

            if(localNotificationBundle == null)
            {
                return null;
            }

            String pushNotificationJSON = localNotificationBundle.getString(LocalNotification.NOTIFICATION);

            JSONObject pushNotificationJSONObject = new JSONObject(pushNotificationJSON);

            return LocalNotification.FromJSONObject(pushNotificationJSONObject);
        }
        catch (Exception e)
        {
            Log.e(LOG, "GetLocalNotification", e);
        }

        return null;
    }

    public static void NotifyNotificationReceived(ILocalNotification localNotification)
    {
        LastReceivedNotification = localNotification;

        if(_notificationReceivedCallback != null)
        {
            _notificationReceivedCallback.OnNotificationReceived();
        }

        NotificationProvider.AddReceivedNotification(localNotification);
        RemovePendingNotification(localNotification);
    }

    public static List<ILocalNotification> GetReceivedNotificationsListInternal()
    {
        return NotificationProvider.GetReceivedNotificationsList();
    }

    public static void ClearReceivedNotificationsListInternal()
    {
        NotificationProvider.ClearReceivedNotifications();
    }

    public static boolean AreNotificationEnabledInternal()
    {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N)
        {
            return GetNotificationManager().areNotificationsEnabled();
        }

        return false;
    }
}