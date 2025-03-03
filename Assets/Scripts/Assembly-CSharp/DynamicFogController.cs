using UnityEngine;

public class DynamicFogController : MonoBehaviour
{
    // Time parameters for the day-night cycle
    public float dayCycleLengthHours = 0.0f;
    public float dayCycleLengthMinutes = 15.0f; // Set to 1 minute for testing
    public float dayCycleLengthSeconds = 0.0f;

    // Fog colors for each phase
    public Color dayFogColor = Color.white;
    public Color sunsetFogColor = new Color(1.0f, 0.5f, 0.0f);
    public Color nightFogColor = new Color(0.1f, 0.1f, 0.2f);
    public Color sunriseFogColor = new Color(1.0f, 0.7f, 0.3f);

    // Fog density for each phase
    public float dayFogDensity = 0.5f;
    public float sunsetFogDensity = 0.5f;
    public float nightFogDensity = 0.5f;
    public float sunriseFogDensity = 0.5f;

    // Fog start and end distances for each phase
    public float dayFogStart = 5.0f;
    public float dayFogEnd = 100.0f;
    public float sunsetFogStart = -35.0f;
    public float sunsetFogEnd = 120.0f;
    public float nightFogStart = 0.0f;
    public float nightFogEnd = 200.0f;
    public float sunriseFogStart = -35.0f;
    public float sunriseFogEnd = 120.0f;

    // GameObjects to control during certain times of the day
    public GameObject dayObject; // This object is enabled during the day
    public GameObject nightObject; // This object is disabled during the day

    // Internal time tracking
    private float totalCycleTimeSeconds;
    private float currentTime;

    void Start()
    {
        // Calculate total time for the day-night cycle in seconds
        totalCycleTimeSeconds = (dayCycleLengthHours * 3600.0f) + (dayCycleLengthMinutes * 60.0f) + dayCycleLengthSeconds;

        // Default to 24 hours if the total cycle time is set to 0
        if (totalCycleTimeSeconds == 0)
        {
            totalCycleTimeSeconds = 24.0f * 3600.0f; // Default 24 hours
        }

        // Ensure fog mode is set to Linear for control over start and end distances
        RenderSettings.fogMode = FogMode.Linear;

        // Set initial fog to day settings when the script starts
        RenderSettings.fogColor = dayFogColor;
        RenderSettings.fogDensity = dayFogDensity;
        RenderSettings.fogStartDistance = dayFogStart;
        RenderSettings.fogEndDistance = dayFogEnd;

        // Start with daytime (currentTime is 0, representing the start of the day)
        currentTime = 0;

        // Initially disable the nightObject because it's daytime
        if (nightObject != null)
        {
            nightObject.SetActive(false);
            Debug.Log("Night object is initially disabled.");
        }

        // Initially enable the dayObject because it's daytime
        if (dayObject != null)
        {
            dayObject.SetActive(true);
            Debug.Log("Day object is initially enabled.");
        }
    }

    void Update()
    {
        // Increment the current time based on real-time progress
        currentTime += Time.deltaTime;

        // Keep the time in the bounds of the cycle (0 to totalCycleTimeSeconds)
        currentTime = currentTime % totalCycleTimeSeconds;

        // Calculate the current time normalized between 0 and 1
        float timeNormalized = currentTime / totalCycleTimeSeconds;

        // Determine the current fog color, density, start/end distances based on the time
        if (timeNormalized < 0.25f) // Day phase
        {
            // Disable the nightObject during the day phase
            if (nightObject != null)
            {
                nightObject.SetActive(false);
            }

            // Enable the dayObject during the day phase
            if (dayObject != null)
            {
                dayObject.SetActive(true);
            }

            float t = timeNormalized / 0.25f;
            RenderSettings.fogColor = Color.Lerp(dayFogColor, sunsetFogColor, t);
            RenderSettings.fogDensity = Mathf.Lerp(dayFogDensity, sunsetFogDensity, t);
            RenderSettings.fogStartDistance = Mathf.Lerp(dayFogStart, sunsetFogStart, t);
            RenderSettings.fogEndDistance = Mathf.Lerp(dayFogEnd, sunsetFogEnd, t);
        }
        else if (timeNormalized < 0.5f) // Sunset phase
        {
            // Enable the nightObject during sunset
            if (nightObject != null)
            {
                nightObject.SetActive(true);
            }

            // Disable the dayObject during sunset
            if (dayObject != null)
            {
                dayObject.SetActive(false);
            }

            float t = (timeNormalized - 0.25f) / 0.25f;
            RenderSettings.fogColor = Color.Lerp(sunsetFogColor, nightFogColor, t);
            RenderSettings.fogDensity = Mathf.Lerp(sunsetFogDensity, nightFogDensity, t);
            RenderSettings.fogStartDistance = Mathf.Lerp(sunsetFogStart, nightFogStart, t);
            RenderSettings.fogEndDistance = Mathf.Lerp(sunsetFogEnd, nightFogEnd, t);
        }
        else if (timeNormalized < 0.75f) // Night phase
        {
            // Keep the nightObject enabled at night
            if (nightObject != null)
            {
                nightObject.SetActive(true);
            }

            // Disable the dayObject during night
            if (dayObject != null)
            {
                dayObject.SetActive(false);
            }

            float t = (timeNormalized - 0.5f) / 0.25f;
            RenderSettings.fogColor = Color.Lerp(nightFogColor, sunriseFogColor, t);
            RenderSettings.fogDensity = Mathf.Lerp(nightFogDensity, sunriseFogDensity, t);
            RenderSettings.fogStartDistance = Mathf.Lerp(nightFogStart, sunriseFogStart, t);
            RenderSettings.fogEndDistance = Mathf.Lerp(nightFogEnd, sunriseFogEnd, t);
        }
        else // Sunrise phase
        {
            // Disable the nightObject during sunrise
            if (nightObject != null)
            {
                nightObject.SetActive(false);
            }

            // Enable the dayObject during sunrise
            if (dayObject != null)
            {
                dayObject.SetActive(true);
            }

            float t = (timeNormalized - 0.75f) / 0.25f;
            RenderSettings.fogColor = Color.Lerp(sunriseFogColor, dayFogColor, t);
            RenderSettings.fogDensity = Mathf.Lerp(sunriseFogDensity, dayFogDensity, t);
            RenderSettings.fogStartDistance = Mathf.Lerp(sunriseFogStart, dayFogStart, t);
            RenderSettings.fogEndDistance = Mathf.Lerp(sunriseFogEnd, dayFogEnd, t);
        }
    }
}
