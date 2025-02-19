using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq; // For checking running processes (used later)

public class DiscordController : MonoBehaviour
{
    private const long clientId = 1235329861980127343; // Your actual Client ID
    private Discord.Discord discord;
    private Discord.ActivityManager activityManager;
    private static long gameStartTime; // Store the start time of the entire game

    [System.Serializable]
    public class SceneNameMapping
    {
        public string sceneName;
        public string displayName;
    }

    public SceneNameMapping[] customSceneNames;

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        // Check if Discord is running before initializing
        if (IsDiscordRunning())
        {
            try
            {
                discord = new Discord.Discord(clientId, (UInt64)Discord.CreateFlags.Default);
                activityManager = discord.GetActivityManager();

                if (gameStartTime == 0)
                    gameStartTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); // Set only once on game start

                UpdatePresence("Loading...", "default_icon", string.Empty, $"Unity {Application.unityVersion} | Version {Application.version}", gameStartTime);

                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to initialize Discord SDK: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("Discord is not running or installed. Skipping Discord integration.");
        }
    }

    void Update()
    {
        // Only run callbacks if Discord is initialized
        if (discord != null)
        {
            discord?.RunCallbacks();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string roomName = GetCustomSceneName(scene.name);
        SetRoom(roomName);
    }

    private string GetCustomSceneName(string sceneName)
    {
        foreach (var mapping in customSceneNames)
        {
            if (mapping.sceneName == sceneName)
            {
                return mapping.displayName;
            }
        }

        return sceneName;
    }

    public void SetRoom(string roomName)
    {
        string roomImage = "default_icon";
        string detailsText = roomName;
        string stateText = $"Unity {Application.unityVersion} | Version {Application.version}";

        bool iconFound = false;

        if (roomName.Contains("Island Central | Halloween", StringComparison.OrdinalIgnoreCase))
        {
            roomImage = "town_halloween";
            iconFound = true;
        }
        else if (roomName.Contains("Island Central | Rainbow", StringComparison.OrdinalIgnoreCase))
        {
            roomImage = "rainbow_town";
            iconFound = true;
        }
        else if (roomName.Contains("Halloween", StringComparison.OrdinalIgnoreCase))
        {
            roomImage = "halloween";
            iconFound = true;
        }
        else if (roomName.Contains("Holiday", StringComparison.OrdinalIgnoreCase))
        {
            // Random holiday icons for any Holiday event
            string[] holidayIcons = { "holiday", "cfc", "olaf" };
            roomImage = holidayIcons[new System.Random().Next(holidayIcons.Length)];
            iconFound = true;
        }
        else if (roomName.Contains("Rainbow Migration", StringComparison.OrdinalIgnoreCase))
        {
            roomImage = "rainbow";
            iconFound = true;
        }
        else if (roomName.Contains("Anniversary", StringComparison.OrdinalIgnoreCase))
        {
            roomImage = "anniversary";
            iconFound = true;
        }
        else if (roomName.Contains("Valentines", StringComparison.OrdinalIgnoreCase))
        {
            roomImage = "valentines";
            iconFound = true;
        }
        else if (roomName.Contains("Arcade", StringComparison.OrdinalIgnoreCase) ||
                 roomName.Contains("Bean", StringComparison.OrdinalIgnoreCase) ||
                 roomName.Contains("Ice", StringComparison.OrdinalIgnoreCase) ||
                 roomName.Contains("Jetpack", StringComparison.OrdinalIgnoreCase) ||
                 roomName.Contains("Roundup", StringComparison.OrdinalIgnoreCase) ||
                 roomName.Contains("Pizzatron", StringComparison.OrdinalIgnoreCase) ||
                 roomName.Contains("Smoothie", StringComparison.OrdinalIgnoreCase))
        {
            roomImage = "arcade";
            iconFound = true;
        }
        else if (roomName.Contains("April", StringComparison.OrdinalIgnoreCase) ||
                 roomName.Contains("???", StringComparison.OrdinalIgnoreCase))
        {
            roomImage = "box_dimension";
            iconFound = true;
        }
        else if (roomName.Contains("Waddle", StringComparison.OrdinalIgnoreCase))
        {
            roomImage = "sunset";
            iconFound = true;
        }
        else if (roomName.Contains("Splash", StringComparison.OrdinalIgnoreCase))
        {
            roomImage = "summersplash";
            iconFound = true;
        }
        else if (roomName.Contains("World", StringComparison.OrdinalIgnoreCase))
        {
            roomImage = "wpd";
            iconFound = true;
        }
        else if (roomName.Contains("Medieval", StringComparison.OrdinalIgnoreCase))
        {
            roomImage = "medieval";
            iconFound = true;
        }
        else if (roomName.Contains("Credits", StringComparison.OrdinalIgnoreCase))
        {
            roomImage = "credits";
            iconFound = true;
        }
        else if (roomName.Contains("Penglantian", StringComparison.OrdinalIgnoreCase) ||
                 roomName.Contains("Expedition", StringComparison.OrdinalIgnoreCase))
        {
            roomImage = "penglantian";
            iconFound = true;
        }
        else if (roomName.Contains("Igloo", StringComparison.OrdinalIgnoreCase))
        {
            roomImage = "igloo";
            iconFound = true;
        }
        else if (roomName.Contains("Summit", StringComparison.OrdinalIgnoreCase))
        {
            roomImage = "summit";
            iconFound = true;
        }

        // Monthly icon check (only if no special event icon is detected)
        int currentMonth = DateTime.Now.Month;
        if (!iconFound)
        {
            switch (currentMonth)
            {
                case 2:
                    roomImage = "valentines";
                    iconFound = true;
                    Debug.Log($"[Discord RPC] Current month is February (Month: {currentMonth}). Using 'valentines' icon for room '{roomName}'.");
                    break;
                case 10:
                    roomImage = "halloween";
                    iconFound = true;
                    Debug.Log($"[Discord RPC] Current month is October (Month: {currentMonth}). Using 'halloween' icon for room '{roomName}'.");
                    break;
                case 12:
                    // Random holiday icons for December
                    string[] holidayIcons = { "holiday", "cfc", "olaf" };
                    roomImage = holidayIcons[new System.Random().Next(holidayIcons.Length)];
                    iconFound = true;
                    Debug.Log($"[Discord RPC] Current month is December (Month: {currentMonth}). Using 'holiday' icon for room '{roomName}'.");
                    break;
                default:
                    roomImage = "default_icon";
                    iconFound = true;
                    Debug.Log($"[Discord RPC] Current month is {currentMonth}. Using 'default_icon' for room '{roomName}'.");
                    break;
            }
        }

        if (iconFound)
        {
            Debug.Log($"[Discord RPC] Icon FOUND for room '{roomName}': {roomImage}");
        }
        else
        {
            Debug.Log($"[Discord RPC] Icon NOT FOUND for room '{roomName}', using default icon.");
        }

        // Only update Discord presence if Discord is initialized
        if (discord != null)
        {
            UpdatePresence(stateText, roomImage, detailsText, string.Empty, gameStartTime);
        }
    }

    private void UpdatePresence(string state, string imageKey, string details, string party, long startTimestamp)
    {
        var activity = new Discord.Activity
        {
            State = state,
            Details = details,
            Assets =
            {
                LargeImage = imageKey,
                LargeText = state
            },
            Timestamps =
            {
                Start = startTimestamp // Game start time remains constant
            }
        };

        activityManager.UpdateActivity(activity, result =>
        {
            if (result == Discord.Result.Ok)
                Debug.Log($"Discord RPC Updated: {state} - {details} (Game started at: {startTimestamp})");
            else
                Debug.LogError("Failed to update Discord RPC.");
        });
    }

    void OnApplicationQuit()
    {
        // Only dispose of discord if it was initialized
        // if (discord != null)
        {
            discord.Dispose();
        }
    }

    // Check if Discord is running on the system
    private bool IsDiscordRunning()
    {
        try
        {
            var processes = System.Diagnostics.Process.GetProcessesByName("Discord");
            return processes.Any(); // If any Discord process is found, it's running
        }
        catch (Exception ex)
        {
            Debug.LogError("Error checking Discord status: " + ex.Message);
            return false;
        }
    }
}
