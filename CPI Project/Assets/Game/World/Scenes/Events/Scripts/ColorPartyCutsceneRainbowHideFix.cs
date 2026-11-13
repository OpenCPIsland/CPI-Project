using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ColorPartyCutsceneRainbowHideFix : MonoBehaviour
{
    [Header("Cutscene Scene Name")]
    [SerializeField] private string cutsceneSceneName = "ColorParty2018_Town_Cutscene01";

    private List<Renderer> renderers = new();
    private List<SkinnedMeshRenderer> skinnedRenderers = new();
    private List<SpriteRenderer> spriteRenderers = new();
    private List<CanvasGroup> canvasGroups = new();

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        renderers.AddRange(GetComponentsInChildren<Renderer>(true));
        skinnedRenderers.AddRange(GetComponentsInChildren<SkinnedMeshRenderer>(true));
        spriteRenderers.AddRange(GetComponentsInChildren<SpriteRenderer>(true));
        canvasGroups.AddRange(GetComponentsInChildren<CanvasGroup>(true));
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == cutsceneSceneName)
            {
                ApplyVisibility(false);
                break;
            }
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == cutsceneSceneName)
        {
            ApplyVisibility(false);
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if (scene.name == cutsceneSceneName)
        {
            ApplyVisibility(true);
        }
    }

    private void ApplyVisibility(bool visible)
    {
        foreach (var r in renderers) if (r != null) r.enabled = visible;
        foreach (var r in skinnedRenderers) if (r != null) r.enabled = visible;
        foreach (var s in spriteRenderers) if (s != null) s.enabled = visible;
        foreach (var c in canvasGroups) if (c != null) c.alpha = visible ? 1f : 0f;
    }
}
