package com.igorgalimski.unitylocalnotification;

import android.util.Log;

import org.json.JSONException;
import org.json.JSONObject;

public class LocalNotification implements ILocalNotification
{
    public static final String NOTIFICATION = "notification";
    public static final String LOCAL_NOTIFICATION = "localNotification";
    
    public static final String ID_KEY = "id";
    public static final String AUTO_CANCEL_KEY = "autoCancel";
    public static final String TITLE_KEY = "title";
    public static final String BODY_KEY = "body";
    public static final String DATA_KEY = "data";
    public static final String SMALL_ICON_ID_KEY = "smallIconId";
    public static final String LARGE_ICON_ID_KEY = "largeIconId";
    public static final String FIRE_IN_SECONDS_KEY = "fireInSeconds";
    public static final String FIRED_SECONDS_KEY = "firedSeconds";

    private String _id;
    private boolean _autoCancel;
    private String _title;
    private String _body;
    private String _data;
    private String _smallIconId;
    private String _largeIconId;
    private double _fireInSeconds;
    private long _firedSeconds;

    public LocalNotification(String id, boolean autoCancel, String title, String body, String data, String smallIconId, String largeIconId, double fireInSeconds, long firedSeconds)
    {
        _id = id;
        _autoCancel = autoCancel;
        _title = title;
        _body = body;
        _data = data;
        _smallIconId = smallIconId;
        _largeIconId = largeIconId;
        _fireInSeconds = fireInSeconds;
        _firedSeconds = firedSeconds;
    }

    @Override
    public String GetID()
    {
        return _id;
    }

    @Override
    public void SetID(int id) {
        _id = String.valueOf(id);
    }

    @Override
    public boolean GetAutoCancel() {
        return _autoCancel;
    }

    @Override
    public String GetTitle()
    {
        return _title;
    }

    @Override
    public String GetBody()
    {
        return _body;
    }

    @Override
    public String GetData()
    {
        return _data;
    }

    @Override
    public String GetSmallIconId() {
        return _smallIconId;
    }

    @Override
    public String GetLargeIconId() {
       return _largeIconId;
    }

    @Override
    public double GetFireInSeconds()
    {
        return _fireInSeconds;
    }

    @Override
    public long GetFiredSeconds() {
        return _firedSeconds;
    }

    @Override
    public void SetFiredSeconds(long firedSeconds)
    {
        _firedSeconds = firedSeconds;
    }

    @Override
    public JSONObject GetAsObject()
    {
        JSONObject jsonObject = new JSONObject();
        try
        {
            jsonObject.put(ID_KEY, GetValue(_id));
            jsonObject.put(AUTO_CANCEL_KEY, _autoCancel);
            jsonObject.put(TITLE_KEY, GetValue(_title));
            jsonObject.put(BODY_KEY, GetValue(_body));
            jsonObject.put(DATA_KEY, GetValue(_data));
            jsonObject.put(SMALL_ICON_ID_KEY, GetValue(_smallIconId));
            jsonObject.put(LARGE_ICON_ID_KEY, GetValue(_largeIconId));
            jsonObject.put(FIRE_IN_SECONDS_KEY, _fireInSeconds);
            jsonObject.put(FIRED_SECONDS_KEY, _firedSeconds);
        }
        catch (JSONException e)
        {
            Log.e(NotificationManager.LOG, "GetAsObject", e);
        }

        return jsonObject;
    }

    private Object GetValue(String value)
    {
        return value == null ? JSONObject.NULL : value;
    }

    @Override
    public int hashCode() 
    {
        return Integer.valueOf(_id);
    }

    public boolean equals(Object o)
    {
        if(o == null)
        {
            return false;
        }

        if (!(o instanceof LocalNotification))
        {
            return false;
        }

        LocalNotification oNotification = (LocalNotification) o;

        return oNotification._id.equals(_id);
    }

    public static ILocalNotification FromJSONObject(JSONObject jsonObject) 
    {
        if (jsonObject == null) 
        {
            return null;
        }

        String id = null;
        Boolean autoCancel = false;
        String title = null;
        String body = null;
        String data = null;
        String smallIconId = null;
        String largeIconId = null;
        double fireInSeconds = 0;
        long firedSeconds = 0;

        try
        {
            if(!jsonObject.has(ID_KEY))
            {
                return null;
            }

            id = jsonObject.getString(ID_KEY);
            autoCancel = jsonObject.getBoolean(AUTO_CANCEL_KEY);
            title = jsonObject.getString(TITLE_KEY);
            body = jsonObject.getString(BODY_KEY);
            data = jsonObject.getString(DATA_KEY);
            smallIconId = jsonObject.getString(SMALL_ICON_ID_KEY);
            largeIconId = jsonObject.getString(LARGE_ICON_ID_KEY);
            fireInSeconds = jsonObject.getDouble(FIRE_IN_SECONDS_KEY);
            firedSeconds = jsonObject.getLong(FIRED_SECONDS_KEY);
        }
        catch (JSONException e)
        {
            Log.e(NotificationManager.LOG, "FromJSONObject", e);
        }

        LocalNotification localNotification = new LocalNotification(id, autoCancel, title, body, data, smallIconId, largeIconId, fireInSeconds, firedSeconds);
        return localNotification;
    }
    
    public static ILocalNotification GetFromBridge(ILocalNotificationBridge localNotificationBridge)
    {
        ILocalNotification localNotification = new LocalNotification(localNotificationBridge.GetID(),
                localNotificationBridge.GetAutoCancel(),
                localNotificationBridge.GetTitle(),
                localNotificationBridge.GetBody(),
                localNotificationBridge.GetData(),
                localNotificationBridge.GetSmallIconId(),
                localNotificationBridge.GetLargeIconId(),
                localNotificationBridge.GetFireInSeconds(),
                localNotificationBridge.GetFiredSeconds());

        return localNotification;
    }
}