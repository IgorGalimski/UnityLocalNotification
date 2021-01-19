package com.igorgalimski.unitylocalnotification;

import org.json.JSONObject;

public interface ILocalNotification
{
    String GetID();

    void SetID(int id);
    
    boolean GetAutoCancel();
    
    String GetTitle();

    String GetBody();

    String GetData();
    
    String GetBigIconId();

    int GetFireInSeconds();

    long GetFiredSeconds();

    void SetFiredSeconds(long firedSeconds);

    JSONObject GetAsObject();
}