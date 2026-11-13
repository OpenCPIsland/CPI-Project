using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FogZone : MonoBehaviour
{
    [Header("Target Collider (Trigger Zone)")]
    [Tooltip("Drag the trigger collider here.")]
    public Collider triggerCollider;

    [Header("Default Fog Settings (Restored on Exit)")]
    public bool defaultFogEnabled = true;
    public Color defaultFogColor = Color.gray;
    public FogMode defaultFogMode = FogMode.Exponential;
    public float defaultFogDensity = 0.01f;
    public float defaultLinearFogStart = 0f;
    public float defaultLinearFogEnd = 300f;

    [Header("Custom Fog Settings (Applied on Enter)")]
    public bool customFogEnabled = true;
    public Color customFogColor = Color.white;
    public FogMode customFogMode = FogMode.Exponential;
    public float customFogDensity = 0.02f;
    public float customLinearFogStart = 0f;
    public float customLinearFogEnd = 150f;

    void Awake()
    {
        if (triggerCollider == null)
            triggerCollider = GetComponent<Collider>();

        if (triggerCollider != null)
            triggerCollider.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyFog(
                customFogEnabled,
                customFogColor,
                customFogMode,
                customFogDensity,
                customLinearFogStart,
                customLinearFogEnd
            );
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyFog(
                defaultFogEnabled,
                defaultFogColor,
                defaultFogMode,
                defaultFogDensity,
                defaultLinearFogStart,
                defaultLinearFogEnd
            );
        }
    }

    private void ApplyFog(
        bool enabled, Color color, FogMode mode, float density,
        float startDistance, float endDistance)
    {
        RenderSettings.fog = enabled;
        RenderSettings.fogColor = color;
        RenderSettings.fogMode = mode;
        RenderSettings.fogDensity = density;
        RenderSettings.fogStartDistance = startDistance;
        RenderSettings.fogEndDistance = endDistance;
    }
}
