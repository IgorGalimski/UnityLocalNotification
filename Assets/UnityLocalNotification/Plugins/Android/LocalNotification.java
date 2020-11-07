package com.igorgalimski.unitylocalnotification;

import org.json.JSONException;
import org.json.JSONObject;

public class LocalNotification implements ILocalNotification
{
    private static final String ID_KEY = "id";
    private static final String TITLE_KEY = "title";
    private static final String BODY_KEY = "body";
    private static final String DATA_KEY = "data";
    private static final String SECONDS_KEY = "seconds";

    private String _id;
    private String _title;
    private String _body;
    private String _data;
    private int _fireInSeconds;

    public LocalNotification(String id, String title, String body, String data, int fireInSeconds)
    {
        _id = id;
        _title = title;
        _body = body;
        _data = data;
        _fireInSeconds = fireInSeconds;
    }

    @Override
    public String GetID()
    {
        return _id;
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
    public int GetFireInSeconds()
    {
        return _fireInSeconds;
    }

    @Override
    public JSONObject GetAsObject()
    {
        JSONObject jsonObject = new JSONObject();
        try
        {
            jsonObject.put(ID_KEY, _id);
            jsonObject.put(TITLE_KEY, _title);
            jsonObject.put(BODY_KEY, _body);
            jsonObject.put(DATA_KEY, _data);
            jsonObject.put(SECONDS_KEY, _fireInSeconds);
        }
        catch (JSONException e)
        {
            e.printStackTrace();
        }

        return jsonObject;
    }

    public static ILocalNotification FromJSONObject(JSONObject jsonObject) {
        if (jsonObject == null) {
            return null;
        }

        String id = null;
        String title = null;
        String body = null;
        String data = null;
        int seconds = 0;
        try
        {
            if(!jsonObject.has(ID_KEY))
            {
                return null;
            }

            id = jsonObject.getString(ID_KEY);
            title = jsonObject.getString(TITLE_KEY);
            body = jsonObject.getString(BODY_KEY);
            data = jsonObject.getString(DATA_KEY);
            seconds = jsonObject.getInt(SECONDS_KEY);
        }
        catch (JSONException e)
        {
            e.printStackTrace();
        }

        LocalNotification localNotification = new LocalNotification(id, title, body, data, seconds);
        return localNotification;
    }
}
