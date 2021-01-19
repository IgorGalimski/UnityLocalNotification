package com.igorgalimski.unitylocalnotification;

import org.json.JSONObject;

public interface ILocalNotification extends ILocalNotificationBridge
{
    void SetID(int id);

    void SetFiredSeconds(long firedSeconds);

    JSONObject GetAsObject();

    static ILocalNotification GetFromBridge(ILocalNotificationBridge localNotificationBridge)
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