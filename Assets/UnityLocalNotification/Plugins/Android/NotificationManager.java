package com.igorgalimski.unitylocalnotification;

import android.app.Activity;
import android.app.ActivityManager;
import android.app.AlarmManager;
import android.app.Notification;
import android.app.NotificationChannel;
import android.app.PendingIntent;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.res.Resources;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.Build;
import android.os.Bundle;
import android.util.Log;

import androidx.core.app.NotificationCompat;

import com.unity3d.player.UnityPlayer;

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

    public static ILocalNotificationBridge LastReceivedNotification;

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
        try
        {
            _notificationReceivedCallback = notificationReceivedCallback;

            ComponentName receiver = new ComponentName(GetContext(), NotificationBroadcastReceiver.class);
            PackageManager pm = GetContext().getPackageManager();

            pm.setComponentEnabledSetting(receiver,
                    PackageManager.COMPONENT_ENABLED_STATE_ENABLED,
                    PackageManager.DONT_KILL_APP);
        }
        catch (Exception exception)
        {
            Log.e(LOG, "InitializeInternal", exception);
        }
    }

    public static void CreateChannelInternal(INotificationChannel notificationChannel)
    {
        try
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
        catch (Exception exception)
        {
            Log.e(LOG, "CreateChannelInternal", exception);
        }
    }

    public static void ScheduleLocalNotificationInternal(ILocalNotificationBridge localNotificationBridge)
    {
        try
        {
            ILocalNotification localNotification = ILocalNotification.GetFromBridge(localNotificationBridge);

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

            long futureInMillis;

            if(localNotification.GetFiredSeconds() == 0)
            {
               futureInMillis = (long) (System.currentTimeMillis() + localNotification.GetFireInSeconds()*1000);

               long fireSecondsUTC = futureInMillis/1000;
               localNotification.SetFiredSeconds(fireSecondsUTC);
            }
            else
            {
               futureInMillis = localNotification.GetFiredSeconds()*1000;
            }

            notificationBuilder.setWhen(futureInMillis);

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

            Bundle notificationBundle = GetNotificationBundle(localNotification);

            Intent intent = new Intent(GetContext(), GetMainActivity());
            intent.putExtra(LocalNotification.LOCAL_NOTIFICATION, notificationBundle);

            PendingIntent activity = PendingIntent.getActivity(GetContext(), id, intent, 0);
            notificationBuilder.setContentIntent(activity);

            Notification notification = notificationBuilder.build();

            Intent notificationIntent = new Intent(GetContext(), NotificationBroadcastReceiver.class);

            notificationIntent.putExtra(LocalNotification.LOCAL_NOTIFICATION, notificationBundle);

            notificationIntent.putExtra(LocalNotification.NOTIFICATION, notification);
            notificationIntent.addFlags(Intent.FLAG_ACTIVITY_SINGLE_TOP | Intent.FLAG_ACTIVITY_CLEAR_TOP);

            PendingIntent pendingIntent = PendingIntent.getBroadcast(GetContext(), id, notificationIntent, PendingIntent.FLAG_UPDATE_CURRENT);

            if (Build.VERSION.SDK_INT < Build.VERSION_CODES.M)
            {
                GetAlarmManager().set(AlarmManager.RTC_WAKEUP, futureInMillis, pendingIntent);
            }
            else
            {
                GetAlarmManager().setExactAndAllowWhileIdle(AlarmManager.RTC_WAKEUP, futureInMillis, pendingIntent);
            }

            AddPendingNotification(localNotification);

        }
        catch (Exception exception)
        {
            Log.e(LOG, "ScheduleLocalNotificationInternal", exception);
        }
    }

    private static Integer GetDrawableId(String resourceId)
    {
        int id = 0;

        try
        {
            if(resourceId != null)
            {
                Resources res = GetContext().getResources();
                if (res != null)
                {
                    id = res.getIdentifier(resourceId, "mipmap", GetContext().getPackageName());
                    if (id == 0)
                    {
                        id = res.getIdentifier(resourceId, "drawable", GetContext().getPackageName());
                    }
                }
            }
        }
        catch (Exception exception)
        {
            Log.e(LOG, "GetDrawableId", exception);
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
            Log.e(LOG, "GetDrawable", exception);
        }

        return null;
    }

    private static Bundle GetNotificationBundle(ILocalNotification localNotification)
    {
        Bundle bundle = new Bundle();

        try
        {
            bundle.putString(LocalNotification.NOTIFICATION, localNotification.GetAsObject().toString());
        }
        catch (Exception exception)
        {
            Log.e(LOG, "GetNotificationBundle", exception);
        }

        return bundle;
    }
    
    public static void RemoveScheduledNotificationsInternal()
    {
        try
        {
            for (ILocalNotificationBridge localNotification : GetPendingNotifications())
            {
                CancellById(Integer.valueOf(localNotification.GetID()));
            }

             NotificationProvider.ClearPendingNotifications();
        }
        catch (Exception exception)
        {
            Log.e(LOG, "RemoveScheduledNotificationsInternal", exception);
        }
    }

    public static void CancellById(Integer id)
    {
        try
        {
            Intent intent = new Intent(GetContext(), NotificationBroadcastReceiver.class);
            PendingIntent broadcast = PendingIntent.getBroadcast(GetContext(), id, intent, PendingIntent.FLAG_UPDATE_CURRENT);

            if (broadcast != null)
            {
                GetAlarmManager().cancel(broadcast);
                broadcast.cancel();
            }
        }
        catch (Exception exception)
        {
            Log.e(LOG, "RemoveScheduledNotifications", exception);
        }
    }

    public static ILocalNotificationBridge GetOpenedNotificationInternal()
    {
        try
        {
            Activity unityPlayerActivity = UnityPlayer.currentActivity;
            Intent activityIntent = unityPlayerActivity.getIntent();

            return GetLocalNotification(activityIntent);
        }
        catch (Exception exception)
        {
            Log.e(LOG, "GetOpenedNotificationInternal", exception);
        }

        return null;
    }

    private static List<ILocalNotification> GetPendingNotifications()
    {
        return NotificationProvider.GetPendingNotifications();
    }

    private static void AddPendingNotification(ILocalNotification localNotification)
    {
        try
        {
            List<ILocalNotification> pendingIntents = GetPendingNotifications();
            pendingIntents.add(localNotification);

            NotificationProvider.SetPendingNotifications(pendingIntents);
        }
        catch (Exception exception)
        {
            Log.e(LOG, "AddPendingNotification", exception);
        }
    }

    private static void RemovePendingNotification(ILocalNotification localNotification)
    {
        try
        {
            List<ILocalNotification> pendingNotifications = GetPendingNotifications();
            pendingNotifications.remove(localNotification);

            NotificationProvider.SetPendingNotifications(pendingNotifications);
        }
        catch (Exception exception)
        {
            Log.e(LOG, "RemovePendingNotification", exception);
        }
    }

    public static void RemoveReceivedNotificationsInternal()
    {
        try
        {
            GetNotificationManager().cancelAll();
        }
        catch (Exception exception)
        {
            Log.e(LOG, "RemoveReceivedNotificationsInternal", exception);
        }
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
        catch (Exception exception)
        {
            Log.e(LOG, "GetLocalNotification", exception);
        }

        return null;
    }

    public static void NotifyNotificationReceived(ILocalNotification localNotification)
    {
        try
        {
            LastReceivedNotification = localNotification;

            if(_notificationReceivedCallback != null)
            {
                _notificationReceivedCallback.OnNotificationReceived();
            }

            if(!IsAppOnForeground())
            {
                NotificationProvider.AddReceivedNotification(localNotification);
            }
            
            CancellById(Integer.valueOf(localNotification.GetID()));

            RemovePendingNotification(localNotification);
        }
        catch (Exception exception)
        {
            Log.e(LOG, "NotifyNotificationReceived", exception);
        }
    }

    private static boolean IsAppOnForeground()
    {
        try
        {
            ActivityManager activityManager = (ActivityManager) GetContext().getSystemService(Context.ACTIVITY_SERVICE);
            List<ActivityManager.RunningAppProcessInfo> appProcesses = activityManager.getRunningAppProcesses();
            if (appProcesses == null)
            {
                return false;
            }

            final String packageName = GetContext().getPackageName();
            for (ActivityManager.RunningAppProcessInfo appProcess : appProcesses)
            {
                if (appProcess.importance == ActivityManager.RunningAppProcessInfo.IMPORTANCE_FOREGROUND
                        && appProcess.processName.equals(packageName))
                {
                    return true;
                }
            }
        }
        catch (Exception exception)
        {
            Log.e(LOG, "IsAppOnForeground", exception);
        }

        return false;
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
        try
        {
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N)
            {
                return GetNotificationManager().areNotificationsEnabled();
            }
        }
        catch (Exception exception)
        {
            Log.e(LOG, "AreNotificationEnabledInternal", exception);
        }

        return false;
    }
}