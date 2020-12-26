package com.igorgalimski.unitylocalnotification;

import org.json.JSONException;
import org.json.JSONObject;

public class LocalNotification implements ILocalNotification
{
    private static final String ID_KEY = "id";
    private static final String TITLE_KEY = "title";
    private static final String BODY_KEY = "body";
    private static final String DATA_KEY = "data";
    private static final String FIRE_IN_SECONDS_KEY = "fireInSeconds";
    private static final String FIRED_SECONDS_KEY = "firedSeconds";

    private String _id;
    private String _title;
    private String _body;
    private String _data;
    private int _fireInSeconds;
    private int _firedSeconds;

    public LocalNotification(String id, String title, String body, String data, int fireInSeconds, int firedSeconds)
    {
        _id = id;
        _title = title;
        _body = body;
        _data = data;
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
    public int GetFiredSeconds() {
        return _firedSeconds;
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
            jsonObject.put(FIRE_IN_SECONDS_KEY, _fireInSeconds);
            jsonObject.put(FIRED_SECONDS_KEY, _firedSeconds);
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
        int fireInSeconds = 0;
        int firedSeconds = 0;

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
            fireInSeconds = jsonObject.getInt(FIRE_IN_SECONDS_KEY);
            firedSeconds = jsonObject.getInt(FIRED_SECONDS_KEY);
        }
        catch (JSONException e)
        {
            e.printStackTrace();
        }

        LocalNotification localNotification = new LocalNotification(id, title, body, data, fireInSeconds, firedSeconds);
        return localNotification;
    }
}
