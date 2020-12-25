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

    private static final String RECEIVED_NOTIFICATION_IDS_SHARED_PREFS = "RECEIVED_INTENTS";
    private static final String NOTIFICATION_CHANNEL_ID_SHARED_PREFS = "NOTIFICATION_CHANNEL";

    private static SharedPreferences _prefs;
    private static SharedPreferences.Editor _prefsEditor;

    private static JSONArray _receivedNotificationsArray;
    private static List<ILocalNotification> _receivedNotifications;

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

    public static HashSet<String> GetPendingIntents()
    {
        return (HashSet<String>) GetPrefs().getStringSet(NOTIFICATION_IDS_SHARED_PREFS, new HashSet<String>());
    }

    public static void SetPendingIntents(HashSet<String> intents)
    {
        GetEditor().putStringSet(NOTIFICATION_IDS_SHARED_PREFS, intents);
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

    private static JSONArray ReceivedNotificationsArray()
    {
        if(_receivedNotificationsArray == null)
        {
            _receivedNotificationsArray = new JSONArray();
        }

        return _receivedNotificationsArray;
    }
}
