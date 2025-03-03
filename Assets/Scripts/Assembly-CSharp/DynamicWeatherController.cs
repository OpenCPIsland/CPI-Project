using UnityEngine;
using System.Collections; // This directive allows IEnumerator and coroutines

public class DynamicWeatherController : MonoBehaviour
{
    // Optional Particle systems for snow and rain
    public ParticleSystem snowParticleSystem; // Particle system for snow (optional)
    public ParticleSystem rainParticleSystem;  // Particle system for rain (optional)

    // Optional GameObjects for snow and rain
    public GameObject snowGameObject; // GameObject for snow (optional)
    public GameObject rainGameObject;  // GameObject for rain (optional)

    // AudioClip for rain sound (selectable from project assets)
    public AudioClip rainAudioClip; // AudioClip for rain sound (optional)
    private AudioSource rainAudioSource; // AudioSource for playing the rain sound

    // AudioClips for wind sounds (selectable from project assets)
    public AudioClip windLightAudioClip; // AudioClip for light wind sound (optional)
    public AudioClip windHighAudioClip;  // AudioClip for high wind sound (optional)
    private AudioSource windLightAudioSource; // AudioSource for playing light wind sound
    private AudioSource windHighAudioSource;  // AudioSource for playing high wind sound

    // Time parameters for snow, rain, and wind durations
    public float minWeatherDurationSeconds = 5.0f; // Minimum duration for any effect
    public float maxWeatherDurationSeconds = 30.0f; // Maximum duration for any effect

    // Time parameters for random delays before effects start
    public float minDelayBetweenEffects = 5.0f; // Minimum delay before starting a new effect
    public float maxDelayBetweenEffects = 20.0f; // Maximum delay before starting a new effect

    // Volume Controls
    public float rainVolume = 1.0f; // Default to max volume
    public float windLightVolume = 1.0f; // Default to max volume
    public float windHighVolume = 1.0f; // Default to max volume

    // Coroutine tracking for each effect
    private Coroutine rainCoroutine;
    private Coroutine snowCoroutine;
    private Coroutine windLightCoroutine;
    private Coroutine windHighCoroutine;

    void Start()
    {
        // Initialize AudioSource components for sounds
        if (rainAudioClip != null)
        {
            rainAudioSource = gameObject.AddComponent<AudioSource>();
            rainAudioSource.clip = rainAudioClip;
            rainAudioSource.loop = true; // Loop the sound
            rainAudioSource.volume = rainVolume; // Set initial volume
        }

        if (windLightAudioClip != null)
        {
            windLightAudioSource = gameObject.AddComponent<AudioSource>();
            windLightAudioSource.clip = windLightAudioClip;
            windLightAudioSource.loop = true; // Loop the sound
            windLightAudioSource.volume = windLightVolume; // Set initial volume
        }

        if (windHighAudioClip != null)
        {
            windHighAudioSource = gameObject.AddComponent<AudioSource>();
            windHighAudioSource.clip = windHighAudioClip;
            windHighAudioSource.loop = true; // Loop the sound
            windHighAudioSource.volume = windHighVolume; // Set initial volume
        }

        // Start the weather effect coroutines
        rainCoroutine = StartCoroutine(HandleRainEffect());
        snowCoroutine = StartCoroutine(HandleSnowEffect());
        windLightCoroutine = StartCoroutine(HandleWindLightEffect());
        windHighCoroutine = StartCoroutine(HandleWindHighEffect());
    }

    // Handle random rain effects
    private IEnumerator HandleRainEffect()
    {
        while (true)
        {
            // Wait for a random delay before starting the rain
            float delay = Random.Range(minDelayBetweenEffects, maxDelayBetweenEffects);
            yield return new WaitForSeconds(delay);

            // Activate rain effect
            ActivateRainEffect();
            
            // Wait for a random duration while the rain plays
            float rainDuration = Random.Range(minWeatherDurationSeconds, maxWeatherDurationSeconds);
            yield return new WaitForSeconds(rainDuration);

            // Stop rain effect
            StopRainEffect();
        }
    }

    // Handle random snow effects
    private IEnumerator HandleSnowEffect()
    {
        while (true)
        {
            // Wait for a random delay before starting the snow
            float delay = Random.Range(minDelayBetweenEffects, maxDelayBetweenEffects);
            yield return new WaitForSeconds(delay);

            // Activate snow effect
            ActivateSnowEffect();

            // Wait for a random duration while the snow plays
            float snowDuration = Random.Range(minWeatherDurationSeconds, maxWeatherDurationSeconds);
            yield return new WaitForSeconds(snowDuration);

            // Stop snow effect
            StopSnowEffect();
        }
    }

    // Handle random light wind effects
    private IEnumerator HandleWindLightEffect()
    {
        while (true)
        {
            // Wait for a random delay before starting the light wind
            float delay = Random.Range(minDelayBetweenEffects, maxDelayBetweenEffects);
            yield return new WaitForSeconds(delay);

            // Activate light wind effect
            ActivateLightWindEffect();

            // Wait for a random duration while the light wind plays
            float windLightDuration = Random.Range(minWeatherDurationSeconds, maxWeatherDurationSeconds);
            yield return new WaitForSeconds(windLightDuration);

            // Stop light wind effect
            StopLightWindEffect();
        }
    }

    // Handle random high wind effects
    private IEnumerator HandleWindHighEffect()
    {
        while (true)
        {
            // Wait for a random delay before starting the high wind
            float delay = Random.Range(minDelayBetweenEffects, maxDelayBetweenEffects);
            yield return new WaitForSeconds(delay);

            // Activate high wind effect
            ActivateHighWindEffect();

            // Wait for a random duration while the high wind plays
            float windHighDuration = Random.Range(minWeatherDurationSeconds, maxWeatherDurationSeconds);
            yield return new WaitForSeconds(windHighDuration);

            // Stop high wind effect
            StopHighWindEffect();
        }
    }

    // Rain effect activation and stopping
    private void ActivateRainEffect()
    {
        if (rainParticleSystem != null) rainParticleSystem.Play();
        if (rainGameObject != null) rainGameObject.SetActive(true);
        if (rainAudioSource != null && !rainAudioSource.isPlaying)
        {
            rainAudioSource.volume = rainVolume;
            rainAudioSource.Play();
        }
        Debug.Log("Rain started.");
    }

    private void StopRainEffect()
    {
        if (rainParticleSystem != null) rainParticleSystem.Stop();
        if (rainGameObject != null) rainGameObject.SetActive(false);
        if (rainAudioSource != null && rainAudioSource.isPlaying)
        {
            rainAudioSource.Stop();
        }
        Debug.Log("Rain stopped.");
    }

    // Snow effect activation and stopping
    private void ActivateSnowEffect()
    {
        if (snowParticleSystem != null) snowParticleSystem.Play();
        if (snowGameObject != null) snowGameObject.SetActive(true);
        Debug.Log("Snow started.");
    }

    private void StopSnowEffect()
    {
        if (snowParticleSystem != null) snowParticleSystem.Stop();
        if (snowGameObject != null) snowGameObject.SetActive(false);
        Debug.Log("Snow stopped.");
    }

    // Light wind effect activation and stopping
    private void ActivateLightWindEffect()
    {
        if (windLightAudioSource != null && !windLightAudioSource.isPlaying)
        {
            windLightAudioSource.volume = windLightVolume;
            windLightAudioSource.Play();
        }
        Debug.Log("Light wind started.");
    }

    private void StopLightWindEffect()
    {
        if (windLightAudioSource != null && windLightAudioSource.isPlaying)
        {
            windLightAudioSource.Stop();
        }
        Debug.Log("Light wind stopped.");
    }

    // High wind effect activation and stopping
    private void ActivateHighWindEffect()
    {
        if (windHighAudioSource != null && !windHighAudioSource.isPlaying)
        {
            windHighAudioSource.volume = windHighVolume;
            windHighAudioSource.Play();
        }
        Debug.Log("High wind started.");
    }

    private void StopHighWindEffect()
    {
        if (windHighAudioSource != null && windHighAudioSource.isPlaying)
        {
            windHighAudioSource.Stop();
        }
        Debug.Log("High wind stopped.");
    }

    // Public methods to adjust volume dynamically during runtime
    public void SetRainVolume(float volume)
    {
        rainVolume = volume;
        if (rainAudioSource != null) rainAudioSource.volume = rainVolume;
    }

    public void SetWindLightVolume(float volume)
    {
        windLightVolume = volume;
        if (windLightAudioSource != null) windLightAudioSource.volume = windLightVolume;
    }

    public void SetWindHighVolume(float volume)
    {
        windHighVolume = volume;
        if (windHighAudioSource != null) windHighAudioSource.volume = windHighVolume;
    }
}
