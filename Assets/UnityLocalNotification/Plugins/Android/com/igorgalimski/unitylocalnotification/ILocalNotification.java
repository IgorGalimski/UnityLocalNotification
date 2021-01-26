package com.igorgalimski.unitylocalnotification;

import org.json.JSONObject;

public interface ILocalNotification extends ILocalNotificationBridge
{
    void SetID(int id);

    void SetFiredSeconds(long firedSeconds);

    JSONObject GetAsObject();
}