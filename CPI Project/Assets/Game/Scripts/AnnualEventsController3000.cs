using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using ClubPenguin.Core;
using ClubPenguin.Net;

public class AnnualEventsController3000 : MonoBehaviour
{
    public static AnnualEventsController3000 Instance { get; private set; }

    private enum OverrideMode
    {
        Automatic = 0,
        ForceRegular = 1,
        ForceEvent = 2
    }

    private const string PLAYERPREFS_OVERRIDE_MODE_KEY = "AnnualEventsController.OverrideMode";
    private const string PLAYERPREFS_FORCED_EVENT_KEY = "AnnualEventsController.ForcedEventKey";
    private const string PLAYERPREFS_SERVER_SCHEDULE_KEY = "AnnualEventsController.ServerSchedule";
    private const string SERVER_SCHEDULE_PATH = "cdn/party-schedule.json";
    private const int SERVER_SCHEDULE_MAX_BYTES = 65536;
    private const int SERVER_SCHEDULE_MAX_ENTRIES = 200;

    [UnityEngine.Scripting.Preserve]
    private OverrideMode overrideMode = OverrideMode.Automatic;
    private string forcedEventKey = null;

    private static string GetPlatformKey(string key)
    {
#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
        if (Application.isEditor)
        {
            return "Editor_" + key;
        }
#endif
        return key;
    }

    [Serializable]
    public class EventInfo
    {
        [Header("Scheduled Event Dates")]
        [Tooltip("Use the event ID from the ScheduledEventDate asset. (example: date_22_halloween2018 results to ID 22.)")]
        public string eventID;

        [Tooltip("Use the ScheduledEventDate name that appears in the embedded_content_manifest.txt (example: date_22_halloween2018 results to halloween2018).")]
        public string eventName;

        [Tooltip("If enabled, this event will be excluded from annual events. Use this for the party switcher.")]
        public bool excludeFromDateSystem = false;

        [Tooltip("Start date of the event (set at midnight). Example: October 31 00:00.")]
        public int startMonth;
        public int startDay;

        [Tooltip("End date of the event (also set at midnight). To keep the event active through November 1st, set this to November 2 00:00.")]
        public int endMonth;
        public int endDay;

        [Header("Audio")]
        [Tooltip("Map scene names to event specific audio keys.")]
        public SceneAudioMapping[] SceneAudioMappings;

        [Header("Snowball")]
        [Tooltip("Optional material swap prefab for this event.")]
        public GameObject SnowballPrefab;
        public Material OriginalMaterial;
        public Material EventMaterial;
    }

    [Serializable]
    public class SceneAudioMapping
    {
        [Tooltip("Scene name this mapping applies to.")]
        public string SceneName;

        [Tooltip("Audio key used when the event is not active.")]
        public string DefaultAudioKey;

        [Tooltip("Audio key used while the event is active.")]
        public string EventAudioKey;
    }

    [Serializable]
    private class ServerSchedule
    {
        public int version;
        public string mode;
        public string party;
        public ServerEntry[] calendar;
    }

    [Serializable]
    private class ServerEntry
    {
        public int id;
        public string name;
        public string start;
        public string end;
        public bool calendar;
    }

    [Header("Annual events (loops every year).")]
    public EventInfo[] events;

    private ServerSchedule serverSchedule;
    private bool fetching;
    private int fetchFailures;
    private string lastFetchFailure;
    private string lastApplied;
    private float nextFetchTime;
    private bool settingsSeen;
    private bool serverTimeSeen;

    public bool ServerControlsParties
    {
        get { return serverSchedule != null && !IsOffline(); }
    }

    private static bool IsOffline()
    {
        return Service.IsSet<ClubPenguin.GameSettings>() && !ClubPenguin.DisableWhenNoGameServer.IsGameServerAvailable();
    }


    private DateTimeOffset CurrentTime()
    {
        if (Service.IsSet<INetworkServicesManager>())
        {
            INetworkServicesManager network = Service.Get<INetworkServicesManager>();
            if (network.GameTimeMilliseconds > 0)
            {
                return new DateTimeOffset(DateTime.SpecifyKind(network.ServerDateTime, DateTimeKind.Utc));
            }
        }
        return DateTimeOffset.UtcNow;
    }


    public bool IsPartySwitchForced
    {
        get
        {
            if (overrideMode != OverrideMode.ForceEvent)
            {
                return false;
            }

            if (string.IsNullOrEmpty(forcedEventKey))
            {
                return false;
            }

            if (events == null)
            {
                return false;
            }

            for (int i = 0; i < events.Length; i++)
            {
                var e = events[i];
                if (e == null)
                {
                    continue;
                }

                if (!e.excludeFromDateSystem)
                {
                    continue;
                }

                string key = BuildEventKey(e);
                if (forcedEventKey == key || forcedEventKey == e.eventName || forcedEventKey == e.eventID)
                {
                    return true;
                }
            }

            return false;
        }
    }

    private Dictionary<string, string> activeSceneKeys = new Dictionary<string, string>();
    private HashSet<string> seenScenes = new HashSet<string>();
    private float refreshInterval = 900f;
    private float nextRefreshTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadOverrideState();
        LoadCachedServerSchedule();

        if (events.Length == 0)
        {
            Debug.LogError("No events configured.");
            return;
        }

        ApplyEvents();
        nextRefreshTime = Time.time + refreshInterval;
    }

    private void Update()
    {
        bool reapply = false;
        if (!settingsSeen && Service.IsSet<ClubPenguin.GameSettings>())
        {
            settingsSeen = true;
            reapply = serverSchedule != null;
        }
        if (!serverTimeSeen && Service.IsSet<INetworkServicesManager>() && Service.Get<INetworkServicesManager>().GameTimeMilliseconds > 0)
        {
            serverTimeSeen = true;
            reapply = true;
        }

        if (Time.time >= nextRefreshTime)
        {
            reapply = true;
            nextRefreshTime = Time.time + refreshInterval;
        }
        if (reapply)
        {
            ApplyEvents();
        }

        if (!fetching && Time.time >= nextFetchTime && settingsSeen && !IsOffline())
        {
            string url = ServerScheduleUrl();
            if (url != null)
            {
                fetching = true;
                StartCoroutine(FetchServerSchedule(url));
            }
            else
            {
                nextFetchTime = Time.time + 1f;
            }
        }
    }

    private static string ServerScheduleUrl()
    {
        try
        {
            return ContentHelper.GetCdnUrl() + SERVER_SCHEDULE_PATH;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private IEnumerator FetchServerSchedule(string url)
    {
        string failure = null;
        bool noSchedule = false;
        try
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.timeout = 10;
                yield return request.SendWebRequest();
                if (request.responseCode == 404)
                {
                    noSchedule = true;
                }
                else if (request.result != UnityWebRequest.Result.Success)
                {
                    failure = request.error;
                }
                else
                {
                    byte[] data = request.downloadHandler.data;
                    ServerSchedule parsed;
                    if (data == null || data.Length > SERVER_SCHEDULE_MAX_BYTES)
                    {
                        failure = "bad size";
                    }
                    else
                    {
                        string json = System.Text.Encoding.UTF8.GetString(data);
                        if (TryParseSchedule(json, out parsed))
                        {
                            serverSchedule = parsed;
                            fetchFailures = 0;
                            PlayerPrefs.SetString(GetPlatformKey(PLAYERPREFS_SERVER_SCHEDULE_KEY), json);
                            PlayerPrefs.Save();
                            ApplyEvents();
                        }
                        else
                        {
                            failure = "bad schedule";
                        }
                    }
                }
            }

            if (noSchedule && serverSchedule != null)
            {
                serverSchedule = null;
                PlayerPrefs.DeleteKey(GetPlatformKey(PLAYERPREFS_SERVER_SCHEDULE_KEY));
                ApplyEvents();
            }
            if (failure != null && failure != lastFetchFailure)
            {
            }
            lastFetchFailure = failure;
        }
        finally
        {
            if (failure != null) fetchFailures++;
            bool retrySoon = failure != null && serverSchedule == null && fetchFailures < 3;
            nextFetchTime = Time.time + (retrySoon ? 60f : refreshInterval);
            fetching = false;
        }
    }

    private void LoadCachedServerSchedule()
    {
        string json = PlayerPrefs.GetString(GetPlatformKey(PLAYERPREFS_SERVER_SCHEDULE_KEY), null);
        ServerSchedule parsed;
        if (!string.IsNullOrEmpty(json) && TryParseSchedule(json, out parsed))
        {
            serverSchedule = parsed;
        }
    }

    private static readonly int[] DaysInMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

    private static bool TryMonthDay(string text, out int month, out int day)
    {
        month = 0;
        day = 0;
        if (text == null || text.Length != 5 || text[2] != '-')
        {
            return false;
        }
        if (!int.TryParse(text.Substring(0, 2), out month) || !int.TryParse(text.Substring(3, 2), out day))
        {
            return false;
        }
        return month >= 1 && month <= 12 && day >= 1 && day <= DaysInMonth[month - 1];
    }

    private static bool TryParseSchedule(string json, out ServerSchedule schedule)
    {
        schedule = null;
        try
        {
            ServerSchedule parsed = JsonUtility.FromJson<ServerSchedule>(json);
            if (parsed == null || parsed.version != 1 || parsed.calendar == null || parsed.calendar.Length > SERVER_SCHEDULE_MAX_ENTRIES)
            {
                return false;
            }
            if (parsed.mode != "auto" && parsed.mode != "party" && parsed.mode != "none")
            {
                return false;
            }
            List<ServerEntry> good = new List<ServerEntry>();
            foreach (ServerEntry entry in parsed.calendar)
            {
                int m, d;
                if (entry != null && entry.id > 0 && !string.IsNullOrEmpty(entry.name) && TryMonthDay(entry.start, out m, out d) && TryMonthDay(entry.end, out m, out d))
                {
                    good.Add(entry);
                }
            }
            parsed.calendar = good.ToArray();
            schedule = parsed;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private ServerEntry FindServerEntry(EventInfo info)
    {
        if (serverSchedule == null || info == null)
        {
            return null;
        }
        foreach (ServerEntry entry in serverSchedule.calendar)
        {
            if (entry.id.ToString() == info.eventID && entry.name == info.eventName)
            {
                return entry;
            }
        }
        return null;
    }

    private void GetEventDates(EventInfo info, out bool excluded, out int startMonth, out int startDay, out int endMonth, out int endDay)
    {
        excluded = info.excludeFromDateSystem;
        startMonth = info.startMonth;
        startDay = info.startDay;
        endMonth = info.endMonth;
        endDay = info.endDay;
        ServerEntry entry = ServerControlsParties ? FindServerEntry(info) : null;
        if (entry != null)
        {
            excluded = !entry.calendar;
            TryMonthDay(entry.start, out startMonth, out startDay);
            TryMonthDay(entry.end, out endMonth, out endDay);
        }
    }

    public void ForceStartParty()
    {
        if (events == null || events.Length == 0)
        {
            Debug.LogError("No events configured.");
            return;
        }

        overrideMode = OverrideMode.ForceEvent;

        string best = FindBestEventKeyToForce(DateTimeOffset.UtcNow, DateTime.UtcNow.Year);
        
        if (string.IsNullOrEmpty(best))
        {
            foreach (var e in events)
            {
                if (e != null && !e.excludeFromDateSystem)
                {
                    forcedEventKey = BuildEventKey(e);
                    break;
                }
            }
            if (string.IsNullOrEmpty(forcedEventKey) && events.Length > 0)
            {
                forcedEventKey = BuildEventKey(events[0]);
            }
        }
        else
        {
            forcedEventKey = best;
        }

        SaveOverrideState();
        ApplyEvents();
    }

    public void ForceEndParty()
    {
        overrideMode = OverrideMode.ForceRegular;
        SaveOverrideState();
        ApplyEvents();
    }

    public void SetDefaultMode()
    {
        overrideMode = OverrideMode.Automatic;
        SaveOverrideState();
        ApplyEvents();
    }

    public void ForceParty(string eventID, string eventName)
    {
        overrideMode = OverrideMode.ForceEvent;
        forcedEventKey = (eventID ?? "") + "|" + (eventName ?? "");
        SaveOverrideState();
        ApplyEvents();
    }

    public void ForcePartyKey(string partyKey)
    {
        if (string.IsNullOrEmpty(partyKey))
        {
            Debug.LogError("ForcePartyKey called with empty partyKey");
            return;
        }

        string targetKey = partyKey;

        if (events != null)
        {
            foreach (var e in events)
            {
                if (e == null)
                {
                    continue;
                }

                string key = BuildEventKey(e);
                if (key == partyKey)
                {
                    targetKey = key;
                    break;
                }

                if (!string.IsNullOrEmpty(e.eventName) && e.eventName == partyKey)
                {
                    targetKey = key;
                    break;
                }

                if (!string.IsNullOrEmpty(e.eventID) && e.eventID == partyKey)
                {
                    targetKey = key;
                    break;
                }
            }
        }

        overrideMode = OverrideMode.ForceEvent;
        forcedEventKey = targetKey;
        SaveOverrideState();
        ApplyEvents();
    }

    private void LoadOverrideState()
    {
        int mode = PlayerPrefs.GetInt(GetPlatformKey(PLAYERPREFS_OVERRIDE_MODE_KEY), 0);
        if (mode < 0 || mode > 2)
        {
            mode = 0;
        }
        overrideMode = (OverrideMode)mode;

        forcedEventKey = PlayerPrefs.GetString(GetPlatformKey(PLAYERPREFS_FORCED_EVENT_KEY), null);
        if (string.IsNullOrEmpty(forcedEventKey))
        {
            forcedEventKey = null;
        }
    }

    private void SaveOverrideState()
    {
        PlayerPrefs.SetInt(GetPlatformKey(PLAYERPREFS_OVERRIDE_MODE_KEY), (int)overrideMode);

        if (string.IsNullOrEmpty(forcedEventKey))
        {
            PlayerPrefs.DeleteKey(GetPlatformKey(PLAYERPREFS_FORCED_EVENT_KEY));
        }
        else
        {
            PlayerPrefs.SetString(GetPlatformKey(PLAYERPREFS_FORCED_EVENT_KEY), forcedEventKey);
        }

        PlayerPrefs.Save();
    }

    private string BuildEventKey(EventInfo info)
    {
        if (info == null)
        {
            return null;
        }
        string id = info.eventID ?? "";
        string name = info.eventName ?? "";
        return id + "|" + name;
    }

    private string FindBestEventKeyToForce(DateTimeOffset nowUtc, int currentYear)
    {
        if (events == null || events.Length == 0)
        {
            return null;
        }

        foreach (var eventInfo in events)
        {
            if (eventInfo == null)
            {
                continue;
            }

            if (eventInfo.excludeFromDateSystem)
            {
                continue;
            }

            int startYear = currentYear;
            int endYear = (eventInfo.endMonth < eventInfo.startMonth) ? currentYear + 1 : currentYear;

            DateTimeOffset startDateTime;
            DateTimeOffset endDateTime;

            try
            {
                startDateTime = new DateTimeOffset(startYear, eventInfo.startMonth, eventInfo.startDay, 0, 0, 0, TimeSpan.Zero);
                endDateTime = new DateTimeOffset(endYear, eventInfo.endMonth, eventInfo.endDay, 0, 0, 0, TimeSpan.Zero);
            }
            catch
            {
                continue;
            }

            bool active = nowUtc >= startDateTime && nowUtc < endDateTime;
            if (active)
            {
                return BuildEventKey(eventInfo);
            }
        }

        return BuildEventKey(events[0]);
    }

    private void ApplyEvents()
    {
        activeSceneKeys.Clear();
        seenScenes.Clear();

        bool server = ServerControlsParties;
        OverrideMode mode = overrideMode;
        string forced = forcedEventKey;
        if (server)
        {
            mode = serverSchedule.mode == "party" ? OverrideMode.ForceEvent : serverSchedule.mode == "none" ? OverrideMode.ForceRegular : OverrideMode.Automatic;
            forced = serverSchedule.mode == "party" ? serverSchedule.party : null;
        }

        DateTimeOffset currentDateUtc = CurrentTime();
        int currentYear = currentDateUtc.Year;
        string applied = (server ? "server" : "local") + " mode=" + mode + " party=" + (forced ?? "-");
        if (applied != lastApplied)
        {
            lastApplied = applied;
        }

        if (!server && mode == OverrideMode.ForceEvent && string.IsNullOrEmpty(forced) && events != null && events.Length > 0)
        {
            foreach (var e in events)
            {
                if (e != null && !e.excludeFromDateSystem)
                {
                    forcedEventKey = BuildEventKey(e);
                    break;
                }
            }
            if (string.IsNullOrEmpty(forcedEventKey))
            {
                forcedEventKey = BuildEventKey(events[0]);
            }
            forced = forcedEventKey;
            SaveOverrideState();
        }

        int forcedEventId = -1;
        if (mode == OverrideMode.ForceEvent && events != null)
        {
            foreach (var eventInfo in events)
            {
                int eventId;
                if (eventInfo != null && (BuildEventKey(eventInfo) == forced || eventInfo.eventID == forced || eventInfo.eventName == forced) && int.TryParse(eventInfo.eventID, out eventId))
                {
                    forcedEventId = eventId;
                    break;
                }
            }
        }
        LocalizedEventDateText.SetForcedEventId(forcedEventId);

        foreach (var eventInfo in events)
        {
            string currentEventKey = BuildEventKey(eventInfo);
            
            bool excluded;
            int evStartMonth, evStartDay, evEndMonth, evEndDay;
            GetEventDates(eventInfo, out excluded, out evStartMonth, out evStartDay, out evEndMonth, out evEndDay);

            if (!server && excluded && mode == OverrideMode.ForceEvent && currentEventKey != forced)
            {
                continue;
            }

            string path = $"definitions/scheduledeventdates/date_{eventInfo.eventID}_{eventInfo.eventName}";
            var eventAsset = Resources.Load<ScriptableObject>(path);
            
            if (eventAsset == null)
            {
                path = $"definitions/scheduledeventdates/holiday/date_{eventInfo.eventID}_{eventInfo.eventName}";
                eventAsset = Resources.Load<ScriptableObject>(path);
            }

            if (eventAsset != null)
            {
                var type = eventAsset.GetType();
                var datesField = type.GetField("Dates", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                var datesValue = datesField.GetValue(eventAsset);
                if (datesValue != null)
                {
                    var startDateField = datesValue.GetType().GetField("StartDate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var endDateField = datesValue.GetType().GetField("EndDate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    if (startDateField != null && endDateField != null)
                    {
                        var startDate = startDateField.GetValue(datesValue);
                        var endDate = endDateField.GetValue(datesValue);

                        if (startDate != null && endDate != null)
                        {
                            bool isEventActive = false;
                            
                            if (mode == OverrideMode.ForceEvent && currentEventKey == forced)
                            {
                                isEventActive = true;
                            }
                            else if (mode == OverrideMode.ForceRegular)
                            {
                                isEventActive = false;
                            }
                            else
                            {
                                isEventActive = false;
                            }
                            
                            int startYear, endYear;
                            int startMonth, startDay, endMonth, endDay;
                            
                            if (mode == OverrideMode.ForceEvent)
                            {
                                if (isEventActive)
                                {
                                    DateTime today = currentDateUtc.UtcDateTime;
                                    DateTime from = server ? today.AddDays(-1) : today;
                                    startYear = from.Year;
                                    startMonth = from.Month;
                                    startDay = from.Day;
                                    
                                    DateTime endDateCalc = today.AddDays(2);
                                    endYear = endDateCalc.Year;
                                    endMonth = endDateCalc.Month;
                                    endDay = endDateCalc.Day;
                                }
                                else
                                {
                                    startYear = 2024;
                                    startMonth = 1;
                                    startDay = 1;
                                    endYear = 2024;
                                    endMonth = 2;
                                    endDay = 1;
                                }
                            }
                            else if (mode == OverrideMode.ForceRegular)
                            {
                                startYear = 2024;
                                startMonth = 1;
                                startDay = 1;
                                endYear = 2024;
                                endMonth = 2;
                                endDay = 1;
                            }
                            else if (excluded)
                            {
                                startYear = 2024;
                                startMonth = 1;
                                startDay = 1;
                                endYear = 2024;
                                endMonth = 2;
                                endDay = 1;
                            }
                            else
                            {
                                startYear = currentYear;
                                endYear = (evEndMonth < evStartMonth) ? currentYear + 1 : currentYear;
                                startMonth = evStartMonth;
                                startDay = evStartDay;
                                endMonth = evEndMonth;
                                endDay = evEndDay;
                            }

                            SetFieldOrPropertyValue(startDate, "day", startDay);
                            SetFieldOrPropertyValue(startDate, "month", startMonth);
                            SetFieldOrPropertyValue(startDate, "year", startYear);

                            SetFieldOrPropertyValue(endDate, "day", endDay);
                            SetFieldOrPropertyValue(endDate, "month", endMonth);
                            SetFieldOrPropertyValue(endDate, "year", endYear);

                            DateTimeOffset startDateTime = new DateTimeOffset(startYear, startMonth, startDay, 0, 0, 0, TimeSpan.Zero);
                            DateTimeOffset endDateTime = new DateTimeOffset(endYear, endMonth, endDay, 0, 0, 0, TimeSpan.Zero);

                            long startTimestamp = startDateTime.ToUnixTimeMilliseconds();
                            long endTimestamp = endDateTime.ToUnixTimeMilliseconds();

                            SetFieldOrPropertyValue(startDate, "TimeStampInMilliseconds", startTimestamp);
                            SetFieldOrPropertyValue(endDate, "TimeStampInMilliseconds", endTimestamp);

                            bool scheduleActive = currentDateUtc >= startDateTime && currentDateUtc < endDateTime;

                            bool isActive;
                            if (mode == OverrideMode.ForceRegular)
                            {
                                isActive = false;
                            }
                            else if (mode == OverrideMode.ForceEvent)
                            {
                                isActive = (BuildEventKey(eventInfo) == forced);
                            }
                            else
                            {
                                isActive = scheduleActive;
                            }

                            foreach (var mapping in eventInfo.SceneAudioMappings)
                            {
                                if (isActive)
                                {
                                    activeSceneKeys[mapping.SceneName] = mapping.EventAudioKey;
                                }
                                else
                                {
                                    if (!activeSceneKeys.ContainsKey(mapping.SceneName))
                                    {
                                        activeSceneKeys[mapping.SceneName] = mapping.DefaultAudioKey;
                                    }
                                }
                                seenScenes.Add(mapping.SceneName);
                            }

                            if (eventInfo.SnowballPrefab != null)
                            {
                                var tr = eventInfo.SnowballPrefab.GetComponent<TrailRenderer>();
                                if (tr != null)
                                {
                                    tr.material = isActive ? eventInfo.EventMaterial : eventInfo.OriginalMaterial;
                                }
                            }

                        }
                    }
                }
            }
        }

        ApplySceneAudioKeys();
    }

    private void ApplySceneAudioKeys()
    {
        foreach (var sceneName in seenScenes)
        {
            string scenePath = $"definitions/scene/scene_{sceneName}";
            var sceneAsset = Resources.Load<ScriptableObject>(scenePath);

            if (sceneAsset != null)
            {
                var type = sceneAsset.GetType();
                var audioKeyField = type.GetField("SceneAudioContentKey", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (audioKeyField != null)
                {
                    string keyToUse = activeSceneKeys.ContainsKey(sceneName) ? activeSceneKeys[sceneName] : null;
                    if (!string.IsNullOrEmpty(keyToUse))
                    {
                        var contentKey = new PrefabContentKey(keyToUse);
                        contentKey.Key = keyToUse;
                        audioKeyField.SetValue(sceneAsset, contentKey);
                    }
                }
                else
                {
                    Debug.LogError($"SceneAudioContentKey field not found in scene '{sceneName}'");
                }
            }
            else
            {
                Debug.LogError($"Scene asset not found at path: {scenePath}");
            }
        }
    }

    private void SetFieldOrPropertyValue(object target, string name, object newValue)
    {
        var type = target.GetType();

        var field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(target, newValue);
            return;
        }

        var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (property != null && property.CanWrite)
        {
            property.SetValue(target, newValue);
            return;
        }

        Debug.LogError($"Field or property {name} not found in {type.Name}!");
    }
}

/* Example:
   Halloween Event
   Start Date: October 31 at midnight (00:00 UTC)
   End Date: November 2 at midnight (00:00 UTC)
   This means the event is active for the full day of October 31 and November 1.
*/
