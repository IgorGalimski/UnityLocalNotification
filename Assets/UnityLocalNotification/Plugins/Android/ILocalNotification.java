package com.igorgalimski.unitylocalnotification;

import org.json.JSONObject;

public interface ILocalNotification
{
    String GetID();

    void SetID(int id);
    
    String GetTitle();

    String GetBody();

    String GetData();

    int GetFireInSeconds();

    long GetFiredSeconds();

    void SetFiredSeconds(long firedSeconds);

    JSONObject GetAsObject();
}
