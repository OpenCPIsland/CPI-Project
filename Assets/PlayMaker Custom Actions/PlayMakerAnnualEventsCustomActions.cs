using UnityEngine;
using HutongGames.PlayMaker;
using Disney.Kelowna.Common;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

[ActionCategory("Events")]
public class SetEventInfo : FsmStateAction
{
    [RequiredField] public FsmString EventName;
    public FsmInt StartMonth;
    public FsmInt StartDay;
    public FsmInt EndMonth;
    public FsmInt EndDay;
    public FsmEvent NextState;

    // Use FSM variables to pass date information directly
    public static FsmInt EventStartMonth;
    public static FsmInt EventStartDay;
    public static FsmInt EventEndMonth;
    public static FsmInt EventEndDay;

    public override void OnEnter()
    {
        // Set FSM variables directly
        EventStartMonth = StartMonth;
        EventStartDay = StartDay;
        EventEndMonth = EndMonth;
        EventEndDay = EndDay;

        int currentYear = DateTime.Now.Year;
        Debug.Log($"Event: {EventName.Value}, Start: {StartMonth.Value}/{StartDay.Value}/{currentYear} 00:00:00, End: {EndMonth.Value}/{EndDay.Value} {currentYear} 00:00:00");
        Fsm.Event(NextState);
        Finish();
    }
}

[ActionCategory("Events")]
public class UpdateSceneAudioContentKeys : FsmStateAction
{
    [RequiredField][ArrayEditor(VariableType.String)] public FsmArray SceneNames;
    [RequiredField][ArrayEditor(VariableType.String)] public FsmArray DefaultAudioKeys;
    [RequiredField][ArrayEditor(VariableType.String)] public FsmArray EventAudioKeys;

    public override void OnEnter()
    {
        if (SceneNames.Length != DefaultAudioKeys.Length || SceneNames.Length != EventAudioKeys.Length)
        {
            Debug.LogError("Array lengths do not match! Ensure SceneNames, DefaultAudioKeys, and EventAudioKeys have the same length.");
            Finish();
            return;
        }

        int currentYear = DateTime.Now.Year;
        DateTime currentDate = DateTime.Now;

        DateTime eventStartDate = new DateTime(currentYear, SetEventInfo.EventStartMonth.Value, SetEventInfo.EventStartDay.Value, 0, 0, 0);
        DateTime eventEndDate = new DateTime(currentYear, SetEventInfo.EventEndMonth.Value, SetEventInfo.EventEndDay.Value, 0, 0, 0);

        for (int i = 0; i < SceneNames.Length; i++)
        {
            string sceneName = SceneNames.Get(i).ToString().Trim();
            string defaultAudioKey = DefaultAudioKeys.Get(i).ToString().Trim();
            string eventAudioKey = EventAudioKeys.Get(i).ToString().Trim();
            string scenePath = $"definitions/scene/Scene_{sceneName}";

            var sceneDefinition = Resources.Load<ScriptableObject>(scenePath);
            if (sceneDefinition != null)
            {
                var type = sceneDefinition.GetType();
                var audioKeyField = type.GetField("SceneAudioContentKey");
                if (audioKeyField != null)
                {
                    if (currentDate >= eventStartDate && currentDate < eventEndDate.AddDays(1))
                    {
                        audioKeyField.SetValue(sceneDefinition, new PrefabContentKey(eventAudioKey));
                        Debug.Log($"✅ Set event audio key for scene: {sceneName}");
                    }
                    else
                    {
                        audioKeyField.SetValue(sceneDefinition, new PrefabContentKey(defaultAudioKey));
                        Debug.Log($"✅ Set default audio key for scene: {sceneName}");
                    }
                }
                else
                {
                    Debug.LogError($"❌ Audio key field not found in {sceneName}!");
                }
            }
            else
            {
                Debug.LogError($"❌ Scene asset not found at path: {scenePath}!");
            }
        }

        Finish();
    }
}

[ActionCategory("Events")]
public class ManageAdditiveScenes : FsmStateAction
{
    [RequiredField, ArrayEditor(VariableType.String)] public FsmArray SceneNames;
    public FsmBool LoadScenes;
    public FsmBool AllowRandomScene;
    public FsmBool AllowRandomPrefab;
    [ArrayEditor(VariableType.GameObject)] public FsmArray Prefabs;
    [ArrayEditor(VariableType.Vector3)] public FsmArray SpawnPositions;

    public override void OnEnter()
    {
        int currentYear = DateTime.Now.Year;
        DateTime currentDate = DateTime.Now;

        DateTime eventStartDate = new DateTime(currentYear, SetEventInfo.EventStartMonth.Value, SetEventInfo.EventStartDay.Value, 0, 0, 0);
        DateTime eventEndDate = new DateTime(currentYear, SetEventInfo.EventEndMonth.Value, SetEventInfo.EventEndDay.Value, 0, 0, 0);

        List<string> scenesToLoad = new List<string>();
        if (AllowRandomScene.Value && SceneNames.Length > 0)
        {
            string randomScene = SceneNames.Get(UnityEngine.Random.Range(0, SceneNames.Length)).ToString();
            scenesToLoad.Add(randomScene);
        }
        else
        {
            scenesToLoad.AddRange(Array.ConvertAll(SceneNames.Values, s => s.ToString()));
        }

        foreach (string sceneName in scenesToLoad)
        {
            if (LoadScenes.Value && currentDate >= eventStartDate && currentDate < eventEndDate.AddDays(1))
            {
                Debug.Log($"Loading scene additively: {sceneName}");
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            }
            else
            {
                Debug.Log($"Unloading additive scene: {sceneName}");
                SceneManager.UnloadSceneAsync(sceneName);
            }
        }

        if (Prefabs.Length > 0 && LoadScenes.Value && currentDate >= eventStartDate && currentDate < eventEndDate.AddDays(1))
        {
            int prefabCount = AllowRandomPrefab.Value ? 1 : Prefabs.Length;
            for (int i = 0; i < prefabCount; i++)
            {
                GameObject prefabToSpawn = AllowRandomPrefab.Value
                    ? (GameObject)Prefabs.Get(UnityEngine.Random.Range(0, Prefabs.Length))
                    : (GameObject)Prefabs.Get(i);

                Vector3 spawnPosition = (SpawnPositions.Length > i) ? (Vector3)SpawnPositions.Get(i) : Vector3.zero;
                GameObject spawnedPrefab = GameObject.Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
                Debug.Log($"Spawned prefab: {spawnedPrefab.name} at position: {spawnPosition}");
            }
        }

        Finish();
    }
}
