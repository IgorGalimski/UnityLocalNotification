package com.igorgalimski.unitylocalnotification;

public interface ILocalNotificationBridge
{
    int GetID();
    
    boolean GetAutoCancel();
    
    String GetTitle();

    String GetBody();

    String GetData();
    
    String GetSmallIconId();
    
    String GetLargeIconId();

    double GetFireInSeconds();

    long GetFiredSeconds();
}