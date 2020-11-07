package com.igorgalimski.unitylocalnotification;

import org.json.JSONObject;

public interface ILocalNotification
{
    String GetTitle();

    String GetBody();

    String GetData();

    int GetFireInSeconds();

    JSONObject GetAsObject();
}
