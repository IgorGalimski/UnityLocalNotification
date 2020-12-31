package com.igorgalimski.unitylocalnotification;

import android.content.Context;
import android.content.SharedPreferences;

import org.json.JSONArray;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;

public class NotificationProvider
{
    private static final String NOTIFICATION_IDS_SHARED_PREFS = "NOTIFICATIONS";

    private static final String RECEIVED_NOTIFICATION_IDS_SHARED_PREFS = "RECEIVED_NOTIFICATIONS";
    private static final String PENDING_NOTIFICATION_IDS_SHARED_PREFS = "PENDING_NOTIFICATIONS";
    private static final String NOTIFICATION_CHANNEL_ID_SHARED_PREFS = "NOTIFICATION_CHANNEL";

    private static SharedPreferences _prefs;
    private static SharedPreferences.Editor _prefsEditor;

    private static JSONArray _receivedNotificationsArray;
    private static List<ILocalNotification> _receivedNotifications;

    private static JSONArray _pendingNotificationsArray;
    private static HashSet<ILocalNotification> _pendingNotifications;

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
    }

    public static void SetPendingNotifications(HashSet<ILocalNotification> notifications)
    {
        _pendingNotifications = notifications;

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
                String receivedNotificationsList = GetPrefs().getString(RECEIVED_NOTIFICATION_IDS_SHARED_PREFS, "[{}]");

                JSONArray jsonArray = new JSONArray(receivedNotificationsList);
                for (int i=0; i < jsonArray.length(); i++)
                {
                    JSONObject jsonObject = jsonArray.getJSONObject(i);
                    ILocalNotification localNotification = LocalNotification.FromJSONObject(jsonObject);

                    if(localNotification != null)
                    {
                        _receivedNotifications.add(localNotification);
                    }

                    ReceivedNotificationsArray().put(jsonObject);
                }
            }
            catch (Exception exception)
            {

            }
        }

        return _receivedNotifications;
    }

    public static HashSet<ILocalNotification> GetPendingNotifications()
    {
        if(_pendingNotifications == null)
        {
            try
            {
                _pendingNotifications = new HashSet<>();
                String pendingNotificationsList = GetPrefs().getString(PENDING_NOTIFICATION_IDS_SHARED_PREFS, "[{}]");

                JSONArray jsonArray = new JSONArray(pendingNotificationsList);
                for (int i=0; i < jsonArray.length(); i++)
                {
                    JSONObject jsonObject = jsonArray.getJSONObject(i);
                    ILocalNotification localNotification = LocalNotification.FromJSONObject(jsonObject);

                    if(localNotification != null)
                    {
                        _pendingNotifications.add(localNotification);
                    }

                    GetPendingNotificationsArray().put(jsonObject);
                }
            }
            catch (Exception exception)
            {

            }
        }

        return _pendingNotifications;
    }

    public static void ClearPendingNotifications()
    {
        GetPendingNotifications().clear();

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
