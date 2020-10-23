package com.igorgalimski.unitylocalnotification;

public interface LocalNotification
{
    String GetTitle();

    String GetBody();

    String GetData();

    int GetFireInSeconds();
}
