package com.igorgalimski.unitylocalnotification;

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
    private int _fireInSeconds;
    private long _firedSeconds;

    public LocalNotification(String id, boolean autoCancel, String title, String body, String data, String smallIconId, String largeIconId, int fireInSeconds, long firedSeconds)
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
    public int GetFireInSeconds()
    {
        return _fireInSeconds;
    }

    @Override
    public long GetFiredSeconds() {
        return _firedSeconds;
    }

    @Override
    public void SetFiredSeconds(long firedSeconds) {
        _firedSeconds = firedSeconds;
    }

    @Override
    public JSONObject GetAsObject()
    {
        JSONObject jsonObject = new JSONObject();
        try
        {
            jsonObject.put(ID_KEY, _id);
            jsonObject.put(AUTO_CANCEL_KEY, _autoCancel);
            jsonObject.put(TITLE_KEY, _title);
            jsonObject.put(BODY_KEY, _body);
            jsonObject.put(DATA_KEY, _data);
            jsonObject.put(SMALL_ICON_ID_KEY, _smallIconId);
            jsonObject.put(LARGE_ICON_ID_KEY, _largeIconId);
            jsonObject.put(FIRE_IN_SECONDS_KEY, _fireInSeconds);
            jsonObject.put(FIRED_SECONDS_KEY, _firedSeconds);
        }
        catch (JSONException e)
        {
            e.printStackTrace();
        }

        return jsonObject;
    }

    public static ILocalNotificationBridge FromJSONObject(JSONObject jsonObject) {
        if (jsonObject == null) {
            return null;
        }

        String id = null;
        Boolean autoCancel = false;
        String title = null;
        String body = null;
        String data = null;
        String smallIconId = null;
        String largeIconId = null;
        int fireInSeconds = 0;
        int firedSeconds = 0;

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
            fireInSeconds = jsonObject.getInt(FIRE_IN_SECONDS_KEY);
            firedSeconds = jsonObject.getInt(FIRED_SECONDS_KEY);
        }
        catch (JSONException e)
        {
            e.printStackTrace();
        }

        LocalNotification localNotification = new LocalNotification(id, autoCancel, title, body, data, largeIconId, smallIconId, fireInSeconds, firedSeconds);
        return localNotification;
    }
}