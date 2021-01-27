package com.igorgalimski.unitylocalnotification;

import android.content.Context;
import android.content.SharedPreferences;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.List;

public class NotificationProvider
{
    private static final String NOTIFICATION_IDS_SHARED_PREFS = "NOTIFICATIONS";

    private static final String RECEIVED_NOTIFICATION_IDS_SHARED_PREFS = "RECEIVED_NOTIFICATIONS";
    private static final String PENDING_NOTIFICATION_IDS_SHARED_PREFS = "PENDING_NOTIFICATIONS";
    private static final String NOTIFICATION_CHANNEL_ID_SHARED_PREFS = "NOTIFICATION_CHANNEL";

    private static final String OPEN_ACTIVITY_SHARED_PREFS = "OPEN_ACTIVITY";

    private static SharedPreferences _prefs;
    private static SharedPreferences.Editor _prefsEditor;

    private static JSONArray _receivedNotificationsArray;
    private static List<ILocalNotification> _receivedNotifications;

    private static JSONArray _pendingNotificationsArray;
    private static List<ILocalNotification> _pendingNotifications;

    private static SharedPreferences GetPrefs()
    {
        if(_prefs == null)
        {
            _prefs = NotificationManager.GetContext().getSharedPreferences(NOTIFICATION_IDS_SHARED_PREFS, Context.MODE_PRIVATE);
        }

        return _prefs;
    }

    private static SharedPreferences.Editor GetEditor()
    {
         if(_prefsEditor == null)
         {
             _prefsEditor = GetPrefs().edit();
         }

         return _prefsEditor;
    }

    public static String GetNotificationChannelID()
    {
        return GetPrefs().getString(NOTIFICATION_CHANNEL_ID_SHARED_PREFS, "");
    }

    public static void SetNotificationChannelID(String channelID)
    {
        GetEditor().putString(NOTIFICATION_CHANNEL_ID_SHARED_PREFS, channelID);
        GetEditor().apply();
    }

    public static void SetOpenAppActivity(String openAppActivity)
    {
        GetEditor().putString(OPEN_ACTIVITY_SHARED_PREFS, openAppActivity);
        GetEditor().apply();
    }

    public static String GetOpenAppActivity()
    {
        return GetPrefs().getString(OPEN_ACTIVITY_SHARED_PREFS, "");
    }

    public static void SetPendingNotifications(List<ILocalNotification> notifications)
    {
        _pendingNotifications = notifications;
        _pendingNotificationsArray = null;

        for (ILocalNotification localNotification: notifications)
        {
            GetPendingNotificationsArray().put(localNotification.GetAsObject());
        }

        String notificationArrayString = GetPendingNotificationsArray().toString();

        GetEditor().putString(PENDING_NOTIFICATION_IDS_SHARED_PREFS, notificationArrayString);
        GetEditor().apply();
    }

    public static void AddReceivedNotification(ILocalNotification localNotification)
    {
        ReceivedNotificationsArray().put(localNotification.GetAsObject());
        GetReceivedNotificationsList().add(localNotification);

        String notificationArrayString = ReceivedNotificationsArray().toString();

        GetEditor().putString(RECEIVED_NOTIFICATION_IDS_SHARED_PREFS, notificationArrayString);
        GetEditor().apply();
    }

    public static void ClearReceivedNotifications()
    {
        GetReceivedNotificationsList().clear();
        _receivedNotificationsArray = null;

        GetEditor().remove(RECEIVED_NOTIFICATION_IDS_SHARED_PREFS);
        GetEditor().apply();
    }

    public static List<ILocalNotification> GetReceivedNotificationsList()
    {
        if(_receivedNotifications == null)
        {
            try
            {
                _receivedNotifications = new ArrayList<>();

                GetNotificationsFromPrefs(RECEIVED_NOTIFICATION_IDS_SHARED_PREFS, _receivedNotifications, ReceivedNotificationsArray());
            }
            catch (JSONException exception)
            {
            }
        }

        return _receivedNotifications;
    }

    public static List<ILocalNotification> GetPendingNotifications()
    {
        if(_pendingNotifications == null)
        {
            try
            {
                _pendingNotifications = new ArrayList<>();

                GetNotificationsFromPrefs(PENDING_NOTIFICATION_IDS_SHARED_PREFS, _pendingNotifications, GetPendingNotificationsArray());
            }
            catch (Exception exception)
            {
            }
        }

        return _pendingNotifications;
    }
    
    private static void GetNotificationsFromPrefs(String prefsKey, 
                                                  List<ILocalNotification> notificationsList,
                                                  JSONArray notificationsArray) throws JSONException 
    {
        String pendingNotificationsList = GetPrefs().getString(prefsKey, "[]");

        JSONArray jsonArray = new JSONArray(pendingNotificationsList);
        for (int i=0; i < jsonArray.length(); i++)
        {
            JSONObject jsonObject = jsonArray.getJSONObject(i);
            ILocalNotification localNotification = LocalNotification.FromJSONObject(jsonObject);

            if(localNotification != null)
            {
                notificationsList.add(localNotification);
                notificationsArray.put(jsonObject);
            }
        }
    }

    public static void ClearPendingNotifications()
    {
        GetPendingNotifications().clear();
        _pendingNotificationsArray = null;

        GetEditor().remove(PENDING_NOTIFICATION_IDS_SHARED_PREFS);
        GetEditor().apply();
    }

    private static JSONArray ReceivedNotificationsArray()
    {
        if(_receivedNotificationsArray == null)
        {
            _receivedNotificationsArray = new JSONArray();
        }

        return _receivedNotificationsArray;
    }

    private static JSONArray GetPendingNotificationsArray()
    {
        if(_pendingNotificationsArray == null)
        {
            _pendingNotificationsArray = new JSONArray();
        }

        return _pendingNotificationsArray;
    }
}