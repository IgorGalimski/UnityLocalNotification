package com.igorgalimski.unitylocalnotification;

public interface INotificationChannel
{
    String GetId();
    String GetName();
    String GetDescription();
    Boolean GetShowBadge();
    int GetImportance();
    Boolean GetVibration();
    Boolean GetLight();
}
