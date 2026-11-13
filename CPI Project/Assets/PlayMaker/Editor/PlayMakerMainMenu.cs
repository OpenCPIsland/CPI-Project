// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

#define PM_POOL

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using HutongGames.PlayMakerEditor;
using UnityEditor;
using UnityEngine;

#if UNITY_2019_1_OR_NEWER
using UnityEditor.ShortcutManagement;
#endif

[Localizable(false)]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
internal static class PlayMakerMainMenu
{
    // Change MenuRoot to move the Playmaker Menu
    // E.g., MenuRoot = "Plugins/PlayMaker/"
    private const string MenuRoot = "PlayMaker/"; 

	[MenuItem(MenuRoot + "PlayMaker Editor", false, 0)]
	public static void OpenFsmEditor()
	{
		FsmEditorWindow.OpenWindow();
	    if (EditorStartupPrefs.ShowWelcomeScreen)
	    {
	        PlayMakerWelcomeWindow.Open();
	    }
	}

	#region EDITOR WINDOWS 

    private const string editorsRoot = MenuRoot + "Editor Windows/";
    private const int iEditors = 1; //10;

    [MenuItem(editorsRoot + "Action Browser", false, iEditors)]
    public static void OpenActionWindow()
    {
        FsmEditorWindow.OpenToolWindow<FsmActionWindow>();
    }

    [MenuItem(editorsRoot + "State Browser", false, iEditors + 1)]
    public static void OpenStateSelectorWindow()
    {
        FsmEditorWindow.OpenToolWindow<FsmStateWindow>();
    }

	[MenuItem(editorsRoot + "FSM Browser", false, iEditors + 2)]
	public static void OpenFsmSelectorWindow()
	{
        FsmEditorWindow.OpenToolWindow<FsmSelectorWindow>();
	}

    [MenuItem(editorsRoot + "Templates Browser", false, iEditors + 3)]
	public static void OpenFsmTemplateWindow()
	{
        FsmEditorWindow.OpenToolWindow<FsmTemplateWindow>();
	}

    [MenuItem(editorsRoot + "Event Browser", false, iEditors + 4)]
    public static void OpenGlobalEventsWindow()
    {
        FsmEditorWindow.OpenToolWindow<FsmEventsWindow>();
    }

    [MenuItem(editorsRoot + "Global Variables", false, iEditors + 5)]
	public static void OpenGlobalVariablesWindow()
	{
        FsmEditorWindow.OpenToolWindow<FsmGlobalsWindow>();
	}

    #if PM_POOL && PLAYMAKER_SOURCE
    
	[MenuItem(editorsRoot + "Pool Browser", false, iEditors + 6)]
	public static void OpenPoolBrowserWindow()
	{
		FsmEditorWindow.OpenToolWindow<PoolBrowser>();
	}

    #endif

    [MenuItem(editorsRoot + "FSM Controls", false, iEditors + 6)]
    public static void OpenFsmControlsWindow()
    {
        FsmEditorWindow.OpenToolWindow<FsmControlsWindow>();
    }

    [MenuItem(editorsRoot + "Edit Tools", false, iEditors + 6)]
    public static void OpenToolWindow()
    {
        FsmEditorWindow.OpenToolWindow<ContextToolWindow>();
    }

    // -----------------------------------------

    [MenuItem(editorsRoot + "Timeline Log", false, iEditors + 17)]
    public static void OpenTimelineWindow()
    {
        FsmEditorWindow.OpenToolWindow<FsmTimelineWindow>();
    }

    [MenuItem(editorsRoot + "FSM Log", false, iEditors + 18)]
	public static void OpenFsmLogWindow()
	{
        FsmEditorWindow.OpenToolWindow<FsmLogWindow>();
	}

    [MenuItem(editorsRoot + "Editor Log", false, iEditors + 29)]
	public static void OpenReportWindow()
	{
        FsmEditorWindow.OpenToolWindow<ReportWindow>();
	}

    #endregion

	#region COMPONENTS

    private const int iComponents = 1;  // iEditors + 10;

	[MenuItem(MenuRoot + "Components/Add FSM To Selected Objects", true)]
	public static bool ValidateAddFsmToSelected()
	{
		return Selection.activeGameObject != null;
	}

	[MenuItem(MenuRoot + "Components/Add FSM To Selected Objects", false, iComponents)]
	public static void AddFsmToSelected()
	{
		FsmBuilder.AddFsmToSelected();
		//PlayMakerFSM playmakerFSM = Selection.activeGameObject.AddComponent<PlayMakerFSM>();
		//FsmEditor.SelectFsm(playmakerFSM.Fsm);
	}

	[MenuItem(MenuRoot + "Components/Add PlayMakerGUI to Scene", true)]
	public static bool ValidateAddPlayMakerGUI()
	{
#if UNITY_2022_3_OR_NEWER
        return Object.FindFirstObjectByType<PlayMakerGUI>() == null;
#else
        return Object.FindObjectOfType(typeof(PlayMakerGUI)) as PlayMakerGUI == null;
#endif
	}

    [MenuItem(MenuRoot + "Components/Add PlayMakerGUI to Scene", false, iComponents + 1)]
	public static void AddPlayMakerGUI()
	{
		PlayMakerGUI.Instance.enabled = true;
	}



#if PM2    
    
    [MenuItem(MenuRoot + "Components/Add PlayMakerGlobals Asset to Project")]
    public static void CreatePlayMakerGlobals()
    {
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<PlayMakerGlobals>(),"Assets/PlayMakerGlobals.asset");
    }

#if PM_SOURCE

    [MenuItem(MenuRoot + "Experimental/Undefine PM2")]
    public static void UnDefinePM2()
    {
        PlayMakerDefines.RemoveScriptingDefineSymbolFromAllTargets("PM2");
    }

#endif

#else

#if PM_SOURCE

    [MenuItem(MenuRoot + "Experimental/Define PM2")]
    public static void DefinePM2()
    {
        PlayMakerDefines.AddScriptingDefineSymbolToAllTargets("PM2");
    }

#endif

#endif


	#endregion

	#region TOOLS

    private const string toolsRoot = MenuRoot + "Tools/";
    private const int iTools = iComponents;// + 10;

    [MenuItem(toolsRoot + "Export Globals", false, iTools)]
    public static void ExportGlobals()
    {
        GlobalsAsset.Export();
    }

    [MenuItem(toolsRoot + "Import Globals", false, iTools + 1)]
    public static void ImportGlobals()
    {
        GlobalsAsset.Import();
    }

    [MenuItem(toolsRoot + "Custom Action Wizard", false, iTools + 12)]
    public static PlayMakerCustomActionWizard CreateWizard()
    {
        return EditorWindow.GetWindow<PlayMakerCustomActionWizard>(true, "Custom Action Wizard");
    }

    [MenuItem(toolsRoot + "Documentation Helpers", false, iTools + 13)]
    public static void DocHelpers()
    {
        EditorWindow.GetWindow<PlayMakerDocHelpers>(true, "Doc Helpers");
    }

    /* In PlayMakerProjectTools.cs
    [MenuItem(toolsRoot + "Update All Loaded FSMs", false, iTools + 24)]
    public static void UpdateAllLoadedFSMs()
    {
        ProjectTools.ReSaveAllLoadedFSMs();
    }

    [MenuItem(toolsRoot + "Update All FSMs in Build", false, iTools + 25)]
    public static void UpdateAllFSMsInBuild()
    {
        ProjectTools.UpdateScenesInBuild();
    }*/

    [MenuItem(toolsRoot + "Load All PlayMaker Prefabs", false, iTools + 25)]
    public static void LoadAllPrefabsInProject()
    {
        var paths = Files.LoadAllPlaymakerPrefabs();

        if (paths.Count == 0)
        {
            EditorUtility.DisplayDialog("Loading PlayMaker Prefabs", "No PlayMaker Prefabs Found!", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Loaded PlayMaker Prefabs", "Prefabs found: " + paths.Count + "\nCheck console for details...", "OK");
        }
    }

    [MenuItem(toolsRoot + "Submit Bug Report", false,  iTools + 86)]
    public static PlayMakerBugReportWindow SubmitBug()
    {
        return EditorWindow.GetWindow<PlayMakerBugReportWindow>(true, "Bug Report Window");
    }

#if UNITY_5_0 || UNITY_5
    [MenuItem(toolsRoot + "Post-Update Check", false, 67)]
    public static void RunAutoUpdater()
    {
        PlayMakerAutoUpdater.OpenAutoUpdater();
    }
#endif

	#endregion

	#region HELP

    private const string helpRoot = MenuRoot + "Help/";
    private const int iHelp = 1; // iTools + 100;

    [MenuItem(helpRoot + "Guided Tour", false, iHelp)]
    public static void GuidedTour()
    {
        PlayMakerGuidedTour.Open();
    }

    [MenuItem(helpRoot + "Online Manual", false, iHelp + 1)]
	public static void OnlineManual()
	{
		EditorCommands.OpenWikiHelp();
	}

    [MenuItem(helpRoot + "YouTube Channel", false, iHelp + 2)]
	public static void YouTubeChannel()
	{
		Application.OpenURL("http://www.youtube.com/user/HutongGamesLLC");
	}

    [MenuItem(helpRoot + "PlayMaker Forums", false, iHelp + 3)]
	public static void PlayMakerForum()
	{
		Application.OpenURL("http://hutonggames.com/playmakerforum/");
	}

    /*
    [MenuItem(helpRoot + "Check For Updates", false, iHelp + 3)]
    public static void CheckForUpdates()
    {
        AssetStore.Open("content/368");
    }*/


    [MenuItem(helpRoot + "Submit Bug Report", false,  iHelp + 20)]
    public static void SubmitBugFromHelp()
    {
        EditorWindow.GetWindow<PlayMakerBugReportWindow>(true);
    }

    [MenuItem(helpRoot + "About PlayMaker...", false, iHelp + 40)]
    public static void OpenAboutWindow()
    {
        EditorWindow.GetWindow<AboutWindow>(true, "About PlayMaker");
    }

    #endregion


#if PM2

    [MenuItem(MenuRoot + "Notes", false, iHelp + 1)]
    public static void Notes()
    {
        PlayMakerNotesWindow.Open();
    }

#endif

    // PlayMakerWelcomeWindow.cs
    //[MenuItem("PlayMaker/Welcome Screen", false, 1000)]

    #region ADDONS

    private const string addonsRoot = MenuRoot + "Addons/";
    private const int iAddons = 1000;

    [MenuItem(addonsRoot + "Download Addons", false, iAddons)]
    public static void OpenAddonsWiki()
    {
        Application.OpenURL("https://hutonggames.fogbugz.com/default.asp?W714");
    }

    #endregion


#if UNITY_2019_1_OR_NEWER

    [Shortcut("PlayMaker/Toggle Lock Selection", KeyCode.L, ShortcutModifiers.Action)]
    public static void ToggleFsmEditorLock()
    {
        FsmEditor.ToggleLockSelection();
    }

#endif

}
