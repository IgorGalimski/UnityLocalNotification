package com.igorgalimski.unitylocalnotification;

public interface ILocalNotification
{
    String GetTitle();

    String GetBody();

    String GetData();

    int GetFireInSeconds();
}
