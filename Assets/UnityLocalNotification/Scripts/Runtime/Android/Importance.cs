namespace UnityLocalNotifications.Android
{
    public enum Importance
    {
        /// <summary>
        /// A notification with no importance: does not show in the shade.
        /// </summary>
        None = 0,
        
        /// <summary>
        /// Min importance, notification is shown without sound and does not appear in the status bar
        /// </summary>
        Min = 1,

        /// <summary>
        /// Low importance, notification is shown everywhere, but is not intrusive.
        /// </summary>
        Low = 2,

        /// <summary>
        /// Default importance, notification is shown everywhere, makes noise, but does not intrude visually.
        /// </summary>
        Default = 3,

        /// <summary>
        /// High importance, notification is shown everywhere, makes noise and is shown on the screen.
        /// </summary>
        High = 4,
    }
}