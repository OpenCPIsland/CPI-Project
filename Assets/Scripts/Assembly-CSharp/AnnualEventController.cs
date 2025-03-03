using UnityEngine;
using System;
using System.Reflection;

public class AnnualEventsController : MonoBehaviour
{
    // Singleton instance
    public static AnnualEventsController Instance { get; private set; }

    [Serializable]
    public class EventInfo
    {
        public string eventID;
        public string eventName;
        public int startMonth;
        public int startDay;
        public int endMonth;
        public int endDay;
    }

    public EventInfo[] events;  // Array of events

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (events.Length == 0)
        {
            Debug.LogError("No events configured.");
            return;
        }

        int currentYear = DateTime.Now.Year;

        foreach (var eventInfo in events)
        {
            string path = $"definitions/scheduledeventdates/Date_{eventInfo.eventID}_{eventInfo.eventName}";
            var eventAsset = Resources.Load<ScriptableObject>(path);

            if (eventAsset != null)
            {
                var type = eventAsset.GetType();
                var datesField = type.GetField("Dates", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (datesField != null)
                {
                    var datesValue = datesField.GetValue(eventAsset);

                    if (datesValue != null)
                    {
                        // Access StartDate and EndDate as separate objects
                        var startDateField = datesValue.GetType().GetField("StartDate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        var endDateField = datesValue.GetType().GetField("EndDate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                        if (startDateField != null && endDateField != null)
                        {
                            var startDate = startDateField.GetValue(datesValue);
                            var endDate = endDateField.GetValue(datesValue);

                            if (startDate != null && endDate != null)
                            {
                                // Set values for StartDate
                                SetFieldOrPropertyValue(startDate, "day", eventInfo.startDay);
                                SetFieldOrPropertyValue(startDate, "month", eventInfo.startMonth);
                                SetFieldOrPropertyValue(startDate, "year", currentYear);

                                // Set values for EndDate
                                SetFieldOrPropertyValue(endDate, "day", eventInfo.endDay);
                                SetFieldOrPropertyValue(endDate, "month", eventInfo.endMonth);
                                SetFieldOrPropertyValue(endDate, "year", currentYear);

                                // Calculate and set timestamps for MIDNIGHT (00:00:00)
                                DateTime startDateTime = new DateTime(currentYear, eventInfo.startMonth, eventInfo.startDay, 0, 0, 0);
                                DateTime endDateTime = new DateTime(currentYear, eventInfo.endMonth, eventInfo.endDay, 0, 0, 0);
                                long startTimestamp = new DateTimeOffset(startDateTime).ToUnixTimeMilliseconds();
                                long endTimestamp = new DateTimeOffset(endDateTime).ToUnixTimeMilliseconds();

                                SetFieldOrPropertyValue(startDate, "TimeStampInMilliseconds", startTimestamp);
                                SetFieldOrPropertyValue(endDate, "TimeStampInMilliseconds", endTimestamp);

                                Debug.Log($"✅ Updated event {eventInfo.eventID} ({eventInfo.eventName}) to start and end at midnight.");
                            }
                            else
                            {
                                Debug.LogError($"❌ StartDate or EndDate is null in {eventInfo.eventName} asset!");
                            }
                        }
                        else
                        {
                            Debug.LogError($"❌ StartDate or EndDate fields not found in {eventInfo.eventID} ({eventInfo.eventName}).");
                        }
                    }
                    else
                    {
                        Debug.LogError($"❌ Dates field is null in {eventInfo.eventName} asset!");
                    }
                }
                else
                {
                    Debug.LogError($"❌ Dates field not found in {eventInfo.eventID} ({eventInfo.eventName}).");
                }
            }
            else
            {
                Debug.LogError($"❌ Failed to load event data for ID: {eventInfo.eventID} at path: {path}");
            }
        }
    }

    // Helper method to set field or property values using reflection
    private void SetFieldOrPropertyValue(object target, string name, object newValue)
    {
        var type = target.GetType();

        // Try to find a field first
        var field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(target, newValue);
            return;
        }

        // If field not found, try to find a property
        var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (property != null && property.CanWrite)
        {
            property.SetValue(target, newValue);
            return;
        }

        Debug.LogError($"❌ Field or property {name} not found in {type.Name}!");
    }
}
