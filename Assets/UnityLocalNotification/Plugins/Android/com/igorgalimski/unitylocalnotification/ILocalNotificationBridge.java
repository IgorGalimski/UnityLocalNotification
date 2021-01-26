package com.igorgalimski.unitylocalnotification;

public interface ILocalNotificationBridge
{
    String GetID();
    
    boolean GetAutoCancel();
    
    String GetTitle();

    String GetBody();

    String GetData();
    
    String GetSmallIconId();
    
    String GetLargeIconId();

    double GetFireInSeconds();

    long GetFiredSeconds();
}