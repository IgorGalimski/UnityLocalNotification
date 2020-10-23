package com.igorgalimski.unitylocalnotification;

public class LocalNotification implements ILocalNotification
{
    private String _title;
    private String _body;
    private String _data;
    private int _fireInSeconds;

    public LocalNotification(String title, String body, String data, int fireInSeconds)
    {
        _title = title;
        _body = body;
        _data = data;
        _fireInSeconds = fireInSeconds;
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
}
