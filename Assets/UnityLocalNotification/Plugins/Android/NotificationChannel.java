package com.igorgalimski.unitylocalnotification;

public interface NotificationChannel
{
    String GetId();
    String GetName();
    String GetDescription();
    Boolean GetShowBadge();
    int GetImportance();
}
