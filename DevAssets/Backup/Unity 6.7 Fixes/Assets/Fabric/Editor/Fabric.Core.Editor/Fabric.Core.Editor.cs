using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Fabric.MIDI;
using Fabric.TimelineComponent;
using Fabric.Wav;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[assembly: ComVisible(false)]
[assembly: AssemblyFileVersion("4.0.3.0")]
[assembly: AssemblyTrademark("")]
[assembly: Guid("9b155348-94cc-4296-ba3e-3e322dedfb15")]
[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: AssemblyTitle("Fabric.Core.Editor")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("TazMan-Audio")]
[assembly: AssemblyProduct("Fabric.Core.Editor")]
[assembly: AssemblyCopyright("Copyright © TazMan-Audio 2026")]
[assembly: AssemblyVersion("4.0.3.0")]
namespace Fabric
{
	public static class StringExtensions
	{
		public static string Quotify(this string s)
		{
			return $"\"{s}\"";
		}
	}
	internal class NewAboutDialogWindow : EditorWindow
	{
		[MenuItem("Window/Fabric/About", false, 25)]
		private static void Init()
		{
			NewAboutDialogWindow newAboutDialogWindow = ScriptableObject.CreateInstance<NewAboutDialogWindow>();
			newAboutDialogWindow.position = new Rect(Screen.width / 2, Screen.height / 2, 280f, 80f);
			newAboutDialogWindow.Show();
		}

		private void OnGUI()
		{
			string text = "2.4";
			string text2 = "";
			text2 = " (>Unity 5.6+)";
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(" Fabric " + text + text2);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(15f);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("By");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Tazman-Audio Ltd (Anastasios Brakis)");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
		}
	}
	public class AboutDialog
	{
		private static Dictionary<string, string> audioClipReferences = new Dictionary<string, string>();

		[MenuItem("Window/Fabric/Utils/ConvertSamplesToSecs")]
		private static void ConvertSamplesToSecs()
		{
			float num = 44100f;
			SequenceComponent[] array = UnityEngine.Object.FindObjectsByType(typeof(SequenceComponent)) as SequenceComponent[];
			for (int i = 0; i < array.Length; i++)
			{
				array[i]._transitionOffset = array[i]._transitionOffset / num;
				array[i]._transitionOffsetRandomization = array[i]._transitionOffsetRandomization / num;
			}
			AudioComponent[] array2 = UnityEngine.Object.FindObjectsByType(typeof(AudioComponent)) as AudioComponent[];
			for (int j = 0; j < array2.Length; j++)
			{
				double delay = array2[j].Delay / (double)num;
				array2[j].Delay = delay;
			}
		}

		[MenuItem("Window/Fabric/Utils/EnableDynamicAudioClipLoading")]
		private static void EnableDynamicAudioClipLoading()
		{
			AudioComponent[] componentsInChildren = Selection.activeGameObject.GetComponentsInChildren<AudioComponent>(includeInactive: true);
			foreach (AudioComponent audioComponent in componentsInChildren)
			{
				if (!audioComponent._dynamicAudioClipLoading)
				{
					string assetPath = AssetDatabase.GetAssetPath(audioComponent.AudioClip);
					assetPath = Regex.Replace(assetPath, ".wav", "", RegexOptions.IgnoreCase);
					int num = assetPath.LastIndexOf("Resources/");
					if (num >= 0)
					{
						assetPath = assetPath.Remove(0, num);
						assetPath = assetPath.Replace("Resources/", "");
					}
					audioComponent._audioClipHandle.SetAudioClipPath(assetPath);
					audioComponent.AudioClip = null;
					audioComponent._dynamicAudioClipLoading = true;
				}
			}
		}

		[MenuItem("Window/Fabric/Fabric Manual", false, 26)]
		private static void FabricManual()
		{
			string url = "http://fabric-manual.com";
			Application.OpenURL(url);
		}

		[MenuItem("Window/Fabric/Fabric Resources", false, 27)]
		private static void FabricResources()
		{
			string url = "http://www.tazman-audio.co.uk/#!resources/c1fms";
			Application.OpenURL(url);
		}

		[MenuItem("Window/Fabric/Fabric Forums", false, 28)]
		private static void FabricForums()
		{
			string url = "http://www.tazman-audio.co.uk/#!moot-forum/c1nh8";
			Application.OpenURL(url);
		}

		[MenuItem("Window/Fabric/Fabric Support", false, 29)]
		private static void FabricSupport()
		{
			string url = "http://www.tazman-audio.co.uk/#!support/c3vn";
			Application.OpenURL(url);
		}

		[MenuItem("Window/Fabric/Utils/DisableDynamicAudioClipLoading")]
		private static void DisableDynamicAudioClipLoading()
		{
			AudioComponent[] componentsInChildren = Selection.activeGameObject.GetComponentsInChildren<AudioComponent>(includeInactive: true);
			foreach (AudioComponent audioComponent in componentsInChildren)
			{
				if (audioComponent._dynamicAudioClipLoading)
				{
					audioComponent.AudioClip = Resources.Load(audioComponent._audioClipHandle.GetAudioClipPath()) as AudioClip;
					audioComponent._dynamicAudioClipLoading = false;
				}
			}
		}

		[MenuItem("Window/Fabric/Post Install Checker", false, 22)]
		private static void PostInstallChecker()
		{
			string text = FabricManagerEditor.GetFabricEditorPath() + "/";
			string text2 = FabricManagerEditor.GetFabricEditorPath() + "/../";
			if (EditorUtility.DisplayDialog("Post Install Checker", "Do you wish to remove the Modular Synth and Loudness Meter plugins?", "Yes", "No"))
			{
				DeleteAsset(text2 + "Fabric.LoudnessMeter.dll");
				DeleteAsset(text + "Fabric.LoudnessMeter.Editor.dll");
				DeleteAsset(text2 + "Fabric.ModularSynth.dll");
				DeleteAsset(text + "Fabric.ModularSynth.Editor.dll");
			}
			DeleteAsset(text + "Resources");
		}

		[MenuItem("Window/Fabric/Utils/Clean Baked Audio Sources")]
		private static void FabricUtilsCleanBakedAudioSources()
		{
			if (!(Selection.activeGameObject != null))
			{
				return;
			}
			AudioSource[] componentsInChildren = Selection.activeGameObject.GetComponentsInChildren<AudioSource>(includeInactive: true);
			AudioSource[] array = componentsInChildren;
			foreach (AudioSource audioSource in array)
			{
				if (!audioSource.transform.GetComponent<AudioVoice>())
				{
					UnityEngine.Object.DestroyImmediate(audioSource.gameObject);
				}
			}
		}

		private static void DeleteAsset(string path)
		{
			if (FileUtil.DeleteFileOrDirectory(path))
			{
				UnityEngine.Debug.Log("Fabric Post Install Checker deleted: " + path);
			}
		}

		[MenuItem("Window/Fabric/Utils/DeleteAudioSources")]
		public static void DeleteAudioSourcesEditor()
		{
			FabricManagerEditor.DeleteAudioSources(Selection.activeGameObject);
		}

		[MenuItem("Window/Fabric/Utils/CopyAudioClipReferences")]
		private static void Copy()
		{
			UnityEngine.Debug.Log("Copying references...");
			string name = FabricManager.Instance.gameObject.name;
			audioClipReferences.Clear();
			AudioComponent[] componentsInChildren = Selection.activeGameObject.GetComponentsInChildren<AudioComponent>(includeInactive: true);
			foreach (AudioComponent audioComponent in componentsInChildren)
			{
				if (audioComponent._dynamicAudioClipLoading)
				{
					string path = name + "/";
					int depth = 0;
					audioComponent.BuildComponentEventPathName(ref path, depth);
					if (!audioClipReferences.ContainsKey(path))
					{
						audioClipReferences.Add(path, audioComponent._audioClipHandle.GetAudioClipPath());
					}
				}
			}
			UnityEngine.Debug.Log("Done!!");
		}

		[MenuItem("Window/Fabric/Utils/PasteAudioClipReferences")]
		private static void Paste()
		{
			UnityEngine.Debug.Log("Pasting references...");
			string name = FabricManager.Instance.gameObject.name;
			AudioComponent[] componentsInChildren = Selection.activeGameObject.GetComponentsInChildren<AudioComponent>(includeInactive: true);
			foreach (AudioComponent audioComponent in componentsInChildren)
			{
				string path = name + "/";
				int depth = 0;
				audioComponent.BuildComponentEventPathName(ref path, depth);
				if (audioClipReferences.ContainsKey(path))
				{
					audioComponent._audioClipHandle.SetAudioClipPath(audioClipReferences[path]);
					audioComponent._dynamicAudioClipLoading = true;
				}
			}
			UnityEngine.Debug.Log("Done!!");
		}
	}
	internal class PrefixStringToDialogComponent : EditorWindow
	{
		private string stringToAdd = "";

		[MenuItem("Window/Fabric/Utils/PrefixStringToDialogComponents")]
		private static void Init()
		{
			PrefixStringToDialogComponent prefixStringToDialogComponent = ScriptableObject.CreateInstance<PrefixStringToDialogComponent>();
			prefixStringToDialogComponent.position = new Rect(Screen.width / 2, Screen.height / 2, 280f, 80f);
			prefixStringToDialogComponent.Show();
		}

		private List<DialogAudioComponent> CollectComponents()
		{
			List<DialogAudioComponent> list = new List<DialogAudioComponent>();
			for (int i = 0; i < Selection.gameObjects.Length; i++)
			{
				DialogAudioComponent[] componentsInChildren = Selection.gameObjects[i].GetComponentsInChildren<DialogAudioComponent>(includeInactive: true);
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					if (!list.Contains(componentsInChildren[j]))
					{
						list.Add(componentsInChildren[j]);
					}
				}
			}
			return list;
		}

		private void OnGUI()
		{
			stringToAdd = EditorGUILayout.TextField("String to Prefix:", stringToAdd);
			GUILayout.Space(10f);
			if (GUILayout.Button("Prefix String"))
			{
				List<DialogAudioComponent> list = CollectComponents();
				if (list.Count != 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						DialogAudioComponent dialogAudioComponent = list[i];
						dialogAudioComponent._audioClipReference = dialogAudioComponent._audioClipReference.Insert(0, stringToAdd);
					}
					Close();
					EditorUtility.DisplayDialog("Conversion Report", "Dialog components converted: " + list.Count, "Done", "Close");
				}
			}
			else if (GUILayout.Button("Remove Prefix"))
			{
				List<DialogAudioComponent> list2 = CollectComponents();
				if (list2.Count != 0)
				{
					for (int j = 0; j < list2.Count; j++)
					{
						DialogAudioComponent dialogAudioComponent2 = list2[j];
						dialogAudioComponent2._audioClipReference = dialogAudioComponent2._audioClipReference.Replace(stringToAdd, "");
					}
					Close();
					EditorUtility.DisplayDialog("Conversion Report", "Dialog components converted: " + list2.Count, "Done", "Close");
				}
			}
			else if (GUILayout.Button("Close"))
			{
				Close();
			}
		}
	}
	[CustomEditor(typeof(DebugLog))]
	public class DebugLogEditor : Editor
	{
		private DebugLog _debugLog;

		private void OnEnable()
		{
			_debugLog = base.target as DebugLog;
		}

		public override void OnInspectorGUI()
		{
			MenuBar.OnGUI("290597-debuglog", box: true);
			GUILayout.BeginVertical("Box");
			base.OnInspectorGUI();
			GUILayout.EndVertical();
			if (_debugLog._destroy)
			{
				UnityEngine.Object.DestroyImmediate(_debugLog);
			}
		}
	}
	public class AudioAssetImporterWindow : EditorWindow
	{
		private Vector2 scrollPosition = default(Vector2);

		private static AudioComponentImportType audioComponentType;

		public static bool addEventListener = false;

		public static Component component = null;

		[MenuItem("Window/Fabric/Audio Asset Importer", false, 8)]
		private static void Init()
		{
			AudioAssetImporterWindow audioAssetImporterWindow = (AudioAssetImporterWindow)EditorWindow.GetWindow(typeof(AudioAssetImporterWindow));
				audioAssetImporterWindow.titleContent = new GUIContent("Asset Importer");
		}

		private void OnSelectionChange()
		{
			Repaint();
		}

		private void PlaymodeStateChanged()
		{
			component = null;
			Repaint();
		}

		private void PlaymodeStateChanged(PlayModeStateChange state)
		{
			PlaymodeStateChanged();
		}

		private void OnEnable()
		{
			EditorApplication.playModeStateChanged -= PlaymodeStateChanged;
			EditorApplication.playModeStateChanged += PlaymodeStateChanged;
		}

		private void OnGUI()
		{
			GUILayout.BeginVertical("Box");
			MenuBar.OnGUI("442466", box: true);
			GUILayout.Space(5f);
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, "Box");
			FabricManager instance = FabricManager.Instance;
			Component component = null;
			if (Selection.activeGameObject != null)
			{
				Component component2 = Selection.activeGameObject.GetComponent<Component>();
				if (component2 as AudioComponent == null)
				{
					component = component2;
				}
			}
			if (component != AudioAssetImporterWindow.component && component != null)
			{
				AudioAssetImporterWindow.component = component;
			}
			if (instance != null)
			{
				string path = "";
				int depth = 0;
				if (AudioAssetImporterWindow.component != null)
				{
					path = instance.name + "/";
					AudioAssetImporterWindow.component.BuildComponentEventPathName(ref path, depth);
				}
				GUILayout.BeginHorizontal();
				GUILayout.Label("Component Destination: ", GUILayout.MinWidth(150f), GUILayout.MaxWidth(150f));
				GUILayout.Label(path, "Box", GUILayout.MinWidth(500f));
				if (GUILayout.Button("Clear", GUILayout.MinWidth(60f)))
				{
					AudioAssetImporterWindow.component = null;
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(8f);
				GUILayout.BeginHorizontal();
				GUILayout.Label("Auto-Import Assets: ", GUILayout.MinWidth(150f), GUILayout.MaxWidth(150f));
				FabricEditorData.GetData()._audioAssetImporter = GUILayout.Toggle(FabricEditorData.GetData()._audioAssetImporter, "");
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Add EventListener: ", GUILayout.MinWidth(150f), GUILayout.MaxWidth(150f));
				addEventListener = GUILayout.Toggle(addEventListener, "");
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Create as: ", GUILayout.MinWidth(150f), GUILayout.MaxWidth(150f));
				audioComponentType = (AudioComponentImportType)(object)EditorGUILayout.EnumPopup("", audioComponentType);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Reaper Path: ", GUILayout.MinWidth(150f), GUILayout.MaxWidth(150f));
				FabricEditorData.GetData()._reaperPath = GUILayout.TextField(FabricEditorData.GetData()._reaperPath);
				GUILayout.EndHorizontal();
				GUILayout.Space(10f);
				DropAreaGUI(AudioAssetImporterWindow.component);
			}
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			if (instance != null)
			{
				GUIHelpers.CheckGUIHasChanged(instance.gameObject);
			}
		}

		private void DrawLine()
		{
			GUILayout.Box("", GUILayout.ExpandWidth(expand: true), GUILayout.Height(1f));
		}

		private static void AddRandomComponent(Component component, ref List<GameObject> randomComponent)
		{
			int num = randomComponent[0].name.LastIndexOf("_");
			string text = randomComponent[0].name.Remove(num, randomComponent[0].name.Length - num);
			GameObject gameObject = new GameObject(text);
			gameObject.transform.parent = component.transform;
			RandomComponent randomComponent2 = gameObject.AddComponent<RandomComponent>();
			for (int i = 0; i < randomComponent.Count; i++)
			{
				randomComponent[i].transform.parent = randomComponent2.transform;
			}
			randomComponent.Clear();
		}

		public static void ImportAudioClips(UnityEngine.Object[] audioClips, Component component)
		{
			List<GameObject> randomComponent = null;
			if (audioComponentType == AudioComponentImportType.AudioComponent_AutoDetect_RandomComponent)
			{
				randomComponent = new List<GameObject>();
				Array.Sort(audioClips, (UnityEngine.Object x, UnityEngine.Object y) => string.Compare(x.name, y.name));
			}
			for (int num = 0; num < audioClips.Length; num++)
			{
				GameObject gameObject = new GameObject(audioClips[num].name);
				gameObject.transform.parent = component.transform;
				if (audioComponentType == AudioComponentImportType.AudioComponent || audioComponentType == AudioComponentImportType.RandomComponent || audioComponentType == AudioComponentImportType.AudioComponent_AutoDetect_RandomComponent)
				{
					AudioComponent audioComponent = gameObject.AddComponent<AudioComponent>();
					audioComponent.AudioClip = audioClips[num] as AudioClip;
				}
				else if (audioComponentType == AudioComponentImportType.DialogAudioComponent)
				{
					DialogAudioComponent dialogAudioComponent = gameObject.AddComponent<DialogAudioComponent>();
					dialogAudioComponent.AudioClipReference = audioClips[num].name;
				}
				if (addEventListener)
				{
					EventListener eventListener = gameObject.AddComponent<EventListener>();
					eventListener._eventName = audioClips[num].name.Replace("_", "/");
					EventManager.Instance._eventList.Add(eventListener._eventName);
				}
				if (audioComponentType != AudioComponentImportType.AudioComponent_AutoDetect_RandomComponent)
				{
					continue;
				}
				if (randomComponent.Count == 0)
				{
					randomComponent.Add(gameObject);
					continue;
				}
				GameObject gameObject2 = randomComponent[randomComponent.Count - 1];
				int num2 = gameObject2.name.LastIndexOf("_");
				if (num2 >= 0)
				{
					string value = gameObject2.name.Remove(num2, gameObject2.name.Length - num2);
					if (gameObject.name.Contains(value))
					{
						randomComponent.Add(gameObject);
						continue;
					}
					AddRandomComponent(component, ref randomComponent);
					randomComponent.Add(gameObject);
				}
				else
				{
					randomComponent.Clear();
				}
			}
			if (audioComponentType == AudioComponentImportType.AudioComponent_AutoDetect_RandomComponent && randomComponent.Count > 0)
			{
				AddRandomComponent(component, ref randomComponent);
			}
		}

		private bool DropAreaGUI(Component component)
		{
			if (component == null)
			{
				GUI.enabled = false;
			}
			bool result = false;
			UnityEngine.Event current = UnityEngine.Event.current;
			Color backgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = Color.green;
			DrawLine();
			Rect rect = GUILayoutUtility.GetRect(0f, 35f, GUILayout.ExpandWidth(expand: true));
			GUI.Box(rect, "Drag and Drop AudioClips here to automatically create Audio Components");
			GUI.backgroundColor = Color.green;
			DrawLine();
			GUI.backgroundColor = backgroundColor;
			switch (current.type)
			{
			case EventType.DragUpdated:
			case EventType.DragPerform:
			{
				if (!rect.Contains(current.mousePosition))
				{
					return false;
				}
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				if (current.type != EventType.DragPerform)
				{
					break;
				}
				DragAndDrop.AcceptDrag();
				List<UnityEngine.Object> list = new List<UnityEngine.Object>();
				UnityEngine.Object[] objectReferences = DragAndDrop.objectReferences;
				foreach (UnityEngine.Object obj in objectReferences)
				{
					AudioClip audioClip = obj as AudioClip;
					if (audioClip != null)
					{
						list.Add(obj);
					}
				}
				if (list.Count > 0 && component != null)
				{
					if (audioComponentType == AudioComponentImportType.RandomComponent)
					{
						GameObject gameObject = new GameObject("Random Component");
						gameObject.transform.parent = component.transform;
						component = gameObject.AddComponent<RandomComponent>();
					}
					ImportAudioClips(list.ToArray(), component);
					result = true;
				}
				break;
			}
			}
			GUI.enabled = true;
			return result;
		}

		public bool DropAudioClipAreaGUI(RandomAudioClipComponent component)
		{
			bool result = false;
			UnityEngine.Event current = UnityEngine.Event.current;
			Color backgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = Color.green;
			DrawLine();
			Rect rect = GUILayoutUtility.GetRect(0f, 35f, GUILayout.ExpandWidth(expand: true));
			GUI.Box(rect, "Drag and Drop AudioClips here to automatically populate audio clip array\n (Use the inspector lock option to maintain focus)");
			GUI.backgroundColor = Color.green;
			DrawLine();
			GUI.backgroundColor = backgroundColor;
			switch (current.type)
			{
			case EventType.DragUpdated:
			case EventType.DragPerform:
			{
				if (!rect.Contains(current.mousePosition))
				{
					return false;
				}
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				if (current.type != EventType.DragPerform)
				{
					break;
				}
				DragAndDrop.AcceptDrag();
				List<AudioClip> list = new List<AudioClip>();
				UnityEngine.Object[] objectReferences = DragAndDrop.objectReferences;
				foreach (UnityEngine.Object obj in objectReferences)
				{
					AudioClip audioClip = obj as AudioClip;
					if (audioClip != null)
					{
						list.Add(audioClip);
					}
				}
				if (list.Count > 0)
				{
					component._audioClips = list.ToArray();
					result = true;
				}
				break;
			}
			}
			return result;
		}

		private static void Callback(object obj)
		{
		}
	}
	public class AudioAssetImporter : AssetPostprocessor
	{
		public static bool ignoreLoading;

		private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
		{
			if (FabricManager.Instance == null || !FabricEditorData.GetData()._audioAssetImporter)
			{
				return;
			}
			Component component = AudioAssetImporterWindow.component;
			List<AudioClip> list = new List<AudioClip>();
			foreach (string text in importedAssets)
			{
				AudioClip audioClip = AssetDatabase.LoadAssetAtPath(text, typeof(AudioClip)) as AudioClip;
				if ((bool)audioClip)
				{
					list.Add(audioClip);
				}
			}
			AudioAssetImporterWindow.ImportAudioClips(list.ToArray(), component);
		}

		private void OnPostprocessAudio(AudioClip audioClip)
		{
		}
	}
	public class AudioReport : EditorWindow
	{
		private class AudioRef
		{
			public AudioClip _AudioClip;

			public int _Memory;

			public List<GameObject> _AudioComponents = new List<GameObject>();
		}

		private List<AudioRef> _sortedList = new List<AudioRef>();

		private Dictionary<AudioClip, AudioRef> _AudioClipUsage = new Dictionary<AudioClip, AudioRef>();

		private int _totalMemory;

		private Vector2 _scroll = Vector2.zero;

		[MenuItem("Window/Fabric/Audio Clip Report", false, 21)]
		private static void Init()
		{
			EditorWindow.GetWindow<AudioReport>(utility: false, "Audio Report");
		}

		private void OnGUI()
		{
			MenuBar.OnGUI("288091-audioclipreport", box: true);
			if (GUILayout.Button("Analyse Scene"))
			{
				_AudioClipUsage = new Dictionary<AudioClip, AudioRef>();
				_totalMemory = 0;
				AudioComponent[] array = Resources.FindObjectsOfTypeAll(typeof(AudioComponent)) as AudioComponent[];
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].gameObject != null && array[i].hideFlags != HideFlags.HideInHierarchy && (bool)array[i].AudioClip)
					{
						AudioRef audioRef = null;
						if (!_AudioClipUsage.ContainsKey(array[i].AudioClip))
						{
							audioRef = new AudioRef();
							audioRef._AudioClip = array[i].AudioClip;
							_totalMemory += audioRef._Memory;
							_AudioClipUsage.Add(array[i].AudioClip, audioRef);
						}
						else
						{
							audioRef = _AudioClipUsage[array[i].AudioClip];
						}
						audioRef._AudioComponents.Add(array[i].gameObject);
					}
				}
				_sortedList = new List<AudioRef>(_AudioClipUsage.Values);
			}
			_scroll = GUILayout.BeginScrollView(_scroll);
			GUIStyle gUIStyle = new GUIStyle(GUI.skin.label);
			gUIStyle.alignment = TextAnchor.MiddleRight;
			GUIStyle gUIStyle2 = new GUIStyle(EditorStyles.boldLabel);
			gUIStyle2.alignment = TextAnchor.MiddleRight;
			float width = 250f;
			GUILayout.Label("Total Memory Usage: " + ((float)_totalMemory / 1024f / 1024f * 0.51f).ToString("f2"));
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Name", GUILayout.Width(width)))
			{
				_sortedList.Sort((AudioRef a, AudioRef b) => a._AudioClip.name.CompareTo(b._AudioClip.name));
			}
			if (GUILayout.Button("Comp Refs"))
			{
				_sortedList.Sort((AudioRef a, AudioRef b) => b._AudioComponents.Count.CompareTo(a._AudioComponents.Count));
			}
			if (GUILayout.Button("Size"))
			{
				_sortedList.Sort((AudioRef a, AudioRef b) => b._Memory.CompareTo(a._Memory));
			}
			if (GUILayout.Button("Length"))
			{
				_sortedList.Sort((AudioRef a, AudioRef b) => b._AudioClip.length.CompareTo(a._AudioClip.length));
			}
			if (GUILayout.Button("Frequency"))
			{
				_sortedList.Sort((AudioRef a, AudioRef b) => b._AudioClip.frequency.CompareTo(a._AudioClip.frequency));
			}
			if (GUILayout.Button("Channels"))
			{
				_sortedList.Sort((AudioRef a, AudioRef b) => b._AudioClip.channels.CompareTo(a._AudioClip.channels));
			}
			GUILayout.EndHorizontal();
			Color color = new Color(1f, 1f, 1f, 0f);
			foreach (AudioRef sorted in _sortedList)
			{
				AudioClip audioClip = sorted._AudioClip;
				if ((bool)audioClip)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label(audioClip.name, GUILayout.Width(width));
					GUILayout.FlexibleSpace();
					GUILayout.Label(sorted._AudioComponents.Count.ToString());
					Rect lastRect = GUILayoutUtility.GetLastRect();
					GUI.color = color;
					if (GUI.Button(lastRect, ""))
					{
						Selection.objects = sorted._AudioComponents.ToArray();
					}
					GUI.color = Color.white;
					GUILayout.FlexibleSpace();
					GUILayout.Label(((float)sorted._Memory / 1024f / 1024f * 0.51f).ToString("f2"), gUIStyle);
					GUILayout.FlexibleSpace();
					GUILayout.Label(audioClip.length.ToString(), gUIStyle);
					GUILayout.FlexibleSpace();
					GUILayout.Label(audioClip.frequency.ToString(), gUIStyle);
					GUILayout.FlexibleSpace();
					GUILayout.Label(audioClip.channels.ToString(), gUIStyle);
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
				}
			}
			GUILayout.EndScrollView();
		}
	}
	[CanEditMultipleObjects]
	[CustomEditor(typeof(AudioSourcePool))]
	public class AudioSourcePoolEditor : Editor
	{
		private AudioSourcePool _audioSourcePool;

		private Vector2 scrollPosition = default(Vector2);

		private static int maxUsage;

		private void OnEnable()
		{
			_audioSourcePool = base.target as AudioSourcePool;
		}

		public override void OnInspectorGUI()
		{
			MenuBar.OnGUI("442039", box: true);
			GUILayout.BeginVertical("Box");
			DisplayProperties(FabricManager.Instance);
			GUILayout.EndVertical();
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, "Box");
			OnGUI(_audioSourcePool);
			GUILayout.EndScrollView();
			EditorUtility.SetDirty(base.target);
		}

		public static void OnGUI(AudioSourcePool audioSourcePool)
		{
			GUILayout.Space(5f);
			AudioVoice[] allocatedAudioVoices = audioSourcePool.GetAllocatedAudioVoices();
			if (allocatedAudioVoices != null)
			{
				foreach (AudioVoice audioVoice in allocatedAudioVoices)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label("Component: ", GUILayout.MaxWidth(80f));
			EditorGUILayout.ObjectField("", audioVoice._component.gameObject, typeof(GameObject), false);
					GUILayout.Label("Audio Source: ", GUILayout.MaxWidth(100f));
				EditorGUILayout.ObjectField("", audioVoice._audioSource, typeof(AudioSource), false);
					GUILayout.EndHorizontal();
				}
			}
		}

		public static void DisplayProperties(FabricManager fabricManager)
		{
			if (fabricManager == null)
			{
				return;
			}
			float num = EditorGUILayout.Slider("FadeInTime: ", fabricManager._audioSourcePoolFadeInTime, 0f, 5f);
			if (fabricManager._audioSourcePoolFadeInTime != num)
			{
				fabricManager._audioSourcePoolFadeInTime = num;
				if (fabricManager.AudioSourcePoolManager != null)
				{
					fabricManager.AudioSourcePoolManager.SetFadeOutTime(num);
				}
			}
			num = EditorGUILayout.Slider("FadeOutTime: ", fabricManager._audioSourcePoolFadeOutTime, 0f, 5f);
			if (fabricManager._audioSourcePoolFadeOutTime != num)
			{
				fabricManager._audioSourcePoolFadeOutTime = num;
				if (fabricManager.AudioSourcePoolManager != null)
				{
					fabricManager.AudioSourcePoolManager.SetFadeOutTime(num);
				}
			}
			int numAllocatedVoices = 0;
			int numToBeRemovedVoices = 0;
			if (fabricManager.AudioSourcePoolManager != null)
			{
				fabricManager.AudioSourcePoolManager.GetInfo(ref numAllocatedVoices, ref numToBeRemovedVoices);
			}
			if (numAllocatedVoices > maxUsage)
			{
				maxUsage = numAllocatedVoices;
			}
			GUILayout.Label("Allocated voices: " + numAllocatedVoices + "  [Max: " + maxUsage + "]");
			GUILayout.Label("Freeing Voices: " + numToBeRemovedVoices);
			GUILayout.Label("Freeing Voices: " + numToBeRemovedVoices);
		}
	}
	[CustomEditor(typeof(BlendComponent))]
	[CanEditMultipleObjects]
	public class BlendComponentEditor : Editor
	{
		private bool _foldout = true;

		private BlendComponent blendComponent;

		private ComponentEditor componentEditor = new ComponentEditor();

		private SerializedProperty notifyParentComponent;

		[MenuItem("Fabric/Components/BlendComponent")]
		private static void About()
		{
			GameObject activeGameObject = Selection.activeGameObject;
			if (!(activeGameObject == null))
			{
				GameObject gameObject = new GameObject("BlendComponent");
				gameObject.transform.parent = activeGameObject.transform;
				gameObject.AddComponent<BlendComponent>();
			}
		}

		private void OnEnable()
		{
			blendComponent = base.target as BlendComponent;
			componentEditor.RegisterSerialisableObject(base.serializedObject);
			notifyParentComponent = base.serializedObject.FindProperty("_notifyParentComponent");
		}

		private void OnDestroy()
		{
		}

		public override void OnInspectorGUI()
		{
			if (componentEditor.InspectorGUI(blendComponent, "288046-blendcomponent"))
			{
				return;
			}
			_foldout = EditorGUILayout.Foldout(_foldout, "Blend Component Properties");
			if (_foldout)
			{
				GUILayout.BeginVertical("box");
				EditorGUILayout.PropertyField(notifyParentComponent, new GUIContent("Notify Parent when: "));
				Component[] childComponents = blendComponent.GetChildComponents();
				GUIStyle gUIStyle = new GUIStyle();
				gUIStyle.normal.textColor = Color.grey;
				gUIStyle.alignment = TextAnchor.MiddleCenter;
				EditorGUILayout.LabelField("Components", "Volumes", gUIStyle);
				EditorGUILayout.Separator();
				foreach (Component component in childComponents)
				{
					component.Volume = ComponentEditor.DisplayDecibelsVolumeSlider(component.name, component.Volume);
				}
				GUILayout.EndVertical();
			}
			GUIHelpers.CheckGUIHasChanged(base.serializedObject, blendComponent);
		}
	}
	[InitializeOnLoad]
	internal class ComponentHierarchyIcons
	{
		private class ComponentIconType
		{
			public Dictionary<EntityId, Component> components = new Dictionary<EntityId, Component>();

			public Texture2D icon;

			public Texture2D activeIcon;

			public Type type;
		}

		private static List<ComponentIconType> componentIconTypes;

		static ComponentHierarchyIcons()
		{
			componentIconTypes = new List<ComponentIconType>();
			Type[] array = ComponentEditor.FabricComponentCollector.Get<Component>();
			for (int i = 0; i < array.Length; i++)
			{
				ComponentIconType item = new ComponentIconType
				{
					type = array[i],
					icon = GUIHelpers.LoadImage("Icons/" + array[i].Name),
					activeIcon = GUIHelpers.LoadImage("Icons/" + array[i].Name + "Active")
				};
				componentIconTypes.Add(item);
			}
			EditorApplication.hierarchyChanged -= Update;
			EditorApplication.hierarchyChanged += Update;
			EditorApplication.hierarchyWindowItemByEntityIdOnGUI = (EditorApplication.HierarchyWindowItemByEntityIdCallback)Delegate.Combine(EditorApplication.hierarchyWindowItemByEntityIdOnGUI, new EditorApplication.HierarchyWindowItemByEntityIdCallback(HierarchyItem));
		}

		private static void Update()
		{
			if (FabricManager.IsInitialised() && !FabricEditorData.GetData()._enableHiearchyIcons)
			{
				return;
			}
			Component[] array = UnityEngine.Object.FindObjectsByType(typeof(Component)) as Component[];
			Component[] array2 = array;
			foreach (Component component in array2)
			{
				for (int j = 0; j < componentIconTypes.Count; j++)
				{
					if (componentIconTypes[j].type == component.GetType() && !componentIconTypes[j].components.ContainsKey(component.gameObject.GetEntityId()))
					{
						componentIconTypes[j].components.Add(component.gameObject.GetEntityId(), component);
					}
				}
			}
		}

		private static void HierarchyItem(EntityId instanceID, Rect selectionRect)
		{
			if (FabricManager.IsInitialised() && !FabricEditorData.GetData()._enableHiearchyIcons)
			{
				return;
			}
			for (int i = 0; i < componentIconTypes.Count; i++)
			{
				if (!componentIconTypes[i].components.ContainsKey(instanceID))
				{
					continue;
				}
				Component component = componentIconTypes[i].components[instanceID];
				if (component != null)
				{
					Texture2D texture2D = null;
					texture2D = ((!component.IsComponentActive() || !(componentIconTypes[i].activeIcon != null)) ? componentIconTypes[i].icon : componentIconTypes[i].activeIcon);
					if (texture2D != null)
					{
						Rect position = new Rect(selectionRect.x + selectionRect.width - 18f, selectionRect.y, 18f, 18f);
						GUI.Label(position, texture2D);
					}
					break;
				}
				componentIconTypes[i].components.Remove(instanceID);
			}
		}
	}
	public class MenuBar
	{
		public static bool OnGUI(string docName, bool box = false, GameObject gameObject = null)
		{
			if (box)
			{
				GUILayout.BeginHorizontal("Box");
			}
			else
			{
				GUILayout.BeginHorizontal();
			}
			GUILayout.FlexibleSpace();
			if ((bool)gameObject)
			{
				if (GUILayout.Button("Save", GUILayout.MaxWidth(50f)))
				{
					XmlSerialization.SaveToXML(gameObject);
				}
				string[] displayedOptions = new string[3] { "Load", "Load XML", "Load XML To Child" };
				int selectedIndex = 0;
				switch (EditorGUILayout.Popup(selectedIndex, displayedOptions, "Button", GUILayout.MinHeight(18f), GUILayout.MaxWidth(50f)))
				{
				case 1:
					XmlDeserialization.LoadFromXML(gameObject);
					break;
				case 2:
					XmlDeserialization.LoadFromXMLToChild(gameObject);
					break;
				}
			}
			if (GUILayout.Button("?", GUILayout.MaxHeight(15f), GUILayout.MaxWidth(20f)))
			{
				ComponentMenuBar.OpenDocumentLink(docName);
			}
			if (gameObject != null && gameObject.GetComponent<FabricManager>() != null && GUILayout.Button(">>", GUILayout.MaxWidth(40f)))
			{
				GenericMenu genericMenu = new GenericMenu();
				genericMenu.AddItem(new GUIContent("New Child Component"), on: false, AddNewChildComponent, gameObject);
				genericMenu.ShowAsContext();
				UnityEngine.Event.current.Use();
			}
			GUILayout.EndHorizontal();
			return false;
		}

		private static void AddNewChildComponent(object userData)
		{
			GameObject gameObject = (GameObject)userData;
			AddNewChildDialog.Open(gameObject);
		}
	}
	public class ComponentMenuBar
	{
		public static void OpenDocumentLink(string name)
		{
			string url = "http://fabric-manual.com/s/Docs/m/Fabric/l/" + name;
			Application.OpenURL(url);
		}

		public static void OnGUI(Component component, string docName)
		{
			GUILayout.BeginHorizontal("Box");
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Save", "Button", GUILayout.MaxWidth(50f)))
			{
				XmlSerialization.SaveToXML(component.gameObject);
			}
			string[] displayedOptions = new string[3] { "Load", "Load XML", "Load XML To Child" };
			int selectedIndex = 0;
			switch (EditorGUILayout.Popup(selectedIndex, displayedOptions, "Button", GUILayout.MinHeight(18f), GUILayout.MaxWidth(50f)))
			{
			case 1:
				XmlDeserialization.LoadFromXML(component.gameObject);
				break;
			case 2:
				XmlDeserialization.LoadFromXMLToChild(component.gameObject);
				break;
			}
			if (GUILayout.Button("?", GUILayout.MaxWidth(20f)))
			{
				OpenDocumentLink(docName);
			}
			if (GUILayout.Button(">>", GUILayout.MaxWidth(40f)))
			{
				GenericMenu genericMenu = new GenericMenu();
				genericMenu.AddItem(new GUIContent("New Child Component"), on: false, AddNewChildComponent, component);
				genericMenu.AddItem(new GUIContent("New Parent Component"), on: false, AddNewParentComponent, component);
				genericMenu.AddItem(new GUIContent("Replace Component"), on: false, ReplaceComponent, component);
				genericMenu.AddItem(new GUIContent("New Event"), on: false, NewEvent, component);
				genericMenu.AddSeparator("");
				genericMenu.AddItem(new GUIContent("Open RTP Window"), on: false, OpenRTPWindow);
				genericMenu.AddItem(new GUIContent("Open Previewer Window"), on: false, OpenPreviewerWindow);
				if ((bool)(component as Fabric.TimelineComponent.TimelineComponent))
				{
					genericMenu.AddItem(new GUIContent("Open Timeline Window"), on: false, OpenTimelineWindow);
				}
				if ((bool)(component as AudioComponent))
				{
					genericMenu.AddItem(new GUIContent("Edit In Reaper"), on: false, EditInReaper);
				}
				genericMenu.ShowAsContext();
				UnityEngine.Event.current.Use();
			}
			GUILayout.EndHorizontal();
		}

		private static void OpenRTPWindow()
		{
			RTPManagerWidnow.init();
		}

		private static void OpenPreviewerWindow()
		{
		}

		private static void OpenTimelineWindow()
		{
			TimelineUIEditor.Init();
		}

		public static void AddNewChildComponent(object userData)
		{
			Component component = (Component)userData;
			AddNewChildDialog.Open(component.gameObject);
		}

		private static void AddNewParentComponent(object userData)
		{
			Component component = (Component)userData;
			AddNewParentDialog.Open(component);
		}

		private static void ReplaceComponent(object userData)
		{
			Component component = (Component)userData;
			ReplaceComponentDialog.Open(component);
		}

		private static void NewEvent(object userData)
		{
			Component component = (Component)userData;
			AddNewEventNameDialog.Open(component);
		}

		public static void EditInReaper()
		{
			AudioComponent component = Selection.activeGameObject.GetComponent<AudioComponent>();
			if (!component)
			{
				return;
			}
			string assetPath = AssetDatabase.GetAssetPath(component.AudioClip);
			if (assetPath.Length > 0)
			{
				WavReader wavReader = new WavReader();
				if (wavReader.LoadFile(assetPath))
				{
					string arguments = wavReader.m_bwf.Description.Quotify();
					string reaperPath = FabricEditorData.GetData()._reaperPath;
					Process.Start(reaperPath, arguments);
				}
			}
		}
	}
	[CustomEditor(typeof(ProxyComponent))]
	public class ProxyComponentEditor : Editor
	{
		[MenuItem("Fabric/Components/ProxyComponent")]
		private static void About()
		{
			GameObject activeGameObject = Selection.activeGameObject;
			if (!(activeGameObject == null))
			{
				GameObject gameObject = new GameObject("ProxyComponent");
				gameObject.transform.parent = activeGameObject.transform;
				gameObject.AddComponent<ProxyComponent>();
			}
		}

		public override void OnInspectorGUI()
		{
			ProxyComponent proxyComponent = (ProxyComponent)base.target;
			proxyComponent.targetComponment = (Component)EditorGUILayout.ObjectField("Component:", proxyComponent.targetComponment, typeof(Component), true);
		}
	}
	internal enum AudioComponentImportType
	{
		AudioComponent,
		AudioComponent_AutoDetect_RandomComponent,
		DialogAudioComponent,
		RandomComponent
	}
	internal class IsPropertyLinkedWithRTP
	{
		private static Color original;

		private static bool CheckPropertyWithRTP(Component component, string name)
		{
			if (component._RTPManager != null && component._RTPManager._parameters != null)
			{
				for (int i = 0; i < component._RTPManager._parameters.Length; i++)
				{
					if (name == component._RTPManager._parameters[i]._property._name)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static void CheckProperty(Component component, string property)
		{
			original = GUI.backgroundColor;
			if (CheckPropertyWithRTP(component, property))
			{
				GUI.backgroundColor = Color.green;
			}
		}

		public static void EndCheck()
		{
			GUI.backgroundColor = original;
		}
	}
	public class ComponentEditor
	{
		public static class FabricComponentCollector
		{
			public static Type[] Get<T>() where T : class
			{
				List<Type> list = new List<Type>();
				Type[] types = Assembly.GetAssembly(typeof(T)).GetTypes();
				foreach (Type type in types)
				{
					if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(T)))
					{
						list.Add(type);
					}
				}
				return list.ToArray();
			}
		}

		private bool _foldout = true;

		private SerializedObject serializedObject;

		private SerializedProperty priority;

		private SerializedProperty stealingBehaviour;

		private SerializedProperty minimumPlaybackInterval;

		private SerializedProperty overrideFadeProperties;

		private SerializedProperty fadeInTime;

		private SerializedProperty fadeInCurve;

		private SerializedProperty fadeOutTime;

		private SerializedProperty fadeOutCurve;

		private SerializedProperty overrideParentVolume;

		private SerializedProperty volume;

		private SerializedProperty volumeRandomization;

		private SerializedProperty overrideParentPitch;

		private SerializedProperty pitch;

		private SerializedProperty pitchRandomization;

		private SerializedProperty override2DProperties;

		private SerializedProperty pan2D;

		private SerializedProperty pan2DRandomization;

		private SerializedProperty override3DProperties;

		private SerializedProperty panLevel;

		private SerializedProperty spreadLevel;

		private SerializedProperty dopplerLevel;

		private SerializedProperty minDistance;

		private SerializedProperty maxDistance;

		private SerializedProperty rolloffMode;

		private SerializedProperty reverbZoneMix;

		private SerializedProperty multipleInstancesPerGameObject;

		private SerializedProperty componentVirtualization;

		private SerializedProperty overrideVolumeThreshold;

		private SerializedProperty volumeThreshold;

		private SerializedProperty numVirtualizationEvents;

		private SerializedProperty virtualizationBehavior;

		private SerializedProperty overrideParentComponentVirtualization;

		private SerializedProperty overrideBypassProperties;

		private SerializedProperty bypassEffect;

		private SerializedProperty bypassListenerEffect;

		private SerializedProperty bypassReverbZones;

		private SerializedProperty overrideMusicProperties;

		private SerializedProperty musicTimeSettingsindex;

		private SerializedProperty musicTempo;

		private SerializedProperty musicTimeSignatureLower;

		private SerializedProperty musicTimeSignatureUpper;

		private SerializedProperty musicTimeResetOnPlay;

		private SerializedProperty probability;

		private SerializedProperty overrideAudioBus;

		private SerializedProperty overrideAudioMixerGroup;

		private SerializedProperty audioMixerGroup;

		private SerializedProperty overrideSpatializeProperty;

		private SerializedProperty spatialize;

		private SerializedProperty customCurvesType;

		private AudioComponentImportType audioComponentType;

		private bool addEventListener;

		private int selection;

		public void RegisterSerialisableObject(SerializedObject _serializedObject)
		{
			serializedObject = _serializedObject;
			if (serializedObject != null)
			{
				priority = serializedObject.FindProperty("_priority");
				stealingBehaviour = serializedObject.FindProperty("_stealingBehaviour");
				minimumPlaybackInterval = serializedObject.FindProperty("_minimumPlaybackInterval");
				overrideFadeProperties = serializedObject.FindProperty("_overrideFadeProperties");
				fadeInTime = serializedObject.FindProperty("_fadeInTime");
				fadeInCurve = serializedObject.FindProperty("_fadeInCurve");
				fadeOutTime = serializedObject.FindProperty("_fadeOutTime");
				fadeOutCurve = serializedObject.FindProperty("_fadeOutCurve");
				overrideParentVolume = serializedObject.FindProperty("_overrideParentVolume");
				volume = serializedObject.FindProperty("_volume");
				volumeRandomization = serializedObject.FindProperty("_volumeRandomization");
				overrideParentPitch = serializedObject.FindProperty("_overrideParentPitch");
				pitch = serializedObject.FindProperty("_pitch");
				pitchRandomization = serializedObject.FindProperty("_pitchRandomization");
				override2DProperties = serializedObject.FindProperty("_override2DProperties");
				pan2D = serializedObject.FindProperty("_pan2D");
				pan2DRandomization = serializedObject.FindProperty("_pan2DRandomization");
				override3DProperties = serializedObject.FindProperty("_override3DProperties");
				panLevel = serializedObject.FindProperty("_panLevel");
				spreadLevel = serializedObject.FindProperty("_spreadLevel");
				dopplerLevel = serializedObject.FindProperty("_dopplerLevel");
				minDistance = serializedObject.FindProperty("_minDistance");
				maxDistance = serializedObject.FindProperty("_maxDistance");
				rolloffMode = serializedObject.FindProperty("_rolloffMode");
				multipleInstancesPerGameObject = serializedObject.FindProperty("_multipleInstancesPerGameObject");
				componentVirtualization = serializedObject.FindProperty("_componentVirtualization");
				overrideVolumeThreshold = serializedObject.FindProperty("_overrideVolumeThreshold");
				volumeThreshold = serializedObject.FindProperty("_volumeThreshold");
				numVirtualizationEvents = serializedObject.FindProperty("_numVirtualizationEvents");
				virtualizationBehavior = serializedObject.FindProperty("_virtualizationBehavior");
				overrideBypassProperties = serializedObject.FindProperty("_overrideBypassProperties");
				bypassEffect = serializedObject.FindProperty("_bypassEffects");
				bypassListenerEffect = serializedObject.FindProperty("_bypassListenerEffects");
				bypassReverbZones = serializedObject.FindProperty("_bypassReverbZones");
				overrideAudioMixerGroup = serializedObject.FindProperty("_overrideAudioMixerGroup");
				audioMixerGroup = serializedObject.FindProperty("_audioMixerGroup");
				overrideMusicProperties = serializedObject.FindProperty("_overrideMusicTimeSettings");
				musicTimeSettingsindex = serializedObject.FindProperty("_musicTimeSettingsIndex");
				musicTempo = serializedObject.FindProperty("_musicTempo");
				musicTimeSignatureLower = serializedObject.FindProperty("_musicTimeSignatureLower");
				musicTimeSignatureUpper = serializedObject.FindProperty("_musicTimeSignatureUpper");
				musicTimeResetOnPlay = serializedObject.FindProperty("_musicTimeResetOnPlay");
				probability = serializedObject.FindProperty("_probability");
				overrideAudioBus = serializedObject.FindProperty("_overrideAudioBus");
				reverbZoneMix = serializedObject.FindProperty("_reverbZoneMix");
				overrideSpatializeProperty = serializedObject.FindProperty("_overrideSpatializeProperty");
				spatialize = serializedObject.FindProperty("_spatialize");
				customCurvesType = serializedObject.FindProperty("_customCurvesType");
			}
		}

		public bool InspectorGUI(Component component, string docName = "")
		{
			bool result = false;
			serializedObject.Update();
			ComponentMenuBar.OnGUI(component, docName);
			FabricManagerEditor.GetManagerInstance();
			GUILayout.Space(3f);
			_foldout = EditorGUILayout.Foldout(_foldout, "Component Properties");
			if (_foldout)
			{
				GUILayout.BeginVertical("box");
				if (component.HasEventListener)
				{
					GUILayout.BeginVertical("Box");
					if (EditorApplication.isPlaying)
					{
						GUI.enabled = false;
					}
					int num = EditorGUILayout.IntField("Max Instances:", component.MaxInstances);
					if (num > 0 && num != component.MaxInstances)
					{
						component.ResizeMaxInstance(num);
					}
					if (EditorApplication.isPlaying)
					{
						GUI.enabled = true;
					}
					EditorGUILayout.PropertyField(multipleInstancesPerGameObject, new GUIContent("AllowMultipleObjectInst:"));
					EditorGUILayout.PropertyField(stealingBehaviour, new GUIContent("Stealing Mode:"));
					EditorGUILayout.Slider(minimumPlaybackInterval, 0f, 1000f, new GUIContent("Playback Interval (secs):"));
					EditorGUILayout.IntSlider(probability, 0, 100, new GUIContent("Probability:"));
					GUILayout.EndVertical();
					GUILayout.Space(5f);
					GUILayout.BeginVertical("Box");
					bool boolValue = componentVirtualization.boolValue;
					EditorGUILayout.PropertyField(componentVirtualization, new GUIContent("Enable Virtualization: "));
					if (FabricManager.Instance != null && FabricManager.Instance._enableVirtualization && boolValue != componentVirtualization.boolValue && componentVirtualization.boolValue)
					{
						EditorUtility.DisplayDialog("Fabric Warning", "AudioComponent virtualization is already enabled, disable it if using component virtualization", "Ok");
					}
					if (!componentVirtualization.boolValue)
					{
						GUI.enabled = false;
					}
					EditorGUILayout.PropertyField(numVirtualizationEvents, new GUIContent("Virtual Events: "));
					EditorGUILayout.PropertyField(virtualizationBehavior, new GUIContent("Virtualization Behavior: "));
					GUI.enabled = true;
					EditorGUILayout.PropertyField(overrideVolumeThreshold, new GUIContent("Override Volume Threshold: "));
					if (!overrideVolumeThreshold.boolValue)
					{
						GUI.enabled = false;
					}
					EditorGUILayout.Slider(volumeThreshold, 0f, 1f, new GUIContent("Volume Threshold:"));
					GUI.enabled = true;
					GUILayout.EndVertical();
					GUILayout.Space(10f);
				}
				if (!component.IsTopNode)
				{
					EditorGUILayout.PropertyField(overrideAudioBus, new GUIContent("Override AudioBus"));
				}
				if (overrideAudioBus.boolValue || component.IsTopNode)
				{
					List<string> audioBusNames = FabricManager.Instance._audioBusManager.GetAudioBusNames();
					int audioBusIndexByName = FabricManager.Instance._audioBusManager.GetAudioBusIndexByName(component._audioBusName);
					int num2 = EditorGUILayout.Popup("Audio Bus:", audioBusIndexByName, audioBusNames.ToArray());
					if (num2 != audioBusIndexByName || num2 == 0)
					{
						component._audioBusName = audioBusNames[num2];
					}
				}
				if (component._audioBusName != null && component._audioBusName.Length > 0 && component._audioBusName != "None")
				{
					GUI.enabled = false;
				}
				if (!component.IsTopNode)
				{
					EditorGUILayout.PropertyField(overrideAudioMixerGroup, new GUIContent("Override Mixer Group"));
				}
				if (overrideAudioMixerGroup.boolValue || component.IsTopNode)
				{
					EditorGUILayout.PropertyField(audioMixerGroup, new GUIContent("AudioMixer Group: "));
				}
				GUI.enabled = true;
				IsPropertyLinkedWithRTP.CheckProperty(component, "Priority");
				EditorGUILayout.IntSlider(priority, 0, 255, new GUIContent("Priority:"));
				IsPropertyLinkedWithRTP.EndCheck();
				if (!component.IsTopNode)
				{
					EditorGUILayout.PropertyField(overrideFadeProperties, new GUIContent("Override Fade Properties"));
				}
				EditorGUILayout.Slider(fadeInTime, 0f, 100f, new GUIContent("FadeInTime:"));
				EditorGUILayout.Slider(fadeInCurve, 0f, 1f, new GUIContent("FadeInCurve:"));
				EditorGUILayout.Slider(fadeOutTime, 0f, 100f, new GUIContent("FadeOutTime:"));
				EditorGUILayout.Slider(fadeOutCurve, 0f, 1f, new GUIContent("FadeOutCurve:"));
				if (!component.IsTopNode)
				{
					EditorGUILayout.PropertyField(overrideParentVolume, new GUIContent("Override Volume"));
				}
				IsPropertyLinkedWithRTP.CheckProperty(component, "Volume");
				DisplayDecibelsVolumeSlider("Volume (dB):", volume);
				IsPropertyLinkedWithRTP.EndCheck();
				DisplayDecibelsVolumeSlider("Volume Rand (dB):", volumeRandomization, positive_dB: true);
				if (!component.IsTopNode)
				{
					EditorGUILayout.PropertyField(overrideParentPitch, new GUIContent("Override Pitch"));
				}
				IsPropertyLinkedWithRTP.CheckProperty(component, "Pitch");
				DisplaySemitonesPitchSlider("Pitch (Semitones):", pitch);
				IsPropertyLinkedWithRTP.EndCheck();
				DisplayRandSemitonesPitchSlider("Pitch Rand (+/-):", pitchRandomization);
				EditorGUILayout.Slider(reverbZoneMix, 0f, 1.1f, new GUIContent("Reverb Zone Mix:"));
				if (!component.IsTopNode)
				{
					EditorGUILayout.PropertyField(override2DProperties, new GUIContent("Override 2D Properties"));
				}
				if (component.Override2DProperties || component.IsTopNode)
				{
					IsPropertyLinkedWithRTP.CheckProperty(component, "Pan2D");
					EditorGUILayout.Slider(pan2D, -1f, 1f, new GUIContent("Pan Stereo:"));
					IsPropertyLinkedWithRTP.EndCheck();
					EditorGUILayout.Slider(pan2DRandomization, 0f, 1f, new GUIContent("Pan Stereo Rand (+/-): "));
				}
				if (!component.IsTopNode)
				{
					EditorGUILayout.PropertyField(override3DProperties, new GUIContent("Override 3D Properties"));
				}
				if (component.Override3DProperties || component.IsTopNode)
				{
					IsPropertyLinkedWithRTP.CheckProperty(component, "PanLevel");
					EditorGUILayout.Slider(panLevel, 0f, 1f, new GUIContent("Spatial Blend (2D/3D):"));
					IsPropertyLinkedWithRTP.EndCheck();
					IsPropertyLinkedWithRTP.CheckProperty(component, "SpreadLevel");
					EditorGUILayout.Slider(spreadLevel, 0f, 360f, new GUIContent("Spread Level:"));
					IsPropertyLinkedWithRTP.EndCheck();
					IsPropertyLinkedWithRTP.CheckProperty(component, "DopplerLevel");
					EditorGUILayout.Slider(dopplerLevel, 0f, 5f, new GUIContent("Doppler Level:"));
					IsPropertyLinkedWithRTP.EndCheck();
					_ = component._customCurvesType;
					EditorGUILayout.PropertyField(customCurvesType, new GUIContent("Custom Curves Type: "));
					GUILayout.Space(1f);
					GUILayout.BeginVertical("box");
					if (customCurvesType.enumValueIndex == 0)
					{
						EditorGUILayout.PropertyField(minDistance, new GUIContent("Min Distance:"));
						EditorGUILayout.PropertyField(maxDistance, new GUIContent("Max Distance:"));
						EditorGUILayout.PropertyField(rolloffMode, new GUIContent("RolloffMode:"));
					}
					else if (customCurvesType.enumValueIndex == 1)
					{
						CustomCurvesManager customCurvesManager = FabricManager.Instance._customCurvesManager;
						int curveIndexByName = customCurvesManager.GetCurveIndexByName(component._customCurvesName);
						GUILayout.BeginHorizontal();
						curveIndexByName = EditorGUILayout.Popup("Custom Curves: ", curveIndexByName, customCurvesManager.GetNames());
						component._customCurvesName = customCurvesManager.GetCurveNameByIndex(curveIndexByName);
						if (component._customCurvesName == null)
						{
							GUI.enabled = false;
						}
						if (GUILayout.Button("Edit"))
						{
							CustomCurvesEditor.Open();
						}
						GUI.enabled = true;
						GUILayout.EndHorizontal();
					}
					GUILayout.EndVertical();
				}
				GUILayout.Space(1f);
				if (!component.IsTopNode)
				{
					EditorGUILayout.PropertyField(overrideBypassProperties, new GUIContent("Override Bypass Props:"));
				}
				if (component.OverrideBypassProperties || component.IsTopNode)
				{
					EditorGUILayout.PropertyField(bypassEffect, new GUIContent("Bypass Effect:"));
					EditorGUILayout.PropertyField(bypassListenerEffect, new GUIContent("Bypass Listener Effect:"));
					EditorGUILayout.PropertyField(bypassReverbZones, new GUIContent("Bypass Reverb Zones:"));
				}
				if (!component.IsTopNode)
				{
					EditorGUILayout.PropertyField(overrideSpatializeProperty, new GUIContent("Override Spatialize:"));
				}
				if (component.OverrideSpatializeProperties || component.IsTopNode)
				{
					EditorGUILayout.PropertyField(spatialize, new GUIContent("Spatialize:"));
				}
				if (AllowMusicSettingsOverride(component))
				{
					GUILayout.Space(5f);
					if (!component.IsTopNode)
					{
						EditorGUILayout.PropertyField(overrideMusicProperties, new GUIContent("Override Music Properties"));
					}
					if (overrideMusicProperties.boolValue || component.IsTopNode)
					{
						DisplayMusicTimeSettings();
					}
				}
				else if (IsMusicEnabledInComponent(component))
				{
					DisplayMusicTimeSettings();
				}
				DisplayMIDIProperties(component);
				GUILayout.Space(10f);
				GUIStyle gUIStyle = new GUIStyle();
				string text;
				if (component.IsComponentActive())
				{
					gUIStyle.normal.textColor = Color.green;
					text = "Active";
				}
				else
				{
					gUIStyle.normal.textColor = Color.red;
					text = "Inactive";
				}
				GUILayout.BeginHorizontal();
				GUILayout.Label("Component is:");
				GUILayout.Box(text, gUIStyle);
				GUILayout.EndHorizontal();
				GUILayout.Space(10f);
				GUILayout.BeginVertical("Node Info", "Box");
				GUILayout.Space(10f);
				if (component.ParentGameObject != null && component.HasEventListener)
				{
					GUILayout.Space(5f);
					EditorGUILayout.ObjectField("Parent GameObject:", component.ParentGameObject, typeof(GameObject), true);
					GUILayout.Space(2f);
				}
				if (component.HasEventListener)
				{
					GUILayout.Label("Instances: " + component.GetNumActiveComponentInstances() + " (" + component.MaxInstances + ")");
					GUILayout.Label("Virtual Instances: " + component.GetNumVirtualEventInstances());
				}
				GUILayout.Label("Volume: " + component.UpdateContext._volume);
				GUILayout.Label("Volume offset: " + component.VolumeOffset);
				GUILayout.Label("Fade: " + component._fadeParameter.GetCurrentValue().ToString("F2") + "(" + component.UpdateContext._fadeParameter.ToString("F2") + ")");
				GUILayout.Label("Pitch: " + component.UpdateContext._pitch);
				GUILayout.Label("Pitch offset: " + component.PitchOffset);
				GUILayout.Label("Pan2D offset: " + component.Pan2dOffset);
				GUILayout.Label("InstanceID: " + component.gameObject.GetEntityId());
				if (component.GetType() != typeof(AudioComponent))
				{
					GUILayout.Space(5f);
					GUILayout.Label("Status: [ " + component._componentStatus.ToString() + " ]");
					GUILayout.Space(5f);
				}
				if (component.profiler.msPerFrame > 10f)
				{
					gUIStyle.normal.textColor = Color.red;
				}
				else
				{
					gUIStyle.normal.textColor = Color.green;
				}
				GUILayout.Label("CPU: " + component.profiler.msPerFrame.ToString("0.000") + "ms ( Max:" + component.profiler.maxMsPerFrame.ToString("0.000") + "ms )", gUIStyle);
				GUILayout.EndVertical();
				GUILayout.Space(5f);
				if (component.GetType() == typeof(RandomAudioClipComponent))
				{
					Selection.GetFiltered(typeof(AudioClip), SelectionMode.DeepAssets);
					GUILayout.BeginVertical();
					result = DropAudioClipAreaGUI(component as RandomAudioClipComponent);
					GUILayout.EndVertical();
				}
				else if (component.GetType() != typeof(AudioComponent) && component.GetType() != typeof(DialogAudioComponent) && component.GetType() != typeof(WwwAudioComponent))
				{
					Selection.GetFiltered(typeof(AudioClip), SelectionMode.DeepAssets);
					GUILayout.BeginVertical();
					result = DropAreaGUI(component);
					GUILayout.Space(5f);
					GUILayout.BeginHorizontal();
					GUILayout.Label("Create as: ", GUILayout.MaxWidth(150f));
					audioComponentType = (AudioComponentImportType)(object)EditorGUILayout.EnumPopup("", audioComponentType);
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
					GUILayout.Label("Create EventListener: ", GUILayout.MaxWidth(150f));
					addEventListener = GUILayout.Toggle(addEventListener, "");
					GUILayout.EndHorizontal();
					Color backgroundColor = GUI.backgroundColor;
					GUI.backgroundColor = Color.green;
					DrawLine();
					GUI.backgroundColor = backgroundColor;
					GUILayout.EndVertical();
				}
				GUILayout.Space(5f);
				GUILayout.EndVertical();
			}
			GUIHelpers.CheckGUIHasChanged(serializedObject, component);
			return result;
		}

		private void ResetMusicTimeSettings()
		{
			musicTimeSettingsindex.intValue = -1;
		}

		public void DisplayMusicTimeSettings()
		{
			GUILayout.BeginVertical("Box");
			int num = GlobalUIs.DrawMusicSelection(musicTimeSettingsindex.intValue);
			if (num >= 0)
			{
				musicTimeSettingsindex.intValue = num;
				if (musicTimeSettingsindex.intValue == 0)
				{
					float bpm = musicTempo.floatValue;
					int timeSignatureLower = musicTimeSignatureLower.intValue;
					int timeSignatureUpper = musicTimeSignatureUpper.intValue;
					GlobalUIs.DrawMusicTimingSettings(null, ref bpm, ref timeSignatureLower, ref timeSignatureUpper);
					musicTempo.floatValue = bpm;
					musicTimeSignatureLower.intValue = timeSignatureLower;
					musicTimeSignatureUpper.intValue = timeSignatureUpper;
				}
			}
			EditorGUILayout.PropertyField(musicTimeResetOnPlay, new GUIContent("Reset OnPlay"));
			GUILayout.EndVertical();
		}

		public static bool AllowMusicSettingsOverride(Component component)
		{
			return false;
		}

		public static bool IsMusicEnabledInTheHierarchy(Component component, int depth = 0)
		{
			if (component != null && component.gameObject.transform.parent != null && (bool)component.gameObject.transform.parent.gameObject)
			{
				Component component2 = component.gameObject.transform.parent.gameObject.GetComponent<Component>();
				if (component2 != null && IsMusicEnabledInTheHierarchy(component2, ++depth))
				{
					return true;
				}
			}
			if (depth >= 1)
			{
				return IsMusicEnabledInComponent(component);
			}
			return false;
		}

		public static bool IsMusicEnabledInComponent(Component component)
		{
			bool result = false;
			MusicComponent musicComponent = component as MusicComponent;
			SequenceComponent sequenceComponent = component as SequenceComponent;
			SwitchComponent switchComponent = component as SwitchComponent;
			MIDIComponent mIDIComponent = component as MIDIComponent;
			if (musicComponent != null)
			{
				result = true;
			}
			else if (sequenceComponent != null)
			{
				if (sequenceComponent._sequenceType == SequenceComponentType.PlayOnAdvance && sequenceComponent._sequenceAdvanceMode == SequenceComponentAdvanceMode.OnMusicSync)
				{
					result = true;
				}
			}
			else if (switchComponent != null)
			{
				if (switchComponent._switchComponentSwitchType == SwitchComponentSwitchType.OnMusicSync)
				{
					result = true;
				}
			}
			else if (mIDIComponent != null)
			{
				result = true;
			}
			return result;
		}

		public static bool IsMIDIEnabledInTheHierarchy(Component component, int depth = 0)
		{
			if (component != null && component.ParentComponent != null)
			{
				Component component2 = component.ParentComponent.gameObject.GetComponent<Component>();
				if (component2 != null && IsMIDIEnabledInTheHierarchy(component2, ++depth))
				{
					return true;
				}
			}
			if (depth >= 1)
			{
				if (!component.gameObject.GetComponent<MIDIComponent>())
				{
					return false;
				}
				return true;
			}
			return false;
		}

		public void DisplayMIDIProperties(Component component)
		{
			string[] displayedOptions = MIDIComponentEditor.MidiNotes();
			GUILayout.Space(10f);
			GUILayout.BeginVertical("MIDI Properties", "box");
			GUILayout.Space(15f);
			component._midiProperties._overrideParentNoteTracking = EditorGUILayout.Toggle(new GUIContent("Override Note Tracking:"), component._midiProperties._overrideParentNoteTracking);
			if (component._midiProperties._overrideParentNoteTracking)
			{
				component._midiProperties._noteTracking = EditorGUILayout.Toggle(new GUIContent("Enable Note Tracking:"), component._midiProperties._noteTracking);
				_ = component._midiProperties._rootNote;
				component._midiProperties._rootNote = EditorGUILayout.Popup("Root Note:", Math.Abs(component._midiProperties._rootNote), displayedOptions) * -1;
			}
			GUILayout.Space(3f);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Transpose Offset:", GUILayout.MaxWidth(110f));
			component._midiProperties._transpose = EditorGUILayout.IntSlider("", component._midiProperties._transpose, -24, 24);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Velocity Offset:", GUILayout.MaxWidth(110f));
			component._midiProperties._velocity = EditorGUILayout.IntSlider("", component._midiProperties._velocity, -127, 127);
			GUILayout.EndHorizontal();
			GUILayout.Space(3f);
			GUILayout.BeginHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Key Min:", GUILayout.MaxWidth(100f));
			component._midiProperties._keyRangeMin = EditorGUILayout.Popup("", component._midiProperties._keyRangeMin, displayedOptions, GUILayout.MaxWidth(60f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Key Max:", GUILayout.MaxWidth(100f));
			component._midiProperties._keyRangeMax = EditorGUILayout.Popup("", component._midiProperties._keyRangeMax, displayedOptions, GUILayout.MaxWidth(60f));
			GUILayout.EndHorizontal();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Velocity Min:", GUILayout.MaxWidth(100f));
			component._midiProperties._velocityRangeMin = EditorGUILayout.IntField("", component._midiProperties._velocityRangeMin, GUILayout.MaxWidth(60f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Velocity Max:", GUILayout.MaxWidth(100f));
			component._midiProperties._velocityRangeMax = EditorGUILayout.IntField("", component._midiProperties._velocityRangeMax, GUILayout.MaxWidth(60f));
			GUILayout.EndHorizontal();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Channel Min:", GUILayout.MaxWidth(100f));
			component._midiProperties._channelRangeMin = EditorGUILayout.IntField("", component._midiProperties._channelRangeMin, GUILayout.MaxWidth(60f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Channel Max:", GUILayout.MaxWidth(100f));
			component._midiProperties._channelRangeMax = EditorGUILayout.IntField("", component._midiProperties._channelRangeMax, GUILayout.MaxWidth(60f));
			GUILayout.EndHorizontal();
			GUILayout.EndHorizontal();
			if (GUILayout.Button("Arrange Children"))
			{
				Component[] componentsInChildren = component.gameObject.GetComponentsInChildren<Component>();
				Array.Sort(componentsInChildren, (Component x, Component y) => string.Compare(x.name, y.name));
				for (int num = 0; num < componentsInChildren.Length; num++)
				{
					componentsInChildren[num]._midiProperties._keyRangeMin = component._midiProperties._keyRangeMin + num;
					componentsInChildren[num]._midiProperties._keyRangeMax = component._midiProperties._keyRangeMin + num;
				}
			}
			GUILayout.EndVertical();
		}

		public static void DisplaySemitonesPitchSlider(string label, SerializedProperty pitch, int min = -36, int max = 36)
		{
			int value = (int)(12f * Mathf.Log(pitch.floatValue, 2f));
			value = EditorGUILayout.IntSlider(label, value, min, max);
			float f = Mathf.Pow(2f, 1f / 12f);
			pitch.floatValue = Mathf.Pow(f, value);
		}

		public static float DisplaySemitonesPitchSlider(string label, float pitch, int min = -36, int max = 36)
		{
			float num = 12f * Mathf.Log(pitch, 2f);
			num = (float)Math.Round(num, 2);
			num = EditorGUILayout.Slider(label, num, min, max);
			float f = Mathf.Pow(2f, 1f / 12f);
			return Mathf.Pow(f, num);
		}

		public static void DisplayRandSemitonesPitchSlider(string label, SerializedProperty pitch)
		{
			float f = pitch.floatValue + 1f;
			float num = 12f * Mathf.Log(f, 2f);
			num = (float)Math.Round(num, 2);
			num = EditorGUILayout.Slider(label, num, 0f, 36f);
			float f2 = Mathf.Pow(2f, 1f / 12f);
			pitch.floatValue = Mathf.Pow(f2, num) - 1f;
		}

		public static void DisplayDecibelsVolumeSlider(string label, SerializedProperty volume, bool positive_dB = false)
		{
			bool flag = true;
			if (positive_dB)
			{
				float linear = 1f - volume.floatValue;
				float value = AudioTools.LinearToDB(linear) * -1f;
				value = EditorGUILayout.Slider(label, value, 0f, 96f);
				linear = AudioTools.DBToLinear(value * -1f);
				if (flag)
				{
					volume.floatValue = 1f - linear;
				}
				return;
			}
			float value2 = AudioTools.LinearToDB(volume.floatValue);
			value2 = EditorGUILayout.Slider(label, value2, -64f, 0f);
			if (value2 <= -64f)
			{
				if (flag)
				{
					volume.floatValue = 0f;
				}
			}
			else if (flag)
			{
				volume.floatValue = AudioTools.DBToLinear(value2);
			}
		}

		public static float DisplayDecibelsVolumeSlider(string label, float volume, bool positive_dB = false)
		{
			if (positive_dB)
			{
				float linear = 1f - volume;
				float value = AudioTools.LinearToDB(linear) * -1f;
				value = EditorGUILayout.Slider(label, value, 0f, 96f);
				linear = AudioTools.DBToLinear(value * -1f);
				volume = 1f - linear;
			}
			else
			{
				float value2 = AudioTools.LinearToDB(volume);
				value2 = EditorGUILayout.Slider(label, value2, -64f, 0f);
				volume = ((!(value2 <= -64f)) ? AudioTools.DBToLinear(value2) : 0f);
			}
			return volume;
		}

		private void DrawLine()
		{
			GUILayout.Box("", GUILayout.ExpandWidth(expand: true), GUILayout.Height(1f));
		}

		private void AddRandomComponent(Component component, ref List<GameObject> randomComponent)
		{
			int num = randomComponent[0].name.LastIndexOf("_");
			string name = randomComponent[0].name.Remove(num, randomComponent[0].name.Length - num);
			GameObject gameObject = new GameObject(name);
			gameObject.transform.parent = component.transform;
			RandomComponent randomComponent2 = gameObject.AddComponent<RandomComponent>();
			for (int i = 0; i < randomComponent.Count; i++)
			{
				randomComponent[i].transform.parent = randomComponent2.transform;
			}
			randomComponent.Clear();
		}

		private void ImportAudioClips(UnityEngine.Object[] audioClips, Component component)
		{
			List<GameObject> randomComponent = null;
			if (audioComponentType == AudioComponentImportType.AudioComponent_AutoDetect_RandomComponent)
			{
				randomComponent = new List<GameObject>();
				Array.Sort(audioClips, (UnityEngine.Object x, UnityEngine.Object y) => string.Compare(x.name, y.name));
			}
			for (int num = 0; num < audioClips.Length; num++)
			{
				GameObject gameObject = new GameObject(audioClips[num].name);
				gameObject.transform.parent = component.transform;
				if (audioComponentType == AudioComponentImportType.AudioComponent || audioComponentType == AudioComponentImportType.RandomComponent || audioComponentType == AudioComponentImportType.AudioComponent_AutoDetect_RandomComponent)
				{
					AudioComponent audioComponent = gameObject.AddComponent<AudioComponent>();
					audioComponent.AudioClip = audioClips[num] as AudioClip;
				}
				else if (audioComponentType == AudioComponentImportType.DialogAudioComponent)
				{
					DialogAudioComponent dialogAudioComponent = gameObject.AddComponent<DialogAudioComponent>();
					dialogAudioComponent.AudioClipReference = audioClips[num].name;
				}
				if (addEventListener)
				{
					EventListener eventListener = gameObject.AddComponent<EventListener>();
					eventListener._eventName = audioClips[num].name.Replace("_", "/");
					EventManager.Instance._eventList.Add(eventListener._eventName);
				}
				if (audioComponentType != AudioComponentImportType.AudioComponent_AutoDetect_RandomComponent)
				{
					continue;
				}
				if (randomComponent.Count == 0)
				{
					randomComponent.Add(gameObject);
					continue;
				}
				GameObject gameObject2 = randomComponent[randomComponent.Count - 1];
				int num2 = gameObject2.name.LastIndexOf("_");
				if (num2 >= 0)
				{
					string value = gameObject2.name.Remove(num2, gameObject2.name.Length - num2);
					if (gameObject.name.Contains(value))
					{
						randomComponent.Add(gameObject);
						continue;
					}
					AddRandomComponent(component, ref randomComponent);
					randomComponent.Add(gameObject);
				}
				else
				{
					randomComponent.Clear();
				}
			}
			if (audioComponentType == AudioComponentImportType.AudioComponent_AutoDetect_RandomComponent && randomComponent.Count > 0)
			{
				AddRandomComponent(component, ref randomComponent);
			}
		}

		public bool DropAreaGUI(Component component)
		{
			bool result = false;
			UnityEngine.Event current = UnityEngine.Event.current;
			Color backgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = Color.green;
			DrawLine();
			Rect rect = GUILayoutUtility.GetRect(0f, 35f, GUILayout.ExpandWidth(expand: true));
			GUI.Box(rect, "Drag and Drop AudioClips here to automatically create Audio Components\n (Use the inspector lock option to maintain focus)");
			GUI.backgroundColor = Color.green;
			DrawLine();
			GUI.backgroundColor = backgroundColor;
			switch (current.type)
			{
			case EventType.DragUpdated:
			case EventType.DragPerform:
			{
				if (!rect.Contains(current.mousePosition))
				{
					return false;
				}
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				if (current.type != EventType.DragPerform)
				{
					break;
				}
				DragAndDrop.AcceptDrag();
				List<UnityEngine.Object> list = new List<UnityEngine.Object>();
				UnityEngine.Object[] objectReferences = DragAndDrop.objectReferences;
				foreach (UnityEngine.Object obj in objectReferences)
				{
					AudioClip audioClip = obj as AudioClip;
					if (audioClip != null)
					{
						list.Add(obj);
					}
				}
				if (list.Count > 0)
				{
					if (audioComponentType == AudioComponentImportType.RandomComponent)
					{
						GameObject gameObject = new GameObject("Random Component");
						gameObject.transform.parent = component.transform;
						component = gameObject.AddComponent<RandomComponent>();
					}
					ImportAudioClips(list.ToArray(), component);
					result = true;
				}
				break;
			}
			}
			return result;
		}

		public bool DropAudioClipAreaGUI(RandomAudioClipComponent component)
		{
			bool result = false;
			UnityEngine.Event current = UnityEngine.Event.current;
			Color backgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = Color.green;
			DrawLine();
			Rect rect = GUILayoutUtility.GetRect(0f, 35f, GUILayout.ExpandWidth(expand: true));
			GUI.Box(rect, "Drag and Drop AudioClips here to automatically populate audio clip array\n (Use the inspector lock option to maintain focus)");
			GUI.backgroundColor = Color.green;
			DrawLine();
			GUI.backgroundColor = backgroundColor;
			switch (current.type)
			{
			case EventType.DragUpdated:
			case EventType.DragPerform:
			{
				if (!rect.Contains(current.mousePosition))
				{
					return false;
				}
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				if (current.type != EventType.DragPerform)
				{
					break;
				}
				DragAndDrop.AcceptDrag();
				List<AudioClip> list = new List<AudioClip>();
				UnityEngine.Object[] objectReferences = DragAndDrop.objectReferences;
				foreach (UnityEngine.Object obj in objectReferences)
				{
					AudioClip audioClip = obj as AudioClip;
					if (audioClip != null)
					{
						list.Add(audioClip);
					}
				}
				if (list.Count > 0)
				{
					component._audioClips = list.ToArray();
					result = true;
				}
				break;
			}
			}
			return result;
		}

		private static void Callback(object obj)
		{
		}
	}
	public class MarkerRegionViewer : EditorWindow
	{
		private static AudioComponent _audioComponent;

		private static void Open(AudioComponent audioComponent)
		{
			_audioComponent = audioComponent;
			MarkerRegionViewer markerRegionViewer = (MarkerRegionViewer)EditorWindow.GetWindow(typeof(MarkerRegionViewer));
			markerRegionViewer.titleContent = new GUIContent("Marker Viewer");
		}

		private void OnGUI()
		{
			if (GUILayout.Button("Close"))
			{
				Close();
			}
		}
	}
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SilentComponent))]
	public class SilentComponentEditor : Editor
	{
		[MenuItem("Fabric/Components/SilentComponent")]
		private static void MenuItenAdd()
		{
			GameObject activeGameObject = Selection.activeGameObject;
			if (!(activeGameObject == null))
			{
				GameObject gameObject = new GameObject("SilentComponent");
				gameObject.transform.parent = activeGameObject.transform;
				gameObject.AddComponent<SilentComponent>();
			}
		}

		public override void OnInspectorGUI()
		{
			MenuBar.OnGUI("288071-silentcomponent", box: true);
			GUILayout.BeginVertical("Box");
			base.OnInspectorGUI();
			GUILayout.EndVertical();
		}
	}
	[CanEditMultipleObjects]
	[CustomEditor(typeof(AudioComponent))]
	public class AudioComponentEditor : Editor
	{
		private SerializedProperty dontPlay;

		private SerializedProperty delay;

		private SerializedProperty delayInBeats;

		private SerializedProperty dontStopOnDestroy;

		private SerializedProperty loop;

		private SerializedProperty ignoreNativeLoop;

		private SerializedProperty randomizeStart;

		private SerializedProperty randomizeStartPercentage;

		private SerializedProperty numLoops;

		private SerializedProperty randomizePosition;

		private SerializedProperty randomizeMinPosition;

		private SerializedProperty randomizeMaxPosition;

		private SerializedProperty ignoreVirtualization;

		private SerializedProperty audioClip;

		private SerializedProperty dynamicAudioClipLoading;

		private SerializedProperty dynamicAsyncAudioClipLoading;

		private SerializedProperty audioClipAssetPath;

		private SerializedProperty syncWithMusicTime;

		private SerializedProperty musicTimeSettingsindex;

		private SerializedProperty UseLoopMarkers;

		private SerializedProperty randomizeStartLoopMarkerIndex;

		private SerializedProperty randomizeEndLoopMarkerIndex;

		private SerializedProperty randomizeRegionIndex;

		private SerializedProperty musicTimeResetOnPlay;

		private SerializedProperty syncRegionsWithTempo;

		private static string[] loopOptions = new string[2] { "Num Of Loops: ", "Infinite" };

		private float[] audioData;

		private bool _foldout = true;

		private int loopSelection = 1;

		private AudioComponent audioComponent;

		private ComponentEditor componentEditor = new ComponentEditor();

		private WaveformDisplay waveformDisplay = new WaveformDisplay();

		private GameObject audioClipPreviewer;

		private AudioSource audioSourcePreviewer;

		private Vector2 markersScrollView;

		private Vector2 regionsScrollView;

		[MenuItem("Fabric/Components/AudioComponent")]
		private static void About()
		{
			GameObject activeGameObject = Selection.activeGameObject;
			if (!(activeGameObject == null))
			{
				GameObject gameObject = new GameObject("AudioComponent");
				gameObject.transform.parent = activeGameObject.transform;
				gameObject.AddComponent<AudioComponent>();
			}
		}

		private void OnEnable()
		{
			audioComponent = base.target as AudioComponent;
			componentEditor.RegisterSerialisableObject(base.serializedObject);
			waveformDisplay.audioComponent = audioComponent;
			dontPlay = base.serializedObject.FindProperty("_dontPlay");
			delay = base.serializedObject.FindProperty("_delay");
			delayInBeats = base.serializedObject.FindProperty("_delayInBeats");
			dontStopOnDestroy = base.serializedObject.FindProperty("_dontStopOnDestroy");
			loop = base.serializedObject.FindProperty("_loop");
			ignoreNativeLoop = base.serializedObject.FindProperty("_ignoreNativeLoop");
			randomizeStart = base.serializedObject.FindProperty("_randomizeStart");
			randomizeStartPercentage = base.serializedObject.FindProperty("_randomizeStartPercentage");
			numLoops = base.serializedObject.FindProperty("_numLoops");
			randomizePosition = base.serializedObject.FindProperty("_randomizePosition");
			randomizeMinPosition = base.serializedObject.FindProperty("_randomizeMinPosition");
			randomizeMaxPosition = base.serializedObject.FindProperty("_randomizeMaxPosition");
			ignoreVirtualization = base.serializedObject.FindProperty("_ignoreVirtualization");
			audioClip = base.serializedObject.FindProperty("_audioClip");
			dynamicAudioClipLoading = base.serializedObject.FindProperty("_dynamicAudioClipLoading");
			dynamicAsyncAudioClipLoading = base.serializedObject.FindProperty("_dynamicAsyncAudioClipLoading");
			audioClipAssetPath = base.serializedObject.FindProperty("_audioClipAssetPath");
			syncWithMusicTime = base.serializedObject.FindProperty("_syncWithMusicTime");
			musicTimeSettingsindex = base.serializedObject.FindProperty("_musicTimeSettingsIndex");
			UseLoopMarkers = base.serializedObject.FindProperty("_useLoopMarkers");
			randomizeStartLoopMarkerIndex = base.serializedObject.FindProperty("_randomizeStartLoopMarkerIndex");
			randomizeEndLoopMarkerIndex = base.serializedObject.FindProperty("_randomizeEndLoopMarkerIndex");
			randomizeRegionIndex = base.serializedObject.FindProperty("_randomizeRegionIndex");
			musicTimeResetOnPlay = base.serializedObject.FindProperty("_musicTimeResetOnPlay");
			syncRegionsWithTempo = base.serializedObject.FindProperty("_syncRegionsWithTempo");
			GUIHelpers.RegisterEditorUpdate(base.Repaint);
		}

		private void OnDisable()
		{
			GUIHelpers.UnregisterEditorUpdate(base.Repaint);
		}

		public void OnDestroy()
		{
			if (audioClipPreviewer != null)
			{
				UnityEngine.Object.DestroyImmediate(audioClipPreviewer);
				audioClipPreviewer = null;
			}
		}

		private void OnFocus()
		{
		}

		public override void OnInspectorGUI()
		{
			componentEditor.InspectorGUI(audioComponent, "288034-audiocomponent");
			base.serializedObject.Update();
			_foldout = EditorGUILayout.Foldout(_foldout, "Audio Component Properties");
			if (!_foldout)
			{
				return;
			}
			GUILayout.BeginVertical("box");
			EditorGUILayout.PropertyField(ignoreVirtualization, new GUIContent("Ignore Virtualization:"));
			EditorGUILayout.PropertyField(dontPlay, new GUIContent("Dont Play:"));
			EditorGUILayout.Slider(delay, 0f, 600f, new GUIContent("Delay:"));
			EditorGUILayout.PropertyField(dontStopOnDestroy, new GUIContent("Dont Stop On Destroy:"));
			if (audioComponent.DontStopOnDestroy)
			{
				audioComponent.Loop = false;
			}
			else
			{
				EditorGUILayout.PropertyField(loop, new GUIContent("Loop:"));
				EditorGUILayout.PropertyField(ignoreNativeLoop, new GUIContent("Ignore Native Loop:"));
				GUILayout.BeginHorizontal();
				EditorGUILayout.PropertyField(randomizeStart, new GUIContent("Randomize Start: "));
				if (!randomizeStart.boolValue)
				{
					GUI.enabled = false;
				}
				EditorGUILayout.PropertyField(randomizeStartPercentage, new GUIContent("Percentage: "));
				GUI.enabled = true;
				GUILayout.EndHorizontal();
				GUILayout.BeginVertical("box");
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Load Markers/Regions"))
				{
					LoadMarkers();
				}
				if (GUILayout.Button("Unload Markers/Regions"))
				{
					audioComponent._markers.Clear();
					audioComponent._regions.Clear();
					audioComponent._loopMarkersLoaded = false;
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(5f);
				if (!audioComponent._loopMarkersLoaded)
				{
					GUI.enabled = false;
				}
				GUILayout.BeginVertical("Box");
				GUILayout.Label("Markers (" + audioComponent._markers.Count + ")");
				int num = ((audioComponent._markers.Count > 0) ? Math.Min(audioComponent._markers.Count * 20, 100) : 0);
				markersScrollView = GUILayout.BeginScrollView(markersScrollView, GUILayout.Height(num));
				for (int i = 0; i < audioComponent._markers.Count; i++)
				{
					Marker marker = audioComponent._markers[i];
					GUILayout.BeginHorizontal();
					GUILayout.Label("  |-- " + marker.name, GUILayout.MinWidth(100f));
					GUILayout.Label("Type:", GUILayout.MaxWidth(40f));
					marker.type = (MarkerType)(object)EditorGUILayout.EnumPopup("", marker.type);
					GUILayout.EndHorizontal();
				}
				GUILayout.EndScrollView();
				GUILayout.Space(3f);
				if (!audioComponent._loopMarkersLoaded)
				{
					GUI.enabled = false;
				}
				EditorGUILayout.PropertyField(UseLoopMarkers, new GUIContent("Loop Markers:"));
				if (UseLoopMarkers.boolValue)
				{
					string[] array = new string[audioComponent._markers.Count];
					for (int j = 0; j < array.Length; j++)
					{
						array[j] = audioComponent._markers[j].name;
					}
					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(randomizeStartLoopMarkerIndex, new GUIContent("Randomize Start Marker:"));
					EditorGUILayout.PropertyField(randomizeEndLoopMarkerIndex, new GUIContent("Randomize End Marker:"));
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
					GUILayout.BeginHorizontal();
					GUILayout.Label("Start Loop: ", GUILayout.MaxWidth(80f));
					int num2 = EditorGUILayout.Popup(audioComponent._startLoopMarkerIndex, array);
					if (num2 < audioComponent._endLoopMarkerIndex)
					{
						audioComponent._startLoopMarkerIndex = num2;
					}
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
					GUILayout.Label("End Loop: ", GUILayout.MaxWidth(80f));
					int num3 = EditorGUILayout.Popup(audioComponent._endLoopMarkerIndex, array);
					if (num3 > audioComponent._startLoopMarkerIndex)
					{
						audioComponent._endLoopMarkerIndex = num3;
					}
					GUILayout.EndHorizontal();
					GUILayout.EndHorizontal();
				}
				else
				{
					if (randomizeStart.boolValue)
					{
						GUI.enabled = false;
					}
					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(randomizeStartLoopMarkerIndex, new GUIContent("Randomize Start: "));
					GUILayout.EndHorizontal();
					GUI.enabled = false;
				}
				GUILayout.Space(3f);
				GUIHelpers.DrawLine(Color.gray);
				GUILayout.Space(3f);
				GUILayout.Label("Regions (" + audioComponent._regions.Count + ")");
				GUI.enabled = true;
				if (audioComponent._regions.Count == 0)
				{
					GUI.enabled = false;
				}
				int num4 = ((audioComponent._regions.Count > 0) ? Math.Min(audioComponent._regions.Count * 20, 100) : 0);
				regionsScrollView = GUILayout.BeginScrollView(regionsScrollView, GUILayout.Height(num4));
				for (int k = 0; k < audioComponent._regions.Count; k++)
				{
					Region region = audioComponent._regions[k];
					GUILayout.BeginHorizontal();
					GUILayout.Label("  |-- " + region.name);
					GUILayout.EndHorizontal();
				}
				GUILayout.EndScrollView();
				if (!UseLoopMarkers.boolValue)
				{
					string[] array2 = new string[audioComponent._regions.Count];
					for (int l = 0; l < array2.Length; l++)
					{
						array2[l] = audioComponent._regions[l].name;
					}
					EditorGUILayout.PropertyField(randomizeRegionIndex, new GUIContent("Randomize Region:"));
					GUILayout.BeginHorizontal();
					GUILayout.Label("Loop Region: ", GUILayout.MaxWidth(80f));
					audioComponent._loopRegionIndex = EditorGUILayout.Popup(audioComponent._loopRegionIndex, array2);
					GUILayout.EndHorizontal();
				}
				EditorGUILayout.PropertyField(syncRegionsWithTempo, new GUIContent("Sync Regions With Tempo:"));
				GUI.enabled = syncRegionsWithTempo.boolValue && audioComponent._regions.Count > 0;
				EditorGUILayout.FloatField("Audio Clip Tempo: ", audioComponent._audioClipTempo);
				GUI.enabled = true;
				GUILayout.EndVertical();
				GUI.enabled = true;
				GUILayout.BeginHorizontal();
				loopSelection = ((numLoops.intValue <= 0) ? 1 : 0);
				loopSelection = GUILayout.SelectionGrid(loopSelection, loopOptions, 1, EditorStyles.radioButton);
				if (loopSelection == 1)
				{
					GUI.enabled = false;
					numLoops.intValue = -1;
				}
				else if (numLoops.intValue == -1)
				{
					numLoops.intValue = 1;
				}
				int num5 = EditorGUILayout.IntField("", numLoops.intValue, GUILayout.MaxWidth(50f));
				if (loopSelection == 0 && num5 < 1)
				{
					num5 = 1;
				}
				EditorGUILayout.LabelField("[ " + audioComponent._numLoopsLeft + " ]");
				if (loopSelection == 1)
				{
					GUI.enabled = true;
				}
				GUILayout.EndHorizontal();
				if (!audioComponent._loopMarkersLoaded)
				{
					if (numLoops.intValue != num5 && num5 > 0 && audioComponent.AudioClip != null)
					{
						audioComponent._markers.Clear();
						Marker marker2 = new Marker();
						marker2.offsetTime = 0f;
						Marker marker3 = new Marker();
						marker3.offsetTime = audioComponent.AudioClip.length;
						audioComponent._markers.Add(marker2);
						audioComponent._markers.Add(marker3);
					}
					else if (audioComponent._markers.Count == 2)
					{
						audioComponent._markers.Clear();
					}
				}
				numLoops.intValue = num5;
				GUILayout.EndVertical();
			}
			GUILayout.Space(5f);
			waveformDisplay.DrawWaveform();
			if (audioClipPreviewer != null && audioSourcePreviewer.isPlaying)
			{
				if (GUILayout.Button("Stop AudioClip"))
				{
					AudioClipPreviewerStop();
				}
			}
			else if (GUILayout.Button("Play AudioClip"))
			{
				AudioClipPreviewerPlay();
			}
			GUILayout.Space(5f);
			GUILayout.BeginVertical("Box");
			EditorGUILayout.PropertyField(randomizePosition, new GUIContent("Randomize 3D Position:"));
			if (!randomizePosition.boolValue)
			{
				GUI.enabled = false;
			}
			GUILayout.BeginHorizontal();
			EditorGUILayout.MinMaxSlider(new GUIContent("Min/Max 3D Position:"), ref audioComponent._randomizeMinPosition, ref audioComponent._randomizeMaxPosition, 0f, FabricEditorData.GetData()._maxRandomization3DDistance);
			audioComponent._randomizeMinPosition = EditorGUILayout.FloatField(audioComponent._randomizeMinPosition, GUILayout.MaxWidth(50f));
			audioComponent._randomizeMaxPosition = EditorGUILayout.FloatField(audioComponent._randomizeMaxPosition, GUILayout.MaxWidth(50f));
			GUILayout.EndHorizontal();
			FabricEditorData.GetData()._maxRandomization3DDistance = EditorGUILayout.FloatField("Max Range: ", FabricEditorData.GetData()._maxRandomization3DDistance);
			if (randomizeMinPosition.floatValue > randomizeMaxPosition.floatValue)
			{
				randomizeMinPosition.floatValue = randomizeMaxPosition.floatValue;
			}
			if (!randomizePosition.boolValue)
			{
				GUI.enabled = true;
			}
			GUILayout.EndVertical();
			GUILayout.Space(5f);
			GUILayout.BeginVertical("Box");
			EditorGUILayout.PropertyField(dynamicAudioClipLoading, new GUIContent("Dynamic AudioClip Loading: "));
			if (dynamicAudioClipLoading.boolValue)
			{
				audioComponent._audioClipHandle.UseAudioClipPath = EditorGUILayout.Toggle("Use AudioClip Path: ", audioComponent._audioClipHandle.UseAudioClipPath);
				if (audioComponent._audioClipHandle.UseAudioClipPath)
				{
					if (!SetAudioComponentAudioClipWithPath())
					{
						audioComponent._dynamicAudioClipLoading = true;
						GUILayout.EndVertical();
						return;
					}
				}
				else
				{
					SetAudioComponentAudioClip();
				}
			}
			else if (!dynamicAudioClipLoading.boolValue)
			{
				SetAudioComponentAudioClip();
			}
			GUILayout.EndVertical();
			GUILayout.Space(5f);
			GUILayout.BeginVertical("", "Box");
			if (!ComponentEditor.IsMusicEnabledInTheHierarchy(audioComponent))
			{
				syncWithMusicTime.boolValue = GUILayout.Toggle(syncWithMusicTime.boolValue, "Sync With Global Music Settings");
				if (syncWithMusicTime.boolValue && !audioComponent._overrideMusicTimeSettings)
				{
					if (FabricManager.Instance == null)
					{
						GUIStyle gUIStyle = new GUIStyle();
						gUIStyle.normal.textColor = Color.red;
						gUIStyle.alignment = TextAnchor.MiddleCenter;
						GUILayout.Label("Music Time Settings not available, Fabric manager is not present", gUIStyle);
					}
					else
					{
						string[] array3 = new string[FabricManager.Instance._musicTimeSignatures.Count];
						for (int m = 0; m < FabricManager.Instance._musicTimeSignatures.Count; m++)
						{
							array3[m] = FabricManager.Instance._musicTimeSignatures[m]._name;
						}
						musicTimeSettingsindex.intValue = EditorGUILayout.Popup("Music Time Setting: ", musicTimeSettingsindex.intValue, array3);
					}
					if (musicTimeSettingsindex.intValue >= 0 && musicTimeSettingsindex.intValue < FabricManager.Instance._musicTimeSignatures.Count)
					{
						FabricManagerEditor.MusicSyncToTarget(FabricManager.Instance._musicTimeSignatures[musicTimeSettingsindex.intValue]);
					}
					EditorGUILayout.PropertyField(musicTimeResetOnPlay, new GUIContent("Reset OnPlay"));
					EditorGUILayout.PropertyField(delayInBeats, new GUIContent("Delay In Beats"));
				}
			}
			else
			{
				if (syncWithMusicTime.boolValue)
				{
					syncWithMusicTime.boolValue = false;
					musicTimeSettingsindex.intValue = -1;
				}
				EditorGUILayout.PropertyField(delayInBeats, new GUIContent("Delay In Beats"));
			}
			GUILayout.EndVertical();
			GUILayout.Space(5f);
			GUILayout.Label("Status:                           [ " + audioComponent.CurrentState.ToString() + " ]");
			GUILayout.EndVertical();
			GUIHelpers.CheckGUIHasChanged(base.serializedObject, audioComponent);
		}

		private void LoadMarkers()
		{
			audioComponent._markers.Clear();
			string assetPath = AssetDatabase.GetAssetPath(audioComponent.AudioClip);
			if (assetPath.Length <= 0)
			{
				return;
			}
			WavReader wavReader = new WavReader();
			if (!wavReader.LoadFile(assetPath))
			{
				return;
			}
			if (wavReader.m_cuePoints.Count == 0 && wavReader.m_regions.Count == 0 && wavReader.m_sampleLoops.Count == 0)
			{
				EditorUtility.DisplayDialog("Loading markers failed", "The wavfile MUST have at least one marker, a region or a sample loop present", "Ok");
			}
			else
			{
				for (int i = 0; i < wavReader.m_cuePoints.Count; i++)
				{
					CuePoint cuePoint = wavReader.m_cuePoints[i];
					Marker marker = new Marker();
					marker.name = cuePoint.label;
					marker.offsetTime = (float)cuePoint.dwSampleOffset / (float)audioComponent.AudioClip.frequency;
					marker.offsetSamples = cuePoint.dwSampleOffset;
					audioComponent._markers.Add(marker);
				}
				if (audioComponent._markers.Count > 1)
				{
					audioComponent._loopMarkersLoaded = true;
				}
				for (int j = 0; j < wavReader.m_sampleLoops.Count; j++)
				{
					SampleLoop sampleLoop = wavReader.m_sampleLoops[j];
					Region region = new Region();
					region.name = sampleLoop.label;
					region.offsetTime = (float)sampleLoop.dwStart / (float)audioComponent.AudioClip.frequency;
					region.lengthTime = (float)sampleLoop.dwEnd / (float)audioComponent.AudioClip.frequency;
					audioComponent._regions.Add(region);
					audioComponent._loopMarkersLoaded = true;
				}
				for (int k = 0; k < wavReader.m_regions.Count; k++)
				{
					Fabric.Wav.Region region2 = wavReader.m_regions[k];
					Region region3 = new Region();
					region3.name = region2.cuepoint.label;
					region3.offsetTime = (float)region2.cuepoint.dwSampleOffset / (float)audioComponent.AudioClip.frequency;
					region3.lengthTime = (float)region2.length / (float)audioComponent.AudioClip.frequency;
					audioComponent._regions.Add(region3);
					audioComponent._loopMarkersLoaded = true;
				}
			}
			wavReader.CloseFile();
		}

		private void DragAndDropAudioClip(Rect drop_area, ref string audioClipPath)
		{
			UnityEngine.Event current = UnityEngine.Event.current;
			switch (current.type)
			{
			case EventType.DragUpdated:
			case EventType.DragPerform:
			{
				if (!drop_area.Contains(current.mousePosition))
				{
					break;
				}
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				if (current.type != EventType.DragPerform)
				{
					break;
				}
				DragAndDrop.AcceptDrag();
				UnityEngine.Object obj = DragAndDrop.objectReferences[0];
				if (!(obj != null))
				{
					break;
				}
				AudioClip audioClip = obj as AudioClip;
				if (audioClip != null)
				{
					audioClipPath = ProcessAudioClipPath(audioClip);
					if (audioClipPath == null)
					{
						EditorUtility.DisplayDialog("Fabric Error", "AudioClip not located in Resources, Data or StreamingAssets folders", "Ok");
					}
					waveformDisplay.waveformAudioClip = audioClip;
				}
				break;
			}
			}
		}

		private bool SetAudioComponentAudioClipWithPath()
		{
			EditorGUILayout.PropertyField(dynamicAsyncAudioClipLoading, new GUIContent("Async Loading: ", "Async loading of audio file, must be located outside the resources folder"));
			GUILayout.Space(2f);
			string text = null;
			if (!audioComponent._audioClipHandle.IsAudioClipPathSet())
			{
				text = ProcessAudioClipPath(audioComponent.AudioClip);
				if (text == null)
				{
					DisplayAudioClipNotValidDialog();
					return false;
				}
				audioComponent._audioClipHandle.SetAudioClipPath(text);
				waveformDisplay.SetWaveformAudioClip(audioComponent.AudioClip);
				audioComponent.AudioClip = null;
			}
			GUILayout.BeginHorizontal();
			GUILayout.Label("AudioClip (Resources): ", GUILayout.MaxWidth(150f));
			Rect rect = GUILayoutUtility.GetRect(100f, 20f, GUILayout.ExpandWidth(expand: true));
			string text2 = "Drop Audio Clip here!!";
			Color backgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = Color.red;
			if (audioComponent._audioClipHandle.IsAudioClipPathSet())
			{
				GUI.backgroundColor = Color.green;
				text2 = audioComponent._audioClipHandle.GetAudioClipPath();
			}
			GUI.Box(rect, text2);
			GUI.backgroundColor = backgroundColor;
			text = audioComponent._audioClipHandle.GetAudioClipPath();
			DragAndDropAudioClip(rect, ref text);
			audioComponent._audioClipHandle.SetAudioClipPath((text == null) ? "" : text);
			if (GUILayout.Button("Clear", GUILayout.MaxWidth(50f)))
			{
				audioComponent._audioClipHandle.SetAudioClipPath("");
				waveformDisplay.waveformAudioClip = null;
				waveformDisplay.SetWaveformAudioClip(audioComponent.AudioClip);
			}
			GUILayout.EndHorizontal();
			return true;
		}

		private void SetAudioComponentAudioClip()
		{
			if (audioComponent._audioClipHandle.IsAudioClipPathSet())
			{
				audioComponent.AudioClip = Resources.Load(audioComponent._audioClipHandle.GetAudioClipPath()) as AudioClip;
				waveformDisplay.SetWaveformAudioClip(audioComponent.AudioClip);
				audioComponent._audioClipHandle.SetAudioClipPath("");
			}
			GUILayout.Space(5f);
			EditorGUILayout.PropertyField(audioClip, new GUIContent("Audio Clip:"));
			if (dynamicAudioClipLoading.boolValue && !audioComponent._audioClipHandle.UseAudioClipPath && audioComponent.AudioClip != null && audioComponent.AudioClip.preloadAudioData)
			{
				GUILayout.Space(5f);
				GUILayout.BeginVertical("Box");
				Color backgroundColor = GUI.backgroundColor;
				GUI.color = Color.red;
				EditorGUILayout.LabelField("'Preload AudioData' option is set on this audio clip");
				EditorGUILayout.LabelField("Audio data will not automatically unload from memory");
				GUI.color = backgroundColor;
				GUILayout.EndVertical();
			}
		}

		private string ProcessAudioClipPath(AudioClip audioClip)
		{
			if (audioClip != null)
			{
				string assetPath = AssetDatabase.GetAssetPath(audioClip);
				if (dynamicAsyncAudioClipLoading.boolValue)
				{
					if (audioClipAssetPath.enumValueIndex == 0)
					{
						int num = assetPath.LastIndexOf("Assets/");
						if (num >= 0)
						{
							assetPath = assetPath.Remove(0, num);
							return assetPath.Replace("Assets/", "");
						}
						return null;
					}
					if (audioClipAssetPath.enumValueIndex == 2)
					{
						int num2 = assetPath.LastIndexOf("StreamingAssets/");
						if (num2 >= 0)
						{
							assetPath = assetPath.Remove(0, num2);
							return assetPath.Replace("StreamingAssets/", "");
						}
						return null;
					}
					return "";
				}
				assetPath = Regex.Replace(assetPath, ".wav", "", RegexOptions.IgnoreCase);
				assetPath = Regex.Replace(assetPath, ".ogg", "", RegexOptions.IgnoreCase);
				assetPath = Regex.Replace(assetPath, ".mp3", "", RegexOptions.IgnoreCase);
				int num3 = assetPath.LastIndexOf("Resources/");
				if (num3 >= 0)
				{
					assetPath = assetPath.Remove(0, num3);
					return assetPath.Replace("Resources/", "");
				}
				return null;
			}
			return "";
		}

		private void DisplayAudioClipNotValidDialog()
		{
			if (EditorUtility.DisplayDialog("Fabric Error", "AudioClip not located in Resources, Data or StreamingAssets folders\n\nPlease choose one of the following options.", "Ignore UseAudioClipPath", "Reset AudioClip"))
			{
				audioComponent._audioClipHandle.SetAudioClipPath("");
				audioComponent._audioClipHandle.UseAudioClipPath = false;
			}
			else
			{
				audioComponent.AudioClip = null;
			}
		}

		private void AudioClipPreviewerStop()
		{
			if (audioClipPreviewer != null)
			{
				audioSourcePreviewer.Stop();
			}
		}

		private void AudioClipPreviewerPlay()
		{
			if (audioClipPreviewer == null)
			{
				audioClipPreviewer = new GameObject();
				audioClipPreviewer.name = "FabricAudioClipPreviewer";
				audioSourcePreviewer = audioClipPreviewer.AddComponent<AudioSource>();
				audioClipPreviewer.hideFlags = HideFlags.HideAndDontSave;
			}
			AudioListener audioListener = UnityEngine.Object.FindAnyObjectByType<AudioListener>();
			if (audioListener != null)
			{
				audioClipPreviewer.transform.position = audioListener.transform.position;
			}
			if (audioComponent._dynamicAudioClipLoading && audioComponent._audioClipHandle.IsAudioClipPathSet())
			{
				audioSourcePreviewer.clip = (AudioClip)Resources.Load(audioComponent._audioClipHandle.GetAudioClipPath(), typeof(AudioClip));
			}
			else
			{
				audioSourcePreviewer.clip = audioComponent.AudioClip;
			}
			audioSourcePreviewer.Play();
		}
	}
	public class CoreAudioComponentEditor
	{
		private bool _foldout = true;

		private SerializedObject serializedObject;

		private SerializedProperty dontPlay;

		private SerializedProperty delay;

		private SerializedProperty dontStopOnDestroy;

		private SerializedProperty loop;

		private SerializedProperty ignoreVirtualization;

		private SerializedProperty audioClip;

		private SerializedProperty musicTimeSettingsindex;

		public void RegisterSerialisableObject(SerializedObject _serializedObject)
		{
			serializedObject = _serializedObject;
			if (serializedObject != null)
			{
				dontPlay = serializedObject.FindProperty("_dontPlay");
				delay = serializedObject.FindProperty("_delay");
				dontStopOnDestroy = serializedObject.FindProperty("_dontStopOnDestroy");
				loop = serializedObject.FindProperty("_loop");
				ignoreVirtualization = serializedObject.FindProperty("_ignoreVirtualization");
				audioClip = serializedObject.FindProperty("_audioClip");
				musicTimeSettingsindex = serializedObject.FindProperty("_musicTimeSettingsIndex");
			}
		}

		public void InspectorGUI(AudioComponent audioComponent)
		{
			serializedObject.Update();
			_foldout = EditorGUILayout.Foldout(_foldout, "Component Properties");
			if (_foldout)
			{
				GUILayout.BeginVertical("box");
				EditorGUILayout.PropertyField(ignoreVirtualization, new GUIContent("Ignore Virtualization:"));
				EditorGUILayout.PropertyField(dontPlay, new GUIContent("Dont Play:"));
				EditorGUILayout.Slider(delay, 0f, 600f, new GUIContent("Delay:"));
				EditorGUILayout.PropertyField(dontStopOnDestroy, new GUIContent("Dont Stop On Destroy:"));
				if (audioComponent.DontStopOnDestroy)
				{
					audioComponent.Loop = false;
				}
				else
				{
					EditorGUILayout.PropertyField(loop, new GUIContent("Loop:"));
				}
				EditorGUILayout.PropertyField(audioClip, new GUIContent("Audio Clip:"));
				if (FabricManager.Instance._musicTimeSignatures.Count > 0)
				{
					GUILayout.BeginVertical("Music Time Setting", "Box");
					string[] array = new string[FabricManager.Instance._musicTimeSignatures.Count];
					for (int i = 0; i < FabricManager.Instance._musicTimeSignatures.Count; i++)
					{
						array[i] = FabricManager.Instance._musicTimeSignatures[i]._name;
					}
					musicTimeSettingsindex.intValue = EditorGUILayout.Popup(musicTimeSettingsindex.intValue, array);
					GUILayout.EndVertical();
				}
				GUILayout.Label("Status:                           [ " + audioComponent.CurrentState.ToString() + " ]");
				GUILayout.EndVertical();
			}
			GUIHelpers.CheckGUIHasChanged(serializedObject, audioComponent);
		}
	}
	[CustomEditor(typeof(AssetLoader))]
	public class AssetLoaderEditor : Editor
	{
		private void DrawAssetList(ref string[] assetList)
		{
			if (assetList == null)
			{
				assetList = new string[1];
			}
			int num = EditorGUILayout.IntField("Size:", assetList.Length);
			if (num != assetList.Length)
			{
				string[] array = new string[num];
				for (int i = 0; i < num; i++)
				{
					if (assetList.Length > i)
					{
						array[i] = assetList[i];
					}
				}
				assetList = array;
			}
			for (int j = 0; j < assetList.Length; j++)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("Asset:");
				assetList[j] = GUILayout.TextField(assetList[j]);
				GUILayout.EndHorizontal();
			}
		}

		public override void OnInspectorGUI()
		{
			MenuBar.OnGUI("290598-assetloader", box: true);
			GUILayout.BeginVertical("Box");
			DrawDefaultInspector();
			GUILayout.EndVertical();
		}
	}
	[InitializeOnLoad]
	internal class ComponentPreviewer
	{
		private static bool _isEnabled;

		[DidReloadScripts]
		private static void OnScriptsReloaded()
		{
			FabricManager instance = FabricManager.Instance;
			if ((bool)instance && !EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
			{
				instance.RestartPreviewer();
			}
		}

		static ComponentPreviewer()
		{
			EditorApplication.playModeStateChanged -= PlaymodeStateChange;
			EditorApplication.playModeStateChanged += PlaymodeStateChange;
			if ((bool)FabricManager.Instance && !EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
			{
				Enable(FabricManager.Instance._enableEditorPreviewer);
			}
		}

		private static void Reset()
		{
			_isEnabled = false;
		}

		public static void Enable(bool enable)
		{
			FabricManager instance = FabricManager.Instance;
			if (!(instance == null))
			{
				if (!_isEnabled && enable)
				{
					instance.EnablePreviewer(enable: true);
					EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(Update));
					EditorApplication.hierarchyChanged -= ProjectHierarchyChange;
					EditorApplication.hierarchyChanged += ProjectHierarchyChange;
					_isEnabled = enable;
				}
				else if (_isEnabled && !enable)
				{
					instance.EnablePreviewer(enable: false);
					EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(Update));
					EditorApplication.hierarchyChanged -= ProjectHierarchyChange;
					FabricManagerEditor.DeleteAudioSources(instance.gameObject);
					_isEnabled = enable;
				}
			}
		}

		private static void PlaymodeStateChange()
		{
			if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
			{
				Enable(enable: false);
			}
			if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
			{
				FabricManager instance = FabricManager.Instance;
				if (instance != null)
				{
					Enable(instance._enableEditorPreviewer);
				}
			}
		}

		private static void PlaymodeStateChange(PlayModeStateChange state)
		{
			PlaymodeStateChange();
		}

		private static void ProjectHierarchyChange()
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode || !FabricManager.IsInitialised() || GUIHelpers.IsGameObjectPrefab(FabricManager.Instance.gameObject))
			{
				return;
			}
			FabricManager.UpdateHierarchy();
			GroupComponent[] externalGroupComponents = GetExternalGroupComponents();
			GroupComponent[] array = externalGroupComponents;
			foreach (GroupComponent groupComponent in array)
			{
				if (!groupComponent.IsFabricHierarchyPresent())
				{
					FabricManager.UpdateHierarchy(groupComponent);
					groupComponent.RegisterWithMainHierarchy();
				}
			}
			List<GroupComponentProxy> list = new List<GroupComponentProxy>();
			FabricManager.Instance.GetComponentsInChildren(includeInactive: true, list);
			foreach (GroupComponentProxy item in list)
			{
				bool flag = true;
				GroupComponent[] array2 = externalGroupComponents;
				foreach (GroupComponent groupComponent2 in array2)
				{
					if (item._groupComponent == groupComponent2)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					UnityEngine.Object.DestroyImmediate(item.gameObject);
				}
			}
		}

		public static GroupComponent[] GetExternalGroupComponents()
		{
			List<GroupComponent> list = new List<GroupComponent>();
			GroupComponent[] array = UnityEngine.Object.FindObjectsByType(typeof(GroupComponent)) as GroupComponent[];
			GroupComponent[] array2 = array;
			foreach (GroupComponent groupComponent in array2)
			{
				if (!groupComponent.IsFabricHierarchyPresent() && !groupComponent.IsInstance)
				{
					list.Add(groupComponent);
				}
			}
			return list.ToArray();
		}

		private static void Update()
		{
			if (!EditorApplication.isPlayingOrWillChangePlaymode && FabricManager.IsInitialised())
			{
				EventManager.Instance.UpdateInternal();
				FabricManager.Instance.Update();
			}
		}
	}
	public class AudioSourcePoolEditorWindow : EditorWindow
	{
		private Vector2 scrollPosition = default(Vector2);

		[MenuItem("Window/Fabric/AudioSource Pool", false, 9)]
		private static void Init()
		{
			AudioSourcePoolEditorWindow audioSourcePoolEditorWindow = (AudioSourcePoolEditorWindow)EditorWindow.GetWindow(typeof(AudioSourcePoolEditorWindow));
			audioSourcePoolEditorWindow.titleContent = new GUIContent("AudioSourcePool");
		}

		private void OnGUI()
		{
			GUILayout.BeginVertical("Box");
			MenuBar.OnGUI("442039", box: true);
			GUILayout.Space(5f);
			AudioSourcePoolEditor.DisplayProperties(FabricManager.Instance);
			GUILayout.EndVertical();
			if (FabricManager.Instance.AudioSourcePoolManager != null)
			{
				scrollPosition = GUILayout.BeginScrollView(scrollPosition, "Box");
				AudioSourcePoolEditor.OnGUI(FabricManager.Instance.AudioSourcePoolManager);
				GUILayout.EndScrollView();
			}
			else if (FabricManager.Instance != null && FabricManager.Instance._audioSourcePool == 0)
			{
				GUILayout.Label("Not Audio Source Pool present, set the pool size in the Fabric manager");
			}
		}

		private void OnInspectorUpdate()
		{
			Repaint();
		}
	}
	public class AudioBusEditorWindow : EditorWindow
	{
		private Vector2 scrollPosition = default(Vector2);

		private string text = "";

		[MenuItem("Window/Fabric/Audio Buses", false, 6)]
		private static void Init()
		{
			AudioBusEditorWindow audioBusEditorWindow = (AudioBusEditorWindow)EditorWindow.GetWindow(typeof(AudioBusEditorWindow));
			audioBusEditorWindow.titleContent = new GUIContent("Audio Buses");
		}

		private void OnGUI()
		{
			FabricManager instance = FabricManager.Instance;
			if (instance == null)
			{
				return;
			}
			GUILayout.BeginVertical("Box");
			MenuBar.OnGUI("442040", box: true);
			GUILayout.Space(5f);
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, "Box");
			for (int i = 0; i < instance._audioBusManager._audioBuses.Count; i++)
			{
				GUILayout.BeginVertical("Box");
				GUILayout.BeginHorizontal();
				AudioBus audioBus = instance._audioBusManager._audioBuses[i];
				GUILayout.BeginHorizontal("Box");
				using (new FixedWidthLabel("Name: "))
				{
					audioBus._name = EditorGUILayout.TextField(audioBus._name);
				}
				using (new FixedWidthLabel("Mixer Group: "))
				{
					audioBus._audioMixerGroup = (AudioMixerGroup)EditorGUILayout.ObjectField(audioBus._audioMixerGroup, typeof(AudioMixerGroup), true);
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginVertical("Box");
				GUILayout.BeginHorizontal();
				using (new FixedWidthLabel("Limit Voices: ", 100))
				{
					audioBus._limitAudioComponents = GUILayout.Toggle(audioBus._limitAudioComponents, "");
				}
				if (!audioBus._limitAudioComponents)
				{
					GUI.enabled = false;
				}
				using (new FixedWidthLabel("Max Voices: ", 100))
				{
					audioBus._audioComponentsLimit = EditorGUILayout.IntField(audioBus._audioComponentsLimit);
				}
				GUILayout.Label("[ " + audioBus.GetActiveAudioComponents() + " ]");
				GUI.enabled = true;
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
				if (GUILayout.Button("Del", GUILayout.MinHeight(23f)))
				{
					instance._audioBusManager._audioBuses.RemoveAt(i);
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(3f);
				GUILayout.BeginHorizontal("Box");
				if (audioBus._audioMixerGroup != null)
				{
					GUI.enabled = false;
				}
				using (new FixedWidthLabel("Volume: "))
				{
					audioBus._volume = EditorGUILayout.Slider(audioBus._volume, 0f, 1f);
				}
				using (new FixedWidthLabel("Pitch: "))
				{
					audioBus._pitch = EditorGUILayout.Slider(audioBus._pitch, -3f, 3f);
				}
				GUI.enabled = true;
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}
			GUILayout.Space(10f);
			GUILayout.BeginVertical();
			GUILayout.Label("Name: ", GUILayout.MaxWidth(100f));
			text = GUILayout.TextField(text);
			if (GUILayout.Button("Add New Bus"))
			{
				instance._audioBusManager.CreateAudioBus(text);
				text = "";
				GUIHelpers.ApplyChangesToPrefab(instance.gameObject);
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			GUIHelpers.CheckGUIHasChanged(instance.gameObject);
		}

		private void OnInspectorUpdate()
		{
			Repaint();
		}
	}
	public class CustomCurvesEditor : EditorWindow
	{
		private Vector2 filterScrollPosition = default(Vector2);

		public static CustomCurvesEditor window;

		public static void Open()
		{
			if (window == null)
			{
				window = ScriptableObject.CreateInstance<CustomCurvesEditor>();
				window.position = new Rect(UnityEngine.Event.current.mousePosition.x, UnityEngine.Event.current.mousePosition.y, 280f, 200f);
				window.name = "Custom Curves";
				window.Show();
			}
		}

		private void OnInspectorUpdate()
		{
			Repaint();
		}

		private void OnGUI()
		{
			Component component = null;
			if (Selection.activeGameObject != null)
			{
				component = Selection.activeGameObject.GetComponent<Component>();
			}
			filterScrollPosition = GUILayout.BeginScrollView(filterScrollPosition, "Box");
			if (component != null)
			{
				CustomCurvesEditorWindow.Display(component.CustomCurves);
			}
			GUILayout.EndScrollView();
			if (GUILayout.Button("Close"))
			{
				Close();
			}
		}
	}
	public class CustomCurvesEditorWindow : EditorWindow
	{
		private Vector2 scrollPosition = default(Vector2);

		private string globalCustomCurvesName = "";

		private int globalCustomCurvesIndex;

		[MenuItem("Window/Fabric/Custom Curves Editor", false, 7)]
		private static void Init()
		{
			CustomCurvesEditorWindow customCurvesEditorWindow = (CustomCurvesEditorWindow)EditorWindow.GetWindow(typeof(CustomCurvesEditorWindow));
			customCurvesEditorWindow.titleContent = new GUIContent("Custom Curves");
		}

		private static AnimationCurve DisplayCustomCurve(string label, AnimationCurve customCurve, float min, float max)
		{
			if (customCurve != null)
			{
				customCurve = EditorGUILayout.CurveField("", customCurve, Color.green, new Rect(min, 0f, max, 1.1f), GUILayout.Height(70f));
			}
			return customCurve;
		}

		public static void Display(CustomCurves customCurves)
		{
			if (customCurves != null)
			{
				GUILayout.BeginHorizontal("Box");
				using (new FixedWidthLabel("Name: "))
				{
					customCurves._name = EditorGUILayout.TextField(customCurves._name);
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				customCurves._minDistance = EditorGUILayout.FloatField("Min Distance: ", customCurves._minDistance);
				customCurves._maxDistance = EditorGUILayout.FloatField("Max Distance: ", customCurves._maxDistance);
				GUILayout.EndHorizontal();
				GUILayout.Space(3f);
				customCurves._enableCustomRolloff = EditorGUILayout.Toggle("Custom RollOff", customCurves._enableCustomRolloff);
				if (customCurves._enableCustomRolloff)
				{
					customCurves._customRolloff = DisplayCustomCurve("Custom RollOff", customCurves._customRolloff, customCurves._minDistance, customCurves._maxDistance);
				}
				customCurves._enableSpatialBlend = EditorGUILayout.Toggle("Spatial Blend", customCurves._enableSpatialBlend);
				if (customCurves._enableSpatialBlend)
				{
					customCurves._spatialBlend = DisplayCustomCurve("Spatial Blend", customCurves._spatialBlend, 0f, 1f);
				}
				customCurves._enableReverbZoneMix = EditorGUILayout.Toggle("Reverb ZoneMix", customCurves._enableReverbZoneMix);
				if (customCurves._enableReverbZoneMix)
				{
					customCurves._reverbZoneMix = DisplayCustomCurve("Reverb ZoneMix", customCurves._reverbZoneMix, 0f, 1f);
				}
				customCurves._enableSpread = EditorGUILayout.Toggle("Spread", customCurves._enableSpread);
				if (customCurves._enableSpread)
				{
					customCurves._spread = DisplayCustomCurve("Spread", customCurves._spread, 0f, 1f);
				}
			}
		}

		private void OnGUI()
		{
			FabricManager instance = FabricManager.Instance;
			if (!(instance == null))
			{
				MenuBar.OnGUI("442040", box: true);
				GUILayout.Space(5f);
				scrollPosition = GUILayout.BeginScrollView(scrollPosition, "Box");
				GUILayout.BeginVertical("Box");
				GUILayout.BeginHorizontal();
				bool flag = GUILayout.Button("Add");
				globalCustomCurvesName = GUILayout.TextField(globalCustomCurvesName);
				if (flag)
				{
					instance._customCurvesManager.CreateCustomCurves(globalCustomCurvesName);
					globalCustomCurvesName = "";
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(5f);
				GUILayout.BeginHorizontal();
				globalCustomCurvesIndex = EditorGUILayout.Popup("Custom Curves: ", globalCustomCurvesIndex, instance._customCurvesManager.GetNames());
				if (globalCustomCurvesIndex < instance._customCurvesManager._customCurveList.Count && GUILayout.Button("Del", GUILayout.Width(48f)))
				{
					instance._customCurvesManager._customCurveList.RemoveAt(globalCustomCurvesIndex);
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(10f);
				GUILayout.BeginVertical();
				CustomCurves customCurvesByIndex = instance._customCurvesManager.GetCustomCurvesByIndex(globalCustomCurvesIndex);
				if (customCurvesByIndex != null)
				{
					Display(customCurvesByIndex);
				}
				GUILayout.EndVertical();
				GUILayout.EndVertical();
				GUILayout.EndScrollView();
				GUIHelpers.CheckGUIHasChanged(instance.gameObject);
			}
		}
	}
	public class AddNewEventNameDialog : EditorWindow
	{
		private static string _eventName;

		private static Component _component;

		[MenuItem("CONTEXT/Component/New Fabric Event")]
		private static void NewEventAction(MenuCommand command)
		{
			Component component = command.context as Component;
			if (component != null)
			{
				Open(component);
			}
		}

		public static void Open(Component component)
		{
			_component = component;
			AddNewEventNameDialog addNewEventNameDialog = ScriptableObject.CreateInstance<AddNewEventNameDialog>();
			addNewEventNameDialog.position = new Rect(Screen.width / 2, Screen.height / 2, 320f, 50f);
			addNewEventNameDialog.titleContent = new GUIContent("New Event");
			addNewEventNameDialog.Show();
		}

		private void OnGUI()
		{
			GUILayout.BeginHorizontal("box");
			GUILayout.BeginHorizontal();
			GUILayout.Label("EventName: ", GUILayout.MaxWidth(80f));
			_eventName = EditorGUILayout.TextField("", _eventName);
			GUILayout.EndHorizontal();
			if (GUILayout.Button("Add Event"))
			{
				string[] array = new string[1] { _eventName };
				EventListener eventListener = _component.gameObject.AddComponent<EventListener>();
				eventListener._eventName = array[0];
				EventManager.Instance.AddEventNames(array);
				Close();
			}
			GUILayout.EndHorizontal();
		}
	}
	public class ReplaceComponentDialog : EditorWindow
	{
		private static Component _component;

		private int selection;

		[MenuItem("CONTEXT/Component/Replace Fabric Component")]
		public static void ReplaceComponent(MenuCommand command)
		{
			Component component = command.context as Component;
			if (component != null)
			{
				Open(component);
			}
		}

		public static void Open(Component component)
		{
			_component = component;
			ReplaceComponentDialog replaceComponentDialog = ScriptableObject.CreateInstance<ReplaceComponentDialog>();
			replaceComponentDialog.position = new Rect(Screen.width / 2, Screen.height / 2, 320f, 50f);
			replaceComponentDialog.titleContent = new GUIContent("Replace Component");
			replaceComponentDialog.Show();
		}

		private void OnGUI()
		{
			Type[] array = ComponentEditor.FabricComponentCollector.Get<Component>();
			string[] array2 = new string[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array[i].Name;
			}
			GUILayout.BeginHorizontal("box");
			GUILayout.BeginHorizontal();
			GUILayout.Label("Component: ", GUILayout.MaxWidth(80f));
			selection = EditorGUILayout.Popup("", selection, array2);
			GUILayout.EndHorizontal();
			GameObject gameObject = _component.gameObject;
			if (GUILayout.Button("Replace"))
			{
				UnityEngine.Object.DestroyImmediate(_component);
				gameObject.AddComponent(array[selection]);
				Close();
			}
			GUILayout.EndHorizontal();
		}
	}
	public class AddNewParentDialog : EditorWindow
	{
		private static Component _component;

		private int selection;

		private string componentName = "New Component";

		[MenuItem("CONTEXT/Component/New Fabric Component Parent")]
		private static void AddParentComponent(MenuCommand command)
		{
			Component component = command.context as Component;
			if (component != null)
			{
				Open(component);
			}
		}

		public static void Open(Component component)
		{
			_component = component;
			AddNewParentDialog addNewParentDialog = ScriptableObject.CreateInstance<AddNewParentDialog>();
			addNewParentDialog.position = new Rect(Screen.width / 2, Screen.height / 2, 320f, 50f);
			addNewParentDialog.titleContent = new GUIContent("New Event");
			addNewParentDialog.Show();
		}

		private void OnGUI()
		{
			Type[] array = ComponentEditor.FabricComponentCollector.Get<Component>();
			string[] array2 = new string[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array[i].Name;
			}
			GUILayout.BeginHorizontal("box");
			GUILayout.BeginHorizontal();
			GUILayout.Label("Component: ", GUILayout.MaxWidth(80f));
			selection = EditorGUILayout.Popup("", selection, array2);
			GUILayout.EndHorizontal();
			componentName = EditorGUILayout.TextField(componentName);
			if (GUILayout.Button("Add"))
			{
				GameObject gameObject = new GameObject();
				gameObject.name = componentName;
				gameObject.transform.parent = _component.gameObject.transform.parent;
				_component.gameObject.transform.parent = gameObject.transform;
				gameObject.AddComponent(array[selection]);
				Close();
			}
			GUILayout.EndHorizontal();
		}
	}
	public class AddNewChildDialog : EditorWindow
	{
		private static GameObject _gameObject;

		private int selection;

		private string componentName = "New Component";

		[MenuItem("CONTEXT/Component/New Fabric Component Child")]
		private static void AddChildComponent(MenuCommand command)
		{
			Component component = command.context as Component;
			if (component != null)
			{
				Open(component.gameObject);
			}
		}

		public static void Open(GameObject gameObject)
		{
			_gameObject = gameObject;
			AddNewChildDialog addNewChildDialog = ScriptableObject.CreateInstance<AddNewChildDialog>();
			addNewChildDialog.position = new Rect(Screen.width / 2, Screen.height / 2, 320f, 50f);
			addNewChildDialog.titleContent = new GUIContent("New Event");
			addNewChildDialog.Show();
		}

		private void OnGUI()
		{
			Type[] array = ComponentEditor.FabricComponentCollector.Get<Component>();
			string[] array2 = new string[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array[i].Name;
			}
			GUILayout.BeginHorizontal("box");
			GUILayout.BeginHorizontal();
			GUILayout.Label("Component: ", GUILayout.MaxWidth(80f));
			selection = EditorGUILayout.Popup("", selection, array2);
			GUILayout.EndHorizontal();
			componentName = EditorGUILayout.TextField(componentName);
			if (GUILayout.Button("Add"))
			{
				GameObject gameObject = new GameObject();
				gameObject.name = componentName;
				gameObject.transform.parent = _gameObject.transform;
				gameObject.AddComponent(array[selection]);
				Close();
			}
			GUILayout.EndHorizontal();
		}
	}
	public class EventMonitorEditor : EditorWindow
	{
		private enum EventWidth
		{
			EVENT_NAME = 200,
			COMPONENT = 125,
			PARENT_GAMEOBJECT = 125,
			POSITION = 100,
			DISTANCE = 80,
			VOLUME = 85,
			VOLUME_OFFSET = 97,
			PITCH = 80,
			PAN2D = 80,
			INSTANCES = 80,
			VIRTUAL_INSTANCES = 120,
			STATUS = 100,
			MUTE = 80
		}

		private Vector2 scrollPosition = default(Vector2);

		private bool solo;

		private Component soloComponent;

		private EventManager eventManager;

		[MenuItem("Window/Fabric/Event Monitor", false, 4)]
		private static void Init()
		{
			EventMonitorEditor eventMonitorEditor = (EventMonitorEditor)EditorWindow.GetWindow(typeof(EventMonitorEditor));
			eventMonitorEditor.titleContent = new GUIContent("Event Monitor");
		}

		private void OnFocus()
		{
			SceneView.duringSceneGui -= OnSceneGUI;
			SceneView.duringSceneGui += OnSceneGUI;
		}

		private void OnEnable()
		{
			OnFocus();
			GUIHelpers.RegisterEditorUpdate(base.Repaint);
		}

		private void OnDisable()
		{
			GUIHelpers.UnregisterEditorUpdate(base.Repaint);
		}

		private void OnDestroy()
		{
			SceneView.duringSceneGui -= OnSceneGUI;
		}

		private void OnGUI()
		{
			FabricManager fabricManager = GetFabricManager.Instance();
			if (fabricManager == null)
			{
				return;
			}
			eventManager = GetFabricManager.Instance().GetComponent<EventManager>();
			if (eventManager == null)
			{
				return;
			}
			GUILayout.BeginHorizontal("Box");
			FabricEditorData.GetData()._displayEventMonitorSceneInfo = EditorGUILayout.Toggle("Display Scene Info: ", FabricEditorData.GetData()._displayEventMonitorSceneInfo);
			FabricEditorData.GetData()._eventMonitorPersistenceTime = EditorGUILayout.FloatField("Persist Event Timer (sec): ", FabricEditorData.GetData()._eventMonitorPersistenceTime);
			if (eventManager._activeEventPersistenceTime != FabricEditorData.GetData()._eventMonitorPersistenceTime)
			{
				eventManager._activeEventPersistenceTime = FabricEditorData.GetData()._eventMonitorPersistenceTime;
			}
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Expand Component Instances"))
			{
				for (int i = 0; i < eventManager._activeEvents.Count; i++)
				{
					eventManager._activeEvents[i]._componentInstanceFoldout = true;
				}
			}
			if (GUILayout.Button("Collapse Component Instances"))
			{
				for (int j = 0; j < eventManager._activeEvents.Count; j++)
				{
					eventManager._activeEvents[j]._componentInstanceFoldout = false;
				}
			}
			if (GUILayout.Button("Expand Virtual Instances"))
			{
				for (int k = 0; k < eventManager._activeEvents.Count; k++)
				{
					eventManager._activeEvents[k]._virtualEventsFoldout = true;
				}
			}
			if (GUILayout.Button("Collapse Virtual Instances"))
			{
				for (int l = 0; l < eventManager._activeEvents.Count; l++)
				{
					eventManager._activeEvents[l]._virtualEventsFoldout = false;
				}
			}
			MenuBar.OnGUI("288082-eventmonitor");
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			GUIStyle gUIStyle = new GUIStyle(GUI.skin.label);
			gUIStyle.alignment = TextAnchor.MiddleCenter;
			GUILayout.BeginHorizontal("Box");
			if (GUILayout.Button("Event", GUILayout.Width(200f)))
			{
				eventManager._activeEvents.Sort((EventManager.ActiveEvent a, EventManager.ActiveEvent b) => b._eventName.CompareTo(a._eventName));
			}
			GUILayout.Button("Component", GUILayout.Width(125f));
			GUILayout.Button("Parent GameObject", GUILayout.Width(125f));
			GUILayout.Button("Pos", GUILayout.Width(100f));
			GUILayout.Button("Dist", GUILayout.Width(80f));
			GUILayout.Button("Volume (dB)", GUILayout.Width(85f));
			GUILayout.Button("Vol Offset (dB)", GUILayout.Width(97f));
			GUILayout.Button("Pitch", GUILayout.Width(80f));
			GUILayout.Button("Pitch Offset", GUILayout.Width(80f));
			GUILayout.Button("Pan2D", GUILayout.Width(80f));
			GUILayout.Button("Instances", GUILayout.Width(80f));
			GUILayout.Button("Virtual Instances", GUILayout.Width(120f));
			GUILayout.Button("Status", GUILayout.Width(100f));
			GUILayout.Button("Mute", GUILayout.MaxWidth(80f));
			GUILayout.EndHorizontal();
			for (int num = 0; num < eventManager._activeEvents.Count; num++)
			{
				EventManager.ActiveEvent activeEvent = eventManager._activeEvents[num];
				if (activeEvent == null)
				{
					continue;
				}
				GUILayout.BeginVertical("Box");
				GUILayout.BeginHorizontal("Box");
				GUILayout.Label(activeEvent._eventName, GUILayout.Width(200f));
				GUILayout.EndHorizontal();
				Component component = activeEvent._component;
				activeEvent._componentInstanceFoldout = EditorGUILayout.Foldout(activeEvent._componentInstanceFoldout, "Component Instances [" + component._componentInstances.Length + "]");
				if (activeEvent._componentInstanceFoldout)
				{
					for (int num2 = 0; num2 < component._componentInstances.Length; num2++)
					{
						ComponentInstance componentInstance = component._componentInstances[num2];
						Component instance = componentInstance._instance;
						GUILayout.BeginHorizontal();
						GUILayout.Label("|->", gUIStyle, GUILayout.Width(200f));
						EditorGUILayout.ObjectField(instance, typeof(Component), true, GUILayout.Width(125f));
						bool flag = instance.IsPlaying();
						if (flag && componentInstance._parentGameObject != null)
						{
							EditorGUILayout.ObjectField(componentInstance._parentGameObject, typeof(GameObject), true, GUILayout.Width(125f));
						}
						else
						{
							GUILayout.Label("-", gUIStyle, GUILayout.Width(125f));
						}
						if (flag && componentInstance._parentGameObject != null)
						{
							GUILayout.Label(componentInstance._parentGameObject.transform.position.ToString(), gUIStyle, GUILayout.Width(100f));
							float num3 = 0f;
							if (FabricManager.Instance._audioListener != null)
							{
								num3 = Vector3.Distance(componentInstance._parentGameObject.transform.position, FabricManager.Instance._audioListener.transform.position);
							}
							else if (Camera.main != null)
							{
								num3 = Vector3.Distance(componentInstance._parentGameObject.transform.position, Camera.main.transform.position);
							}
							GUILayout.Label(num3.ToString("N2"), gUIStyle, GUILayout.Width(80f));
						}
						else
						{
							GUILayout.Label("-", gUIStyle, GUILayout.Width(80f));
						}
						if (flag)
						{
							GUILayout.Label(AudioTools.LinearToDB(instance.Volume).ToString("N2"), gUIStyle, GUILayout.Width(85f));
							GUILayout.Label(AudioTools.LinearToDB(instance.UpdateContext._volume).ToString("N2"), gUIStyle, GUILayout.Width(97f));
							GUILayout.Label(instance.Pitch.ToString("N2"), gUIStyle, GUILayout.Width(80f));
							GUILayout.Label(instance.PitchOffset.ToString("N2"), gUIStyle, GUILayout.Width(80f));
							GUILayout.Label(instance.Pan2D.ToString("N2"), gUIStyle, GUILayout.Width(80f));
							if (!instance.IsInstance)
							{
								GUILayout.Label(instance.GetNumActiveComponentInstances().ToString() + "(" + activeEvent._component.MaxInstances + ")", gUIStyle, GUILayout.Width(80f));
								GUILayout.Label(instance.GetNumVirtualEventInstances().ToString(), gUIStyle, GUILayout.Width(120f));
							}
							else
							{
								GUILayout.Label("IsInstance", gUIStyle, GUILayout.Width(80f));
								GUILayout.Label("-", gUIStyle, GUILayout.Width(120f));
							}
							if ((bool)(instance as AudioComponent))
							{
								GUILayout.Label(((AudioComponent)instance).CurrentState.ToString(), gUIStyle, GUILayout.Width(100f));
							}
							else
							{
								GUILayout.Label(instance._componentStatus.ToString(), gUIStyle, GUILayout.Width(100f));
							}
						}
						else
						{
							GUILayout.Label("-", gUIStyle, GUILayout.Width(85f));
							GUILayout.Label("-", gUIStyle, GUILayout.Width(97f));
							GUILayout.Label("-", gUIStyle, GUILayout.Width(80f));
							GUILayout.Label("-", gUIStyle, GUILayout.Width(80f));
							GUILayout.Label("-", gUIStyle, GUILayout.Width(80f));
							GUILayout.Label("-", gUIStyle, GUILayout.Width(80f));
							GUILayout.Label("-", gUIStyle, GUILayout.Width(120f));
							GUILayout.Label("-", gUIStyle, GUILayout.Width(100f));
						}
						instance.Mute = GUILayout.Toggle(activeEvent._component.Mute, "M", "button", GUILayout.MaxWidth(80f));
						GUILayout.EndHorizontal();
					}
				}
				activeEvent._virtualEventsFoldout = EditorGUILayout.Foldout(activeEvent._virtualEventsFoldout, "Virtual Events [" + component._componentVirtualizationEvents.Count + "/" + component._numVirtualizationEvents + "]");
				if (activeEvent._virtualEventsFoldout)
				{
					for (int num4 = 0; num4 < component._componentVirtualizationEvents.Count; num4++)
					{
						GUILayout.BeginHorizontal();
						VirtualizationEventInstance virtualizationEventInstance = component._componentVirtualizationEvents[num4];
						GUILayout.Label("|->", gUIStyle, GUILayout.Width(200f));
						GUILayout.Label("", GUILayout.Width(125f));
						EditorGUILayout.ObjectField(virtualizationEventInstance._event.parentGameObject, typeof(GameObject), true, GUILayout.Width(125f));
						GUILayout.EndHorizontal();
					}
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndScrollView();
		}

		private void OnSceneGUI(SceneView sceneView)
		{
			if (!(eventManager != null) || eventManager._activeEvents == null || !FabricEditorData.GetData()._displayEventMonitorSceneInfo)
			{
				return;
			}
			Handles.BeginGUI();
			for (int i = 0; i < eventManager._activeEvents.Count; i++)
			{
				EventManager.ActiveEvent activeEvent = eventManager._activeEvents[i];
				if (activeEvent != null && !(activeEvent._parentGameObject == null))
				{
					Transform transform = activeEvent._parentGameObject.transform;
					GUIStyle gUIStyle = new GUIStyle();
					gUIStyle.fontSize = 14;
					gUIStyle.normal.textColor = Color.white;
					Handles.Label(transform.position + Vector3.up * 2f, "Pos: " + transform.position.ToString() + "\nEvent: " + activeEvent._eventName + "\nVol: " + activeEvent._component.Volume + "\nPitch: " + activeEvent._component.Pitch, gUIStyle);
				}
			}
			Handles.EndGUI();
		}

		private void SoloComponent(Component component, bool solo)
		{
			if (solo)
			{
				for (int i = 0; i < eventManager._activeEvents.Count; i++)
				{
					if (eventManager._activeEvents[i] != null && component != eventManager._activeEvents[i]._component)
					{
						eventManager._activeEvents[i]._component.Mute = true;
					}
				}
				Component[] componentsInChildren = component.GetComponentsInChildren<Component>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					componentsInChildren[j].Mute = false;
				}
				Mixer.UnmuteParentComponent(component);
				soloComponent = component;
			}
			else
			{
				if (solo || !(soloComponent != null))
				{
					return;
				}
				for (int k = 0; k < eventManager._activeEvents.Count; k++)
				{
					if (eventManager._activeEvents[k] != null)
					{
						eventManager._activeEvents[k]._component.Mute = false;
					}
				}
				soloComponent = null;
			}
		}

		private void OnInspectorUpdate()
		{
			Repaint();
		}
	}
	public class AudioEditorUtils
	{
		public static void DisplaySemitonesPitchSlider(string label, SerializedProperty pitch, int min = -36, int max = 36)
		{
			int value = (int)(12f * Mathf.Log(pitch.floatValue, 2f));
			value = EditorGUILayout.IntSlider(label, value, min, max);
			float f = Mathf.Pow(2f, 1f / 12f);
			pitch.floatValue = Mathf.Pow(f, value);
		}

		public static void DisplayDecibelsVolumeSlider(string label, SerializedProperty volume)
		{
			float value = AudioTools.LinearToDB(volume.floatValue);
			value = EditorGUILayout.Slider(label, value, -64f, 0f);
			if (value <= -64f)
			{
				volume.floatValue = 0f;
			}
			else
			{
				volume.floatValue = AudioTools.DBToLinear(value);
			}
		}

		public static void DisplaySemitonesPitchSlider(Rect rect, string label, SerializedProperty pitch, int min = -36, int max = 36)
		{
			int value = (int)(12f * Mathf.Log(pitch.floatValue, 2f));
			value = EditorGUI.IntSlider(rect, label, value, min, max);
			float f = Mathf.Pow(2f, 1f / 12f);
			pitch.floatValue = Mathf.Pow(f, value);
		}

		public static void DisplayRandomSemitonesPitchSlider(Rect rect, string label, SerializedProperty pitch, int min = -36, int max = 36)
		{
			int value = (int)(12f * Mathf.Log(pitch.floatValue + 1f, 2f));
			value = EditorGUI.IntSlider(rect, label, value, min, max);
			float f = Mathf.Pow(2f, 1f / 12f);
			pitch.floatValue = Mathf.Pow(f, value) - 1f;
		}

		public static void DisplayDecibelsVolumeSlider(Rect rect, string label, SerializedProperty volume)
		{
			float value = AudioTools.LinearToDB(volume.floatValue);
			value = EditorGUI.Slider(rect, label, value, -64f, 0f);
			if (value <= -64f)
			{
				volume.floatValue = 0f;
			}
			else
			{
				volume.floatValue = AudioTools.DBToLinear(value);
			}
		}
	}
	[CustomPropertyDrawer(typeof(DecibelsSliderAttribute))]
	public class DecibelsSliderAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
		{
			AudioEditorUtils.DisplayDecibelsVolumeSlider(position, label.text, prop);
		}
	}
	[CustomPropertyDrawer(typeof(FabricEventAttribute))]
	public class FabricEventAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
		{
			if (EventManager.Instance == null)
			{
				prop.stringValue = EditorGUI.TextField(position, label, prop.stringValue);
				return;
			}
			int num = EventManager.Instance.FindEventNameIndexByName(prop.stringValue);
			string[] array = EventManager.Instance._eventList.ToArray();
			int num2 = EditorGUI.Popup(position, label.text, num, array);
			if (num2 != num)
			{
				prop.stringValue = array[num2];
			}
		}
	}
	[CustomPropertyDrawer(typeof(RandomSemitoneSliderAttribute))]
	public class RandomSemitoneSliderAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
		{
			AudioEditorUtils.DisplayRandomSemitonesPitchSlider(position, label.text, prop);
		}
	}
	[CustomPropertyDrawer(typeof(SemitoneSliderAttribute))]
	public class SemitoneSliderAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
		{
			AudioEditorUtils.DisplaySemitonesPitchSlider(position, label.text, prop);
		}
	}
	public class PreviewWindow
	{
		public Rect rect;

		public PreviewWindowData _windowData;

		public Previewer _previewer;

		public virtual PreviewWindowData Initialise(string name, Previewer previewer)
		{
			return null;
		}

		public virtual void Initialise(PreviewWindowData data, Previewer previewer)
		{
		}

		public virtual bool OnWindow(bool ignoreClose = false)
		{
			return false;
		}
	}
	public class PreviewComponentWindow : PreviewWindow
	{
		private bool _paused;

		private Vector2 scrollVector;

		public override void Initialise(PreviewWindowData data, Previewer previewer)
		{
			GUIStyle gUIStyle = new GUIStyle();
			_previewer = previewer;
			_windowData = data;
			float num = gUIStyle.CalcSize(new GUIContent(_windowData._name)).x + 50f;
			if (num < 200f)
			{
				num = 200f;
			}
			float num2 = 127f;
			CheckComponent();
			if (_windowData._component != null && _windowData._component.HasVolumeMeter)
			{
				num2 += 40f;
			}
			rect = new Rect(_windowData._x, _windowData._y, num, num2);
		}

		private bool CheckComponent()
		{
			if (_windowData != null && _windowData.Guid != null && _windowData.Guid.Length > 0 && _windowData._component == null)
			{
				Component[] componentsInChildren = GetFabricManager.Instance().gameObject.GetComponentsInChildren<Component>(includeInactive: true);
				Component[] array = componentsInChildren;
				foreach (Component component in array)
				{
					if (component.Guid == _windowData.Guid)
					{
						_windowData._component = component;
						return true;
					}
				}
			}
			if (!(_windowData._component == null))
			{
				return true;
			}
			return false;
		}

		public override PreviewWindowData Initialise(string name, Previewer previewer)
		{
			_windowData = PreviewWindowData.CreateWindow(PreviewWindowType.Component);
			_windowData._name = name;
			Initialise(_windowData, previewer);
			return _windowData;
		}

		public void SetComponent(Component component)
		{
			_windowData._component = component;
		}

		public static Vector2 DrawComponent(Component component, Rect rect, ref bool pause, Vector2 scrollVector)
		{
			GUILayout.BeginHorizontal();
			GUI.enabled = component != null && component.IsPlaying();
			if (GUILayout.Button("Stop"))
			{
				component.Stop();
				Context context = new Context();
				component.UpdateInternal(ref context);
				component.PreviewUpdate();
				pause = false;
			}
			bool flag = GUILayout.Toggle(pause, "Pause", "Button");
			if (pause != flag)
			{
				component.Pause(flag);
				pause = flag;
			}
			if (component != null && component.HasEventListener)
			{
				GUI.enabled = FabricManager.Instance._enableEditorPreviewer;
			}
			if (GUILayout.Button("Play"))
			{
				pause = false;
				Camera camera = null;
				if (SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera != null)
				{
					camera = SceneView.lastActiveSceneView.camera;
				}
				if (component != null)
				{
					component.Play((camera != null) ? camera.gameObject : FabricManager.Instance.gameObject);
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
			scrollVector = GUILayout.BeginScrollView(scrollVector, "box");
			GUILayout.BeginVertical("Box");
			GUILayout.BeginHorizontal();
			GUILayout.Label("Volume: ", GUILayout.MaxWidth(50f));
			component.Volume = GUILayout.HorizontalSlider(component.Volume, 0f, 1f);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Pitch: ", GUILayout.MaxWidth(50f));
			component.Pitch = GUILayout.HorizontalSlider(component.Pitch, -3f, 3f);
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			if (component._RTPManager != null && component._RTPManager._parameters != null && component._RTPManager._parameters.Length > 0)
			{
				GUILayout.BeginVertical("Box");
				for (int i = 0; i < component._RTPManager._parameters.Length; i++)
				{
					RTPParameterToProperty rTPParameterToProperty = component._RTPManager._parameters[i];
					GUILayout.BeginHorizontal();
					GUIStyle gUIStyle = new GUIStyle();
					float x = gUIStyle.CalcSize(new GUIContent(rTPParameterToProperty._parameter.Name)).x;
					GUILayout.Label(rTPParameterToProperty._parameter.Name + ": ", GUILayout.MaxWidth(x + 20f));
					rTPParameterToProperty._parameter.SetValue(GUILayout.HorizontalSlider(rTPParameterToProperty._parameter.GetValue(), rTPParameterToProperty._parameter._min, rTPParameterToProperty._parameter._max));
					GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();
			}
			if (component is Fabric.TimelineComponent.TimelineComponent)
			{
				GUILayout.BeginVertical("Box");
				TimelineParameter[] components = component.GetComponents<TimelineParameter>();
				foreach (TimelineParameter timelineParameter in components)
				{
					GUILayout.BeginHorizontal();
					GUIStyle gUIStyle2 = new GUIStyle();
					float x2 = gUIStyle2.CalcSize(new GUIContent(timelineParameter._name)).x;
					GUILayout.Label(timelineParameter._name + ": ", GUILayout.MaxWidth(x2 + 20f));
					timelineParameter.SetValue(GUILayout.HorizontalSlider(timelineParameter.GetValue(), timelineParameter._min, timelineParameter._max));
					GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();
			}
			CustomProperties(component);
			GUILayout.EndScrollView();
			if (component.HasVolumeMeter)
			{
				GUILayout.BeginVertical("Box");
				float width = rect.width;
				VolumeMeter volumeMeter = component.VolumeMeter;
				if ((bool)volumeMeter)
				{
					VolumeMeterEditor.DrawMeter2("", volumeMeter.volumeMeterState.mPeaks.mChannels[0], 10, width, isVertical: true);
					GUILayout.Space(3f);
					VolumeMeterEditor.DrawMeter2("", volumeMeter.volumeMeterState.mPeaks.mChannels[1], 10, width, isVertical: true);
				}
				GUILayout.EndVertical();
			}
			GUILayout.Space(5f);
			GUI.enabled = true;
			GUILayout.BeginHorizontal();
			if (component.IsPlaying())
			{
				GUILayout.Label("Status: Playing", GUILayout.MaxWidth(110f));
			}
			else
			{
				GUILayout.Label("Status: Stopped", GUILayout.MaxWidth(110f));
			}
			GUILayout.EndHorizontal();
			return scrollVector;
		}

		public override bool OnWindow(bool ignoreClose = false)
		{
			GUI.enabled = FabricManager.Instance._enableEditorPreviewer;
			GUILayout.BeginVertical();
			if (!CheckComponent())
			{
				return true;
			}
			if (_windowData._component != null)
			{
				scrollVector = DrawComponent(_windowData._component, rect, ref _paused, scrollVector);
			}
			bool result = false;
			if (!ignoreClose)
			{
				result = GUILayout.Button("Close");
			}
			GUILayout.EndVertical();
			_windowData._x = rect.x;
			_windowData._y = rect.y;
			GUI.enabled = true;
			return result;
		}

		private static void CustomProperties(Component component)
		{
			if (component is SequenceComponent)
			{
				SequenceComponent sequenceComponent = (SequenceComponent)component;
				if (GUILayout.Button("Advance Sequence"))
				{
					Event obj = new Event();
					obj.EventAction = EventAction.AdvanceSequence;
					sequenceComponent.OnProcessEvent(obj, null);
				}
			}
			else if (component is SwitchComponent)
			{
				SwitchComponent switchComponent = (SwitchComponent)component;
				List<string> list = new List<string>();
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < switchComponent.transform.childCount; i++)
				{
					Component component2 = switchComponent.transform.GetChild(i).GetComponent<Component>();
					if (component2 != null)
					{
						list.Add(component2.name);
						if (component2 == switchComponent._selectedComponent)
						{
							num = num2;
						}
						num2++;
					}
				}
				num2 = EditorGUILayout.Popup(num, list.ToArray());
				if (num2 != num)
				{
					Event obj2 = new Event();
					obj2.EventAction = EventAction.SetSwitch;
					obj2._parameter = list[num2];
					switchComponent.OnProcessEvent(obj2, null);
				}
			}
			else
			{
				if (!(component is MusicComponent))
				{
					return;
				}
				MusicComponent musicComponent = (MusicComponent)component;
				List<string> list2 = new List<string>();
				int num3 = 0;
				int num4 = 0;
				for (int j = 0; j < musicComponent.transform.childCount; j++)
				{
					Component component3 = musicComponent.transform.GetChild(j).GetComponent<Component>();
					if (component3 != null)
					{
						list2.Add(component3.name);
						if ((bool)(component3 = musicComponent._toComponent))
						{
							num3 = num4;
						}
						num4++;
					}
				}
				num4 = EditorGUILayout.Popup(num3, list2.ToArray());
				if (num4 != num3)
				{
					Event obj3 = new Event();
					obj3.EventAction = EventAction.SetSwitch;
					obj3._parameter = list2[num4];
					musicComponent.OnProcessEvent(obj3, null);
				}
			}
		}

		private Component BuildSelection(Component component)
		{
			string[] array = new string[component.transform.childCount];
			int selectedIndex = 0;
			for (int i = 0; i < component.transform.childCount; i++)
			{
				Component component2 = component.transform.GetChild(i).GetComponent<Component>();
				array[i] = component2.name;
				if (component2 == component)
				{
					selectedIndex = i;
				}
			}
			selectedIndex = EditorGUILayout.Popup(selectedIndex, array);
			GUILayout.EndHorizontal();
			return component.transform.GetChild(selectedIndex).GetComponent<Component>();
		}
	}
	public class PreviewEventWindow : PreviewWindow
	{
		public Component _component;

		private bool _paused;

		private static GUIStyle playThumbIcon = new GUIStyle();

		private static GUIStyle pauseThumbIcon = new GUIStyle();

		private static GUIStyle stopThumbIcon = new GUIStyle();

		private Vector2 scrollVector;

		public override void Initialise(PreviewWindowData data, Previewer previewer)
		{
			GUIStyle gUIStyle = new GUIStyle();
			_previewer = previewer;
			_windowData = data;
			float num = gUIStyle.CalcSize(new GUIContent(data._name)).x + 50f;
			if (num < 200f)
			{
				num = 200f;
			}
			float height = 130f;
			rect = new Rect(_windowData._x, _windowData._y, num, height);
		}

		private static void LoadImages()
		{
			int num = 32;
			playThumbIcon.onNormal.background = GUIHelpers.LoadImage("icons/Play");
			playThumbIcon.onActive.background = GUIHelpers.LoadImage("icons/PlayActive");
			playThumbIcon.fixedWidth = num;
			playThumbIcon.fixedHeight = num;
			stopThumbIcon.onNormal.background = GUIHelpers.LoadImage("icons/Stop");
			stopThumbIcon.onActive.background = GUIHelpers.LoadImage("icons/Stop");
			pauseThumbIcon.onNormal.background = GUIHelpers.LoadImage("icons/Pause");
			pauseThumbIcon.onActive.background = GUIHelpers.LoadImage("icons/Pause");
		}

		public override PreviewWindowData Initialise(string name, Previewer previewer)
		{
			_windowData = PreviewWindowData.CreateWindow(PreviewWindowType.Event);
			_windowData._name = name;
			Initialise(_windowData, previewer);
			LoadImages();
			return _windowData;
		}

		public override bool OnWindow(bool ignoreClose = false)
		{
			GUI.enabled = FabricManager.Instance._enableEditorPreviewer;
			GUILayout.BeginVertical();
			_windowData._eventItem._event._eventName = EventManagerEditor.DropDownEventNames(_windowData._eventItem._event._eventName);
			EditorGUIUtility.labelWidth = 80f;
			_windowData._eventItem._event.EventAction = (EventAction)(object)EditorGUILayout.EnumPopup("Event Action:", _windowData._eventItem._event.EventAction);
			EditorGUIUtility.labelWidth = 0f;
			GUILayout.BeginHorizontal();
			bool flag = (GUI.enabled = EventManager.Instance.IsEventActive(_windowData._eventItem._event._eventName, null));
			if (GUILayout.Button("Stop"))
			{
				_windowData._eventItem._event.parentGameObject = null;
				_windowData._eventItem._event.EventAction = EventAction.StopAll;
				_windowData._eventItem._event._forceEventAction = true;
				EventManager.Instance.PostEvent(_windowData._eventItem._event);
				_windowData._eventItem._event.EventAction = EventAction.PlaySound;
			}
			if (!flag)
			{
				_paused = false;
			}
			string text = (_paused ? "Unpause" : "Pause");
			if (GUILayout.Button(text))
			{
				_windowData._eventItem._event.parentGameObject = null;
				_windowData._eventItem._event.EventAction = ((!_paused) ? EventAction.PauseSound : EventAction.UnpauseSound);
				_windowData._eventItem._event._forceEventAction = true;
				EventManager.Instance.PostEvent(_windowData._eventItem._event);
				_paused = !_paused;
			}
			GUI.enabled = FabricManager.Instance._enableEditorPreviewer;
			if (GUILayout.Button("Post"))
			{
				_windowData._eventItem._event.parentGameObject = null;
				_windowData._eventItem._event._forceEventAction = false;
				EventManager.Instance.PostEvent(_windowData._eventItem._event);
				_paused = false;
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(2f);
			if (flag)
			{
				GUILayout.Label("Status: Playing", GUILayout.MaxWidth(110f));
			}
			else
			{
				GUILayout.Label("Status: Stopped", GUILayout.MaxWidth(110f));
			}
			GUILayout.Space(5f);
			bool result = false;
			if (!ignoreClose)
			{
				result = GUILayout.Button("Close");
			}
			GUILayout.EndVertical();
			_windowData._x = rect.x;
			_windowData._y = rect.y;
			GUI.enabled = true;
			return result;
		}

		private Component BuildSelection(Component component)
		{
			string[] array = new string[component.transform.childCount];
			int selectedIndex = 0;
			for (int i = 0; i < component.transform.childCount; i++)
			{
				Component component2 = component.transform.GetChild(i).GetComponent<Component>();
				array[i] = component2.name;
				if (component2 == component)
				{
					selectedIndex = i;
				}
			}
			selectedIndex = EditorGUILayout.Popup(selectedIndex, array);
			GUILayout.EndHorizontal();
			return component.transform.GetChild(selectedIndex).GetComponent<Component>();
		}
	}
	public class Previewer : EditorWindow
	{
		private Vector2 scrollVector;

		private Vector2 scrollPreviewWindows;

		[SerializeField]
		private PreviewerSession currentPreviewerSession;

		private bool componentPaused;

		private bool _prevEnabledEditorPreviewer;

		private string sessionToCreateName = "";

		private Vector2 scrollArea;

		private bool _foldout;

		[MenuItem("Window/Fabric/Previewer", false, 16)]
		public static void init()
		{
			Previewer previewer = (Previewer)EditorWindow.GetWindow(typeof(Previewer));
			previewer.titleContent = new GUIContent("Previewer");
		}

		private static string GetPreviewerSessionFolder()
		{
			string fabricEditorPath = FabricManagerEditor.GetFabricEditorPath();
			if (fabricEditorPath.Length > 0)
			{
				string text = FabricManagerEditor.GetFabricEditorPath() + "/PreviewerSessions/";
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				return text;
			}
			return null;
		}

		public void SetPreviewerSession(string name)
		{
			string previewerSessionFolder = GetPreviewerSessionFolder();
			if (previewerSessionFolder != null)
			{
				currentPreviewerSession = (PreviewerSession)AssetDatabase.LoadAssetAtPath(previewerSessionFolder + name, typeof(PreviewerSession));
				if (currentPreviewerSession == null)
				{
					currentPreviewerSession = ScriptableObject.CreateInstance<PreviewerSession>();
					string path = AssetDatabase.GenerateUniqueAssetPath(previewerSessionFolder + name);
					AssetDatabase.CreateAsset(currentPreviewerSession, path);
				}
			}
		}

		private void EnablePreviewer()
		{
			if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
			{
				_prevEnabledEditorPreviewer = FabricManager.Instance._enableEditorPreviewer;
				FabricManager.Instance._enableEditorPreviewer = true;
				ComponentPreviewer.Enable(FabricManager.Instance._enableEditorPreviewer);
			}
		}

		private void DisablePreviewer()
		{
			if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
			{
				FabricManager.Instance._enableEditorPreviewer = _prevEnabledEditorPreviewer;
				ComponentPreviewer.Enable(FabricManager.Instance._enableEditorPreviewer);
			}
		}

		private void OnEnable()
		{
			if (currentPreviewerSession == null)
			{
				SetPreviewerSession("Default.asset");
			}
			GUIHelpers.RegisterEditorUpdate(base.Repaint);
			Refresh();
			EnablePreviewer();
		}

		private void OnDisable()
		{
			GUIHelpers.UnregisterEditorUpdate(base.Repaint);
			DisablePreviewer();
		}

		private void OnDestroy()
		{
			AssetDatabase.SaveAssets();
			DisablePreviewer();
			Reset();
		}

		private void OnFocus()
		{
		}

		private void OnLostFocus()
		{
		}

		private void OnSelectionChange()
		{
			Repaint();
		}

		private void Refresh()
		{
			if (currentPreviewerSession != null)
			{
				currentPreviewerSession.Refresh();
			}
		}

		private void Reset()
		{
		}

		private void doWindow(int id)
		{
			if (id < currentPreviewerSession.previewWindows.Count)
			{
				PreviewWindow previewWindow = currentPreviewerSession.previewWindows[id];
				if (previewWindow.OnWindow())
				{
					currentPreviewerSession.windowData.Remove(previewWindow._windowData);
					currentPreviewerSession.previewWindows.Remove(previewWindow);
					Repaint();
				}
				GUI.DragWindow();
			}
		}

		private void CheckContextMenu()
		{
			if (UnityEngine.Event.current.type != EventType.ContextClick)
			{
				return;
			}
			GenericMenu genericMenu = new GenericMenu();
			if (Selection.activeGameObject != null)
			{
				Component component = Selection.activeGameObject.GetComponent<Component>();
				if (component != null)
				{
					genericMenu.AddItem(new GUIContent("Add Component"), on: false, AddComponent);
				}
			}
			genericMenu.AddItem(new GUIContent("Add Event"), on: false, AddEvent);
			genericMenu.ShowAsContext();
			UnityEngine.Event.current.Use();
		}

		private void AddComponent()
		{
			Component component = Selection.activeGameObject.GetComponent<Component>();
			CreatePreviewComponentWindow(component);
		}

		private void CreatePreviewComponentWindow(Component component)
		{
			if (component != null)
			{
				PreviewComponentWindow previewComponentWindow = new PreviewComponentWindow();
				PreviewWindowData previewWindowData = previewComponentWindow.Initialise(component.name, this);
				previewComponentWindow.SetComponent(component);
				currentPreviewerSession.previewWindows.Add(previewComponentWindow);
				component.Guid = Guid.NewGuid().ToString();
				previewWindowData.Guid = component.Guid;
				currentPreviewerSession.windowData.Add(previewWindowData);
			}
		}

		private void AddEvent()
		{
			PreviewEventWindow previewEventWindow = new PreviewEventWindow();
			PreviewWindowData previewWindowData = previewEventWindow.Initialise("Event", this);
			previewWindowData._eventItem = new EventEditor.EventItem();
			previewWindowData._eventItem._event._eventName = "_UnSet_";
			currentPreviewerSession.previewWindows.Add(previewEventWindow);
			currentPreviewerSession.windowData.Add(previewWindowData);
		}

		public void OnGUI()
		{
			MenuBar.OnGUI("288086-previewer", box: true);
			if (GetFabricManager.Instance() == null)
			{
				GUI.enabled = false;
			}
			DropArea();
			scrollArea = GUILayout.BeginScrollView(scrollArea, "Box");
			DrawProperties();
			GUI.enabled = FabricManager.Instance._enableEditorPreviewer;
			DrawPreviewWindows();
			DrawGlobalProperties();
			GUI.enabled = true;
			GUILayout.EndScrollView();
			if (GUI.changed)
			{
				EditorUtility.SetDirty(currentPreviewerSession);
			}
		}

		public void DrawPreviewWindows()
		{
			scrollPreviewWindows = GUILayout.BeginScrollView(scrollPreviewWindows, "Box");
			if (currentPreviewerSession != null)
			{
				CheckContextMenu();
				BeginWindows();
				for (int i = 0; i < currentPreviewerSession.previewWindows.Count; i++)
				{
					PreviewWindow previewWindow = currentPreviewerSession.previewWindows[i];
					if (previewWindow != null)
					{
						previewWindow.rect = GUI.Window(i, previewWindow.rect, doWindow, previewWindow._windowData._name);
					}
				}
				EndWindows();
			}
			GUILayout.EndScrollView();
		}

		public void DrawGlobalProperties()
		{
			_foldout = EditorGUILayout.Foldout(_foldout, "Global Properties");
			if (_foldout)
			{
				GUILayout.BeginHorizontal("Box", GUILayout.MaxHeight(200f));
				GlobalParameterEditor.DrawGlobalParameters(EventManager.Instance);
				GlobalParameterEditor.DrawGlobalSwitches(EventManager.Instance);
				GUILayout.EndHorizontal();
			}
		}

		public void DrawProperties()
		{
			GUILayout.BeginHorizontal(GUILayout.MaxHeight(120f));
			GUILayout.BeginVertical("Box", GUILayout.MaxHeight(120f), GUILayout.MaxWidth(200f));
			GUILayout.BeginHorizontal();
			GUILayout.Label("Current Session: ");
			currentPreviewerSession = (PreviewerSession)EditorGUILayout.ObjectField("", currentPreviewerSession, typeof(PreviewerSession), false);
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
			GUILayout.BeginHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Name: ");
			sessionToCreateName = EditorGUILayout.TextField("", sessionToCreateName);
			GUILayout.EndHorizontal();
			if (GUILayout.Button("Create Session"))
			{
				currentPreviewerSession = PreviewerSession.CreateAsset<PreviewerSession>(sessionToCreateName);
				return;
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
			GUI.enabled = !EditorApplication.isPlayingOrWillChangePlaymode;
			bool flag = EditorGUILayout.Toggle(new GUIContent("Editor Previewer: "), FabricManager.Instance._enableEditorPreviewer);
			if (flag != FabricManager.Instance._enableEditorPreviewer)
			{
				FabricManager.Instance._enableEditorPreviewer = flag;
				ComponentPreviewer.Enable(flag);
			}
			FabricEditorData.GetData()._previewerGUIUpdateRate = EditorGUILayout.Slider("Update GUI Rate (ms): ", FabricEditorData.GetData()._previewerGUIUpdateRate, 0.05f, 1f);
			GUI.enabled = false;
			GUILayout.Space(5f);
			if (GUILayout.Button("Reset All"))
			{
				Reset();
			}
			GUILayout.EndVertical();
			if (currentPreviewerSession != null)
			{
				GUILayout.BeginVertical("Box", GUILayout.ExpandHeight(expand: true), GUILayout.ExpandWidth(expand: true));
				if (Selection.activeGameObject != null)
				{
					Component component = Selection.activeGameObject.GetComponent<Component>();
					if (component != null)
					{
						scrollVector = PreviewComponentWindow.DrawComponent(component, default(Rect), ref componentPaused, scrollVector);
					}
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();
		}

		public void DropArea()
		{
			UnityEngine.Event current = UnityEngine.Event.current;
			switch (current.type)
			{
			case EventType.DragUpdated:
			case EventType.DragPerform:
			{
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				if (current.type != EventType.DragPerform)
				{
					break;
				}
				DragAndDrop.AcceptDrag();
				UnityEngine.Object[] objectReferences = DragAndDrop.objectReferences;
				foreach (UnityEngine.Object obj in objectReferences)
				{
					GameObject gameObject = obj as GameObject;
					Component component = null;
					if (gameObject != null)
					{
						component = gameObject.GetComponent<Component>();
					}
					if (!(component != null) || !(currentPreviewerSession != null))
					{
						continue;
					}
					for (int j = 0; j < currentPreviewerSession.previewWindows.Count; j++)
					{
						if (currentPreviewerSession.previewWindows[j] is PreviewComponentWindow previewComponentWindow2 && previewComponentWindow2._windowData._component == component)
						{
							return;
						}
					}
					CreatePreviewComponentWindow(component);
				}
				break;
			}
			}
		}
	}
	public enum PreviewWindowType
	{
		Component,
		Event
	}
	[Serializable]
	public class PreviewWindowData
	{
		[SerializeField]
		public float _x;

		[SerializeField]
		public float _y;

		[SerializeField]
		public EventEditor.EventItem _eventItem;

		[SerializeField]
		public string Guid;

		[SerializeField]
		public Component _component;

		[SerializeField]
		public string _name = "";

		[SerializeField]
		private PreviewWindowType _type;

		public PreviewWindowType Type => _type;

		private PreviewWindowData(PreviewWindowType type)
		{
			_type = type;
		}

		public static PreviewWindowData CreateWindow(PreviewWindowType type)
		{
			if (type == PreviewWindowType.Component)
			{
				return new PreviewWindowData(PreviewWindowType.Component);
			}
			return new PreviewWindowData(PreviewWindowType.Event);
		}
	}
	[Serializable]
	internal class PreviewerSession : ScriptableObject
	{
		[SerializeField]
		public List<PreviewWindowData> windowData = new List<PreviewWindowData>();

		[NonSerialized]
		public List<PreviewWindow> previewWindows = new List<PreviewWindow>();

		public static T CreateAsset<T>(string name) where T : ScriptableObject
		{
			string fabricEditorPath = FabricManagerEditor.GetFabricEditorPath();
			string text = fabricEditorPath + "/PreviewerSessions/";
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			T val = ScriptableObject.CreateInstance<T>();
			string path = AssetDatabase.GenerateUniqueAssetPath(text + name + ".asset");
			AssetDatabase.CreateAsset(val, path);
			return val;
		}

		public void Refresh()
		{
			for (int i = 0; i < windowData.Count; i++)
			{
				if (windowData[i].Type == PreviewWindowType.Component)
				{
					PreviewComponentWindow previewComponentWindow = new PreviewComponentWindow();
					previewComponentWindow.Initialise(windowData[i], null);
					previewWindows.Add(previewComponentWindow);
				}
				else
				{
					PreviewEventWindow previewEventWindow = new PreviewEventWindow();
					previewEventWindow.Initialise(windowData[i], null);
					previewWindows.Add(previewEventWindow);
				}
			}
		}
	}
	public static class DictonaryHelper
	{
		public static void RenameKey<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey fromKey, TKey toKey)
		{
			TValue value = dic[fromKey];
			dic.Remove(fromKey);
			dic[toKey] = value;
		}
	}
	public class GlobalParameterEditor : EditorWindow
	{
		private Vector2 scrollPosition = default(Vector2);

		private static string globalRTPParameterName;

		private static int globalRTPParameterSelection = 0;

		private static string globalSwitchName;

		private static string subSwitchName;

		private static int globalSwitchSelection = 0;

		private static Vector2 switchScrollView = default(Vector2);

		private static float globalParameterHeight = 200f;

		private static float globalSwitchHeight = 200f;

		[MenuItem("Window/Fabric/Global Parameters", false, 13)]
		private static void Init()
		{
			GlobalParameterEditor globalParameterEditor = (GlobalParameterEditor)EditorWindow.GetWindow(typeof(GlobalParameterEditor));
			globalParameterEditor.titleContent = new GUIContent("Global Parameters");
		}

		private void OnEnable()
		{
			globalRTPParameterSelection = 0;
			globalSwitchSelection = 0;
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(UpdateGUI));
		}

		private void OnDestroy()
		{
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(UpdateGUI));
		}

		private void UpdateGUI()
		{
			Repaint();
		}

		private void OnGUI()
		{
			MenuBar.OnGUI("442041-global-parameters", box: true);
			if (!(GetFabricManager.Instance() == null))
			{
				EventManager component = GetFabricManager.Instance().GetComponent<EventManager>();
				if (!(component == null))
				{
					GUILayout.Space(5f);
					scrollPosition = GUILayout.BeginScrollView(scrollPosition);
					GUILayout.BeginHorizontal(GUILayout.MinHeight(globalParameterHeight));
					DrawGlobalParameters(component);
					GUILayout.EndHorizontal();
					GUILayout.Space(10f);
					GUILayout.BeginHorizontal(GUILayout.MinHeight(globalSwitchHeight));
					DrawGlobalSwitches(component);
					GUILayout.EndHorizontal();
					GUILayout.EndScrollView();
				}
			}
		}

		public static void DrawGlobalParameters(EventManager eventManager)
		{
			GlobalParameterManager.GlobalParametersFastList globalRTParameters = eventManager._globalParameterManager._globalRTParameters;
			string[] array = null;
			if (eventManager != null)
			{
				array = globalRTParameters.Keys();
			}
			if (array == null || array.Length == 0)
			{
				globalRTPParameterSelection = 0;
			}
			GUILayout.BeginVertical("Box", GUILayout.MinHeight(globalParameterHeight));
			GUILayout.BeginHorizontal();
			bool flag = GUILayout.Button("Add New Global Parameter");
			globalRTPParameterName = EditorGUILayout.TextField("", globalRTPParameterName);
			if (flag && !EventManagerEditor.IsNullOrWhiteSpace(globalRTPParameterName))
			{
				globalRTPParameterName.Trim();
				if (globalRTParameters.Contains(globalRTPParameterName))
				{
					EditorUtility.DisplayDialog("Fabric Warning", "Global Parameter [" + globalRTPParameterName + "] already exists, choose a different name", "Ok");
				}
				else
				{
					globalRTParameters.Add(globalRTPParameterName, new GlobalParameter());
				}
				globalRTPParameterName = "";
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Global Parameter:", GUILayout.MaxWidth(110f));
			int num = EditorGUILayout.Popup(globalRTPParameterSelection, array);
			if (array.Length > 0 && num != globalRTPParameterSelection)
			{
				globalRTPParameterSelection = num;
			}
			if (GUILayout.Button("Del", GUILayout.Width(48f)) && array.Length > 0 && globalRTPParameterSelection >= 0)
			{
				globalRTParameters.Remove(array[globalRTPParameterSelection]);
				globalRTPParameterSelection--;
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal("Box");
			if (globalRTPParameterSelection >= 0 && globalRTPParameterSelection < array.Length)
			{
				GlobalParameter globalParameter = globalRTParameters.FindItemByIndex(globalRTPParameterSelection);
				if (globalParameter != null)
				{
					GUILayout.BeginVertical();
					string text = EditorGUILayout.TextField("Name: ", array[globalRTPParameterSelection]);
					if (text != array[globalRTPParameterSelection] && !globalRTParameters.UpdateKey(array[globalRTPParameterSelection], text))
					{
						EditorUtility.DisplayDialog("Fabric Warning", "Cant rename Global Parameter [" + text + "] already exists, choose a different name", "Ok");
					}
					globalParameter._min = EditorGUILayout.FloatField("Min: ", globalParameter._min);
					globalParameter._max = EditorGUILayout.FloatField("Max: ", globalParameter._max);
					globalParameter._seekSpeed = EditorGUILayout.FloatField("Seek: ", globalParameter._seekSpeed);
					GUILayout.BeginHorizontal();
					globalParameter.SetValue(EditorGUILayout.FloatField("Value: ", globalParameter.GetValue()));
					EditorGUILayout.LabelField("[ " + globalParameter.GetCurrentValue() + " ]");
					GUILayout.EndHorizontal();
					GUILayout.EndVertical();
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}

		public static void DrawGlobalSwitches(EventManager eventManager)
		{
			GlobalParameterManager.GlobalSwitchFastList globalSwitches = EventManager.Instance._globalParameterManager._globalSwitches;
			string[] array = null;
			if (eventManager != null)
			{
				array = globalSwitches.Keys();
			}
			if (array == null || array.Length == 0)
			{
				globalSwitchSelection = 0;
			}
			GUILayout.BeginVertical("Box", GUILayout.MinHeight(globalSwitchHeight));
			GUILayout.BeginHorizontal();
			bool flag = GUILayout.Button("Add New Global Switch");
			globalSwitchName = EditorGUILayout.TextField("", globalSwitchName);
			if (flag && !EventManagerEditor.IsNullOrWhiteSpace(globalSwitchName))
			{
				globalSwitchName.Trim();
				if (globalSwitches.Contains(globalSwitchName))
				{
					EditorUtility.DisplayDialog("Fabric Warning", "Global Switch [" + globalSwitchName + "] already exists, choose a different name", "Ok");
				}
				else
				{
					globalSwitches.Add(globalSwitchName, new GlobalSwitch());
				}
				globalSwitchName = "";
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			GUILayout.BeginVertical("Box");
			GUILayout.BeginHorizontal();
			GUILayout.Label("Global Switch:", GUILayout.MaxWidth(110f));
			int num = EditorGUILayout.Popup(globalSwitchSelection, array);
			if (array.Length > 0 && num != globalSwitchSelection)
			{
				globalSwitchSelection = num;
			}
			if (GUILayout.Button("Del", GUILayout.Width(48f)) && array.Length > 0 && globalSwitchSelection >= 0)
			{
				eventManager._globalParameterManager._globalSwitches.Remove(array[globalSwitchSelection]);
				globalSwitchSelection--;
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(2f);
			GUILayout.EndVertical();
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			GUILayout.Space(5f);
			if (globalSwitchSelection >= 0 && globalSwitchSelection < array.Length)
			{
				GlobalSwitch globalSwitch = globalSwitches.FindItemByIndex(globalSwitchSelection);
				if (globalSwitch != null)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label("Switch: ", GUILayout.MaxWidth(50f));
					subSwitchName = EditorGUILayout.TextField("", subSwitchName);
					if (GUILayout.Button("Add New Switch"))
					{
						if (!globalSwitch.AddSwitch(subSwitchName))
						{
							EditorUtility.DisplayDialog("Fabric Warning", "Switch [" + subSwitchName + "] already exists, choose a different name", "Ok");
						}
						subSwitchName = "";
					}
					GUILayout.EndHorizontal();
					switchScrollView = GUILayout.BeginScrollView(switchScrollView);
					if ((globalSwitch._defaultSwitch == null || globalSwitch._defaultSwitch.Length == 0) && globalSwitch._switches.Count > 0)
					{
						globalSwitch._defaultSwitch = globalSwitch._switches[0]._name;
					}
					for (int i = 0; i < globalSwitch._switches.Count; i++)
					{
						GUILayout.BeginHorizontal("Box", GUILayout.ExpandWidth(expand: true));
						GlobalSwitch.Switch obj = globalSwitch._switches[i];
						if (GUILayout.Button("Del", GUILayout.MaxWidth(50f)))
						{
							globalSwitch._switches.RemoveAt(i);
						}
						GUILayout.Label((((globalSwitch.GetActiveSwitch() == obj) ? true : false) && Application.isPlaying) ? "Name <- " : "Name: ", GUILayout.MinWidth(70f), GUILayout.MaxWidth(70f));
						string text = EditorGUILayout.TextField("", obj._name);
						if (text != obj._name)
						{
							bool flag2 = false;
							for (int j = 0; j < globalSwitch._switches.Count; j++)
							{
								if (globalSwitch._switches[j]._name == text)
								{
									EditorUtility.DisplayDialog("Fabric Warning", "Switch [" + text + "] already exists, choose a different name", "Ok");
									flag2 = true;
									break;
								}
							}
							if (!flag2)
							{
								obj._name = text;
							}
						}
						if (GUILayout.Toggle(globalSwitch._defaultSwitch == obj._name, "Default: "))
						{
							globalSwitch._defaultSwitch = obj._name;
						}
						GUILayout.EndHorizontal();
					}
					GUILayout.EndScrollView();
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.EndVertical();
		}
	}
	[CanEditMultipleObjects]
	[CustomEditor(typeof(MIDIComponent))]
	public class MIDIComponentEditor : Editor
	{
		private MIDIComponent midiComponent;

		private ComponentEditor componentEditor = new ComponentEditor();

		private SerializedProperty loop;

		private SerializedProperty controlTargetComponents;

		private SerializedProperty ignoreNoteOff;

		private bool _foldout = true;

		[MenuItem("Fabric/Components/MIDIComponent")]
		private static void About()
		{
			GameObject activeGameObject = Selection.activeGameObject;
			if (!(activeGameObject == null))
			{
				GameObject gameObject = new GameObject("MIDIComponent");
				gameObject.transform.parent = activeGameObject.transform;
				gameObject.AddComponent<MIDIComponent>();
			}
		}

		private void OnEnable()
		{
			midiComponent = base.target as MIDIComponent;
			componentEditor.RegisterSerialisableObject(base.serializedObject);
			loop = base.serializedObject.FindProperty("_loop");
			ignoreNoteOff = base.serializedObject.FindProperty("_ignoreNoteOff");
			controlTargetComponents = base.serializedObject.FindProperty("_controlTargetComponents");
			GUIHelpers.RegisterEditorUpdate(base.Repaint);
		}

		private void OnDisable()
		{
			GUIHelpers.UnregisterEditorUpdate(base.Repaint);
		}

		private void OnDestroy()
		{
		}

		public override void OnInspectorGUI()
		{
			componentEditor.InspectorGUI(midiComponent, "627713");
			base.serializedObject.Update();
			_foldout = EditorGUILayout.Foldout(_foldout, "MIDI Component Properties");
			if (_foldout)
			{
				GUILayout.BeginVertical("box");
				EditorGUILayout.PropertyField(ignoreNoteOff, new GUIContent("Ignore NoteOff:"));
				EditorGUILayout.PropertyField(loop, new GUIContent("Loop:"));
				EditorGUILayout.PropertyField(controlTargetComponents, new GUIContent("Control Target Components:"));
				GUILayout.Space(5f);
				int num = midiComponent.midiFilePath.LastIndexOf("/");
				GUILayout.Label("MIDI: " + midiComponent.midiFilePath.Remove(0, num + 1));
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Load"))
				{
					string text = EditorUtility.OpenFilePanel("Load MIDI File", "", "mid");
					if (text != null && text.Length != 0)
					{
						midiComponent.LoadMidi(text);
					}
				}
				if (GUILayout.Button("Reload"))
				{
					midiComponent.ReloadMidi();
				}
				if (GUILayout.Button("Unload"))
				{
					midiComponent.UnloadMidi();
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(10f);
				for (int i = 0; i < midiComponent.midiSequencer.seqEvt.Events.Count; i++)
				{
					MidiSequencerEvent midiSequencerEvent = midiComponent.midiSequencer.seqEvt.Events[i];
					MidiTrack midiTrack = midiComponent.midiSequencer.tracks[i];
					GUILayout.BeginHorizontal("box");
					GUILayout.BeginVertical();
					GUILayout.BeginHorizontal();
					GUILayout.Label("Track_" + i, GUILayout.MaxWidth(60f));
					GUILayout.BeginHorizontal();
					GUILayout.Label(" - Component", GUILayout.MaxWidth(80f));
					midiSequencerEvent.component = (Component)EditorGUILayout.ObjectField("", midiSequencerEvent.component, typeof(Component), true);
					GUILayout.EndHorizontal();
					midiTrack.Mute = GUILayout.Toggle(midiTrack.Mute, new GUIContent("Mute"));
					GUILayout.EndHorizontal();
					GUILayout.Label("Midi Events: " + midiTrack.eventIndex + "/" + midiTrack.MidiEvents.Length);
					GUILayout.EndVertical();
					GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();
			}
			GUIHelpers.CheckGUIHasChanged(base.serializedObject, midiComponent);
		}

		public static string[] MidiNotes()
		{
			List<string> list = new List<string>();
			for (int i = 0; i <= 127; i++)
			{
				list.Add(NoteToString(i));
			}
			return list.ToArray();
		}

		public static string NoteToString(int note)
		{
			return note switch
			{
				0 => "C-1", 
				1 => "C#-1", 
				2 => "D-1", 
				3 => "D#-1", 
				4 => "E-1", 
				5 => "F-1", 
				6 => "F#-1", 
				7 => "G-1", 
				8 => "G#-1", 
				9 => "A-1", 
				10 => "A#-1", 
				11 => "B-1", 
				12 => "C0 ", 
				13 => "C#0", 
				14 => "D0 ", 
				15 => "D#0", 
				16 => "E0 ", 
				17 => "F0 ", 
				18 => "F#0", 
				19 => "G0 ", 
				20 => "G#0", 
				21 => "A0 ", 
				22 => "A#0", 
				23 => "B0 ", 
				24 => "C1 ", 
				25 => "C#1", 
				26 => "D1 ", 
				27 => "D#1", 
				28 => "E1 ", 
				29 => "F1 ", 
				30 => "F#1", 
				31 => "G1 ", 
				32 => "G#1", 
				33 => "A1 ", 
				34 => "A#1", 
				35 => "B1 ", 
				36 => "C2 ", 
				37 => "C#2", 
				38 => "D2 ", 
				39 => "D#2", 
				40 => "E2 ", 
				41 => "F2 ", 
				42 => "F#2", 
				43 => "G2 ", 
				44 => "G#2", 
				45 => "A2 ", 
				46 => "A#2", 
				47 => "B2 ", 
				48 => "C3 ", 
				49 => "C#3", 
				50 => "D3 ", 
				51 => "D#3", 
				52 => "E3 ", 
				53 => "F3 ", 
				54 => "F#3", 
				55 => "G3 ", 
				56 => "G#3", 
				57 => "A3 ", 
				58 => "A#3", 
				59 => "B3 ", 
				60 => "C4 ", 
				61 => "C#4", 
				62 => "D4 ", 
				63 => "D#4", 
				64 => "E4 ", 
				65 => "F4 ", 
				66 => "F#4", 
				67 => "G4 ", 
				68 => "G#4", 
				69 => "A4 ", 
				70 => "A#4", 
				71 => "B4 ", 
				72 => "C5 ", 
				73 => "C#5", 
				74 => "D5 ", 
				75 => "D#5", 
				76 => "E5 ", 
				77 => "F5 ", 
				78 => "F#5", 
				79 => "G5 ", 
				80 => "G#5", 
				81 => "A5 ", 
				82 => "A#5", 
				83 => "B5 ", 
				84 => "C6 ", 
				85 => "C#6", 
				86 => "D6 ", 
				87 => "D#6", 
				88 => "E6 ", 
				89 => "F6 ", 
				90 => "F#6", 
				91 => "G6 ", 
				92 => "G#6", 
				93 => "A6 ", 
				94 => "A#6", 
				95 => "B6 ", 
				96 => "C7 ", 
				97 => "C#7", 
				98 => "D7 ", 
				99 => "D#7", 
				100 => "E7 ", 
				101 => "F7 ", 
				102 => "F#7", 
				103 => "G7 ", 
				104 => "G#7", 
				105 => "A7 ", 
				106 => "A#7", 
				107 => "B7 ", 
				108 => "C8 ", 
				109 => "C#8", 
				110 => "D8 ", 
				111 => "D#8", 
				112 => "E8 ", 
				113 => "F8 ", 
				114 => "F#8", 
				115 => "G8 ", 
				116 => "G#8", 
				117 => "A8 ", 
				118 => "A#8", 
				119 => "B8 ", 
				120 => "C9 ", 
				121 => "C#9", 
				122 => "D9 ", 
				123 => "D#9", 
				124 => "E9 ", 
				125 => "F9 ", 
				126 => "F#9", 
				127 => "G9 ", 
				_ => "ERR", 
			};
		}
	}
	[CustomEditor(typeof(AudioCapture))]
	public class AudioCaptureEditor : Editor
	{
		private AudioCapture audioCapture;

		private string filename;

		private void OnEnable()
		{
			audioCapture = base.target as AudioCapture;
		}

		public override void OnInspectorGUI()
		{
			MenuBar.OnGUI("290596-audiocapture", box: true);
			GUILayout.BeginVertical("Box");
			audioCapture._capture = GUILayout.Toggle(audioCapture._capture, "Capture");
			if (GUILayout.Button("Save"))
			{
				string text = EditorUtility.SaveFilePanel("Save capture", "", "Capture.wav", "wav");
				if (text != null && text.Length != 0)
				{
					audioCapture.Save(text);
				}
			}
			GUILayout.EndVertical();
		}
	}
	internal enum MenuItemsOrder
	{
		EventEditor,
		EventSequencer,
		EventListenerOverride,
		EventLog,
		EventMonitor,
		EventViewer,
		AudioBuses,
		CustomCurvesEditor,
		AudioAssetImporter,
		AudioSourcePool,
		Mixer,
		CompactMixer,
		RuntimeParameter,
		GlobalParameterEditor,
		Timeline,
		MultiWavedisplay,
		ComponentPreviewer,
		GraphView,
		SamplerManager,
		Languages,
		MultiEditAudioClip,
		AudioReport,
		PostInstallChecker,
		WhatsNew,
		VRAudio,
		About,
		FabricManual,
		FabricResources,
		FabricForums,
		FabricSupport
	}
	[InitializeOnLoad]
	internal class PlaymodePersistentMonitor
	{
		private static float timerRepaint;

		static PlaymodePersistentMonitor()
		{
			EditorApplication.playModeStateChanged -= PlayModeChanged;
			EditorApplication.playModeStateChanged += PlayModeChanged;
			EditorApplication.projectChanged -= HierarchyChanged;
			EditorApplication.projectChanged += HierarchyChanged;
			EditorApplication.hierarchyChanged -= HierarchyChanged;
			EditorApplication.hierarchyChanged += HierarchyChanged;
			EditorApplication.hierarchyWindowItemByEntityIdOnGUI = (EditorApplication.HierarchyWindowItemByEntityIdCallback)Delegate.Combine(EditorApplication.hierarchyWindowItemByEntityIdOnGUI, new EditorApplication.HierarchyWindowItemByEntityIdCallback(HierarchyChanged));
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(Update));
		}

		private static void Update()
		{
			if (!EditorApplication.isPlaying)
			{
				return;
			}
			GameObject activeGameObject = Selection.activeGameObject;
			if (!(activeGameObject != null))
			{
				return;
			}
			Component component = activeGameObject.GetComponent<Component>();
			if (component != null)
			{
				if (timerRepaint <= 0f)
				{
					EditorUtility.SetDirty(activeGameObject);
					timerRepaint = 0.2f;
				}
				else
				{
					timerRepaint -= FabricTimer.GetRealtimeDelta();
				}
			}
		}

		private static void HierarchyChanged(EntityId instanceID, Rect selectionRect)
		{
			if (EditorApplication.isPlaying && FabricManager.IsInitialised() && FabricEditorData.GetData()._playmodePersistance)
			{
				GameObject activeGameObject = Selection.activeGameObject;
				if (activeGameObject != null)
				{
					PlaymodePersistance.SetGameObject(activeGameObject);
				}
			}
		}

		private static void HierarchyChanged()
		{
			if (EditorApplication.isPlaying && FabricManager.IsInitialised() && FabricEditorData.GetData()._playmodePersistance)
			{
				GameObject activeGameObject = Selection.activeGameObject;
				if (activeGameObject != null)
				{
					PlaymodePersistance.SetGameObject(activeGameObject);
				}
			}
		}

		private static void PlayModeChanged()
		{
			if (FabricManager.IsInitialised() && FabricEditorData.GetData()._playmodePersistance)
			{
				if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
				{
					PlaymodePersistance.Reset();
				}
				if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
				{
					PlaymodePersistance.ApplyChanges();
				}
			}
		}

		private static void PlayModeChanged(PlayModeStateChange state)
		{
			PlayModeChanged();
		}
	}
	[Serializable]
	internal class CachedComponent
	{
		[SerializeField]
		public UnityEngine.Component _component;

		[SerializeField]
		public Serialization.CachedFieldScan _fieldScan;
	}
	public class PlaymodePersistance
	{
		private static GameObject _gameObject;

		private static Dictionary<EntityId, List<CachedComponent>> _cachedGameObjects = new Dictionary<EntityId, List<CachedComponent>>();

		public static void ApplyChanges()
		{
			foreach (KeyValuePair<EntityId, List<CachedComponent>> cachedGameObject in _cachedGameObjects)
			{
				#pragma warning disable CS0618
				GameObject gameObject = EditorUtility.EntityIdToObject(cachedGameObject.Key) as GameObject;
				#pragma warning restore CS0618
				if (!(gameObject != null))
				{
					continue;
				}
				UnityEngine.Component[] components = gameObject.GetComponents<UnityEngine.Component>();
				bool flag = false;
				for (int i = 0; i < components.Length; i++)
				{
					UnityEngine.Component component = components[i];
					CachedComponent cachedComponent = cachedGameObject.Value[i];
					Dictionary<string, Serialization.IField> targetFields = new Dictionary<string, Serialization.IField>();
					ScanTarget(component, ref targetFields);
					foreach (KeyValuePair<string, object> item in cachedComponent._fieldScan.Update())
					{
						if (targetFields.ContainsKey(item.Key))
						{
							targetFields[item.Key].SetValue(item.Value);
							if (item.Key.EndsWith("#"))
							{
								ScanTarget(component, ref targetFields);
							}
							flag = true;
						}
					}
				}
				if (flag)
				{
					GUIHelpers.ApplyChangesToPrefab(gameObject);
				}
			}
			Reset();
		}

		private static void ScanTarget(UnityEngine.Component component, ref Dictionary<string, Serialization.IField> targetFields)
		{
			targetFields.Clear();
			foreach (Serialization.IField item in Serialization.EnumerateFields(component))
			{
				targetFields[item.FieldName] = item;
			}
		}

		public static void Reset()
		{
			_cachedGameObjects.Clear();
			_gameObject = null;
		}

		public static void SetGameObject(GameObject gameObject)
		{
			if (!(_gameObject != gameObject))
			{
				return;
			}
			EntityId entityId = gameObject.GetEntityId();
			UnityEngine.Component[] components = gameObject.GetComponents(typeof(UnityEngine.Component));
			if (_cachedGameObjects.ContainsKey(entityId))
			{
				if (_cachedGameObjects[entityId].Count != components.Length)
				{
					CollectGameObjectComponents(components, _cachedGameObjects[entityId]);
				}
			}
			else
			{
				List<CachedComponent> list = new List<CachedComponent>();
				CollectGameObjectComponents(components, list);
				_cachedGameObjects.Add(entityId, list);
			}
			_gameObject = gameObject;
		}

		private static void CollectGameObjectComponents(UnityEngine.Component[] components, List<CachedComponent> cachedComponents)
		{
			cachedComponents.Clear();
			foreach (UnityEngine.Component component in components)
			{
				CachedComponent cachedComponent = new CachedComponent();
				cachedComponent._component = component;
				cachedComponent._fieldScan = new Serialization.CachedFieldScan(component);
				cachedComponents.Add(cachedComponent);
			}
		}
	}
	[CanEditMultipleObjects]
	[CustomEditor(typeof(RandomAudioClipComponent))]
	public class RandomAudioClipComponentEditor : Editor
	{
		private ComponentEditor componentEditor = new ComponentEditor();

		private RandomAudioClipComponent randomAudioClipComponent;

		private bool _foldout = true;

		private SerializedProperty playMode;

		private SerializedProperty shareRandomNoRepeatHistory;

		private SerializedProperty looped;

		private SerializedProperty delay;

		private SerializedProperty delayOnFirstPlay;

		[MenuItem("Fabric/Components/RandomAudioClipComponent")]
		private static void About()
		{
			GameObject activeGameObject = Selection.activeGameObject;
			if (!(activeGameObject == null))
			{
				GameObject gameObject = new GameObject("RandomAudioClipComponent");
				gameObject.transform.parent = activeGameObject.transform;
				gameObject.AddComponent<RandomAudioClipComponent>();
			}
		}

		private void OnEnable()
		{
			randomAudioClipComponent = base.target as RandomAudioClipComponent;
			componentEditor.RegisterSerialisableObject(base.serializedObject);
			playMode = base.serializedObject.FindProperty("_playMode");
			shareRandomNoRepeatHistory = base.serializedObject.FindProperty("_shareRandomNoRepeatHistory");
			looped = base.serializedObject.FindProperty("_looped");
			delay = base.serializedObject.FindProperty("_loopDelay");
			delayOnFirstPlay = base.serializedObject.FindProperty("_delayOnFirstPlay");
			randomAudioClipComponent.InitialiseWeights();
			EditorApplication.hierarchyChanged -= OnHierarchyChange;
			EditorApplication.hierarchyChanged += OnHierarchyChange;
		}

		private void OnDestroy()
		{
		}

		private void OnHierarchyChange()
		{
			if (randomAudioClipComponent != null)
			{
				randomAudioClipComponent.InitialiseWeights();
			}
		}

		public override void OnInspectorGUI()
		{
			if (componentEditor.InspectorGUI(randomAudioClipComponent, "288073-randomaudioclipcomponent"))
			{
				return;
			}
			base.serializedObject.Update();
			_foldout = EditorGUILayout.Foldout(_foldout, "Random Component Properties");
			if (_foldout)
			{
				GUILayout.BeginVertical("box");
				int enumValueIndex = playMode.enumValueIndex;
				EditorGUILayout.PropertyField(playMode, new GUIContent("Play Mode: "));
				GUILayout.Space(5f);
				EditorGUILayout.PropertyField(shareRandomNoRepeatHistory, new GUIContent("Share History: "));
				GUILayout.Space(5f);
				if (playMode.enumValueIndex == 0)
				{
					looped.boolValue = false;
					Color backgroundColor = GUI.backgroundColor;
					GUI.color = Color.red;
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					GUILayout.Label("Looped property is not supported in random play mode");
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
					GUI.color = backgroundColor;
					GUILayout.Space(5f);
					GUI.enabled = false;
				}
				EditorGUILayout.PropertyField(looped, new GUIContent("Looped: "));
				if (!looped.boolValue)
				{
					GUI.enabled = false;
				}
				EditorGUILayout.PropertyField(delayOnFirstPlay, new GUIContent("DelayOnFirstPlay: "));
				EditorGUILayout.Slider(delay, 0f, 60f, new GUIContent("Delay (secs): "));
				GUILayout.BeginHorizontal();
				EditorGUILayout.MinMaxSlider(new GUIContent("Delay Randomization: "), ref randomAudioClipComponent._delayRandomization, ref randomAudioClipComponent._delayMaxRandomization, 0f, 240f);
				randomAudioClipComponent._delayRandomization = EditorGUILayout.FloatField("", randomAudioClipComponent._delayRandomization, GUILayout.MaxWidth(64f));
				randomAudioClipComponent._delayMaxRandomization = EditorGUILayout.FloatField("", randomAudioClipComponent._delayMaxRandomization, GUILayout.MaxWidth(64f));
				GUILayout.EndHorizontal();
				if (!looped.boolValue)
				{
					GUI.enabled = true;
				}
				GUILayout.Space(5f);
				if (playMode.enumValueIndex != enumValueIndex)
				{
					randomAudioClipComponent.InitialiseRandomComponent((RandomComponentPlayMode)playMode.enumValueIndex);
				}
				GUILayout.Space(5f);
				if (randomAudioClipComponent._audioClips == null)
				{
					randomAudioClipComponent._audioClips = new AudioClip[1];
				}
				int num = EditorGUILayout.IntField("AudioClips Size:", randomAudioClipComponent._audioClips.Length);
				if (num != randomAudioClipComponent._audioClips.Length)
				{
					AudioClip[] array = new AudioClip[num];
					for (int i = 0; i < num; i++)
					{
						if (randomAudioClipComponent._audioClips.Length > i)
						{
							array[i] = randomAudioClipComponent._audioClips[i];
						}
					}
					randomAudioClipComponent._audioClips = array;
				}
				for (int j = 0; j < randomAudioClipComponent._audioClips.Length; j++)
				{
					AudioClip audioClip = randomAudioClipComponent._audioClips[j];
					GUILayout.BeginHorizontal();
					string text = ":";
					if (audioClip != null && audioClip == randomAudioClipComponent.AudioClip)
					{
						text = ":<--";
					}
					randomAudioClipComponent._audioClips[j] = (AudioClip)EditorGUILayout.ObjectField(j + text, audioClip, typeof(AudioClip), false);
					GUILayout.EndHorizontal();
				}
				GUILayout.Space(5f);
				if (GUILayout.Button("Clear List"))
				{
					randomAudioClipComponent._audioClips = new AudioClip[1];
				}
				GUILayout.EndVertical();
			}
			GUIHelpers.CheckGUIHasChanged(base.serializedObject, randomAudioClipComponent);
		}
	}
	internal class EditEventWindow : EditorWindow
	{
		public delegate void OnCreateHandler(string eventName);

		public delegate void OnRenameHandler(string originalEventName, string eventName);

		public static EditEventWindow window;

		private static string _label;

		private static string _originalEventName = "";

		private static string _eventName = "";

		private bool notification;

		private static event OnCreateHandler OnCreateEvent;

		private static event OnRenameHandler OnRenameEvent;

		public static void Rename(string eventName, OnRenameHandler onRename, float x, float y)
		{
			if (window == null)
			{
				_label = "Rename";
				window = ScriptableObject.CreateInstance<EditEventWindow>();
				window.position = new Rect(x, y, 350f, 100f);
				window.titleContent = new GUIContent(_label);
				window.Show();
				EditEventWindow.OnRenameEvent = onRename.Invoke;
				_originalEventName = eventName;
				_eventName = eventName;
			}
		}

		public static void Create(OnCreateHandler onCreate, float x, float y)
		{
			if (window == null)
			{
				_label = "Add";
				window = ScriptableObject.CreateInstance<EditEventWindow>();
				window.position = new Rect(x, y, 280f, 200f);
				window.titleContent = new GUIContent(_label);
				window.Show();
				EditEventWindow.OnCreateEvent = onCreate.Invoke;
				_eventName = "";
			}
		}

		private void OnGUI()
		{
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			EditorGUIUtility.labelWidth = 100f;
			_eventName = EditorGUILayout.TextField("Event Name: ", _eventName);
			EditorGUIUtility.labelWidth = 0f;
			if (GUILayout.Button(_label) && !notification)
			{
				if (_label == "Add")
				{
					EditEventWindow.OnCreateEvent(_eventName);
					Close();
				}
				else if (_label == "Rename")
				{
					EditEventWindow.OnRenameEvent(_originalEventName, _eventName);
					Close();
				}
				notification = true;
			}
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
		}
	}
	public class EventEditorBase : EditorWindow
	{
		private enum EventEditorWidths
		{
			NO = 40,
			DELETE = 40,
			OVERRIDE_ON_ACTION = 150,
			EVENT_ACTION = 150,
			COMPONENT = 150
		}

		private class EventPropertiesData
		{
			public EventEditorBase eventEditorBase;

			public float x;

			public float y;
		}

		private Vector2 scrollPosition = default(Vector2);

		private Vector2 eventEditorActionListScrollPosition = default(Vector2);

		private Vector2 eventEditorScrollPosition = default(Vector2);

		private Vector2 eventSequencerScrollPosition = default(Vector2);

		private Vector2 eventEditorListScrollPosition = default(Vector2);

		private EventManager eventManager;

		private string prevEventName = "";

		private bool eventSelectionChanged;

		public string _eventName = "_UnSet_";

		private EventListener selectedEventListener;

		private EventEditor.EventItem selectedEventItem;

		private EventEditor.EventEntry selectedEventEntry;

		private ReorderableList eventEntryReorderableList;

		private List<EventListener> _eventListeners = new List<EventListener>();

		private int _currentEventEditorIndex;

		private EventEditor _currentEventEditor;

		private List<EventEditor> _eventEditors = new List<EventEditor>();

		private string[] _eventEditorNames = new string[1];

		private bool _sequencerfoldout;

		private void OnEnable()
		{
			EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
			if (!EditorApplication.isPlaying)
			{
				EditorApplication.hierarchyChanged -= PlaymodeCallback;
				EditorApplication.hierarchyChanged += PlaymodeCallback;
			}
			_eventEditorNames[0] = "_UnSet";
			PlaymodeCallback();
			eventSelectionChanged = true;
		}

		private void OnFocus()
		{
			_eventEditorNames[0] = "_UnSet";
			PlaymodeCallback();
			eventSelectionChanged = true;
		}

		private void CreateEventEntryReordableList()
		{
			if (selectedEventEntry != null)
			{
				eventEntryReorderableList = new ReorderableList(selectedEventEntry._eventList, typeof(EventEditor.EventItem), draggable: true, displayHeader: true, displayAddButton: false, displayRemoveButton: false);
				eventEntryReorderableList.drawElementCallback = EventEntryDrawer2;
				eventEntryReorderableList.onSelectCallback = EventEntryDrawerSelection;
				eventEntryReorderableList.drawHeaderCallback = delegate(Rect rect)
				{
					EditorGUI.LabelField(rect, "Event Actions");
				};
			}
		}

		public void PlaymodeCallback()
		{
			eventManager = EventManager.Instance;
			_eventEditors.Clear();
			List<string> list = new List<string>();
			if (eventManager != null)
			{
				_eventEditors.Add(eventManager._eventEditor);
				list.Add("Global (EventManager)");
			}
			GroupComponent[] array = UnityEngine.Object.FindObjectsByType(typeof(GroupComponent)) as GroupComponent[];
			GroupComponent[] array2 = array;
			foreach (GroupComponent groupComponent in array2)
			{
				if (!groupComponent.IsFabricHierarchyPresent() && !groupComponent.IsInstance)
				{
					_eventEditors.Add(groupComponent._eventEditor);
					list.Add(groupComponent.name);
				}
			}
			if (_eventEditors.Count > 0)
			{
				_currentEventEditor = _eventEditors[0];
				_eventEditorNames = list.ToArray();
				CreateEventEntryReordableList();
				eventSelectionChanged = true;
				Repaint();
				if (_currentEventEditorIndex >= _eventEditors.Count)
				{
					_currentEventEditorIndex = _eventEditors.Count - 1;
				}
			}
		}

		private void OnPlayModeStateChanged(PlayModeStateChange state)
		{
			PlaymodeCallback();
		}

		private string[] BuildEventActions(string label = "ANY")
		{
			string[] names = Enum.GetNames(typeof(EventAction));
			List<string> list = new List<string>();
			list.Add(label);
			for (int i = 0; i < names.Length; i++)
			{
				list.Add(names[i]);
			}
			return list.ToArray();
		}

		private void EventEntryDrawerSelection(ReorderableList list)
		{
			EventEditor.EventEntry entryByEventName = _currentEventEditor.GetEntryByEventName(_eventName);
			if (entryByEventName != null)
			{
				selectedEventItem = entryByEventName._eventList[list.index];
			}
			else
			{
				selectedEventItem = null;
			}
		}

		private void EventEntryDrawer2(Rect position, int index, bool isActive, bool isFocused)
		{
			string[] names = Enum.GetNames(typeof(EventAction));
			EventEditor.EventEntry entryByEventName = _currentEventEditor.GetEntryByEventName(_eventName);
			if (entryByEventName != null && index < entryByEventName._eventList.Count)
			{
				EventEditor.EventItem eventItem = entryByEventName._eventList[index];
				bool flag = ((eventItem._event.EventAction == EventAction.SetGlobalParameter || eventItem._event.EventAction == EventAction.SetGlobalSwitch || eventItem._event.EventAction == EventAction.AddPreset || eventItem._event.EventAction == EventAction.RemovePreset || eventItem._event.EventAction == EventAction.SwitchPreset || eventItem._event.EventAction == EventAction.ResetDynamicMixer || eventItem._event.EventAction == EventAction.TransitionToSnapshot || eventItem._event.EventAction == EventAction.LoadAudioMixer || eventItem._event.EventAction == EventAction.UnloadAudioMixer || eventItem._event.EventAction == EventAction.MicStart || eventItem._event.EventAction == EventAction.MicStop) ? true : false);
				GUI.enabled = ((!eventItem._ignoreEventAction || flag) ? true : false);
				float num = 0f;
				float num2 = 300f;
				EditorGUIUtility.labelWidth = 80f;
				eventItem._event.EventAction = (EventAction)EditorGUI.Popup(new Rect(position.x, position.y, num2, EditorGUIUtility.singleLineHeight), "EventAction:", (int)eventItem._event.EventAction, names);
				EditorGUIUtility.labelWidth = 0f;
				num += num2;
				GUI.enabled = !flag;
				num2 = 120f;
				EditorGUIUtility.labelWidth = 55f;
				bool ignoreEventAction = EditorGUI.Toggle(new Rect(position.x + num, position.y, num2, EditorGUIUtility.singleLineHeight), " Ignore", eventItem._ignoreEventAction);
				eventItem._ignoreEventAction = ignoreEventAction;
				EditorGUIUtility.labelWidth = 0f;
				num += num2 + 10f;
				num2 = 300f;
				EditorGUIUtility.labelWidth = 80f;
				eventItem._component = (Component)EditorGUI.ObjectField(new Rect(position.x + num, position.y, num2, EditorGUIUtility.singleLineHeight), "Component:", eventItem._component, typeof(Component), true);
				EditorGUIUtility.labelWidth = 0f;
				GUI.enabled = true;
			}
		}

		public void DrawEventEditor()
		{
			eventEditorScrollPosition = GUILayout.BeginScrollView(eventEditorScrollPosition);
			GUILayout.BeginHorizontal();
			eventEditorListScrollPosition = GUILayout.BeginScrollView(eventEditorListScrollPosition, "box", GUILayout.MinWidth(200f), GUILayout.ExpandHeight(expand: true));
			GUIStyle gUIStyle = new GUIStyle("box");
			if (EditorGUIUtility.isProSkin)
			{
				gUIStyle.normal.textColor = Color.white;
			}
			else
			{
				gUIStyle.normal.textColor = Color.black;
			}
			GUILayout.Label("Event Groups", gUIStyle, GUILayout.MinWidth(190f));
			GUIStyle boldLabel = EditorStyles.boldLabel;
			boldLabel.fixedHeight = 25f;
			boldLabel.alignment = TextAnchor.MiddleLeft;
			int num = GUILayout.SelectionGrid(_currentEventEditorIndex, _eventEditorNames, 1, boldLabel);
			if (_currentEventEditorIndex != num)
			{
				_eventName = "_UnSet_";
			}
			_currentEventEditorIndex = num;
			if (_eventEditors.Count > 0 && _currentEventEditorIndex < _eventEditors.Count)
			{
				_currentEventEditor = _eventEditors[_currentEventEditorIndex];
			}
			else
			{
				_currentEventEditorIndex = 0;
				_currentEventEditor = null;
			}
			GUILayout.EndScrollView();
			GUILayout.BeginVertical();
			DrawEventListMenu("288080-eventeditor");
			GUILayout.Space(5f);
			GUIStyle gUIStyle2 = new GUIStyle(GUI.skin.label);
			gUIStyle2.alignment = TextAnchor.MiddleCenter;
			EventEditor.EventEntry eventEntry = ((_currentEventEditor != null) ? _currentEventEditor.GetEntryByEventName(_eventName) : null);
			if (selectedEventEntry != eventEntry)
			{
				selectedEventEntry = eventEntry;
				selectedEventItem = null;
				CreateEventEntryReordableList();
			}
			GUILayout.BeginHorizontal();
			GUILayout.Label("", GUILayout.ExpandWidth(expand: true));
			GUI.enabled = ((_eventName != "_UnSet_") ? true : false);
			int num2 = 32;
			GUIStyle gUIStyle3 = new GUIStyle(GUI.skin.button);
			if (EditorGUIUtility.isProSkin)
			{
				gUIStyle3.normal.background = GUIHelpers.LoadImage("icons/EventEditorAdd");
			}
			else
			{
				gUIStyle3.normal.background = GUIHelpers.LoadImage("icons/EventEditorAddFree");
			}
			gUIStyle3.onActive.background = gUIStyle3.normal.background;
			gUIStyle3.fixedWidth = num2;
			gUIStyle3.fixedHeight = num2;
			if (GUILayout.Button("", gUIStyle3))
			{
				if (selectedEventEntry == null)
				{
					selectedEventEntry = new EventEditor.EventEntry();
					selectedEventEntry._eventName = _eventName;
					_currentEventEditor.AddEventEntry(selectedEventEntry);
					CreateEventEntryReordableList();
				}
				selectedEventItem = new EventEditor.EventItem();
				selectedEventEntry._eventList.Add(selectedEventItem);
				eventEntryReorderableList.index = selectedEventEntry._eventList.Count - 1;
			}
			GUIStyle gUIStyle4 = new GUIStyle(GUI.skin.button);
			if (EditorGUIUtility.isProSkin)
			{
				gUIStyle4.normal.background = GUIHelpers.LoadImage("icons/EventEditorDelete");
			}
			else
			{
				gUIStyle4.normal.background = GUIHelpers.LoadImage("icons/EventEditorDeleteFree");
			}
			gUIStyle4.onActive.background = gUIStyle4.normal.background;
			gUIStyle4.fixedWidth = num2;
			gUIStyle4.fixedHeight = num2;
			if (GUILayout.Button("", gUIStyle4) && selectedEventEntry != null)
			{
				selectedEventEntry._eventList.Remove(selectedEventItem);
				eventEntryReorderableList.index = 0;
				if (_currentEventEditor._eventList.Count == 0)
				{
					_currentEventEditor.RemoveEventEntry(selectedEventEntry);
				}
			}
			GUI.enabled = true;
			GUILayout.EndHorizontal();
			eventEditorActionListScrollPosition = GUILayout.BeginScrollView(eventEditorActionListScrollPosition, "Box", GUILayout.MinHeight(200f));
			if (selectedEventEntry != null && eventEntryReorderableList != null)
			{
				eventEntryReorderableList.DoLayoutList();
			}
			GUILayout.EndScrollView();
			GUILayout.Space(3f);
			GUILayout.BeginHorizontal("box", GUILayout.ExpandHeight(expand: true));
			GUILayout.BeginVertical(GUILayout.MaxWidth(100f));
			if (selectedEventEntry != null)
			{
				EditorGUIUtility.labelWidth = 70f;
				selectedEventEntry._delay = EditorGUILayout.FloatField("Delay:", selectedEventEntry._delay);
				selectedEventEntry._probability = EditorGUILayout.IntField("Probability:", selectedEventEntry._probability);
				GUILayout.BeginHorizontal();
				selectedEventEntry._postCountMax = EditorGUILayout.IntField("Post Count:", selectedEventEntry._postCountMax);
				EditorGUILayout.LabelField("[ " + selectedEventEntry._postCount + " ]");
				GUILayout.EndHorizontal();
				EditorGUIUtility.labelWidth = 130f;
				selectedEventEntry._ignoreIncomingGameObject = EditorGUILayout.Toggle("Ignore GameObject:", selectedEventEntry._ignoreIncomingGameObject);
				EditorGUIUtility.labelWidth = 0f;
			}
			GUILayout.EndVertical();
			GUILayout.BeginVertical("box", GUILayout.ExpandHeight(expand: true), GUILayout.ExpandWidth(expand: true));
			if (selectedEventItem != null)
			{
				EventItemParamaterUI(selectedEventItem);
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.EndScrollView();
		}

		public void DrawEventListenerOverride()
		{
			GUILayout.Space(5f);
			GUILayout.BeginVertical(GUILayout.MinHeight(250f));
			GUIStyle gUIStyle = new GUIStyle(GUI.skin.label);
			gUIStyle.alignment = TextAnchor.MiddleCenter;
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, "Box");
			if (eventSelectionChanged)
			{
				_eventListeners.Clear();
				selectedEventListener = null;
				EventListener[] array = UnityEngine.Object.FindObjectsByType(typeof(EventListener)) as EventListener[];
				EventListener[] array2 = array;
				foreach (EventListener eventListener in array2)
				{
					Component component = eventListener.GetComponent<Component>();
					if (component != null && !component.IsInstance && eventListener._eventName == _eventName)
					{
						_eventListeners.Add(eventListener);
					}
				}
				_eventListeners.Sort((EventListener a, EventListener b) => a.gameObject.name.CompareTo(b.gameObject.name));
				eventSelectionChanged = false;
				if (array.Length > 0)
				{
					selectedEventListener = array[0];
				}
			}
			for (int num = 0; num < _eventListeners.Count; num++)
			{
				EventListener eventListener2 = _eventListeners[num];
				Color backgroundColor = GUI.backgroundColor;
				GUI.backgroundColor = ((selectedEventListener == eventListener2) ? Color.green : backgroundColor);
				EditorGUILayout.BeginHorizontal("box");
				GUI.backgroundColor = backgroundColor;
				GUILayout.Label(num.ToString(), gUIStyle, GUILayout.MinWidth(40f));
				GUILayout.BeginHorizontal();
				GUILayout.Label("For Event Listener in");
				EditorGUILayout.ObjectField(eventListener2.gameObject, typeof(GameObject), true, GUILayout.MinWidth(150f));
				GUILayout.EndHorizontal();
				GUI.enabled = false;
				string[] displayedOptions = BuildEventActions();
				int selectedIndex = 0;
				if (eventListener2._overrideEventTriggerAction && eventListener2._overrideParameters != null)
				{
					GUI.enabled = true;
					if (eventListener2._overrideParameters._overrideIncomingEventAction)
					{
						selectedIndex = (int)(eventListener2._overrideParameters._incomingEventAction + 1);
					}
				}
				GUILayout.BeginHorizontal();
				GUILayout.Label("On");
				selectedIndex = EditorGUILayout.Popup("", selectedIndex, displayedOptions);
				GUILayout.Label("Event Action");
				GUILayout.EndHorizontal();
				if (eventListener2._overrideEventTriggerAction && eventListener2._overrideParameters != null)
				{
					if (selectedIndex >= 1)
					{
						eventListener2._overrideParameters._overrideIncomingEventAction = true;
						eventListener2._overrideParameters._incomingEventAction = (EventAction)(selectedIndex - 1);
					}
					else
					{
						eventListener2._overrideParameters._overrideIncomingEventAction = false;
						eventListener2._overrideParameters._incomingEventAction = EventAction.PlaySound;
					}
				}
				GUI.enabled = true;
				string[] displayedOptions2 = BuildEventActions("NONE (Use posted event action)");
				int selectedIndex2 = 0;
				if (eventListener2._overrideEventTriggerAction)
				{
					if (eventListener2._overrideParameters == null)
					{
						eventListener2._overrideParameters = new OverrideParameters();
					}
					selectedIndex2 = (int)(eventListener2._overrideParameters._overrideEventAction + 1);
				}
				GUILayout.BeginHorizontal();
				GUILayout.Label("override with");
				selectedIndex2 = EditorGUILayout.Popup("", selectedIndex2, displayedOptions2);
				GUILayout.Label("Event Action");
				GUILayout.EndHorizontal();
				if (selectedIndex2 >= 1)
				{
					if (eventListener2._overrideParameters == null)
					{
						eventListener2._overrideParameters = new OverrideParameters();
					}
					eventListener2._overrideEventTriggerAction = true;
					eventListener2._overrideParameters._overrideEventAction = (EventAction)(selectedIndex2 - 1);
				}
				else
				{
					eventListener2._overrideEventTriggerAction = false;
					eventListener2._overrideParameters = null;
				}
				if (GUILayout.Button("Del", GUILayout.MinWidth(40f)))
				{
					_eventListeners.Remove(eventListener2);
					UnityEngine.Object.DestroyImmediate(eventListener2);
				}
				EditorGUILayout.EndHorizontal();
				if (GUILayoutUtility.GetLastRect().Contains(UnityEngine.Event.current.mousePosition) && UnityEngine.Event.current != null && UnityEngine.Event.current.type == EventType.MouseDown && UnityEngine.Event.current.button == 0)
				{
					selectedEventListener = eventListener2;
				}
			}
			GUILayout.Space(10f);
			GUILayout.EndScrollView();
			GUILayout.BeginHorizontal();
			Rect rect = GUILayoutUtility.GetRect(100f, 40f, GUILayout.ExpandWidth(expand: true));
			Color backgroundColor2 = GUI.backgroundColor;
			GUI.backgroundColor = Color.red;
			GUI.Box(rect, "Drag and Drop Component to add Event Listener");
			GUI.backgroundColor = backgroundColor2;
			DragAndDropComponent(rect);
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
			GUILayout.BeginVertical("Box");
			if (selectedEventListener != null && selectedEventListener._overrideParameters != null)
			{
				EventListenerEditor.OverrideEventActionProperties(selectedEventListener);
			}
			else
			{
				GUILayout.Label("Select an event listener with event action override enabled");
			}
			GUILayout.EndVertical();
			GUILayout.EndVertical();
		}

		private void DragAndDropComponent(Rect drop_area)
		{
			UnityEngine.Event current = UnityEngine.Event.current;
			switch (current.type)
			{
			case EventType.DragUpdated:
			case EventType.DragPerform:
			{
				if (!drop_area.Contains(current.mousePosition))
				{
					break;
				}
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				if (current.type != EventType.DragPerform)
				{
					break;
				}
				DragAndDrop.AcceptDrag();
				UnityEngine.Object[] objectReferences = DragAndDrop.objectReferences;
				UnityEngine.Object[] array = objectReferences;
				foreach (UnityEngine.Object obj in array)
				{
					GameObject gameObject = obj as GameObject;
					Component component = gameObject.GetComponent<Component>();
					if (gameObject != null && component != null)
					{
						EventListener eventListener = gameObject.AddComponent<EventListener>();
						eventListener._eventName = _eventName;
						eventSelectionChanged = true;
					}
				}
				break;
			}
			}
		}

		public void DrawEventListMenu(string documentLink)
		{
			if (_currentEventEditor == null)
			{
				return;
			}
			GUILayout.BeginHorizontal("Box");
			List<string> list = new List<string>();
			list.Add("_UnSet_");
			if (EventManager.Instance != null && _currentEventEditor == EventManager.Instance._eventEditor)
			{
				if (eventManager._eventList != null && eventManager._eventList.Count > 0 && eventManager._eventList[0] == "UnSet")
				{
					eventManager._eventList[0] = "_UnSet_";
				}
				list = new List<string>(EventManager.Instance._eventList.ToArray());
			}
			else
			{
				if (!_currentEventEditor._eventNames.Contains("_UnSet_"))
				{
					_currentEventEditor._eventNames.Insert(0, "_UnSet_");
				}
				list = new List<string>(_currentEventEditor._eventNames.ToArray());
			}
			int num = 0;
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] == _eventName)
				{
					num = i;
					break;
				}
			}
			string[] array = list.ToArray();
			EditorGUIUtility.labelWidth = 80f;
			int num2 = EditorGUILayout.Popup("Event Name:", num, array, GUILayout.MinWidth(300f));
			EditorGUIUtility.labelWidth = 0f;
			if (num2 != num)
			{
				_eventName = array[num2];
			}
			if (GUILayout.Button("<<", GUILayout.MaxWidth(40f)))
			{
				GenericMenu genericMenu = new GenericMenu();
				EventPropertiesData eventPropertiesData = new EventPropertiesData();
				eventPropertiesData.x = UnityEngine.Event.current.mousePosition.x;
				eventPropertiesData.y = UnityEngine.Event.current.mousePosition.y;
				eventPropertiesData.eventEditorBase = this;
				genericMenu.AddItem(new GUIContent("Add Event"), on: false, AddEvent, eventPropertiesData);
				if (_eventName != "_UnSet_")
				{
					genericMenu.AddItem(new GUIContent("Rename Event"), on: false, RenameEvent, eventPropertiesData);
					genericMenu.AddItem(new GUIContent("Delete Event"), on: false, DeleteEvent);
				}
				genericMenu.AddSeparator("");
				genericMenu.AddItem(new GUIContent("Load Events"), on: false, LoadEvents);
				genericMenu.AddItem(new GUIContent("Save Events"), on: false, SaveEvents);
				genericMenu.AddItem(new GUIContent("Remove Events"), on: false, RemoveEvents);
				genericMenu.ShowAsContext();
			}
			MenuBar.OnGUI(documentLink);
			GUILayout.EndHorizontal();
			if (prevEventName != _eventName)
			{
				eventSelectionChanged = true;
				prevEventName = _eventName;
			}
		}

		public static void AddEvent(object userData)
		{
			EventPropertiesData eventPropertiesData = (EventPropertiesData)userData;
			EditEventWindow.Create(eventPropertiesData.eventEditorBase.OnCreateEvent, eventPropertiesData.x, eventPropertiesData.y);
		}

		private void DeleteEvent()
		{
			if (_eventName != "_UnSet_")
			{
				if (_currentEventEditor == EventManager.Instance._eventEditor)
				{
					EventManager.Instance._eventList.Remove(_eventName);
				}
				_currentEventEditor.RemoveEventName(_eventName);
				_eventName = "_UnSet_";
			}
		}

		private static void RenameEvent(object userData)
		{
			EventPropertiesData eventPropertiesData = (EventPropertiesData)userData;
			EditEventWindow.Rename(eventPropertiesData.eventEditorBase._eventName, eventPropertiesData.eventEditorBase.OnEventRename, eventPropertiesData.x, eventPropertiesData.y);
		}

		public void AddEventNames(string[] events)
		{
			for (int i = 0; i < events.Length; i++)
			{
				if (_currentEventEditor == EventManager.Instance._eventEditor)
				{
					if (!EventManager.Instance._eventList.Contains(events[i]))
					{
						EventManager.Instance._eventList.Add(events[i]);
					}
				}
				else if (!_currentEventEditor._eventNames.Contains(events[i]))
				{
					_currentEventEditor._eventNames.Add(events[i]);
				}
			}
		}

		public void RemoveEventNames(string[] events)
		{
			for (int i = 0; i < events.Length; i++)
			{
				if (_currentEventEditor == EventManager.Instance._eventEditor)
				{
					EventManager.Instance._eventList.Remove(events[i]);
				}
				else
				{
					_currentEventEditor._eventNames.Remove(events[i]);
				}
			}
		}

		public void AddEventNamesFromFile(string filename)
		{
			StreamReader streamReader = new StreamReader(filename, Encoding.Default);
			if (streamReader == null)
			{
				return;
			}
			List<string> list = new List<string>();
			string text;
			do
			{
				text = streamReader.ReadLine();
				if (text != null)
				{
					list.Add(text);
				}
			}
			while (text != null);
			streamReader.Close();
			AddEventNames(list.ToArray());
		}

		public void ExportEventNamesToFile(string filename)
		{
			StreamWriter streamWriter = new StreamWriter(filename);
			if (streamWriter == null)
			{
				return;
			}
			if (_currentEventEditor == EventManager.Instance._eventEditor)
			{
				for (int i = 1; i < EventManager.Instance._eventList.Count; i++)
				{
					streamWriter.WriteLine(EventManager.Instance._eventList[i]);
				}
			}
			else
			{
				for (int j = 1; j < _currentEventEditor._eventNames.Count; j++)
				{
					streamWriter.WriteLine(_currentEventEditor._eventNames[j]);
				}
			}
			streamWriter.Close();
		}

		public void RemoveEventNamesFromFile(string filename)
		{
			StreamReader streamReader = new StreamReader(filename, Encoding.Default);
			if (streamReader == null)
			{
				return;
			}
			List<string> list = new List<string>();
			string text;
			do
			{
				text = streamReader.ReadLine();
				if (text != null)
				{
					list.Add(text);
				}
			}
			while (text != null);
			streamReader.Close();
			RemoveEventNames(list.ToArray());
		}

		private void RemoveEvents()
		{
			string text = EditorUtility.OpenFilePanel("Load Event Names from txt file", "", "txt");
			if (text != null && text.Length > 0)
			{
				string[] files = Directory.GetFiles(text);
				for (int i = 0; i < files.Length; i++)
				{
					RemoveEventNamesFromFile(files[i]);
				}
			}
		}

		private void SaveEvents()
		{
			string text = EditorUtility.SaveFilePanel("Save Event List to a txt file", "", "", "txt");
			if (text != null && text.Length != 0)
			{
				ExportEventNamesToFile(text);
			}
		}

		private void LoadEvents()
		{
			string text = EditorUtility.OpenFilePanel("Load Event Names from txt file", "", "txt");
			if (text != null && text.Length > 0)
			{
				string[] files = Directory.GetFiles(text);
				for (int i = 0; i < files.Length; i++)
				{
					AddEventNamesFromFile(files[i]);
				}
			}
		}

		private void ImportEvents()
		{
		}

		private void ExportEvents()
		{
		}

		private bool IsEventNamePresent(string eventName)
		{
			bool flag = false;
			if (EventManager.Instance != null && EventManager.Instance._eventList.Contains(eventName))
			{
				flag = true;
			}
			for (int i = 0; i < _eventEditors.Count; i++)
			{
				if (_eventEditors[i]._eventNames.Contains(eventName))
				{
					flag = true;
				}
			}
			if (flag)
			{
				EditorUtility.DisplayDialog("Fabric Error", "Event [" + eventName + "] already present, please specify a different event name", "Ok");
			}
			return flag;
		}

		private void OnCreateEvent(string eventName)
		{
			if (!IsEventNamePresent(eventName))
			{
				if (EventManager.Instance != null && _currentEventEditor == EventManager.Instance._eventEditor && !EventManager.Instance._eventList.Contains(eventName))
				{
					EventManager.Instance._eventList.Add(eventName);
				}
				if (!_currentEventEditor._eventNames.Contains(eventName))
				{
					_currentEventEditor._eventNames.Add(eventName);
				}
				Repaint();
			}
		}

		private void OnEventRename(string originalEventName, string eventName)
		{
			if (_currentEventEditor == EventManager.Instance._eventEditor)
			{
				for (int i = 0; i < EventManager.Instance._eventList.Count; i++)
				{
					if (EventManager.Instance._eventList[i] == originalEventName)
					{
						EventManager.Instance._eventList[i] = eventName;
						EventManager.Instance._eventEditor.RenameEventName(originalEventName, eventName);
					}
				}
			}
			_currentEventEditor.RenameEventName(originalEventName, eventName);
			_eventName = eventName;
			Repaint();
		}

		public void DrawEventSequencer()
		{
			if (_eventName == "_UnSet_")
			{
				return;
			}
			GUILayout.BeginVertical("box");
			GUILayout.Space(15f);
			GUIStyle gUIStyle = new GUIStyle(GUI.skin.label);
			gUIStyle.alignment = TextAnchor.MiddleCenter;
			eventSequencerScrollPosition = GUILayout.BeginScrollView(eventSequencerScrollPosition, GUILayout.MinHeight(200f));
			EventSequencer.EventSequencerEntry eventSequencerEntry = EventManager.Instance._eventSequencer._sequenceEntries.FindItem(_eventName);
			if (eventSequencerEntry != null)
			{
				GUILayout.BeginHorizontal("Box");
				eventSequencerEntry._loop = EditorGUILayout.Toggle("Loop: ", eventSequencerEntry._loop);
				if (GUILayout.Button("Clear"))
				{
					eventSequencerEntry = null;
					EventManager.Instance._eventSequencer._sequenceEntries.Clear();
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(10f);
				GUILayout.BeginHorizontal("Box");
				GUILayout.Label("For this event on Notification ->", GUILayout.MaxWidth(200f));
				eventSequencerEntry.notificationType = (EventNotificationType)(object)EditorGUILayout.EnumPopup("", eventSequencerEntry.notificationType, GUILayout.MinWidth(150f));
				GUILayout.Label(" -> post next event");
				GUILayout.EndHorizontal();
			}
			else if (GUILayout.Button("Create"))
			{
				EventManager.Instance._eventSequencer._sequenceEntries.Add(_eventName, new EventSequencer.EventSequencerEntry());
			}
			GUILayout.Space(10f);
			if (eventSequencerEntry != null)
			{
				for (int i = 0; i < eventSequencerEntry.events.Count; i++)
				{
					Color backgroundColor = GUI.backgroundColor;
					GUI.backgroundColor = ((eventSequencerEntry.CurrentIndex() == i) ? Color.green : backgroundColor);
					GUILayout.BeginHorizontal("Box");
					GUILayout.FlexibleSpace();
					GUILayout.Label("Post");
					eventSequencerEntry.events[i].eventName = EventManagerEditor.DropDownEventNames(eventSequencerEntry.events[i].eventName);
					GUILayout.Label("-> and on Notification -> ");
					eventSequencerEntry.events[i].notificationType = (EventNotificationType)(object)EditorGUILayout.EnumPopup("", eventSequencerEntry.events[i].notificationType, GUILayout.MinWidth(320f));
					GUILayout.Label(" -> post next event");
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Del"))
					{
						eventSequencerEntry.events.RemoveAt(i);
					}
					GUILayout.EndHorizontal();
					GUI.backgroundColor = backgroundColor;
				}
				GUILayout.Space(10f);
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			if (GUILayout.Button("Add New Entry"))
			{
				EventSequencer.EventSequencerEntry.EventEntry eventEntry = new EventSequencer.EventSequencerEntry.EventEntry();
				eventEntry.eventName = "_UnSet_";
				eventSequencerEntry.events.Add(eventEntry);
			}
		}

		public static void EventItemParamaterUI(EventEditor.EventItem eventItem)
		{
			if (eventItem._event.EventAction != EventAction.AddPreset && eventItem._event.EventAction != EventAction.RemovePreset && eventItem._event.EventAction != EventAction.SwitchPreset && eventItem._event.EventAction != EventAction.ResetDynamicMixer && eventItem._event.EventAction != EventAction.SetGlobalParameter && eventItem._event.EventAction != EventAction.SetGlobalSwitch && eventItem._event.EventAction != EventAction.TransitionToSnapshot && eventItem._event.EventAction != EventAction.LoadAudioMixer)
			{
				_ = eventItem._event.EventAction;
				_ = 42;
			}
			if (eventItem._event.EventAction == EventAction.SetPitch || eventItem._event.EventAction == EventAction.SetPitchProperty)
			{
				eventItem._eventParameter = EditorGUILayout.Slider(new GUIContent("Pitch:"), eventItem._eventParameter, -5f, 5f);
			}
			else if (eventItem._event.EventAction == EventAction.SetVolume || eventItem._event.EventAction == EventAction.SetVolumeProperty)
			{
				eventItem._eventParameter = EditorGUILayout.Slider(new GUIContent("Volume:"), eventItem._eventParameter, 0f, 1f);
			}
			else if (eventItem._event.EventAction == EventAction.SetPan)
			{
				eventItem._eventParameter = EditorGUILayout.Slider(new GUIContent("Pan:"), eventItem._eventParameter, -1f, 1f);
			}
			else if (eventItem._event.EventAction == EventAction.SetTime)
			{
				eventItem._eventParameter = EditorGUILayout.Slider(new GUIContent("Time:"), eventItem._eventParameter, -1000f, 1000f);
			}
			else if (eventItem._event.EventAction == EventAction.SetMarker)
			{
				eventItem._eventValue = EditorGUILayout.TextField(new GUIContent("Label:"), eventItem._eventValue);
			}
			else if (eventItem._event.EventAction == EventAction.SetAudioClipReference)
			{
				eventItem._eventValue = EditorGUILayout.TextField(new GUIContent("AudioClip Name:"), eventItem._eventValue);
			}
			else if (eventItem._event.EventAction == EventAction.SetParameter)
			{
				if (eventItem._parameterData == null)
				{
					eventItem._parameterData = new ParameterData();
				}
				GUILayout.BeginHorizontal();
				eventItem._eventParameterName = EditorGUILayout.TextField(new GUIContent("Parameter Name:"), eventItem._eventParameterName);
				GUILayout.EndHorizontal();
				eventItem._eventParameter = EditorGUILayout.Slider(new GUIContent("Parameter:"), eventItem._eventParameter, eventItem._min, eventItem._max);
				eventItem._max = EditorGUILayout.FloatField(new GUIContent("Max:"), eventItem._max);
				eventItem._min = EditorGUILayout.FloatField(new GUIContent("Min:"), eventItem._min);
			}
			else if (eventItem._event.EventAction == EventAction.SetSwitch)
			{
				eventItem._eventValue = EditorGUILayout.TextField(new GUIContent("Switch To:"), eventItem._eventValue);
			}
			else if (eventItem._event.EventAction == EventAction.AddPreset || eventItem._event.EventAction == EventAction.RemovePreset)
			{
				eventItem._event._eventName = "DynamicMixer";
				eventItem._eventValue = EditorGUILayout.TextField(new GUIContent("Preset:"), eventItem._eventValue);
			}
			else if (eventItem._event.EventAction == EventAction.SwitchPreset)
			{
				eventItem._event._eventName = "DynamicMixer";
				if (eventItem._switchPresetData == null)
				{
					eventItem._switchPresetData = new SwitchPresetData();
				}
				GUILayout.BeginHorizontal();
				GUILayout.Label("SwitchTo:");
				eventItem._switchPresetData._targetPreset = GUILayout.TextField(eventItem._switchPresetData._targetPreset, GUILayout.MinWidth(300f), GUILayout.MaxWidth(300f));
				GUILayout.EndHorizontal();
			}
			else if (eventItem._event.EventAction == EventAction.SetGlobalParameter)
			{
				GlobalParameterManager.GlobalParametersFastList globalRTParameters = EventManager.Instance._globalParameterManager._globalRTParameters;
				if (eventItem._globalParameterData == null)
				{
					eventItem._globalParameterData = new GlobalParameterData();
				}
				string[] array = globalRTParameters.Keys();
				int num = globalRTParameters.GetIndexByKey(eventItem._globalParameterData._name);
				GUILayout.BeginHorizontal();
				bool flag = false;
				if (num < 0)
				{
					num = 0;
					flag = true;
				}
				int num2 = EditorGUILayout.Popup("Global Parameter:", num, array, GUILayout.MinWidth(240f));
				if ((num2 != num || flag) && num2 < array.Length)
				{
					eventItem._globalParameterData._name = array[num2];
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GlobalParameter globalParameter = globalRTParameters.FindItemByIndex(num2);
				if (globalParameter != null && eventItem._globalParameterData != null)
				{
					float value = eventItem._globalParameterData._value;
					value = EditorGUILayout.Slider("Value", value, globalParameter._min, globalParameter._max);
					eventItem._globalParameterData._value = value;
				}
				GUILayout.EndHorizontal();
			}
			else if (eventItem._event.EventAction == EventAction.SetGlobalSwitch)
			{
				GlobalParameterManager.GlobalSwitchFastList globalSwitches = EventManager.Instance._globalParameterManager._globalSwitches;
				if (globalSwitches != null && globalSwitches._keys.Count > 0)
				{
					eventItem._event._eventName = "GlobalParameter";
					if (eventItem._globalSwitchParameterData == null)
					{
						eventItem._globalSwitchParameterData = new GlobalSwitchParameterData();
						eventItem._globalSwitchParameterData._switch = "";
					}
					int num3 = -1;
					GUILayout.BeginHorizontal();
					string[] array2 = globalSwitches.Keys();
					int num4 = globalSwitches.GetIndexByKey(eventItem._globalSwitchParameterData._name);
					bool flag2 = false;
					if (num4 < 0)
					{
						num4 = 0;
						flag2 = true;
					}
					num3 = EditorGUILayout.Popup("Global Switch:", num4, array2, GUILayout.MinWidth(240f));
					if (num3 != num4 || flag2)
					{
						eventItem._globalSwitchParameterData._name = array2[num3];
					}
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
					GlobalSwitch globalSwitch = globalSwitches.FindItemByIndex(num3);
					List<string> list = new List<string>();
					for (int i = 0; i < globalSwitch._switches.Count; i++)
					{
						list.Add(globalSwitch._switches[i]._name);
					}
					int num5 = list.FindIndex((string x) => eventItem._globalSwitchParameterData._switch.Contains(x));
					bool flag3 = false;
					if (num5 < 0)
					{
						num5 = 0;
						flag3 = true;
					}
					int num6 = EditorGUILayout.Popup("Switch:", num5, list.ToArray(), GUILayout.MinWidth(240f));
					if ((num6 != num5 || flag3) && num6 >= 0)
					{
						eventItem._globalSwitchParameterData._switch = list[num6];
					}
					GUILayout.EndHorizontal();
				}
			}
			else if (eventItem._event.EventAction == EventAction.SetRegion || eventItem._event.EventAction == EventAction.QueueRegion)
			{
				eventItem._eventParameterName = EditorGUILayout.TextField(new GUIContent("Region Name:"), eventItem._eventParameterName);
			}
			else if (eventItem._event.EventAction == EventAction.LoadAudioMixer)
			{
				eventItem._event._eventName = "AudioMixer";
				eventItem._eventParameterName = EditorGUILayout.TextField(new GUIContent("AudioMixer Name:"), eventItem._eventParameterName);
			}
			else if (eventItem._event.EventAction == EventAction.UnloadAudioMixer)
			{
				eventItem._event._eventName = "AudioMixer";
				eventItem._eventParameterName = EditorGUILayout.TextField(new GUIContent("AudioMixer Name:"), eventItem._eventParameterName);
			}
			else if (eventItem._event.EventAction == EventAction.TransitionToSnapshot)
			{
				eventItem._event._eventName = "AudioMixer";
				if (eventItem._transitionToSnapshotData == null)
				{
					eventItem._transitionToSnapshotData = new TransitionToSnapshotData();
				}
				GUILayout.BeginHorizontal();
				GUILayout.Label("Snapshot To Transition:");
				eventItem._transitionToSnapshotData._snapshot = GUILayout.TextField(eventItem._transitionToSnapshotData._snapshot, GUILayout.MinWidth(300f), GUILayout.MaxWidth(300f));
				GUILayout.EndHorizontal();
				eventItem._transitionToSnapshotData._timeToReach = EditorGUILayout.FloatField("Time To Reach: ", eventItem._transitionToSnapshotData._timeToReach);
			}
			else if (eventItem._event.EventAction == EventAction.SetDSPParameter)
			{
				GUILayout.Space(10f);
				eventItem._dspType = (DSPType)(object)EditorGUILayout.EnumPopup("DSP:", eventItem._dspType);
				string[] array3 = null;
				int selectedIndex = 0;
				if (eventItem._dspType == DSPType.External)
				{
					GUILayout.Label("Extrernal Plugins not supported with the event trigger yet");
				}
				else
				{
					array3 = new string[EventTriggerEditor._dspParameterInfo[(int)eventItem._dspType].dspParameters.Length];
					selectedIndex = 0;
					for (int num7 = 0; num7 < EventTriggerEditor._dspParameterInfo[(int)eventItem._dspType].dspParameters.Length; num7++)
					{
						array3[num7] = EventTriggerEditor._dspParameterInfo[(int)eventItem._dspType].dspParameters[num7]._parameter;
						if (array3[num7] == eventItem._eventParameterName)
						{
							selectedIndex = num7;
						}
					}
				}
				if (array3 != null)
				{
					selectedIndex = EditorGUILayout.Popup("Parameter:", selectedIndex, array3);
					eventItem._eventParameterName = array3[selectedIndex];
					eventItem._eventParameter = EditorGUILayout.Slider("Value:", eventItem._eventParameter, EventTriggerEditor._dspParameterInfo[(int)eventItem._dspType].dspParameters[selectedIndex]._min, EventTriggerEditor._dspParameterInfo[(int)eventItem._dspType].dspParameters[selectedIndex]._max);
					eventItem._timeToTarget = EditorGUILayout.FloatField("Time:", eventItem._timeToTarget);
					eventItem._curve = EditorGUILayout.Slider("Curve:", eventItem._curve, 0f, 1f);
				}
				GUILayout.Space(10f);
			}
			else if (eventItem._event.EventAction == EventAction.PlayScheduled || eventItem._event.EventAction == EventAction.StopScheduled)
			{
				eventItem._eventScheduleParameter = EditorGUILayout.DoubleField("Time: ", eventItem._eventScheduleParameter);
			}
		}
	}
	public class EventSequencerEditorWindow : EventEditorBase
	{
		[MenuItem("Window/Fabric/Event Sequencer", false, 1)]
		private static void EventSequencerInit()
		{
			EventSequencerEditorWindow eventSequencerEditorWindow = (EventSequencerEditorWindow)EditorWindow.GetWindow(typeof(EventSequencerEditorWindow));
			eventSequencerEditorWindow.titleContent = new GUIContent("Event Sequencer");
		}

		private void OnGUI()
		{
			DrawEventListMenu("685748");
			if (_eventName != "_UnSet_")
			{
				DrawEventSequencer();
			}
		}
	}
	public class EventListenerEditorWindow : EventEditorBase
	{
		[MenuItem("Window/Fabric/Event Listener Overrides", false, 2)]
		private static void EventListenerOverrides()
		{
			EventListenerEditorWindow eventListenerEditorWindow = (EventListenerEditorWindow)EditorWindow.GetWindow(typeof(EventListenerEditorWindow));
			eventListenerEditorWindow.titleContent = new GUIContent("Event Listener Overrides");
		}

		private void OnGUI()
		{
			DrawEventListMenu("685749");
			if (_eventName != "_UnSet_")
			{
				DrawEventListenerOverride();
			}
		}
	}
	public class EventEditorWindow : EventEditorBase
	{
		[MenuItem("Window/Fabric/Event Editor", false, 0)]
		private static void EventEditorInit()
		{
			EventEditorWindow eventEditorWindow = (EventEditorWindow)EditorWindow.GetWindow(typeof(EventEditorWindow));
			eventEditorWindow.titleContent = new GUIContent("Event Editor");
		}

		private void OnGUI()
		{
			DrawEventEditor();
		}
	}
	[CustomEditor(typeof(EventListComponent))]
	public class EventListComponentEditor : Editor
	{
		private EventListComponent eventListComponent;

		[MenuItem("Fabric/Events/EventListComponent")]
		private static void MenuOption()
		{
			GameObject activeGameObject = Selection.activeGameObject;
			if (!(activeGameObject == null))
			{
				GameObject gameObject = new GameObject("EventListComponent");
				gameObject.transform.parent = activeGameObject.transform;
				gameObject.AddComponent<EventListComponent>();
			}
		}

		private void OnEnable()
		{
			eventListComponent = base.target as EventListComponent;
		}

		public override void OnInspectorGUI()
		{
			MenuBar.OnGUI("288097-eventlistcomponent", box: true);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Load Events From File"))
			{
				string text = EditorUtility.OpenFilePanel("Load Event Names from txt file", "", "txt");
				if (text != null && text.Length > 0)
				{
					string[] files = Directory.GetFiles(text);
					for (int i = 0; i < files.Length; i++)
					{
						eventListComponent.AddEventNamesFromFile(files[i]);
					}
				}
			}
			if (GUILayout.Button("Export Events To File"))
			{
				string text2 = EditorUtility.SaveFilePanel("Export Event List to a txt file", "", "EventList.txt", "txt");
				if (text2 != null && text2.Length != 0)
				{
					eventListComponent.ExportEventNamesToFile(text2);
				}
			}
			if (GUILayout.Button("Unload Events From File"))
			{
				string text3 = EditorUtility.OpenFilePanel("Unload Event Names From txt file", "", "txt");
				if (text3 != null && text3.Length > 0)
				{
					string[] files2 = Directory.GetFiles(text3);
					for (int j = 0; j < files2.Length; j++)
					{
						eventListComponent.RemoveEventNamesFromFile(files2[j]);
					}
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
			DrawDefaultInspector();
			if (GUI.changed)
			{
				EditorUtility.SetDirty(eventListComponent);
			}
		}
	}
	public class CompactMixer : EditorWindow
	{
		private Vector2 scrollPosition = default(Vector2);

		private List<GroupComponent> groupComponents = new List<GroupComponent>();

		public bool _enableDynamicMixer;

		private static GUIStyle activeStyle;

		private static GUIStyle inActiveStyle;

		private float timerRepaint;

		[MenuItem("Window/Fabric/Compact Mixer", false, 11)]
		private static void init()
		{
			CompactMixer compactMixer = (CompactMixer)EditorWindow.GetWindow(typeof(CompactMixer));
			compactMixer.titleContent = new GUIContent("Compact Mixer");
		}

		private void OnEnable()
		{
			EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
			EditorApplication.hierarchyChanged -= PlaymodeCallback;
			EditorApplication.hierarchyChanged += PlaymodeCallback;
			PlaymodeCallback();
			GUIHelpers.RegisterEditorUpdate(base.Repaint);
		}

		private void OnDisable()
		{
			GUIHelpers.UnregisterEditorUpdate(base.Repaint);
		}

		private void OnPlayModeStateChanged(PlayModeStateChange state)
		{
			PlaymodeCallback();
		}

		public void PlaymodeCallback()
		{
			groupComponents.Clear();
			GroupComponent[] array = UnityEngine.Object.FindObjectsByType(typeof(GroupComponent)) as GroupComponent[];
			for (int i = 0; i < array.Length; i++)
			{
				groupComponents.Add(array[i]);
			}
			GroupComponentProxy[] array2 = UnityEngine.Object.FindObjectsByType(typeof(GroupComponentProxy)) as GroupComponentProxy[];
			for (int j = 0; j < array2.Length; j++)
			{
				groupComponents.Add(array2[j]._groupComponent);
			}
			Repaint();
			activeStyle = null;
			inActiveStyle = null;
		}

		private static void InitStyles()
		{
			if (activeStyle == null)
			{
				activeStyle = new GUIStyle(GUI.skin.box);
				activeStyle.normal.background = MakeTex(2, 2, Color.green);
			}
			if (inActiveStyle == null)
			{
				inActiveStyle = new GUIStyle(GUI.skin.box);
				inActiveStyle.normal.background = MakeTex(2, 2, Color.gray);
			}
		}

		private static Texture2D MakeTex(int width, int height, Color col)
		{
			Color[] array = new Color[width * height];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = col;
			}
			Texture2D texture2D = new Texture2D(width, height);
			texture2D.SetPixels(array);
			texture2D.Apply();
			return texture2D;
		}

		private void OnGUI()
		{
			InitStyles();
			if (groupComponents == null)
			{
				return;
			}
			GUILayout.BeginHorizontal("Box");
			_enableDynamicMixer = GUILayout.Toggle(_enableDynamicMixer, "Enable DynamicMixer Control");
			MenuBar.OnGUI("288084-compactmixer");
			GUILayout.EndHorizontal();
			GUILayout.Space(3f);
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			for (int i = 0; i < groupComponents.Count; i++)
			{
				GroupComponent groupComponent = groupComponents[i];
				if (groupComponent == null)
				{
					groupComponents.RemoveAt(i);
					continue;
				}
				GUILayout.BeginHorizontal("Box");
				if (groupComponent.IsComponentActive())
				{
					GUILayout.Box("", activeStyle, GUILayout.MaxWidth(15f));
				}
				else
				{
					GUILayout.Box("", GUILayout.MaxWidth(15f));
				}
				Color backgroundColor = GUI.backgroundColor;
				if (EditorGUIUtility.isProSkin)
				{
					GUI.backgroundColor = Color.blue;
				}
				else
				{
					GUI.backgroundColor = Color.gray;
				}
				if (GUILayout.Button(groupComponent.name, "Box", GUILayout.MaxWidth(200f)))
				{
					Selection.activeGameObject = groupComponent.gameObject;
				}
				GUI.backgroundColor = backgroundColor;
				GUILayout.Label("Volume (dB): ", GUILayout.MaxWidth(80f));
				if (_enableDynamicMixer && Application.isPlaying && GetDynamicMixer.Instance() != null)
				{
					ComponentEditor.DisplayDecibelsVolumeSlider("", groupComponent.Volume * groupComponent.MixerVolume);
				}
				else
				{
					groupComponent.Volume = ComponentEditor.DisplayDecibelsVolumeSlider("", groupComponent.Volume);
				}
				GUILayout.Label("", GUILayout.MaxWidth(10f));
				GUILayout.Label("Pitch: ", GUILayout.MaxWidth(50f));
				if (_enableDynamicMixer && Application.isPlaying && GetDynamicMixer.Instance() != null)
				{
					ComponentEditor.DisplaySemitonesPitchSlider("", groupComponent.Pitch * groupComponent.MixerPitch);
				}
				else
				{
					groupComponent.Pitch = ComponentEditor.DisplaySemitonesPitchSlider("", groupComponent.Pitch);
				}
				groupComponent.Mute = GUILayout.Toggle(groupComponent.Mute, "Mute", "button", GUILayout.MaxWidth(50f));
				bool flag = GUILayout.Toggle(groupComponent.Solo, "Solo", "button", GUILayout.MaxWidth(50f));
				if (flag != groupComponent.Solo && flag)
				{
					for (int j = 0; j < groupComponents.Count; j++)
					{
						if (groupComponents[j] != null && groupComponents[j] != groupComponent)
						{
							groupComponents[j].Mute = true;
						}
					}
					Component[] componentsInChildren = groupComponent.GetComponentsInChildren<Component>();
					for (int k = 0; k < componentsInChildren.Length; k++)
					{
						componentsInChildren[k].Mute = false;
					}
					Mixer.UnmuteParentComponent(groupComponent);
				}
				else if (flag != groupComponent.Solo && !flag)
				{
					for (int l = 0; l < groupComponents.Count; l++)
					{
						if (groupComponents[l] != null && groupComponents[l] != groupComponent)
						{
							groupComponents[l].Mute = false;
						}
					}
				}
				groupComponent.Solo = flag;
				GUIHelpers.CheckGUIHasChanged(groupComponent.gameObject);
				GUILayout.EndHorizontal();
				if (GUILayoutUtility.GetLastRect().Contains(UnityEngine.Event.current.mousePosition) && UnityEngine.Event.current != null && UnityEngine.Event.current.isMouse && UnityEngine.Event.current.button == 0 && UnityEngine.Event.current.type == EventType.MouseDown)
				{
					Selection.activeGameObject = groupComponent.gameObject;
				}
			}
			GUILayout.EndScrollView();
		}

		public void Update()
		{
			if (timerRepaint <= 0f)
			{
				Repaint();
				timerRepaint = 0.16f;
			}
			else
			{
				timerRepaint -= FabricTimer.GetRealtimeDelta();
			}
		}
	}
	[CanEditMultipleObjects]
	[CustomEditor(typeof(MusicComponent))]
	public class MusicComponentEditor : Editor
	{
		private MusicComponent musicComponent;

		private ComponentEditor componentEditor = new ComponentEditor();

		private SerializedProperty musicTimeSettingsindex;

		private SerializedProperty syncToMusicOnFirstPlay;

		private bool _foldout = true;

		private bool _transitionsFoldout = true;

		[MenuItem("Fabric/Components/MusicComponent")]
		private static void About()
		{
			GameObject activeGameObject = Selection.activeGameObject;
			if (!(activeGameObject == null))
			{
				GameObject gameObject = new GameObject("MusicComponent");
				gameObject.transform.parent = activeGameObject.transform;
				gameObject.AddComponent<MusicComponent>();
			}
		}

		private void OnEnable()
		{
			musicComponent = base.target as MusicComponent;
			componentEditor.RegisterSerialisableObject(base.serializedObject);
			musicTimeSettingsindex = base.serializedObject.FindProperty("_musicTimeSettingsindex");
			syncToMusicOnFirstPlay = base.serializedObject.FindProperty("_syncToMusicOnFirstPlay");
			GUIHelpers.RegisterEditorUpdate(base.Repaint);
		}

		private void OnDisable()
		{
			GUIHelpers.UnregisterEditorUpdate(base.Repaint);
		}

		private void OnDestroy()
		{
		}

		public override void OnInspectorGUI()
		{
			componentEditor.InspectorGUI(musicComponent, "288045-musiccomponent");
			base.serializedObject.Update();
			_foldout = EditorGUILayout.Foldout(_foldout, "Music Component Properties");
			if (_foldout)
			{
				GUILayout.BeginVertical("box");
				if (musicComponent._musicTimeResetOnPlay)
				{
					GUI.enabled = false;
				}
				EditorGUILayout.PropertyField(syncToMusicOnFirstPlay, new GUIContent("Sync On Play:"));
				GUI.enabled = true;
				musicComponent._defaultComponent = (Component)EditorGUILayout.ObjectField("Component:", musicComponent._defaultComponent, typeof(Component), true);
				GUILayout.EndVertical();
			}
			GUILayout.Space(5f);
			DrawTransitionList();
			GUIHelpers.CheckGUIHasChanged(base.serializedObject, musicComponent);
		}

		private void DrawTransitionList()
		{
			_transitionsFoldout = EditorGUILayout.Foldout(_transitionsFoldout, "Transitions");
			if (!_transitionsFoldout)
			{
				return;
			}
			Component[] childComponents = musicComponent.GetChildComponents();
			if (childComponents.Length > 0)
			{
				string[] array = new string[childComponents.Length + 1];
				for (int i = 0; i < childComponents.Length; i++)
				{
					array[i] = childComponents[i].name;
				}
				for (int j = 0; j < musicComponent._transitions.Count; j++)
				{
					MusicTransition musicTransition = musicComponent._transitions[j];
					GUILayout.BeginVertical("", "Box");
					GUILayout.BeginHorizontal();
					int indexFromComponent = GetIndexFromComponent(musicTransition._fromComponent._component);
					GUILayout.Label("From: ", GUILayout.MaxWidth(60f));
					musicTransition._fromComponent._component = musicComponent.GetChildComponents()[EditorGUILayout.Popup("", indexFromComponent, array)];
					GUILayout.Label("Sync On:", GUILayout.MaxWidth(60f));
					musicTransition._fromComponent._musicSyncType = (MusicSyncType)(object)EditorGUILayout.EnumPopup("", musicTransition._fromComponent._musicSyncType);
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
					indexFromComponent = ((!(musicTransition._transition._component == null)) ? GetIndexFromComponent(musicTransition._transition._component) : (-1));
					GUILayout.Label("Trans: ", GUILayout.MaxWidth(60f));
					string[] source = array;
					source = MyArray<string>.InsertAt(source, 0, "Ignore");
					indexFromComponent = EditorGUILayout.Popup("", indexFromComponent + 1, source);
					bool flag = false;
					if (indexFromComponent == 0)
					{
						musicTransition._transition._component = null;
						flag = true;
					}
					else
					{
						musicTransition._transition._component = musicComponent.GetChildComponents()[indexFromComponent - 1];
					}
					if (flag)
					{
						GUI.enabled = false;
					}
					GUILayout.Label("Sync On:", GUILayout.MaxWidth(60f));
					musicTransition._transition._musicSyncType = (MusicSyncType)(object)EditorGUILayout.EnumPopup("", musicTransition._transition._musicSyncType);
					if (flag)
					{
						GUI.enabled = true;
					}
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
					indexFromComponent = GetIndexFromComponent(musicTransition._toComponent._component);
					GUILayout.Label("To: ", GUILayout.MaxWidth(60f));
					musicTransition._toComponent._component = musicComponent.GetChildComponents()[EditorGUILayout.Popup("", indexFromComponent, array)];
					GUILayout.Label("Sync On:", GUILayout.MaxWidth(60f));
					musicTransition._toComponent._musicSyncType = (MusicSyncType)(object)EditorGUILayout.EnumPopup("", musicTransition._toComponent._musicSyncType);
					GUILayout.EndHorizontal();
					GUILayout.Space(5f);
					if (GUILayout.Button("Del"))
					{
						musicComponent._transitions.RemoveAt(j);
						break;
					}
					GUILayout.EndVertical();
					GUILayout.Space(5f);
				}
				GUILayout.BeginHorizontal();
				bool flag2 = GUILayout.Button("Add New Transition");
				GUILayout.EndHorizontal();
				if (flag2)
				{
					musicComponent._transitions.Add(new MusicTransition());
				}
			}
			else
			{
				Color backgroundColor = GUI.backgroundColor;
				GUI.color = Color.red;
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.Label("Need to have at least one child component present");
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUI.color = backgroundColor;
			}
		}

		private int GetIndexFromComponent(Component component)
		{
			Component[] childComponents = musicComponent.GetChildComponents();
			for (int i = 0; i < childComponents.Length; i++)
			{
				if (childComponents[i] == component)
				{
					return i;
				}
			}
			return 0;
		}
	}
	[CustomEditor(typeof(SamplePlayerPannerFilter))]
	public class samplePlayerPannerFilterEditor : Editor
	{
		private EditorUndoManager undoManager;

		private SamplePlayerPannerFilter audioPannerFilter;

		private ComponentEditor componentEditor = new ComponentEditor();

		private void OnEnable()
		{
			audioPannerFilter = base.target as SamplePlayerPannerFilter;
			undoManager = new EditorUndoManager(audioPannerFilter, audioPannerFilter.name);
		}

		public override void OnInspectorGUI()
		{
			MenuBar.OnGUI("290595-sampleplayerpanner", box: true);
			GUILayout.BeginVertical("Box");
			undoManager.CheckUndo();
			GUILayout.BeginHorizontal();
			audioPannerFilter._FrontLeftChannel.SetValue(EditorGUILayout.Slider("FrontLeft:", audioPannerFilter._FrontLeftChannel.GetTargetValue(), audioPannerFilter._FrontLeftChannel.Min, audioPannerFilter._FrontLeftChannel.Max));
			GUILayout.Label("(" + audioPannerFilter._FrontLeftChannel.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			audioPannerFilter._FrontRightChannel.SetValue(EditorGUILayout.Slider("FrontRight:", audioPannerFilter._FrontRightChannel.GetTargetValue(), audioPannerFilter._FrontRightChannel.Min, audioPannerFilter._FrontRightChannel.Max));
			GUILayout.Label("(" + audioPannerFilter._FrontRightChannel.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			audioPannerFilter._CenterChannel.SetValue(EditorGUILayout.Slider("Center:", audioPannerFilter._CenterChannel.GetTargetValue(), audioPannerFilter._CenterChannel.Min, audioPannerFilter._CenterChannel.Max));
			GUILayout.Label("(" + audioPannerFilter._CenterChannel.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			audioPannerFilter._RearLeftChannel.SetValue(EditorGUILayout.Slider("RearLeft:", audioPannerFilter._RearLeftChannel.GetTargetValue(), audioPannerFilter._RearLeftChannel.Min, audioPannerFilter._RearLeftChannel.Max));
			GUILayout.Label("(" + audioPannerFilter._RearLeftChannel.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			audioPannerFilter._RearRightChannel.SetValue(EditorGUILayout.Slider("Center:", audioPannerFilter._RearRightChannel.GetTargetValue(), audioPannerFilter._RearRightChannel.Min, audioPannerFilter._RearRightChannel.Max));
			GUILayout.Label("(" + audioPannerFilter._RearRightChannel.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			audioPannerFilter._RearRightChannel.SetValue(EditorGUILayout.Slider("LFE:", audioPannerFilter._LFEChannel.GetTargetValue(), audioPannerFilter._LFEChannel.Min, audioPannerFilter._LFEChannel.Max));
			GUILayout.Label("(" + audioPannerFilter._LFEChannel.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			undoManager.CheckDirty();
			GUILayout.EndVertical();
			GUIHelpers.CheckGUIHasChanged(audioPannerFilter.gameObject);
		}
	}
	public class PersistentData : ScriptableObject
	{
		public bool[] _eventLogFilterEventActions = new bool[Enum.GetNames(typeof(EventAction)).Length];

		public bool _displayEventMonitorSceneInfo = true;

		public bool _displayAudioClipWaveform = true;

		public bool _playmodePersistance;

		public bool _audioAssetImporter;

		public float _maxRandomization3DDistance = 1000f;

		public bool _enableHiearchyIcons = true;

		public string _reaperPath = "C:/Program Files/REAPER (x64)/reaper.exe";

		public Color _eventLogColor = Color.green;

		public float _eventMonitorPersistenceTime = 2f;

		public Queue<EventLogEntry> _audioEventHistory = new Queue<EventLogEntry>();

		public int _logHistorySize = 20;

		public bool _resetEventLogOnPlayModeChange;

		public float _previewerGUIUpdateRate = 0.32f;
	}
	public static class FabricEditorData
	{
		public static PersistentData m_Data;

		public static PersistentData GetData()
		{
			if (m_Data == null)
			{
				string projectDataFolder = GetProjectDataFolder();
				m_Data = (PersistentData)AssetDatabase.LoadAssetAtPath(projectDataFolder + "PersistentData.asset", typeof(PersistentData));
				if (m_Data == null)
				{
					m_Data = ScriptableObject.CreateInstance<PersistentData>();
					string path = AssetDatabase.GenerateUniqueAssetPath(projectDataFolder + "PersistentData.asset");
					AssetDatabase.CreateAsset(m_Data, path);
				}
			}
			return m_Data;
		}

		public static void ApplyChanges()
		{
			if (GUI.changed)
			{
				EditorUtility.SetDirty(GetData());
			}
		}

		private static string GetProjectDataFolder()
		{
			string text = FabricManagerEditor.GetFabricEditorPath() + "/ProjectData/";
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}
	public class RTPMarkersView
	{
		private class DraggingData
		{
			public string key;

			public float delta;
		}

		private RTPMarkers _rtpMarkers;

		private RTPParameter _parameter;

		private string text;

		private float _width;

		private Vector2 scrollView = default(Vector2);

		private GUIFoldoutPanel markersPanel = new GUIFoldoutPanel("Markers", zStatus: false);

		private DraggingData dragging;

		public RTPMarkersView(RTPParameter parameter)
		{
			_rtpMarkers = parameter._markers;
			_parameter = parameter;
		}

		public RTPMarkersView()
		{
		}

		public void SetRTPMarkers(RTPMarkers rtpMarkers)
		{
			_rtpMarkers = rtpMarkers;
		}

		private float PixelsToTime(float pixels)
		{
			return pixels / _width;
		}

		public float TimeToPixels(float time)
		{
			return _width * time;
		}

		public bool OnGUI(float minWidth = -1f, float minHeight = -1f)
		{
			bool result = false;
			if (_rtpMarkers == null)
			{
				return result;
			}
			GUILayoutOption gUILayoutOption = ((minWidth >= 0f) ? GUILayout.MinWidth(minWidth) : null);
			GUILayoutOption gUILayoutOption2 = ((minHeight >= 0f) ? GUILayout.MinHeight(minHeight) : null);
			scrollView = GUILayout.BeginScrollView(scrollView, gUILayoutOption, gUILayoutOption2);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Name: ", GUILayout.MaxWidth(50f));
			text = EditorGUILayout.TextField("", text);
			if (GUILayout.Button("Add"))
			{
				_rtpMarkers.AddMarker(text, 0f);
				result = true;
			}
			GUILayout.EndHorizontal();
			for (int i = 0; i < _rtpMarkers._markers.Count; i++)
			{
				GUILayout.BeginHorizontal("Box");
				RTPMarker rTPMarker = _rtpMarkers._markers[i];
				if (GUILayout.Button("Del", GUILayout.MaxWidth(50f)))
				{
					_rtpMarkers.RemoveMarker(rTPMarker);
					return true;
				}
				GUILayout.Label("Name: ", GUILayout.MaxWidth(45f));
				rTPMarker._label = EditorGUILayout.TextField("", rTPMarker._label, GUILayout.MaxWidth(80f));
				GUILayout.BeginHorizontal();
				rTPMarker._value = GUILayout.HorizontalSlider(rTPMarker._value, 0f, 1f);
				float value = ((_parameter != null) ? Mathf.Lerp(_parameter._min, _parameter._max, rTPMarker._value) : rTPMarker._value);
				EditorGUILayout.FloatField(value, GUILayout.MaxWidth(48f));
				GUILayout.EndHorizontal();
				GUILayout.Label("KeyOff: ", GUILayout.MaxWidth(50f));
				rTPMarker._keyOffEnabled = GUILayout.Toggle(rTPMarker._keyOffEnabled, "");
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
			return result;
		}

		public void OnGraphGUI(Rect rect)
		{
			if (_rtpMarkers == null)
			{
				return;
			}
			_width = rect.width;
			for (int i = 0; i < _rtpMarkers._markers.Count; i++)
			{
				RTPMarker rTPMarker = _rtpMarkers._markers[i];
				float num = TimeToPixels(rTPMarker._value);
				if (num < rect.x)
				{
					num = rect.x;
				}
				if (num > rect.width)
				{
					num = rect.width;
				}
				if (rTPMarker._label != null)
				{
					Rect rect2 = new Rect(num, 20f, rTPMarker._label.Length * 10, 18f);
					GUI.Label(rect2, rTPMarker._label, "Box");
					PerformTimeScrollerDragging(rect2, rTPMarker._label);
					if (HasTimeScrollerDragged(rTPMarker._label))
					{
						float delta = dragging.delta;
						rTPMarker._value += PixelsToTime(delta);
						dragging.delta = 0f;
					}
					if (rect2.Contains(UnityEngine.Event.current.mousePosition) && UnityEngine.Event.current.type == EventType.ContextClick)
					{
						GenericMenu genericMenu = new GenericMenu();
						genericMenu.ShowAsContext();
						UnityEngine.Event.current.Use();
					}
				}
				Drawing.DrawLine(new Vector2(num, 0f), new Vector2(num, rect.height), Color.red, 5f, antiAlias: false);
			}
		}

		private void PerformTimeScrollerDragging(Rect timeScrollerRect, string label)
		{
			if (UnityEngine.Event.current == null || !UnityEngine.Event.current.isMouse || UnityEngine.Event.current.button != 0)
			{
				return;
			}
			if (UnityEngine.Event.current.type == EventType.MouseDown)
			{
				if (timeScrollerRect.Contains(UnityEngine.Event.current.mousePosition))
				{
					dragging = new DraggingData();
					dragging.key = label;
					UnityEngine.Event.current.Use();
				}
			}
			else if (UnityEngine.Event.current.type == EventType.MouseDrag && dragging != null)
			{
				dragging.delta = UnityEngine.Event.current.delta.x;
				UnityEngine.Event.current.Use();
			}
			else
			{
				dragging = null;
			}
		}

		private bool HasTimeScrollerDragged(string label)
		{
			if (UnityEngine.Event.current != null && UnityEngine.Event.current.type == EventType.Layout && dragging != null)
			{
				return dragging.key == label;
			}
			return false;
		}
	}
	[CustomEditor(typeof(RTPModulator))]
	public class RTPModulatorEditor : Editor
	{
		private RTPModulator rtpModulator;

		private Texture2D modulatorTex;

		private void OnEnable()
		{
			rtpModulator = base.target as RTPModulator;
			GenerateModulatorView();
		}

		private void GenerateModulatorView()
		{
			modulatorTex = new Texture2D(400, 64, TextureFormat.RGBA32, mipChain: false);
			modulatorTex.filterMode = FilterMode.Point;
			Color[] array = new Color[modulatorTex.width * modulatorTex.height];
			for (int i = 0; i < modulatorTex.width * modulatorTex.height; i++)
			{
				ref Color reference = ref array[i];
				reference = new Color(0f, 0f, 0f, 0f);
			}
			modulatorTex.SetPixels(array);
			int num = modulatorTex.height - 5;
			for (int j = 0; j < modulatorTex.width; j++)
			{
				float value = rtpModulator.GetValue(j);
				int num2 = ((value > 0f) ? 1 : (-1));
				int num3 = Mathf.CeilToInt(Mathf.Clamp(Mathf.Abs(value) * (float)num, 0f, num));
				modulatorTex.SetPixel(j, Mathf.FloorToInt(num / 2) - Mathf.FloorToInt(num3 / 2) * num2, Color.green);
			}
			modulatorTex.Apply();
		}

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			GUILayout.Space(5f);
			GUILayout.Box(modulatorTex, GUILayout.ExpandWidth(expand: true));
			if (GUIHelpers.CheckGUIHasChanged(rtpModulator.gameObject))
			{
				GenerateModulatorView();
			}
		}
	}
	public class SamplerManager : EditorWindow
	{
		private static bool _isOpened;

		private static SamplerManager _instance;

		private Vector2 scrollPosition;

		[MenuItem("Window/Fabric/Sampler Manager", false, 18)]
		private static void init()
		{
			SamplerManager samplerManager = (SamplerManager)EditorWindow.GetWindow(typeof(SamplerManager));
			samplerManager.titleContent = new GUIContent("SamplerManager");
		}

		public static void Open()
		{
			if (!_isOpened)
			{
				if (_instance == null)
				{
					_instance = ScriptableObject.CreateInstance<SamplerManager>();
					_instance.name = "SampleManager";
				}
				_instance.Show();
			}
		}

		private void OnEnable()
		{
			_isOpened = true;
		}

		private void OnDestroy()
		{
			_isOpened = false;
		}

		private void OnGUI()
		{
			if (SampleManager.Instance == null)
			{
				return;
			}
			MenuBar.OnGUI("288088-samplemanager", box: true);
			List<SampleFile> list = new List<SampleFile>();
			GUILayout.Space(5f);
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			for (int i = 0; i < SampleManager.Instance.GetNumSampleFiles(); i++)
			{
				SampleFile sampleFileByIndex = SampleManager.Instance.GetSampleFileByIndex(i);
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Del", GUILayout.MaxWidth(50f)))
				{
					list.Add(sampleFileByIndex);
				}
				Rect rect = GUILayoutUtility.GetRect(100f, 20f, GUILayout.ExpandWidth(expand: true));
				string text = "Drop Audio Clip here!!";
				Color backgroundColor = GUI.backgroundColor;
				GUI.backgroundColor = Color.red;
				if (sampleFileByIndex._name != null && sampleFileByIndex._name != "")
				{
					GUI.backgroundColor = Color.green;
					text = sampleFileByIndex._audioClipPath;
				}
				GUI.Box(rect, text);
				GUI.backgroundColor = backgroundColor;
				DragAndDropAudioClip(rect, sampleFileByIndex);
				GUILayout.EndHorizontal();
				GUILayout.Space(2f);
			}
			for (int j = 0; j < list.Count; j++)
			{
				SamplePlayerComponent[] componentsInChildren = GetFabricManager.Instance().gameObject.GetComponentsInChildren<SamplePlayerComponent>(includeInactive: true);
				for (int k = 0; k < componentsInChildren.Length; k++)
				{
					componentsInChildren[k].NotifySampleFileRemoved(list[j]);
				}
				SampleManager.Instance.RemoveSampleFile(list[j]);
			}
			if (GUILayout.Button("Add New SampleFile"))
			{
				SampleFile sampleFile = new SampleFile();
				SampleManager.Instance.AddSampleFile(sampleFile);
			}
			GUILayout.EndScrollView();
		}

		private void DragAndDropAudioClip(Rect drop_area, SampleFile sampleFile)
		{
			UnityEngine.Event current = UnityEngine.Event.current;
			switch (current.type)
			{
			case EventType.DragUpdated:
			case EventType.DragPerform:
			{
				if (!drop_area.Contains(current.mousePosition))
				{
					break;
				}
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				if (current.type != EventType.DragPerform)
				{
					break;
				}
				DragAndDrop.AcceptDrag();
				UnityEngine.Object obj = DragAndDrop.objectReferences[0];
				if (!(obj != null))
				{
					break;
				}
				AudioClip audioClip = obj as AudioClip;
				if (!(audioClip != null))
				{
					break;
				}
				string assetPath = AssetDatabase.GetAssetPath(audioClip);
				int num = assetPath.LastIndexOf("Resources/");
				if (num > 0)
				{
					sampleFile._audioClipPath = assetPath.Remove(0, num);
					sampleFile._audioClipPath = sampleFile._audioClipPath.Replace("Resources/", "");
					sampleFile._audioClipPath = sampleFile._audioClipPath.Replace(".wav", "");
					sampleFile._channels = audioClip.channels;
					sampleFile._sampleRate = audioClip.frequency;
					sampleFile._samples = audioClip.samples;
					sampleFile._name = audioClip.name;
					AudioImporter audioImporter = AssetImporter.GetAtPath(assetPath) as AudioImporter;
					if (assetPath == null || assetPath.Length <= 0 || (!assetPath.Contains(".wav") && !assetPath.Contains(".Wav")))
					{
						break;
					}
					WavReader wavReader = new WavReader();
					if (!wavReader.LoadFile(assetPath))
					{
						break;
					}
					sampleFile._markers.Clear();
					for (int i = 0; i < wavReader.m_cuePoints.Count; i++)
					{
						CuePoint cuePoint = wavReader.m_cuePoints[i];
						if (cuePoint != null)
						{
							Marker marker = new Marker();
							marker.name = cuePoint.label;
							marker.offsetSamples = cuePoint.dwSampleOffset;
							sampleFile._markers.Add(marker);
						}
						wavReader.CloseFile();
					}
				}
				else
				{
					EditorUtility.DisplayDialog("Fabric Error", "AudioClip not located in Resources folder", "Ok");
				}
				break;
			}
			}
		}
	}
}
namespace Fabric.Wav
{
	public class RiffParserException : ApplicationException
	{
		public RiffParserException()
		{
		}

		public RiffParserException(string message)
			: base(message)
		{
		}

		public RiffParserException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
	public class RiffParser
	{
		public delegate void ProcessListElement(RiffParser rp, int FourCCType, int length);

		public delegate void ProcessChunkElement(RiffParser rp, int FourCCType, int unpaddedLength, int paddedLength);

		public const int DWORDSIZE = 4;

		public const int TWODWORDSSIZE = 8;

		public static readonly string RIFF4CC = "RIFF";

		public static readonly string RIFX4CC = "RIFX";

		public static readonly string LIST4CC = "LIST";

		private string m_filename;

		private string m_shortname;

		private long m_filesize;

		private int m_datasize;

		private FileStream m_stream;

		private int m_fileriff;

		private int m_filetype;

		private byte[] m_eightBytes = new byte[8];

		private byte[] m_fourBytes = new byte[4];

		public int DataSize => m_datasize;

		public string FileName => m_filename;

		public string ShortName => m_shortname;

		public int FileRIFF => m_fileriff;

		public int FileType => m_filetype;

		public bool OpenFile(string filename)
		{
			if (m_stream != null)
			{
				return false;
			}
			FileInfo fileInfo = new FileInfo(filename);
			m_filename = fileInfo.FullName;
			m_shortname = fileInfo.Name;
			m_filesize = fileInfo.Length;
			m_stream = new FileStream(m_filename, FileMode.Open, FileAccess.Read, FileShare.Read);
			ReadTwoInts(out var FourCC, out var size);
			ReadOneInt(out var FourCC2);
			m_fileriff = FourCC;
			m_filetype = FourCC2;
			string strA = FromFourCC(FourCC);
			if (string.Compare(strA, RIFF4CC) == 0 || string.Compare(strA, RIFX4CC) == 0)
			{
				m_datasize = size;
				if (m_filesize < m_datasize + 8)
				{
					m_stream.Close();
					m_stream = null;
					return false;
				}
				return true;
			}
			m_stream.Close();
			m_stream = null;
			return false;
		}

		public bool ReadElement(ref int bytesleft, ProcessChunkElement chunk, ProcessListElement list)
		{
			if (8 > bytesleft)
			{
				return false;
			}
			ReadTwoInts(out var FourCC, out var size);
			bytesleft -= 8;
			if (bytesleft < size)
			{
				SkipData(bytesleft);
				bytesleft = 0;
				throw new RiffParserException("Element size mismatch for element " + FromFourCC(FourCC) + " need " + size + " but have only " + bytesleft);
			}
			string strA = FromFourCC(FourCC);
			if (string.Compare(strA, LIST4CC) == 0)
			{
				ReadOneInt(out FourCC);
				if (list == null)
				{
					SkipData(size - 4);
				}
				else
				{
					list(this, FourCC, size - 4);
				}
				bytesleft -= size;
			}
			else
			{
				int num = size;
				if ((size & 1) != 0)
				{
					num++;
				}
				if (chunk == null)
				{
					SkipData(num);
				}
				else
				{
					chunk(this, FourCC, size, num);
				}
				bytesleft -= num;
			}
			return true;
		}

		public void ReadTwoInts(out int FourCC, out int size)
		{
			try
			{
				int num = m_stream.Read(m_eightBytes, 0, 8);
				if (8 != num)
				{
					throw new RiffParserException("Unable to read. Corrupt RIFF file " + FileName);
				}
				FourCC = BitConverter.ToInt32(m_eightBytes, 0);
				size = BitConverter.ToInt32(m_eightBytes, 4);
			}
			catch (Exception inner)
			{
				throw new RiffParserException("Problem accessing RIFF file " + FileName, inner);
			}
		}

		public void ReadOneInt(out int FourCC)
		{
			try
			{
				int num = m_stream.Read(m_fourBytes, 0, 4);
				if (4 != num)
				{
					throw new RiffParserException("Unable to read. Corrupt RIFF file " + FileName);
				}
				FourCC = BitConverter.ToInt32(m_fourBytes, 0);
			}
			catch (Exception inner)
			{
				throw new RiffParserException("Problem accessing RIFF file " + FileName, inner);
			}
		}

		public void SkipData(int skipBytes)
		{
			try
			{
				m_stream.Seek(skipBytes, SeekOrigin.Current);
			}
			catch (Exception inner)
			{
				throw new RiffParserException("Problem seeking in file " + FileName, inner);
			}
		}

		public long GetPosition()
		{
			return m_stream.Position;
		}

		public void SeekData(int posBytes)
		{
			try
			{
				m_stream.Seek(posBytes, SeekOrigin.Begin);
			}
			catch (Exception inner)
			{
				throw new RiffParserException("Problem seeking in file " + FileName, inner);
			}
		}

		public int ReadData(byte[] data, int offset, int length)
		{
			try
			{
				return m_stream.Read(data, offset, length);
			}
			catch (Exception inner)
			{
				throw new RiffParserException("Problem reading data in file " + FileName, inner);
			}
		}

		public void CloseFile()
		{
			if (m_stream != null)
			{
				m_stream.Close();
				m_stream = null;
			}
		}

		public static string FromFourCC(int FourCC)
		{
			return new string(new char[4]
			{
				(char)(FourCC & 0xFF),
				(char)((FourCC >> 8) & 0xFF),
				(char)((FourCC >> 16) & 0xFF),
				(char)((FourCC >> 24) & 0xFF)
			});
		}

		public static int ToFourCC(string FourCC)
		{
			if (FourCC.Length != 4)
			{
				throw new Exception("FourCC strings must be 4 characters long " + FourCC);
			}
			return (int)(((uint)FourCC[3] << 24) | ((uint)FourCC[2] << 16) | ((uint)FourCC[1] << 8) | FourCC[0]);
		}

		public static int ToFourCC(char[] FourCC)
		{
			if (FourCC.Length != 4)
			{
				throw new Exception("FourCC char arrays must be 4 characters long " + new string(FourCC));
			}
			return (int)(((uint)FourCC[3] << 24) | ((uint)FourCC[2] << 16) | ((uint)FourCC[1] << 8) | FourCC[0]);
		}

		public static int ToFourCC(char c0, char c1, char c2, char c3)
		{
			return (int)(((uint)c3 << 24) | ((uint)c2 << 16) | ((uint)c1 << 8) | c0);
		}
	}
}
namespace Fabric
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SamplePlayerComponent))]
	public class SamplePlayerComponentEditor : Editor
	{
		private static GUIStyle thumbStyle = new GUIStyle();

		private ComponentEditor componentEditor = new ComponentEditor();

		private CoreAudioComponentEditor coreAudioComponentEditor = new CoreAudioComponentEditor();

		private SamplePlayerComponent samplePlayer;

		private SampleFile currentSampleFile;

		private Texture2D waveformTexture;

		private AudioClip audioClip;

		private float[] audioData;

		private Color[] xy;

		[MenuItem("Fabric/Components/SamplePlayerComponent")]
		private static void About()
		{
			GameObject activeGameObject = Selection.activeGameObject;
			if (!(activeGameObject == null))
			{
				GameObject gameObject = new GameObject("SamplePlayerComponent");
				gameObject.transform.parent = activeGameObject.transform;
				gameObject.AddComponent<SamplePlayerComponent>();
			}
		}

		private void OnEnable()
		{
			samplePlayer = base.target as SamplePlayerComponent;
			componentEditor.RegisterSerialisableObject(base.serializedObject);
			coreAudioComponentEditor.RegisterSerialisableObject(base.serializedObject);
		}

		public override void OnInspectorGUI()
		{
			int num = 0;
			componentEditor.InspectorGUI(samplePlayer, "290186-sampleplayercomponent");
			GUILayout.Space(5f);
			coreAudioComponentEditor.InspectorGUI(samplePlayer);
			GUILayout.Space(5f);
			GUILayout.BeginVertical("Box");
			GUILayout.BeginHorizontal();
			if (SampleManager.Instance != null)
			{
				string[] displayedOptions = SampleManager.Instance.ToStringArray();
				int sampleFileIndexByName = SampleManager.Instance.GetSampleFileIndexByName(samplePlayer.sampleFileName);
				GUILayout.Label("Sample: ", GUILayout.MaxWidth(55f));
				num = EditorGUILayout.Popup(sampleFileIndexByName, displayedOptions);
				if (num != sampleFileIndexByName)
				{
					SampleFile sampleFileByIndex = SampleManager.Instance.GetSampleFileByIndex(num);
					if (sampleFileByIndex != null)
					{
						samplePlayer.sampleFileName = sampleFileByIndex.Name();
						if (sampleFileByIndex._markers.Count > 1)
						{
							samplePlayer._leftLoopMarker = sampleFileByIndex._markers[0].offsetSamples;
							samplePlayer._rightLoopMarker = sampleFileByIndex._markers[1].offsetSamples;
						}
						else
						{
							samplePlayer._leftLoopMarker = 0;
							samplePlayer._rightLoopMarker = sampleFileByIndex._samples;
						}
					}
				}
			}
			if (GUILayout.Button("SamplerManager", GUILayout.MaxWidth(150f)))
			{
				SamplerManager.Open();
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
			if (SampleManager.Instance != null)
			{
				DrawWaveform(new Rect(0f, 0f, (float)Screen.width - 30f, 150f), num);
			}
			GUILayout.EndVertical();
		}

		private void DrawWaveform(Rect rect, int index)
		{
			if (index < 0)
			{
				return;
			}
			int num = (int)rect.width;
			int num2 = (int)rect.height;
			if (waveformTexture == null || waveformTexture.width != num || waveformTexture.height != num2)
			{
				waveformTexture = new Texture2D(num, num2, TextureFormat.RGBA32, mipChain: false);
			}
			SampleFile sampleFileByIndex = SampleManager.Instance.GetSampleFileByIndex(index);
			if (sampleFileByIndex == null)
			{
				return;
			}
			if (sampleFileByIndex != currentSampleFile)
			{
				audioClip = Resources.Load(sampleFileByIndex._audioClipPath) as AudioClip;
				if (audioClip != null)
				{
					int num3 = audioClip.samples * audioClip.channels;
					audioData = new float[num3];
					audioClip.GetData(audioData, 0);
					xy = new Color[num * num2];
					for (int i = 0; i < num * num2; i++)
					{
						ref Color reference = ref xy[i];
						reference = new Color(0.5f, 0.2f, 0.5f, 0f);
					}
				}
				currentSampleFile = sampleFileByIndex;
			}
			if (audioData != null)
			{
				int channels = sampleFileByIndex._channels;
				int samples = sampleFileByIndex._samples;
				int num4 = Mathf.CeilToInt(samples * channels / num);
				waveformTexture.SetPixels(0, 0, num, num2, xy);
				int j = 0;
				int num5 = 20;
				int num6 = num2 / channels;
				for (; j < num; j++)
				{
					num2 = 0;
					int num7 = j * num4 % channels;
					for (int k = 0; k < channels; k++)
					{
						num2 += num6;
						int num8 = j * num4 + k - num7;
						if (num8 < audioData.Length)
						{
							float num9 = audioData[num8];
							int num10 = ((num9 > 0f) ? 1 : (-1));
							int num11 = Mathf.CeilToInt(Mathf.Clamp(Mathf.Abs(num9) * (float)num2, 0f, num2));
							for (int l = 0; l < num11; l++)
							{
								waveformTexture.SetPixel(j, Mathf.FloorToInt(num2 / 2) - Mathf.FloorToInt(num11 / 2) * num10 + l * num10, Color.green);
							}
							num2 += num5;
						}
					}
				}
				float num12 = rect.width / (float)samples;
				if (samplePlayer.sampleFileInstance != null)
				{
					float num13 = (float)samplePlayer.sampleFileInstance._position * num12;
					Drawing.DrawLine(waveformTexture, (int)num13, 0, (int)num13, (int)rect.height, Color.red);
					Drawing.DrawLine(waveformTexture, (int)num13 + 1, 0, (int)num13 + 1, (int)rect.height, Color.red);
					float num14 = (float)samplePlayer.sampleFileInstance._start * num12;
					Drawing.DrawLine(waveformTexture, (int)num14, 0, (int)num14, (int)rect.height, Color.blue);
					float num15 = (float)samplePlayer.sampleFileInstance._end * num12;
					Drawing.DrawLine(waveformTexture, (int)num15, 0, (int)num15, (int)rect.height, Color.blue);
				}
				else
				{
					float num16 = (float)samplePlayer._leftLoopMarker * num12;
					Drawing.DrawLine(waveformTexture, (int)num16, 0, (int)((float)samplePlayer._leftLoopMarker * num12), (int)rect.height, Color.blue);
					float num17 = (float)samplePlayer._rightLoopMarker * num12;
					Drawing.DrawLine(waveformTexture, (int)num17, 0, (int)num17, (int)rect.height, Color.blue);
				}
			}
			waveformTexture.Apply();
			GUILayout.Space(5f);
			GUIHelpers.DrawLine(Color.green);
			if (Application.isPlaying)
			{
				samplePlayer.sampleFileInstance._position = (int)EditorGUILayout.Slider("Position: ", samplePlayer.sampleFileInstance._position, 0f, sampleFileByIndex._samples);
			}
			GUILayout.Box(waveformTexture);
			GUILayout.BeginVertical("Box");
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			Color color = GUI.color;
			GUI.color = Color.white;
			GUILayout.Label("Loop Region");
			GUI.color = color;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
			GUILayout.BeginHorizontal();
			samplePlayer.SetLeftLoopMarker((int)EditorGUILayout.Slider("Left Marker: ", samplePlayer._leftLoopMarker, 0f, sampleFileByIndex._samples));
			samplePlayer.SetRightLoopMarker((int)EditorGUILayout.Slider("Right Marker: ", samplePlayer._rightLoopMarker, 0f, sampleFileByIndex._samples));
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.Space(5f);
			GUIHelpers.DrawLine(Color.green);
			GUILayout.Space(5f);
			GUILayout.BeginVertical("Box");
			for (int m = 0; m < sampleFileByIndex._channels; m++)
			{
				samplePlayer._channelGains[m] = EditorGUILayout.Slider("Channel: " + m, samplePlayer._channelGains[m], 0f, 1f);
			}
			GUILayout.EndVertical();
		}
	}
}
namespace Fabric.Wav
{
	public class CuePoint
	{
		public int dwIdentifier;

		public int dwPosition;

		public int fccChunk;

		public int dwChunkStart;

		public int dwBlockStart;

		public int dwSampleOffset;

		public string label = "";

		public string note = "";
	}
	public class Region
	{
		public CuePoint cuepoint;

		public int length;
	}
	public class SampleLoop
	{
		public int dwIdentifier;

		public int dwType;

		public int dwStart;

		public int dwEnd;

		public int dwFraction;

		public int dwPlayCount;

		public string label = "";
	}
	public struct Bwf
	{
		public string Description { get; set; }

		public string Originator { get; set; }

		public string OriginatorReference { get; set; }

		public DateTime OriginationDateTime { get; set; }

		public string OriginationDate => OriginationDateTime.ToString("yyyy-MM-dd");

		public string OriginationTime => OriginationDateTime.ToString("HH:mm:ss");

		public long TimeReference { get; set; }

		public ushort Version => 1;

		public string UniqueMaterialIdentifier { get; set; }

		public byte[] Reserved { get; private set; }

		public string CodingHistory { get; set; }
	}
	public struct WaveFormatex
	{
		public short wFormatTag;

		public short nChannels;

		public int nSamplesPerSec;

		public int nAvgBytesPerSec;

		public short nBlockAlign;

		public short wBitsPerSample;

		public short cbSize;
	}
	public class WavReader
	{
		public List<CuePoint> m_cuePoints = new List<CuePoint>();

		public List<Region> m_regions = new List<Region>();

		public List<SampleLoop> m_sampleLoops = new List<SampleLoop>();

		public Bwf m_bwf = default(Bwf);

		public WaveFormatex m_waveFormatex = default(WaveFormatex);

		public float[] m_audioData;

		public bool m_loadAudioData;

		private RiffParser m_parser;

		public WavReader()
		{
			m_parser = new RiffParser();
		}

		public bool OpenFile(string file)
		{
			return m_parser.OpenFile(file);
		}

		public void CloseFile()
		{
			m_parser.CloseFile();
		}

		public bool LoadFile(string file)
		{
			if (m_parser.OpenFile(file))
			{
				Process(loadAudioData: true);
				return true;
			}
			return false;
		}

		private void Clear()
		{
			m_cuePoints.Clear();
			m_regions.Clear();
			m_waveFormatex.wFormatTag = 0;
			m_waveFormatex.nChannels = 0;
			m_waveFormatex.nSamplesPerSec = 0;
			m_waveFormatex.nAvgBytesPerSec = 0;
			m_waveFormatex.nBlockAlign = 0;
			m_waveFormatex.wBitsPerSample = 0;
		}

		private void ProcessList(RiffParser rp, int FourCC, int length)
		{
			RiffParser.ProcessChunkElement chunk = ProcessChunk;
			RiffParser.ProcessListElement list = ProcessList;
			while (length > 0 && rp.ReadElement(ref length, chunk, list))
			{
			}
		}

		private void ProcessChunk(RiffParser rp, int FourCC, int unpaddedLength, int length)
		{
			if (RiffParser.ToFourCC("fmt ") == FourCC)
			{
				DecodeWave(rp, length);
			}
			else if (RiffParser.ToFourCC("cue ") == FourCC)
			{
				DecodeMarkers(rp, length);
			}
			else if (RiffParser.ToFourCC("labl") == FourCC)
			{
				DecodeLabels(rp, length);
			}
			else if (RiffParser.ToFourCC("note") == FourCC)
			{
				DecodeNotes(rp, length);
			}
			else if (RiffParser.ToFourCC("data") == FourCC)
			{
				DecodeData(rp, length);
			}
			else if (RiffParser.ToFourCC("ltxt") == FourCC)
			{
				DecodeRegions(rp, length);
			}
			else if (RiffParser.ToFourCC("smpl") == FourCC)
			{
				DecodeSample(rp, length);
			}
			else if (RiffParser.ToFourCC("bext") == FourCC)
			{
				DecodeBwf(rp, length);
			}
			else
			{
				rp.SkipData(length);
			}
		}

		private char ReadFromBufferChar(byte[] buffer, ref int offset)
		{
			char result = (char)buffer[offset];
			offset++;
			return result;
		}

		private short ReadFromBuffer2(byte[] buffer, ref int offset)
		{
			short result = BitConverter.ToInt16(buffer, offset);
			offset += 2;
			return result;
		}

		private int ReadFromBuffer4(byte[] buffer, ref int offset)
		{
			int result = BitConverter.ToInt32(buffer, offset);
			offset += 4;
			return result;
		}

		private CuePoint GetCuePointByIdentifier(int identifier)
		{
			for (int i = 0; i < m_cuePoints.Count; i++)
			{
				CuePoint cuePoint = m_cuePoints[i];
				if (cuePoint != null && cuePoint.dwIdentifier == identifier)
				{
					return cuePoint;
				}
			}
			return null;
		}

		private void DecodeBwf(RiffParser rp, int length)
		{
			byte[] array = new byte[length];
			rp.ReadData(array, 0, length);
			int offset = 4;
			for (int i = 0; i < 256; i++)
			{
				m_bwf.Description += ReadFromBufferChar(array, ref offset);
			}
		}

		private void DecodeSample(RiffParser rp, int length)
		{
			byte[] array = new byte[length];
			rp.ReadData(array, 0, length);
			int offset = 0;
			ReadFromBuffer4(array, ref offset);
			ReadFromBuffer4(array, ref offset);
			ReadFromBuffer4(array, ref offset);
			ReadFromBuffer4(array, ref offset);
			ReadFromBuffer4(array, ref offset);
			ReadFromBuffer4(array, ref offset);
			ReadFromBuffer4(array, ref offset);
			int num = ReadFromBuffer4(array, ref offset);
			ReadFromBuffer4(array, ref offset);
			for (int i = 0; i < num; i++)
			{
				SampleLoop sampleLoop = new SampleLoop();
				sampleLoop.dwIdentifier = ReadFromBuffer4(array, ref offset);
				sampleLoop.dwType = ReadFromBuffer4(array, ref offset);
				sampleLoop.dwStart = ReadFromBuffer4(array, ref offset);
				sampleLoop.dwEnd = ReadFromBuffer4(array, ref offset);
				sampleLoop.dwFraction = ReadFromBuffer4(array, ref offset);
				sampleLoop.dwPlayCount = ReadFromBuffer4(array, ref offset);
				m_sampleLoops.Add(sampleLoop);
			}
		}

		private void DecodeLabels(RiffParser rp, int length)
		{
			byte[] array = new byte[length];
			rp.ReadData(array, 0, length);
			int offset = 0;
			int identifier = ReadFromBuffer4(array, ref offset);
			CuePoint cuePointByIdentifier = GetCuePointByIdentifier(identifier);
			if (cuePointByIdentifier != null)
			{
				for (int i = 4; i < length; i++)
				{
					cuePointByIdentifier.label += (char)array[i];
				}
			}
		}

		private void DecodeData(RiffParser rp, int length)
		{
			if (m_loadAudioData)
			{
				byte[] array = new byte[length];
				rp.ReadData(array, 0, length);
				m_audioData = new float[length / 2];
				EditorUtility.DisplayProgressBar("WavReader", "Starting", 0f);
				int offset = 0;
				int num = m_audioData.Length / 10;
				int num2 = 0;
				for (int i = 0; i < m_audioData.Length; i++)
				{
					m_audioData[i] = (float)((double)ReadFromBuffer2(array, ref offset) * 3.0517578125E-05);
					if (i % num == 0)
					{
						EditorUtility.DisplayProgressBar("WavReader", "Loading Data", (float)num2 / 10f);
						num2++;
					}
				}
				EditorUtility.DisplayProgressBar("WavReader", "Done!", 1f);
				EditorUtility.ClearProgressBar();
			}
			else
			{
				rp.SkipData(length);
			}
		}

		private void DecodeNotes(RiffParser rp, int length)
		{
			byte[] array = new byte[length];
			rp.ReadData(array, 0, length);
			int offset = 0;
			int identifier = ReadFromBuffer4(array, ref offset);
			CuePoint cuePointByIdentifier = GetCuePointByIdentifier(identifier);
			if (cuePointByIdentifier != null)
			{
				for (int i = 4; i < length; i++)
				{
					cuePointByIdentifier.note += (char)array[i];
				}
			}
		}

		private void DecodeRegions(RiffParser rp, int length)
		{
			byte[] array = new byte[length];
			rp.ReadData(array, 0, length);
			int offset = 0;
			int identifier = ReadFromBuffer4(array, ref offset);
			CuePoint cuePointByIdentifier = GetCuePointByIdentifier(identifier);
			if (cuePointByIdentifier != null)
			{
				Region region = new Region();
				region.cuepoint = cuePointByIdentifier;
				region.length = ReadFromBuffer4(array, ref offset);
				ReadFromBuffer4(array, ref offset);
				ReadFromBuffer2(array, ref offset);
				ReadFromBuffer2(array, ref offset);
				ReadFromBuffer2(array, ref offset);
				ReadFromBuffer2(array, ref offset);
				m_regions.Add(region);
			}
		}

		private void DecodeMarkers(RiffParser rp, int length)
		{
			byte[] array = new byte[length];
			rp.ReadData(array, 0, length);
			int offset = 0;
			int num = ReadFromBuffer4(array, ref offset);
			for (int i = 0; i < num; i++)
			{
				CuePoint cuePoint = new CuePoint();
				cuePoint.dwIdentifier = ReadFromBuffer4(array, ref offset);
				cuePoint.dwPosition = ReadFromBuffer4(array, ref offset);
				cuePoint.fccChunk = ReadFromBuffer4(array, ref offset);
				cuePoint.dwChunkStart = ReadFromBuffer4(array, ref offset);
				cuePoint.dwBlockStart = ReadFromBuffer4(array, ref offset);
				cuePoint.dwSampleOffset = ReadFromBuffer4(array, ref offset);
				m_cuePoints.Add(cuePoint);
			}
		}

		private void DecodeWave(RiffParser rp, int length)
		{
			byte[] array = new byte[length];
			rp.ReadData(array, 0, length);
			m_waveFormatex.wFormatTag = BitConverter.ToInt16(array, 0);
			m_waveFormatex.nChannels = BitConverter.ToInt16(array, 2);
			m_waveFormatex.nSamplesPerSec = BitConverter.ToInt32(array, 4);
			m_waveFormatex.nAvgBytesPerSec = BitConverter.ToInt32(array, 8);
			m_waveFormatex.nBlockAlign = BitConverter.ToInt16(array, 12);
			m_waveFormatex.wBitsPerSample = BitConverter.ToInt16(array, 14);
		}

		public void Process(bool loadAudioData = false)
		{
			m_loadAudioData = loadAudioData;
			Clear();
			int bytesleft = m_parser.DataSize;
			RiffParser.ProcessChunkElement chunk = ProcessChunk;
			RiffParser.ProcessListElement list = ProcessList;
			while (bytesleft > 0 && m_parser.ReadElement(ref bytesleft, chunk, list))
			{
			}
			for (int i = 0; i < m_sampleLoops.Count; i++)
			{
				SampleLoop sampleLoop = m_sampleLoops[i];
				CuePoint cuePointByIdentifier = GetCuePointByIdentifier(sampleLoop.dwIdentifier);
				if (cuePointByIdentifier != null)
				{
					sampleLoop.label = cuePointByIdentifier.label;
				}
			}
		}
	}
}
namespace Fabric
{
	public static class XmlDeserialization
	{
		private class DeserializeFromXmlHelper
		{
			private readonly Dictionary<int, UnityEngine.Object> _indexTable = new Dictionary<int, UnityEngine.Object>();

			public static void DeserializeGameObject(XmlDocument xml, GameObject gameObject)
			{
				DeserializeFromXmlHelper deserializeFromXmlHelper = new DeserializeFromXmlHelper();
				deserializeFromXmlHelper.CreateObjects(xml.DocumentElement, gameObject);
				deserializeFromXmlHelper.DeserializeGameObject(xml.DocumentElement);
			}

			public static void DeserializeComponent(XmlDocument xml, UnityEngine.Component component)
			{
				DeserializeFromXmlHelper deserializeFromXmlHelper = new DeserializeFromXmlHelper();
				deserializeFromXmlHelper.DeserializeComponent(xml.DocumentElement, component);
			}

			private void CreateObjects(XmlElement xmlElement, GameObject gameObject)
			{
				string value = xmlElement.Attributes["name"].Value;
				int key = int.Parse(xmlElement.Attributes["id"].Value);
				gameObject.name = value;
				_indexTable[key] = gameObject;
				for (int i = 0; i < xmlElement.ChildNodes.Count; i++)
				{
					if (xmlElement.ChildNodes[i].NodeType == XmlNodeType.Element && !(xmlElement.ChildNodes[i].Name != "Component"))
					{
						XmlElement xmlElement2 = (XmlElement)xmlElement.ChildNodes[i];
						string value2 = xmlElement2.Attributes["type"].Value;
						int key2 = int.Parse(xmlElement2.Attributes["id"].Value);
						UnityEngine.Component value3 = gameObject.AddComponent(FindTypeByName(value2));
						_indexTable[key2] = value3;
					}
				}
				for (int j = 0; j < xmlElement.ChildNodes.Count; j++)
				{
					if (xmlElement.ChildNodes[j].NodeType == XmlNodeType.Element && !(xmlElement.ChildNodes[j].Name != "GameObject"))
					{
						XmlElement xmlElement3 = (XmlElement)xmlElement.ChildNodes[j];
						GameObject gameObject2 = new GameObject();
						gameObject2.transform.parent = gameObject.transform;
						CreateObjects(xmlElement3, gameObject2);
					}
				}
			}

			private static XmlElement GetSingleChild(XmlElement parent, string childName)
			{
				XmlElement result = null;
				int num = 0;
				for (int i = 0; i < parent.ChildNodes.Count; i++)
				{
					XmlNode xmlNode = parent.ChildNodes[i];
					if (xmlNode.NodeType == XmlNodeType.Element && !(xmlNode.Name != childName))
					{
						num++;
						result = (XmlElement)xmlNode;
					}
				}
				if (num == 1)
				{
					return result;
				}
				return null;
			}

			private static XmlElement GetArrayChild(XmlElement parent, string arrayName, int index)
			{
				XmlElement singleChild = GetSingleChild(parent, arrayName);
				int num = 0;
				for (int i = 0; i < singleChild.ChildNodes.Count; i++)
				{
					XmlNode xmlNode = singleChild.ChildNodes[i];
					if (xmlNode.NodeType == XmlNodeType.Element && !(xmlNode.Name != "entry"))
					{
						num++;
						if (num > index)
						{
							return (XmlElement)xmlNode;
						}
					}
				}
				return null;
			}

			private static int GetArraySize(XmlElement arrayElement)
			{
				int num = 0;
				for (int i = 0; i < arrayElement.ChildNodes.Count; i++)
				{
					XmlNode xmlNode = arrayElement.ChildNodes[i];
					if (xmlNode.NodeType == XmlNodeType.Element && !(xmlNode.Name != "entry"))
					{
						num++;
					}
				}
				return num;
			}

			private static XmlElement FindElement(XmlElement parent, string path)
			{
				while (true)
				{
					if (path.StartsWith("_"))
					{
						path = path.Substring(1);
						continue;
					}
					int num = path.IndexOf('.');
					if (num < 0)
					{
						break;
					}
					int num2 = path.IndexOf('[');
					if (num2 >= 0 && num2 < num)
					{
						string arrayName = path.Substring(0, num2);
						int index = int.Parse(path.Substring(num2 + 1, num - 1 - num2 - 1));
						parent = GetArrayChild(parent, arrayName, index);
					}
					else
					{
						parent = GetSingleChild(parent, path.Substring(0, num));
					}
					if (parent == null)
					{
						return null;
					}
					path = path.Substring(num + 1);
				}
				int num3 = path.IndexOf('[');
				if (num3 < 0)
				{
					return GetSingleChild(parent, path);
				}
				string arrayName2 = path.Substring(0, num3);
				int index2 = int.Parse(path.Substring(num3 + 1, path.Length - 1 - num3 - 1));
				return GetArrayChild(parent, arrayName2, index2);
			}

			private void DeserializeGameObject(XmlElement xmlElement)
			{
				for (int i = 0; i < xmlElement.ChildNodes.Count; i++)
				{
					if (xmlElement.ChildNodes[i].NodeType == XmlNodeType.Element && !(xmlElement.ChildNodes[i].Name != "Component"))
					{
						XmlElement xmlElement2 = (XmlElement)xmlElement.ChildNodes[i];
						int key = int.Parse(xmlElement2.Attributes["id"].Value);
						UnityEngine.Component component = _indexTable[key] as UnityEngine.Component;
						DeserializeComponent(xmlElement2, component);
					}
				}
				for (int j = 0; j < xmlElement.ChildNodes.Count; j++)
				{
					if (xmlElement.ChildNodes[j].NodeType == XmlNodeType.Element && !(xmlElement.ChildNodes[j].Name != "GameObject"))
					{
						XmlElement xmlElement3 = (XmlElement)xmlElement.ChildNodes[j];
						DeserializeGameObject(xmlElement3);
					}
				}
			}

			private void DeserializeComponent(XmlElement xmlElement, UnityEngine.Component component)
			{
				string value = xmlElement.Attributes["type"].Value;
				Type type = FindTypeByName(value);
				if (component.GetType() != type)
				{
					UnityEngine.Debug.LogError($"Component is of incorrect type '{component.GetType()}' - the expected type was '{type}'");
					return;
				}
				foreach (Serialization.IField item in Serialization.EnumerateFields(component))
				{
					if (item.FieldName.EndsWith("#"))
					{
						string path = item.FieldName.Substring(0, item.FieldName.Length - 1);
						XmlElement xmlElement2 = FindElement(xmlElement, path);
						if (xmlElement2 != null)
						{
							int arraySize = GetArraySize(xmlElement2);
							item.SetValue(arraySize);
						}
						continue;
					}
					XmlElement xmlElement3 = FindElement(xmlElement, item.FieldName);
					if (xmlElement3 == null)
					{
						continue;
					}
					string innerText = xmlElement3.InnerText;
					if (item.Type == typeof(float))
					{
						item.SetValue(float.Parse(innerText));
					}
					else if (item.Type == typeof(double))
					{
						item.SetValue(double.Parse(innerText));
					}
					else if (item.Type == typeof(int))
					{
						item.SetValue(int.Parse(innerText));
					}
					else if (item.Type == typeof(bool))
					{
						item.SetValue(bool.Parse(innerText));
					}
					else if (item.Type.IsSubclassOf(typeof(UnityEngine.Object)) || item.Type == typeof(UnityEngine.Object))
					{
						switch (xmlElement3.GetAttribute("type"))
						{
						case "asset-guid":
						{
							string assetPath = AssetDatabase.GUIDToAssetPath(innerText);
							UnityEngine.Object value5 = AssetDatabase.LoadMainAssetAtPath(assetPath);
							item.SetValue(value5);
							break;
						}
						case "asset-path":
						{
							UnityEngine.Object value4 = AssetDatabase.LoadMainAssetAtPath(innerText);
							item.SetValue(value4);
							break;
						}
						case "resource-path":
						{
							string attribute = xmlElement3.GetAttribute("restype");
							UnityEngine.Object value3 = Resources.Load(innerText, FindTypeByName(attribute));
							item.SetValue(value3);
							break;
						}
						case "object-id":
						{
							UnityEngine.Object value2 = _indexTable[int.Parse(innerText)];
							item.SetValue(value2);
							break;
						}
						}
					}
					else if (item.Type == typeof(AnimationCurve))
					{
						item.SetValue(DeserializeAnimationCurve(xmlElement3));
					}
					else if (item.Type == typeof(Vector3))
					{
						item.SetValue(DeserializeVector3(xmlElement3));
					}
					else
					{
						item.SetValue(innerText);
					}
				}
			}

			private static AnimationCurve DeserializeAnimationCurve(XmlElement element)
			{
				List<Keyframe> list = new List<Keyframe>();
				for (int i = 0; i < element.ChildNodes.Count; i++)
				{
					XmlNode xmlNode = element.ChildNodes[i];
					if (xmlNode.NodeType == XmlNodeType.Element && !(xmlNode.Name != "key"))
					{
						XmlElement xmlElement = (XmlElement)xmlNode;
						float inTangent = float.Parse(xmlElement.GetAttribute("inTangent"));
						float outTangent = float.Parse(xmlElement.GetAttribute("outTangent"));
						float time = float.Parse(xmlElement.GetAttribute("time"));
						float value = float.Parse(xmlElement.GetAttribute("value"));
						Keyframe item = new Keyframe(time, value, inTangent, outTangent);
						list.Add(item);
					}
				}
				AnimationCurve animationCurve = new AnimationCurve(list.ToArray());
				animationCurve.preWrapMode = (WrapMode)Enum.Parse(typeof(WrapMode), element.GetAttribute("preWrapMode"));
				animationCurve.postWrapMode = (WrapMode)Enum.Parse(typeof(WrapMode), element.GetAttribute("postWrapMode"));
				return animationCurve;
			}

			private static Vector3 DeserializeVector3(XmlElement element)
			{
				XmlElement singleChild = GetSingleChild(element, "x");
				XmlElement singleChild2 = GetSingleChild(element, "y");
				XmlElement singleChild3 = GetSingleChild(element, "z");
				return new Vector3(float.Parse(singleChild.InnerText), float.Parse(singleChild2.InnerText), float.Parse(singleChild3.InnerText));
			}
		}

		public static void LoadFromXMLToChild(GameObject gameObject)
		{
			if (!(gameObject == null))
			{
				string text = EditorUtility.OpenFilePanel("Load XML to Child", "", "xml");
				if (text != null && text.Length != 0)
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.Load(text);
					GameObject gameObject2 = new GameObject();
					gameObject2.transform.parent = gameObject.transform;
					DeserializeFromXmlHelper.DeserializeGameObject(xmlDocument, gameObject2);
				}
			}
		}

		public static void LoadFromXML(GameObject gameObject)
		{
			string text = EditorUtility.OpenFilePanel("Load From XML", "", "xml");
			if (text == null || text.Length == 0)
			{
				return;
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(text);
			UnityEngine.Component[] components = gameObject.GetComponents<UnityEngine.Component>();
			foreach (UnityEngine.Component component in components)
			{
				if (component.GetType() != typeof(Transform))
				{
					UnityEngine.Object.DestroyImmediate(component);
				}
			}
			foreach (object item in gameObject.transform)
			{
				UnityEngine.Object.DestroyImmediate(((Transform)item).gameObject);
			}
			DeserializeFromXmlHelper.DeserializeGameObject(xmlDocument, gameObject);
		}

		[MenuItem("CONTEXT/Transform/Load Fabric XML", false, 654)]
		public static void DeserializeFromXmlOverwrite(MenuCommand command)
		{
			UnityEngine.Component component = command.context as UnityEngine.Component;
			if (!(component == null))
			{
				LoadFromXML(component.gameObject);
			}
		}

		[MenuItem("CONTEXT/Transform/Load Fabric XML To New Child", false, 655)]
		public static void DeserializeFromXmlToChild(MenuCommand command)
		{
			UnityEngine.Component component = command.context as UnityEngine.Component;
			if (!(component == null))
			{
				LoadFromXMLToChild(component.gameObject);
			}
		}

		[MenuItem("CONTEXT/MonoBehaviour/Load Fabric XML", false, 654)]
		public static void DeserializeFromXmlOverwriteComponent(MenuCommand command)
		{
			UnityEngine.Component component = command.context as UnityEngine.Component;
			if (!(component == null))
			{
				LoadFromXML(component.gameObject);
			}
		}

		[MenuItem("CONTEXT/MonoBehaviour/Load Fabric XML To New Child", false, 655)]
		public static void DeserializeFromXmlToChildComponent(MenuCommand command)
		{
			UnityEngine.Component component = command.context as UnityEngine.Component;
			if (!(component == null))
			{
				LoadFromXMLToChild(component.gameObject);
			}
		}

		private static Type FindTypeByName(string typeName)
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				Type type = assembly.GetType(typeName);
				if (type != null)
				{
					return type;
				}
			}
			return null;
		}
	}
	public static class XmlSerialization
	{
		private class FieldArrayEntry
		{
			private readonly int _size;

			private Serialization.IField[] _simpleEntries;

			private FieldDictEntry[] _compoundEntries;

			public FieldArrayEntry(int size)
			{
				_size = size;
			}

			public void SetCompound(int index, string path, Serialization.IField field)
			{
				if (_compoundEntries == null)
				{
					_compoundEntries = new FieldDictEntry[_size];
				}
				if (_compoundEntries[index] == null)
				{
					_compoundEntries[index] = new FieldDictEntry();
				}
				_compoundEntries[index].Add(path, field);
			}

			public void SetSimple(int index, Serialization.IField field)
			{
				if (_simpleEntries == null)
				{
					_simpleEntries = new Serialization.IField[_size];
				}
				_simpleEntries[index] = field;
			}

			public void WriteXml(List<string> xml, string indent)
			{
				if (_simpleEntries != null)
				{
					for (int i = 0; i < _size; i++)
					{
						FieldDictEntry.WriteXmlValue(xml, indent, "entry", _simpleEntries[i]);
					}
					return;
				}
				for (int j = 0; j < _size; j++)
				{
					xml.Add($"{indent}<entry>");
					_compoundEntries[j].WriteXml(xml, indent + "    ");
					xml.Add($"{indent}</entry>");
				}
			}
		}

		private class FieldDictEntry
		{
			private readonly Dictionary<string, Serialization.IField> _fields = new Dictionary<string, Serialization.IField>();

			private readonly Dictionary<string, FieldDictEntry> _children = new Dictionary<string, FieldDictEntry>();

			private readonly Dictionary<string, FieldArrayEntry> _arrays = new Dictionary<string, FieldArrayEntry>();

			public void Add(string path, Serialization.IField field)
			{
				path = path.TrimStart('_');
				int num = path.IndexOf('.');
				if (num < 0)
				{
					if (path.EndsWith("#"))
					{
						string key = path.Substring(0, path.Length - 1);
						_arrays[key] = new FieldArrayEntry((int)field.GetValue());
					}
					else if (path.EndsWith("]"))
					{
						int num2 = path.IndexOf('[');
						string key2 = path.Substring(0, num2);
						int index = int.Parse(path.Substring(num2 + 1, path.Length - 1 - num2 - 1));
						_arrays[key2].SetSimple(index, field);
					}
					else
					{
						_fields[path] = field;
					}
					return;
				}
				string text = path.Substring(0, num);
				string path2 = path.Substring(num + 1);
				int num3 = text.IndexOf('[');
				if (num3 < 0)
				{
					if (!_children.ContainsKey(text))
					{
						_children[text] = new FieldDictEntry();
					}
					_children[text].Add(path2, field);
				}
				else
				{
					string key3 = text.Substring(0, num3);
					int index2 = int.Parse(text.Substring(num3 + 1, text.Length - 1 - num3 - 1));
					_arrays[key3].SetCompound(index2, path2, field);
				}
			}

			public void WriteXml(List<string> xml, string indent)
			{
				foreach (KeyValuePair<string, Serialization.IField> field in _fields)
				{
					string key = field.Key;
					Serialization.IField value = field.Value;
					WriteXmlValue(xml, indent, key, value);
				}
				foreach (KeyValuePair<string, FieldDictEntry> child in _children)
				{
					string key2 = child.Key;
					FieldDictEntry value2 = child.Value;
					xml.Add($"{indent}<{key2}>");
					value2.WriteXml(xml, indent + "    ");
					xml.Add($"{indent}</{key2}>");
				}
				foreach (KeyValuePair<string, FieldArrayEntry> array in _arrays)
				{
					string key3 = array.Key;
					FieldArrayEntry value3 = array.Value;
					xml.Add($"{indent}<{key3}>");
					value3.WriteXml(xml, indent + "    ");
					xml.Add($"{indent}</{key3}>");
				}
			}

			public static void WriteXmlValue(List<string> xml, string indent, string name, Serialization.IField field)
			{
				object value = field.GetValue();
				UnityEngine.Object obj = value as UnityEngine.Object;
				if (obj != null)
				{
					string assetPath = AssetDatabase.GetAssetPath(obj);
					if (!string.IsNullOrEmpty(assetPath))
					{
						int num = assetPath.IndexOf("/Resources/");
						if (num >= 0)
						{
							string text = assetPath.Substring(num + "/Resources/".Length);
							int num2 = text.LastIndexOf(".");
							if (num2 >= 0)
							{
								text = text.Substring(0, num2);
							}
							xml.Add($"{indent}<{name} type=\"resource-path\" restype=\"{obj.GetType()}\">{text}</{name}>");
						}
						else
						{
							xml.Add($"{indent}<{name} type=\"asset-path\">{assetPath}</{name}>");
						}
					}
					else
					{
						xml.Add($"{indent}<{name} type=\"object-id\">{obj.GetEntityId()}</{name}>");
					}
				}
				else if (value is AnimationCurve)
				{
					AnimationCurve animationCurve = value as AnimationCurve;
					xml.Add($"{indent}<{name} preWrapMode=\"{animationCurve.preWrapMode}\" postWrapMode=\"{animationCurve.postWrapMode}\">");
					for (int i = 0; i < animationCurve.length; i++)
					{
						Keyframe keyframe = animationCurve[i];
						xml.Add($"{indent}    <key inTangent=\"{keyframe.inTangent}\" outTangent=\"{keyframe.outTangent}\" time=\"{keyframe.time}\" value=\"{keyframe.value}\"/>");
					}
					xml.Add($"{indent}</{name}>");
				}
				else if (value is Vector3 vector)
				{
					xml.Add($"{indent}<{name}>");
					xml.Add($"{indent}    <x>{vector.x}</x>");
					xml.Add($"{indent}    <y>{vector.y}</y>");
					xml.Add($"{indent}    <z>{vector.z}</z>");
					xml.Add($"{indent}</{name}>");
				}
				else
				{
					xml.Add($"{indent}<{name}>{value}</{name}>");
				}
			}
		}

		public static void SaveToXML(GameObject gameObject)
		{
			if (!(gameObject == null))
			{
				List<string> xml = new List<string>();
				SerializeGameObjectToXml(xml, gameObject);
				WriteXmlToFile(xml);
			}
		}

		[MenuItem("CONTEXT/Transform/Save Fabric Node To XML", false, 656)]
		public static void SerializeToXmlRecursive(MenuCommand command)
		{
			UnityEngine.Component component = command.context as UnityEngine.Component;
			if (!(component == null))
			{
				SaveToXML(component.gameObject);
			}
		}

		[MenuItem("CONTEXT/MonoBehaviour/Save Fabric Node To XML", false, 656)]
		public static void SerializeComponentToXml(MenuCommand command)
		{
			UnityEngine.Component component = command.context as UnityEngine.Component;
			if (!(component == null))
			{
				SaveToXML(component.gameObject);
			}
		}

		private static void WriteXmlToFile(List<string> xml)
		{
			string text = EditorUtility.SaveFilePanel("Save Node XML", "", "", "XML");
			if (text != null && text.Length != 0)
			{
				WriteXmlToFile(xml, text);
			}
		}

		private static void WriteXmlToFile(List<string> xml, string fabricserializationtestXml)
		{
			StreamWriter streamWriter = File.CreateText(fabricserializationtestXml);
			foreach (string item in xml)
			{
				streamWriter.WriteLine(item);
			}
			streamWriter.Dispose();
		}

		private static void SerializeGameObjectToXml(List<string> xml, GameObject gameObject, bool recurse = true, string indent = "")
		{
			xml.Add($"{indent}<GameObject id=\"{gameObject.GetEntityId()}\" name=\"{gameObject.name}\">");
			UnityEngine.Component[] components = gameObject.GetComponents<UnityEngine.Component>();
			foreach (UnityEngine.Component component in components)
			{
				if (component.GetType() != typeof(Transform))
				{
					SerializeComponentToXml(xml, component, indent + "    ");
				}
			}
			if (recurse)
			{
				foreach (object item in gameObject.transform)
				{
					SerializeGameObjectToXml(xml, ((Transform)item).gameObject, recurse, indent + "    ");
				}
			}
			xml.Add($"{indent}</GameObject>");
		}

		private static void SerializeComponentToXml(List<string> xml, UnityEngine.Component component, string indent = "")
		{
			xml.Add($"{indent}<Component type=\"{component.GetType()}\" id=\"{component.GetEntityId()}\">");
			FieldDictEntry fieldDictEntry = new FieldDictEntry();
			foreach (Serialization.IField item in Serialization.EnumerateFields(component))
			{
				fieldDictEntry.Add(item.FieldName, item);
			}
			fieldDictEntry.WriteXml(xml, indent + "    ");
			xml.Add($"{indent}</Component>");
		}
	}
	internal class VRAudioWindow : EditorWindow
	{
		private enum ColumnWidths
		{
			AUDIO_SOURCE_PREFAB_NAME = 200,
			AUDIO_LISTENER_PREFAB_NAME = 200,
			DEFAULT = 70,
			PLATFORM_NAME = 150,
			DELETE = 50
		}

		private Vector2 scrollPos = default(Vector2);

		[MenuItem("Window/Fabric/VRAudio", false, 24)]
		public static void Init()
		{
			VRAudioWindow vRAudioWindow = (VRAudioWindow)EditorWindow.GetWindow(typeof(VRAudioWindow));
			vRAudioWindow.titleContent = new GUIContent("VR Audio");
		}

		private void OnEnable()
		{
		}

		private void OnGUI()
		{
			MenuBar.OnGUI("638651", box: true);
			VRAudioManager vRAudioManager = null;
			if (FabricManager.Instance != null)
			{
				vRAudioManager = FabricManager.Instance._VRAudioManager;
			}
			if (vRAudioManager == null)
			{
				return;
			}
			RuntimePlatform[] array = new RuntimePlatform[vRAudioManager._vrPlatforms.Keys.Count];
			vRAudioManager._vrPlatforms.Keys.CopyTo(array, 0);
			scrollPos = GUILayout.BeginScrollView(scrollPos);
			for (int i = 0; i < vRAudioManager._vrSolutions.Count; i++)
			{
				GUILayout.BeginHorizontal("box");
				VRAudioManager.VRSolution vRSolution = vRAudioManager._vrSolutions[i];
				GUILayout.BeginHorizontal();
				GUILayout.Label("Audio Prefab:", GUILayout.MaxWidth(80f));
				vRSolution._audioSourcePrefab = (GameObject)EditorGUILayout.ObjectField("", vRSolution._audioSourcePrefab, typeof(GameObject), false);
				GUILayout.EndHorizontal();
				if (vRSolution._audioSourcePrefab == null)
				{
					GUI.enabled = false;
				}
				GUILayout.BeginHorizontal();
				GUILayout.Label("Audio Listener Prefab:", GUILayout.MaxWidth(150f));
				vRSolution._audioListenerPrefab = (GameObject)EditorGUILayout.ObjectField("", vRSolution._audioListenerPrefab, typeof(GameObject), false);
				GUILayout.EndHorizontal();
				bool flag = false;
				if (vRAudioManager._defaultVRSolution == i)
				{
					flag = true;
				}
				GUILayout.BeginHorizontal();
				GUILayout.Label("Default:", GUILayout.MaxWidth(50f));
				bool flag2 = EditorGUILayout.Toggle("", flag, GUILayout.MaxWidth(20f));
				GUILayout.EndHorizontal();
				if (flag2 != flag)
				{
					if (flag2)
					{
						vRAudioManager._defaultVRSolution = i;
					}
					else
					{
						vRAudioManager._defaultVRSolution = 0;
					}
				}
				if (array != null)
				{
					for (int j = 0; j < array.Length; j++)
					{
						bool flag3 = false;
						if (vRAudioManager._vrPlatforms.ContainsKey(array[j]))
						{
							GameObject gameObject = null;
							GameObject audioSourcePrefab = vRSolution._audioSourcePrefab;
							if (vRAudioManager._vrPlatforms[array[j]] != null)
							{
								gameObject = vRAudioManager._vrPlatforms[array[j]]._audioSourcePrefab;
							}
							if (gameObject != null && gameObject == audioSourcePrefab)
							{
								flag3 = true;
							}
						}
						bool flag4 = GUILayout.Toggle(flag3, array[j].ToString());
						if (flag4 != flag3 && flag4)
						{
							vRAudioManager._vrPlatforms[array[j]] = vRSolution;
						}
					}
				}
				GUI.enabled = true;
				if (GUILayout.Button("Del", GUILayout.MaxWidth(50f)))
				{
					FabricManager.Instance._VRAudioManager._vrSolutions.Remove(vRSolution);
					vRAudioManager._vrPlatforms.RemoveByValue(vRSolution);
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
			if (GUILayout.Button("Platforms"))
			{
				VRAudioPlatformSelection.Open(UnityEngine.Event.current.mousePosition.x, UnityEngine.Event.current.mousePosition.y, OnVRAudioPlatformClose, array);
			}
			if (GUILayout.Button("Add VR Solution"))
			{
				vRAudioManager._vrSolutions.Add(new VRAudioManager.VRSolution());
			}
		}

		private void OnVRAudioPlatformClose(bool[] runtimePlatforms)
		{
			for (int i = 0; i < runtimePlatforms.Length; i++)
			{
				if (runtimePlatforms[i])
				{
					if (!FabricManager.Instance._VRAudioManager._vrPlatforms.ContainsKey((RuntimePlatform)i))
					{
						FabricManager.Instance._VRAudioManager._vrPlatforms.Add((RuntimePlatform)i, null);
					}
				}
				else if (FabricManager.Instance._VRAudioManager._vrPlatforms.ContainsKey((RuntimePlatform)i))
				{
					FabricManager.Instance._VRAudioManager._vrPlatforms.Remove((RuntimePlatform)i);
				}
			}
			Repaint();
		}
	}
	internal class VRAudioPlatformSelection : EditorWindow
	{
		public delegate void OnCloseHandler(bool[] runtimePlatforms);

		private Vector2 scrollPos = default(Vector2);

		public static VRAudioPlatformSelection window;

		private static bool[] _runtimePlatforms = null;

		private static string[] _runtimePlatformNames = Enum.GetNames(typeof(RuntimePlatform));

		private static event OnCloseHandler OnCloseEvent;

		private void OnEnable()
		{
			_runtimePlatforms = new bool[Enum.GetNames(typeof(RuntimePlatform)).Length];
		}

		private void OnDestroy()
		{
			VRAudioPlatformSelection.OnCloseEvent(_runtimePlatforms);
		}

		public static void Open(float x, float y, OnCloseHandler onClose, RuntimePlatform[] activeRuntimePlatforms)
		{
			if (!(window == null))
			{
				return;
			}
			window = ScriptableObject.CreateInstance<VRAudioPlatformSelection>();
			window.position = new Rect(x, y, 280f, 80f);
			window.Show();
			OnCloseEvent += onClose.Invoke;
			if (activeRuntimePlatforms == null)
			{
				return;
			}
			for (int i = 0; i < activeRuntimePlatforms.Length; i++)
			{
				foreach (RuntimePlatform value in Enum.GetValues(typeof(RuntimePlatform)))
				{
					if (value == activeRuntimePlatforms[i])
					{
						_runtimePlatforms[(int)value] = true;
					}
				}
			}
		}

		private void OnGUI()
		{
			scrollPos = GUILayout.BeginScrollView(scrollPos);
			int num = 0;
			foreach (RuntimePlatform value in Enum.GetValues(typeof(RuntimePlatform)))
			{
				_runtimePlatforms[(int)value] = GUILayout.Toggle(_runtimePlatforms[(int)value], _runtimePlatformNames[num]);
				num++;
			}
			GUILayout.EndScrollView();
			if (GUILayout.Button("Close"))
			{
				Close();
			}
		}
	}
	public class MultiWaveDisplay : EditorWindow
	{
		private Vector2 scrollArea = default(Vector2);

		private List<WaveformDisplay> waveformDisplays = new List<WaveformDisplay>();

		private Component selectedComponent;

		[MenuItem("Window/Fabric/MultiWave Display", false, 15)]
		private static void Init()
		{
			MultiWaveDisplay multiWaveDisplay = (MultiWaveDisplay)EditorWindow.GetWindow(typeof(MultiWaveDisplay));
			multiWaveDisplay.titleContent = new GUIContent("MultiWaveDisplay");
		}

		private void OnEnable()
		{
			EditorApplication.hierarchyChanged -= PlayModeChange;
			EditorApplication.hierarchyChanged += PlayModeChange;
			EditorApplication.projectChanged -= PlayModeChange;
			EditorApplication.projectChanged += PlayModeChange;
			EditorApplication.playModeStateChanged -= PlayModeChange;
			EditorApplication.playModeStateChanged += PlayModeChange;
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(CheckSelection));
		}

		private void PlayModeChange()
		{
			waveformDisplays.Clear();
			selectedComponent = null;
			CheckSelection();
		}

		private void PlayModeChange(PlayModeStateChange state)
		{
			PlayModeChange();
		}

		private void CheckSelection()
		{
			GameObject activeGameObject = Selection.activeGameObject;
			if (activeGameObject != null)
			{
				Component component = activeGameObject.GetComponent<Component>();
				if (selectedComponent != component)
				{
					SelectionChanged(component);
				}
			}
		}

		private void SelectionChanged(Component component)
		{
			if (component != null)
			{
				AudioComponent[] componentsInChildren = component.GetComponentsInChildren<AudioComponent>();
				SetupWaveDisplays(componentsInChildren);
				selectedComponent = component;
				Repaint();
			}
			else
			{
				waveformDisplays.Clear();
			}
		}

		private void SetupWaveDisplays(AudioComponent[] audioComponents)
		{
			waveformDisplays.Clear();
			foreach (AudioComponent audioComponent in audioComponents)
			{
				if (!audioComponent.IsInstance)
				{
					WaveformDisplay waveformDisplay = new WaveformDisplay();
					waveformDisplay.audioComponent = audioComponent;
					waveformDisplay.SetWaveformAudioClip(audioComponent.AudioClip, useWavReader: false);
					waveformDisplays.Add(waveformDisplay);
				}
			}
		}

		private void OnGUI()
		{
			MenuBar.OnGUI("638652", box: true);
			scrollArea = GUILayout.BeginScrollView(scrollArea);
			GUILayout.BeginVertical();
			for (int i = 0; i < waveformDisplays.Count; i++)
			{
				WaveformDisplay waveformDisplay = waveformDisplays[i];
				GUILayout.BeginHorizontal();
				GUILayout.BeginVertical("box");
				GUILayout.Label(waveformDisplay.audioComponent.name, GUILayout.MaxWidth(200f));
				GUILayout.EndVertical();
				waveformDisplay.DrawWaveform(showWaveformOption: false);
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
		}
	}
	public class WaveformAudioClipData
	{
		public int channels = 1;

		public int samples;
	}
	public class WaveformDisplay
	{
		public class ChannelWaveform
		{
			private class WaveformPoint
			{
				public bool valid;

				public Vector2 value;
			}

			private float[] sampleData = new float[0];

			private Color samplePixelColor = new Color(0f, 0f, 0.38f);

			private float samplePixelAlpha = 1.333333f;

			private WaveformPoint[] cachedPoints = new WaveformPoint[0];

			public ChannelWaveform(float[] data)
			{
				sampleData = data;
				cachedPoints = new WaveformPoint[Mathf.CeilToInt((float)sampleData.Length / 2f)];
			}

			public void DrawWaves(Texture2D texture, Rect waveArea)
			{
				int num = (int)(waveArea.height / 2f);
				Vector2 zero = Vector2.zero;
				Vector2 zero2 = Vector2.zero;
				int num2 = 0;
				int num3 = sampleData.Length / (int)waveArea.width;
				for (int i = 0; (float)i < waveArea.width && i * num3 < sampleData.Length; i++)
				{
					WaveformPoint waveformPoint;
					if (cachedPoints[num2 + i] == null)
					{
						waveformPoint = new WaveformPoint();
						cachedPoints[num2 + i] = waveformPoint;
					}
					else
					{
						waveformPoint = cachedPoints[num2 + i];
					}
					if (!waveformPoint.valid)
					{
						float num4 = 1f;
						float num5 = -1f;
						int num6 = i * num3;
						for (int j = 0; j < num3 && num6 + j < sampleData.Length; j++)
						{
							float b = sampleData[num6 + j];
							num4 = Mathf.Min(num4, b);
							num5 = Mathf.Max(num5, b);
						}
						waveformPoint.value.x = waveArea.center.y - num5 * (float)num;
						waveformPoint.value.y = waveArea.center.y - num4 * (float)num;
						waveformPoint.valid = true;
					}
					zero.x = waveArea.x + (float)i;
					zero2.x = zero.x;
					zero.y = waveformPoint.value.x;
					zero2.y = waveformPoint.value.y;
					samplePixelColor.a = samplePixelAlpha;
					Drawing.DrawLine(texture, zero.x, zero.y, zero2.x, zero2.y, samplePixelColor);
				}
			}
		}

		public AudioComponent audioComponent;

		public AudioClip waveformAudioClip;

		public WaveformAudioClipData waveformAudioClipData = new WaveformAudioClipData();

		public string waveformAudioClipName;

		public Texture2D waveformTexture;

		public List<ChannelWaveform> channels = new List<ChannelWaveform>();

		public void SetWaveformAudioClip(AudioClip audioClip, bool useWavReader = true)
		{
			if (audioClip == null || !(waveformAudioClipName != audioClip.name) || !(waveformAudioClip != null) || !FabricEditorData.GetData()._displayAudioClipWaveform)
			{
				return;
			}
			string assetPath = AssetDatabase.GetAssetPath(waveformAudioClip);
			AssetImporter.GetAtPath(assetPath);
			float[] array = null;
			if (useWavReader || audioClip.loadType == AudioClipLoadType.Streaming || audioClip.loadType == AudioClipLoadType.CompressedInMemory)
			{
				WavReader wavReader = new WavReader();
				if (wavReader.LoadFile(assetPath))
				{
					array = wavReader.m_audioData;
					waveformAudioClipData.channels = wavReader.m_waveFormatex.nChannels;
					waveformAudioClipData.samples = array.Length / wavReader.m_waveFormatex.nChannels;
					wavReader.CloseFile();
				}
			}
			else
			{
				if (!audioClip.preloadAudioData)
				{
					audioClip.LoadAudioData();
				}
				if (audioClip.loadState == AudioDataLoadState.Loaded)
				{
					array = new float[audioClip.samples * audioClip.channels];
					audioClip.GetData(array, 0);
				}
				waveformAudioClipData.channels = audioClip.channels;
				waveformAudioClipData.samples = audioClip.samples;
			}
			channels.Clear();
			if (array == null)
			{
				return;
			}
			for (int i = 0; i < waveformAudioClipData.channels; i++)
			{
				float[] array2 = new float[waveformAudioClipData.samples];
				int num = i;
				int num2 = 0;
				while (num < array.Length)
				{
					if (num2 < array2.Length && num < array.Length)
					{
						array2[num2] = array[num];
					}
					num += waveformAudioClipData.channels;
					num2++;
				}
				channels.Add(new ChannelWaveform(array2));
			}
			waveformAudioClip = audioClip;
			waveformAudioClipName = waveformAudioClip.name;
		}

		public void DrawWaveform(bool showWaveformOption = true)
		{
			GUILayout.BeginVertical("box");
			if (showWaveformOption)
			{
				FabricEditorData.GetData()._displayAudioClipWaveform = EditorGUILayout.Toggle("Draw AudioClip Waveform: ", FabricEditorData.GetData()._displayAudioClipWaveform);
			}
			FabricEditorData.ApplyChanges();
			if (!FabricEditorData.GetData()._displayAudioClipWaveform)
			{
				GUILayout.EndVertical();
				return;
			}
			if (audioComponent._dynamicAudioClipLoading && audioComponent._audioClipHandle.IsAudioClipPathSet())
			{
				waveformAudioClip = (AudioClip)Resources.Load(audioComponent._audioClipHandle.GetAudioClipPath(), typeof(AudioClip));
			}
			else
			{
				waveformAudioClip = audioComponent.AudioClip;
			}
			SetWaveformAudioClip(waveformAudioClip, useWavReader: false);
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			Rect lastRect = GUILayoutUtility.GetLastRect();
			lastRect.height = 80f;
			int num = (int)lastRect.width;
			int num2 = (int)lastRect.height;
			if (waveformTexture == null || waveformTexture.width != num || waveformTexture.height != num2)
			{
				waveformTexture = new Texture2D(num, num2, TextureFormat.RGBA32, mipChain: false);
			}
			Color[] array = new Color[num * num2];
			for (int i = 0; i < num * num2; i++)
			{
				ref Color reference = ref array[i];
				reference = new Color(0.5f, 0.2f, 0.5f, 0f);
			}
			waveformTexture.SetPixels(0, 0, num, num2, array);
			if (waveformAudioClip != null)
			{
				float x = 0f;
				float y = 0f;
				float width = lastRect.width;
				float num3 = lastRect.height / (float)channels.Count;
				float num4 = lastRect.width / (float)waveformAudioClipData.samples;
				new Rect(x, y, width, lastRect.height - (float)GUI.skin.box.padding.vertical);
				Rect waveArea = new Rect(x, y, width, num3);
				for (int j = 0; j < channels.Count; j++)
				{
					waveArea.y += (float)j * num3;
					Drawing.DrawLine(waveformTexture, waveArea.x, waveArea.center.y, waveArea.x + waveArea.width, waveArea.center.y, new Color(0f, 0f, 0f));
					channels[j].DrawWaves(waveformTexture, waveArea);
				}
				if (audioComponent.AudioSource != null)
				{
					float num5 = (float)audioComponent.AudioSource.timeSamples * num4;
					Drawing.DrawLine(waveformTexture, (int)num5, 0, (int)num5, (int)lastRect.height, Color.red);
					Drawing.DrawLine(waveformTexture, (int)num5 + 1, 0, (int)num5 + 1, (int)lastRect.height, Color.red);
				}
				for (int k = 0; k < audioComponent._markers.Count; k++)
				{
					Marker marker = audioComponent._markers[k];
					if (marker.type != MarkerType.Ignore)
					{
						float num6 = (float)marker.offsetSamples * num4;
						Drawing.DrawLine(waveformTexture, (int)num6, 0, (int)num6, (int)lastRect.height, Color.white);
					}
				}
				for (int l = 0; l < audioComponent._regions.Count; l++)
				{
					Region region = audioComponent._regions[l];
					float num7 = waveformAudioClip.frequency;
					float num8 = region.offsetTime * num4 * num7;
					float num9 = (region.offsetTime + region.lengthTime) * num4 * num7;
					Drawing.DrawLine(waveformTexture, (int)num8, 0, (int)num8, (int)lastRect.height, Color.magenta);
					Drawing.DrawLine(waveformTexture, (int)num9, 0, (int)num9, (int)lastRect.height, Color.magenta);
				}
				if (audioComponent._loopMarkersLoaded && audioComponent.Loop)
				{
					float num10 = 0f;
					float num11 = (float)waveformAudioClipData.samples * num4;
					if (audioComponent._markers.Count > 1 && audioComponent._useLoopMarkers)
					{
						num10 = (float)audioComponent._markers[audioComponent._startLoopMarkerIndex].offsetSamples * num4;
						num11 = (float)audioComponent._markers[audioComponent._endLoopMarkerIndex].offsetSamples * num4;
					}
					else if (audioComponent._regions.Count > 0)
					{
						float num12 = waveformAudioClip.frequency;
						num10 = audioComponent._regions[audioComponent._loopRegionIndex].offsetTime * num12 * num4;
						num11 = (audioComponent._regions[audioComponent._loopRegionIndex].offsetTime + audioComponent._regions[0].lengthTime) * num4 * num12;
					}
					Drawing.DrawLine(waveformTexture, (int)num10, 0, (int)num10, (int)lastRect.height, Color.green);
					Drawing.DrawLine(waveformTexture, (int)num11, 0, (int)num11, (int)lastRect.height, Color.green);
				}
			}
			waveformTexture.Apply();
			GUIHelpers.DrawLine(Color.green);
			GUILayout.Box(waveformTexture, GUILayout.ExpandWidth(expand: true));
			GUIHelpers.DrawLine(Color.green);
			GUILayout.EndVertical();
		}
	}
	internal class WhatsNew : EditorWindow
	{
		private static string text;

		private Vector2 scrollPos = default(Vector2);

		private static UnityWebRequest whatsNewRequest = null;

		private float timerRepaint = 0.16f;

		[MenuItem("Window/Fabric/Whats New", false, 23)]
		public static void Init()
		{
			GoToWebsite();
		}

		private static void GoToWebsite()
		{
			string url = "http://fabric-manual.com/m/Fabric/l/686014";
			Application.OpenURL(url);
		}

		private static void ShowDialog()
		{
			LoadWhatsNewFile();
			WhatsNew whatsNew = ScriptableObject.CreateInstance<WhatsNew>();
			whatsNew.position = new Rect(Screen.width / 2, Screen.height / 2, 500f, 350f);
			whatsNew.Show();
		}

		private void OnGUI()
		{
			MenuBar.OnGUI("288092-whats-new", box: true);
			scrollPos = GUILayout.BeginScrollView(scrollPos);
			GUILayout.Label(text);
			GUILayout.EndScrollView();
		}

		public void Update()
		{
			if (whatsNewRequest != null && whatsNewRequest.isDone)
			{
				text = string.IsNullOrEmpty(whatsNewRequest.error) ? whatsNewRequest.downloadHandler.text : whatsNewRequest.error;
				Repaint();
				whatsNewRequest.Dispose();
				whatsNewRequest = null;
			}
			if (timerRepaint <= 0f)
			{
				Repaint();
				timerRepaint = 0.16f;
			}
			else
			{
				timerRepaint -= FabricTimer.GetRealtimeDelta();
			}
		}

		private static void LoadWhatsNewFile()
		{
			if (whatsNewRequest != null)
			{
				whatsNewRequest.Dispose();
			}
			text = "";
			whatsNewRequest = UnityWebRequest.Get("https://goo.gl/VC2XIS");
			whatsNewRequest.SendWebRequest();
		}
	}
	[CanEditMultipleObjects]
	[CustomEditor(typeof(WwwAudioComponent))]
	public class WwwAudioComponentEditor : Editor
	{
		private SerializedProperty dontPlay;

		private SerializedProperty delay;

		private SerializedProperty dontStopOnDestroy;

		private SerializedProperty loop;

		private SerializedProperty ignoreVirtualization;

		private SerializedProperty audioClip;

		private SerializedProperty is3D;

		private SerializedProperty isStreaming;

		private SerializedProperty loadOnStart;

		private SerializedProperty ignoreUnloadUnusedAssets;

		private SerializedProperty languageSupported;

		private SerializedProperty audioType;

		private SerializedProperty fileLocation;

		private SerializedProperty audioClipReference;

		private WwwAudioComponent audioComponent;

		private ComponentEditor componentEditor = new ComponentEditor();

		private bool _foldout = true;

		[MenuItem("Fabric/Components/WwwAudioComponent")]
		private static void About()
		{
			GameObject activeGameObject = Selection.activeGameObject;
			if (!(activeGameObject == null))
			{
				GameObject gameObject = new GameObject("WwwAudioComponent");
				gameObject.transform.parent = activeGameObject.transform;
				gameObject.AddComponent<WwwAudioComponent>();
			}
		}

		private void OnEnable()
		{
			audioComponent = base.target as WwwAudioComponent;
			componentEditor.RegisterSerialisableObject(base.serializedObject);
			dontPlay = base.serializedObject.FindProperty("_dontPlay");
			delay = base.serializedObject.FindProperty("_delay");
			dontStopOnDestroy = base.serializedObject.FindProperty("_dontStopOnDestroy");
			loop = base.serializedObject.FindProperty("_loop");
			ignoreVirtualization = base.serializedObject.FindProperty("_ignoreVirtualization");
			audioClip = base.serializedObject.FindProperty("_audioClip");
			ignoreUnloadUnusedAssets = base.serializedObject.FindProperty("_ignoreUnloadUnusedAssets");
			is3D = base.serializedObject.FindProperty("_is3D");
			isStreaming = base.serializedObject.FindProperty("_isStreaming");
			audioType = base.serializedObject.FindProperty("_audioType");
			fileLocation = base.serializedObject.FindProperty("_fileLocation");
			loadOnStart = base.serializedObject.FindProperty("_loadOnStart");
			languageSupported = base.serializedObject.FindProperty("_languageSupported");
			audioClipReference = base.serializedObject.FindProperty("_audioClipReference");
			GUIHelpers.RegisterEditorUpdate(base.Repaint);
		}

		private void OnDisable()
		{
			GUIHelpers.UnregisterEditorUpdate(base.Repaint);
		}

		public override void OnInspectorGUI()
		{
			componentEditor.InspectorGUI(audioComponent, "288076-wwwaudiocomponent");
			base.serializedObject.Update();
			_foldout = EditorGUILayout.Foldout(_foldout, "WWW Audio Component Properties");
			if (_foldout)
			{
				GUILayout.BeginVertical("box");
				EditorGUILayout.PropertyField(ignoreVirtualization, new GUIContent("Ignore Virtualization:"));
				EditorGUILayout.PropertyField(dontPlay, new GUIContent("Dont Play:"));
				EditorGUILayout.Slider(delay, 0f, 600f, new GUIContent("Delay:"));
				EditorGUILayout.PropertyField(dontStopOnDestroy, new GUIContent("Dont Stop On Destroy:"));
				if (audioComponent.DontStopOnDestroy)
				{
					audioComponent.Loop = false;
				}
				else
				{
					EditorGUILayout.PropertyField(loop, new GUIContent("Loop:"));
				}
				GUILayout.Space(5f);
				GUIHelpers.DrawLine(Color.green);
				EditorGUILayout.PropertyField(is3D, new GUIContent("Is 3D:"));
				EditorGUILayout.PropertyField(isStreaming, new GUIContent("Is Streaming:"));
				EditorGUILayout.PropertyField(loadOnStart, new GUIContent("Load On Start:"));
				EditorGUILayout.PropertyField(ignoreUnloadUnusedAssets, new GUIContent("Ignore UnloadUnusedAssets:"));
				EditorGUILayout.PropertyField(languageSupported, new GUIContent("Language Support:"));
				EditorGUILayout.PropertyField(audioType, new GUIContent("Audio Type:"));
				EditorGUILayout.PropertyField(fileLocation, new GUIContent("File Location:"));
				GUILayout.BeginHorizontal();
				EditorGUILayout.PropertyField(audioClipReference, new GUIContent("Audio Clip Reference:"));
				if (fileLocation.enumValueIndex == 0 || fileLocation.enumValueIndex == 1 || fileLocation.enumValueIndex == 2)
				{
					Rect rect = GUILayoutUtility.GetRect(0f, 20f, GUILayout.ExpandWidth(expand: true));
					GUI.Box(rect, "Drop Audio Clip here!!");
					DragAndDropAudioClip(rect);
				}
				GUILayout.EndHorizontal();
				GUIHelpers.DrawLine(Color.green);
				GUILayout.Space(5f);
				GUILayout.Label("Status:                           [ " + audioComponent.CurrentState.ToString() + " ]");
				GUILayout.EndVertical();
				GUIHelpers.CheckGUIHasChanged(base.serializedObject, audioComponent);
			}
		}

		private void DragAndDropAudioClip(Rect drop_area)
		{
			UnityEngine.Event current = UnityEngine.Event.current;
			switch (current.type)
			{
			case EventType.DragUpdated:
			case EventType.DragPerform:
			{
				if (!drop_area.Contains(current.mousePosition))
				{
					break;
				}
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				if (current.type != EventType.DragPerform)
				{
					break;
				}
				DragAndDrop.AcceptDrag();
				UnityEngine.Object obj = DragAndDrop.objectReferences[0];
				if (!(obj != null))
				{
					break;
				}
				AudioClip audioClip = obj as AudioClip;
				if (!(audioClip != null))
				{
					break;
				}
				string assetPath = AssetDatabase.GetAssetPath(audioClip);
				int startIndex = assetPath.IndexOf("Assets/");
				assetPath = assetPath.Remove(startIndex, "Assets/".Length);
				if (fileLocation.enumValueIndex != 0 && fileLocation.enumValueIndex != 1 && fileLocation.enumValueIndex == 2)
				{
					if (!assetPath.Contains("StreamingAssets"))
					{
						EditorUtility.DisplayDialog("Www Audio Component Error", "Audio clip is not located inside the StreamingAssets folder", "Ok");
						break;
					}
					assetPath = assetPath.Remove(assetPath.IndexOf("StreamingAssets/"), "StreamingAssets/".Length);
				}
				if (assetPath.Contains(".wav"))
				{
					audioType.enumValueIndex = GetIndexAudioType(AudioType.WAV);
				}
				else if (assetPath.Contains(".mp3"))
				{
					audioType.enumValueIndex = GetIndexAudioType(AudioType.MPEG);
				}
				else if (assetPath.Contains(".ogg"))
				{
					audioType.enumValueIndex = GetIndexAudioType(AudioType.OGGVORBIS);
				}
				else if (assetPath.Contains(".aif"))
				{
					audioType.enumValueIndex = GetIndexAudioType(AudioType.AIFF);
				}
				else if (assetPath.Contains(".it"))
				{
					audioType.enumValueIndex = GetIndexAudioType(AudioType.IT);
				}
				audioClipReference.stringValue = assetPath;
				GUI.changed = true;
				AudioImporter audioImporter = AssetImporter.GetAtPath("Assets/" + assetPath) as AudioImporter;
				_ = audioImporter != null;
				break;
			}
			}
		}

		private int GetIndexAudioType(AudioType value)
		{
			int num = 0;
			foreach (object value2 in Enum.GetValues(typeof(AudioType)))
			{
				if (((AudioType)value2).Equals(value))
				{
					return num;
				}
				num++;
			}
			return -1;
		}
	}
	[CustomEditor(typeof(DialogAudioComponent))]
	[CanEditMultipleObjects]
	public class DialogAudioComponentEditor : Editor
	{
		private SerializedProperty dontPlay;

		private SerializedProperty delay;

		private SerializedProperty dontStopOnDestroy;

		private SerializedProperty loop;

		private SerializedProperty ignoreVirtualization;

		private SerializedProperty audioClip;

		private SerializedProperty audioClipReference;

		private SerializedProperty dialogAudioLoadedFrom;

		private SerializedProperty dynamicAsyncAudioClipLoading;

		private DialogAudioComponent audioComponent;

		private ComponentEditor componentEditor = new ComponentEditor();

		private bool _foldout = true;

		[MenuItem("Fabric/Components/DialogAudioComponent")]
		private static void About()
		{
			GameObject activeGameObject = Selection.activeGameObject;
			if (!(activeGameObject == null))
			{
				GameObject gameObject = new GameObject("DialogAudioComponent");
				gameObject.transform.parent = activeGameObject.transform;
				gameObject.AddComponent<DialogAudioComponent>();
			}
		}

		private void OnEnable()
		{
			audioComponent = base.target as DialogAudioComponent;
			componentEditor.RegisterSerialisableObject(base.serializedObject);
			dontPlay = base.serializedObject.FindProperty("_dontPlay");
			delay = base.serializedObject.FindProperty("_delay");
			dontStopOnDestroy = base.serializedObject.FindProperty("_dontStopOnDestroy");
			loop = base.serializedObject.FindProperty("_loop");
			ignoreVirtualization = base.serializedObject.FindProperty("_ignoreVirtualization");
			audioClip = base.serializedObject.FindProperty("_audioClip");
			audioClipReference = base.serializedObject.FindProperty("_audioClipReference");
			dialogAudioLoadedFrom = base.serializedObject.FindProperty("_dialogAudioLoadedFrom");
			dynamicAsyncAudioClipLoading = base.serializedObject.FindProperty("_dynamicAsyncAudioClipLoading");
		}

		public override void OnInspectorGUI()
		{
			componentEditor.InspectorGUI(audioComponent, "288075-dialogaudiocomponent");
			base.serializedObject.Update();
			_foldout = EditorGUILayout.Foldout(_foldout, "DialogAudio Component Properties");
			if (_foldout)
			{
				GUILayout.BeginVertical("box");
				EditorGUILayout.PropertyField(ignoreVirtualization, new GUIContent("Ignore Virtualization:"));
				EditorGUILayout.PropertyField(dontPlay, new GUIContent("Dont Play:"));
				EditorGUILayout.Slider(delay, 0f, 600f, new GUIContent("Delay:"));
				EditorGUILayout.PropertyField(dontStopOnDestroy, new GUIContent("Dont Stop On Destroy:"));
				EditorGUILayout.PropertyField(dialogAudioLoadedFrom, new GUIContent("Load Dialog From:"));
				EditorGUILayout.PropertyField(dynamicAsyncAudioClipLoading, new GUIContent("Async Loading: ", "Async loading of audio file, must be located outside the resources folder"));
				if (audioComponent.DontStopOnDestroy)
				{
					audioComponent.Loop = false;
				}
				else
				{
					EditorGUILayout.PropertyField(loop, new GUIContent("Loop:"));
				}
				EditorGUILayout.PropertyField(audioClipReference, new GUIContent("Audio Clip Reference:"));
				GUILayout.Label("Status:                           [ " + audioComponent.CurrentState.ToString() + " ]");
				GUILayout.EndVertical();
				GUIHelpers.CheckGUIHasChanged(base.serializedObject, audioComponent);
			}
		}
	}
	public class CustomPopup
	{
		public interface IReturnList
		{
			string[] GetList();
		}

		public delegate void OnEventHandler();

		private IReturnList returnList;

		private GenericMenu menuToShow;

		private string[] itemList;

		private int selectedItem = -1;

		#pragma warning disable CS0067
		public event OnEventHandler EventNotification;
		#pragma warning restore CS0067

		public void Setup(string[] list)
		{
			if (list.Length == 0)
			{
				list = new string[1] { "" };
			}
			itemList = list;
			menuToShow = new GenericMenu();
			menuToShow.AddItem(new GUIContent("New"), on: false, ItemSelected, 0);
			for (int i = 0; i < itemList.Length; i++)
			{
				menuToShow.AddItem(new GUIContent(itemList[i]), on: false, ItemSelected, i);
			}
		}

		private void ItemSelected(object obj)
		{
			selectedItem = (int)obj;
			menuToShow = null;
		}

		private bool IsPopupListValid()
		{
			if (itemList != null && itemList.Length != 0)
			{
				return true;
			}
			return false;
		}

		public int Popup(int selection)
		{
			if (!IsPopupListValid() && returnList != null)
			{
				Setup(returnList.GetList());
			}
			if (IsPopupListValid())
			{
				if (GUILayout.Button(itemList[selection], "popup") && returnList != null)
				{
					Setup(returnList.GetList());
				}
				if (menuToShow != null)
				{
					menuToShow.ShowAsContext();
					menuToShow = null;
				}
			}
			if (selectedItem != -1)
			{
				selection = selectedItem;
				selectedItem = -1;
			}
			return selection;
		}
	}
	public class GUIHelpers
	{
		public delegate void PreviewerUpdateCallback();

		public static PreviewerUpdateCallback callback;

		private static float timerRepaint = 0f;

		public static bool IsGameObjectPrefab(GameObject go)
		{
			#pragma warning disable CS0618
			PrefabType prefabType = PrefabUtility.GetPrefabType(go);
			if (prefabType != PrefabType.Prefab)
			{
				return false;
			}
			return true;
			#pragma warning restore CS0618
		}

		private static void Update()
		{
			if (callback != null)
			{
				if (timerRepaint <= 0f)
				{
					callback();
					timerRepaint = FabricEditorData.GetData()._previewerGUIUpdateRate;
				}
				else
				{
					timerRepaint -= FabricTimer.GetRealtimeDelta();
				}
			}
		}

		public static void RegisterEditorUpdate(PreviewerUpdateCallback func)
		{
			if (!EditorApplication.isPlaying)
			{
				if (callback == null || callback.GetInvocationList().Length == 0)
				{
					EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(Update));
				}
				callback = (PreviewerUpdateCallback)Delegate.Combine(callback, func);
			}
		}

		public static void UnregisterEditorUpdate(PreviewerUpdateCallback func)
		{
			if (!EditorApplication.isPlaying)
			{
				callback = (PreviewerUpdateCallback)Delegate.Remove(callback, func);
				if (callback == null || callback.GetInvocationList().Length == 0)
				{
					EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(Update));
				}
			}
		}

		public static Texture2D LoadImage(string image)
		{
			string text = FabricManagerEditor.GetFabricEditorPath() + "/";
			return (Texture2D)AssetDatabase.LoadAssetAtPath(text + "Images/" + image + ".png", typeof(Texture2D));
		}

		public static bool CheckGUIHasChanged(SerializedObject serializedObject, Component component)
		{
			if (GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();
				if (component != null)
				{
					component.ApplyPropertiesToInstances();
				}
				SetDirty(component.gameObject);
				return true;
			}
			return false;
		}

		public static bool CheckGUIHasChanged(Component component)
		{
			if (GUI.changed)
			{
				if (component != null)
				{
					component.ApplyPropertiesToInstances();
				}
				SetDirty(component.gameObject);
				return true;
			}
			return false;
		}

		public static bool CheckGUIHasChanged(SerializedObject serializedObject, GameObject gameObject)
		{
			if (CheckGUIHasChanged(gameObject))
			{
				serializedObject.ApplyModifiedProperties();
				return true;
			}
			return false;
		}

		public static bool CheckGUIHasChanged(GameObject gameObject)
		{
			if (GUI.changed)
			{
				SetDirty(gameObject);
				return true;
			}
			return false;
		}

		private static void SetDirty(GameObject gameObject)
		{
			Scene activeScene = SceneManager.GetActiveScene();
			if (!EditorApplication.isPlaying)
			{
				EditorSceneManager.MarkSceneDirty(activeScene);
			}
			EditorUtility.SetDirty(gameObject);
		}

		public static void ApplyChangesToPrefab(GameObject gameObject)
		{
			#pragma warning disable CS0618
			UnityEngine.Object prefabParent = PrefabUtility.GetPrefabParent(gameObject);
			if (!(prefabParent != null))
			{
				return;
			}
			AudioSourcePool componentInChildren = FabricManager.Instance.gameObject.GetComponentInChildren<AudioSourcePool>();
			if (componentInChildren != null)
			{
				UnityEngine.Object.DestroyImmediate(componentInChildren.gameObject);
			}
			switch (PrefabUtility.GetPrefabType(gameObject))
			{
			case PrefabType.Prefab:
				PrefabUtility.ReplacePrefab(prefabParent as GameObject, gameObject, ReplacePrefabOptions.ConnectToPrefab);
				break;
			case PrefabType.PrefabInstance:
			case PrefabType.DisconnectedPrefabInstance:
			{
				GameObject gameObject2 = PrefabUtility.FindRootGameObjectWithSameParentPrefab(gameObject);
				if (gameObject2 != null)
				{
					PrefabUtility.ReplacePrefab(gameObject2, prefabParent, ReplacePrefabOptions.ConnectToPrefab);
				}
				break;
			}
			}
			#pragma warning restore CS0618
			EditorUtility.SetDirty(gameObject);
			AssetDatabase.SaveAssets();
		}

		public static void DrawLine(Color color)
		{
			Color backgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = color;
			GUILayout.Box("", GUILayout.ExpandWidth(expand: true), GUILayout.Height(1f));
			GUI.backgroundColor = backgroundColor;
		}

		public static int Tabs(string[] options, int selected, float width)
		{
			GUILayout.Space(10f);
			Color backgroundColor = GUI.backgroundColor;
			Color color = new Color(0.9f, 0.9f, 0.9f);
			Color color2 = new Color(0.4f, 0.4f, 0.4f);
			GUIStyle gUIStyle = new GUIStyle(GUI.skin.button);
			gUIStyle.padding.bottom = 8;
			GUILayout.BeginHorizontal();
			for (int i = 0; i < options.Length; i++)
			{
				GUI.backgroundColor = ((i == selected) ? color : color2);
				if (GUILayout.Button(options[i], gUIStyle, GUILayout.MaxWidth(width)))
				{
					selected = i;
				}
			}
			GUILayout.EndHorizontal();
			GUI.backgroundColor = backgroundColor;
			return selected;
		}

		public static Component BuildComponentChildrenMenu(Component parentComponent, Component component)
		{
			if (parentComponent == null)
			{
				return null;
			}
			Component[] childComponents = parentComponent.GetChildComponents();
			if (childComponents == null || childComponents.Length == 0)
			{
				return null;
			}
			string[] array = new string[childComponents.Length + 1];
			array[0] = "NONE";
			int selectedIndex = 0;
			for (int i = 0; i < childComponents.Length; i++)
			{
				Component component2 = childComponents[i];
				array[i + 1] = component2.name;
				if (component2 == component)
				{
					selectedIndex = i + 1;
				}
			}
			selectedIndex = EditorGUILayout.Popup(selectedIndex, array);
			if (selectedIndex == 0)
			{
				return null;
			}
			return childComponents[selectedIndex - 1];
		}
	}
	public class FixedWidthLabel : IDisposable
	{
		private readonly ZeroIndent indentReset;

		public FixedWidthLabel(GUIContent label)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(label, GUILayout.Width(GUI.skin.label.CalcSize(label).x + (float)(9 * EditorGUI.indentLevel)));
			indentReset = new ZeroIndent();
		}

		public FixedWidthLabel(string label)
			: this(new GUIContent(label))
		{
		}

		public FixedWidthLabel(GUIContent label, int width)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(label, GUILayout.MaxWidth(width));
			indentReset = new ZeroIndent();
		}

		public FixedWidthLabel(string label, int width)
			: this(new GUIContent(label), width)
		{
		}

		public void Dispose()
		{
			indentReset.Dispose();
			EditorGUILayout.EndHorizontal();
		}
	}
	internal class ZeroIndent : IDisposable
	{
		private readonly int originalIndent;

		public ZeroIndent()
		{
			originalIndent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
		}

		public void Dispose()
		{
			EditorGUI.indentLevel = originalIndent;
		}
	}
	public class DragAndDropArea : Editor
	{
		protected virtual void OnDrop(UnityEngine.Object[] dragged_objects)
		{
		}

		public void DragAndDropAudioClip(float x, float y, string label, bool set)
		{
			Rect rect = GUILayoutUtility.GetRect(x, y, GUILayout.ExpandWidth(expand: true));
			Color backgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = (set ? Color.green : Color.red);
			GUI.Box(rect, label);
			GUI.backgroundColor = backgroundColor;
			UnityEngine.Event current = UnityEngine.Event.current;
			switch (current.type)
			{
			case EventType.DragUpdated:
			case EventType.DragPerform:
				if (!rect.Contains(current.mousePosition))
				{
					break;
				}
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				if (current.type == EventType.DragPerform)
				{
					DragAndDrop.AcceptDrag();
					if (DragAndDrop.objectReferences.Length > 0)
					{
						OnDrop(DragAndDrop.objectReferences);
					}
				}
				break;
			}
		}
	}
	internal class LanguagesWindow : EditorWindow
	{
		private Vector2 scrollPosition;

		private string languageToAdd = "";

		[MenuItem("Window/Fabric/Languages", false, 19)]
		private static void init()
		{
			LanguagesWindow languagesWindow = (LanguagesWindow)EditorWindow.GetWindow(typeof(LanguagesWindow));
			languagesWindow.titleContent = new GUIContent("Languages");
		}

		public void OnGUI()
		{
			FabricManager fabricManager = (FabricManager)UnityEngine.Object.FindAnyObjectByType(typeof(FabricManager));
			if (fabricManager == null)
			{
				return;
			}
			List<LanguageProperties> list = new List<LanguageProperties>();
			MenuBar.OnGUI("288089-languages", box: true);
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, "Box");
			for (int i = 0; i < fabricManager.GetNumLanguages(); i++)
			{
				LanguageProperties languagePropertiesByIndex = fabricManager.GetLanguagePropertiesByIndex(i);
				GUILayout.BeginHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Name:", GUILayout.MaxWidth(40f));
				languagePropertiesByIndex._name = EditorGUILayout.TextField("", languagePropertiesByIndex._name);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Folder:", GUILayout.MaxWidth(40f));
				languagePropertiesByIndex._languageFolder = EditorGUILayout.TextField("", languagePropertiesByIndex._languageFolder);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Prefix:", GUILayout.MaxWidth(40f));
				languagePropertiesByIndex._languagePrefix = EditorGUILayout.TextField("", languagePropertiesByIndex._languagePrefix);
				GUILayout.EndHorizontal();
				bool flag = false;
				if (fabricManager._defaultLanguage == i)
				{
					flag = true;
				}
				GUILayout.BeginHorizontal();
				GUILayout.Label("Default:", GUILayout.MaxWidth(50f));
				bool flag2 = EditorGUILayout.Toggle("", flag, GUILayout.MaxWidth(20f));
				GUILayout.EndHorizontal();
				if (flag2 != flag)
				{
					if (flag2)
					{
						fabricManager._defaultLanguage = i;
					}
					else
					{
						fabricManager._defaultLanguage = -1;
					}
				}
				if (GUILayout.Button("Del"))
				{
					list.Add(languagePropertiesByIndex);
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
			for (int j = 0; j < list.Count; j++)
			{
				fabricManager.RemoveLanguage(list[j]);
			}
			GUILayout.Space(20f);
			GUILayout.BeginHorizontal();
			languageToAdd = GUILayout.TextField(languageToAdd);
			if (GUILayout.Button("Add"))
			{
				fabricManager.AddLanguage(languageToAdd);
			}
			GUILayout.EndHorizontal();
			GUIHelpers.CheckGUIHasChanged(fabricManager.gameObject);
		}
	}
	[CustomEditor(typeof(ChorusFilter))]
	public class ChorusFilterEditor : Editor
	{
		private EditorUndoManager undoManager;

		private ChorusFilter chorusFilter;

		private ComponentEditor componentEditor = new ComponentEditor();

		private void OnEnable()
		{
			chorusFilter = base.target as ChorusFilter;
			undoManager = new EditorUndoManager(chorusFilter, chorusFilter.name);
		}

		public override void OnInspectorGUI()
		{
			MenuBar.OnGUI("290605-chorus", box: true);
			GUILayout.BeginVertical("Box");
			undoManager.CheckUndo();
			GUILayout.BeginHorizontal();
			chorusFilter._dryMix.SetValue(EditorGUILayout.Slider("Dry Mix:", chorusFilter._dryMix.GetTargetValue(), chorusFilter._dryMix.Min, chorusFilter._dryMix.Max));
			GUILayout.Label("(" + chorusFilter._dryMix.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			chorusFilter._wetMix1.SetValue(EditorGUILayout.Slider("Wet Mix1:", chorusFilter._wetMix1.GetTargetValue(), chorusFilter._wetMix1.Min, chorusFilter._wetMix1.Max));
			GUILayout.Label("(" + chorusFilter._wetMix1.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			chorusFilter._wetMix2.SetValue(EditorGUILayout.Slider("Wet Mix2:", chorusFilter._wetMix2.GetTargetValue(), chorusFilter._wetMix2.Min, chorusFilter._wetMix2.Max));
			GUILayout.Label("(" + chorusFilter._wetMix2.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			chorusFilter._wetMix3.SetValue(EditorGUILayout.Slider("Wet Mix3:", chorusFilter._wetMix3.GetTargetValue(), chorusFilter._wetMix3.Min, chorusFilter._wetMix3.Max));
			GUILayout.Label("(" + chorusFilter._wetMix3.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			chorusFilter._delay.SetValue(EditorGUILayout.Slider("Wet Mix3:", chorusFilter._delay.GetTargetValue(), chorusFilter._delay.Min, chorusFilter._delay.Max));
			GUILayout.Label("(" + chorusFilter._delay.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			chorusFilter._rate.SetValue(EditorGUILayout.Slider("Rate:", chorusFilter._rate.GetTargetValue(), chorusFilter._rate.Min, chorusFilter._rate.Max));
			GUILayout.Label("(" + chorusFilter._rate.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			chorusFilter._depth.SetValue(EditorGUILayout.Slider("Depth:", chorusFilter._depth.GetTargetValue(), chorusFilter._depth.Min, chorusFilter._depth.Max));
			GUILayout.Label("(" + chorusFilter._depth.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			chorusFilter._feedback.SetValue(EditorGUILayout.Slider("Feedback:", chorusFilter._feedback.GetTargetValue(), chorusFilter._feedback.Min, chorusFilter._feedback.Max));
			GUILayout.Label("(" + chorusFilter._feedback.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			undoManager.CheckDirty();
			GUILayout.EndVertical();
			GUIHelpers.CheckGUIHasChanged(chorusFilter.gameObject);
		}
	}
	[CustomEditor(typeof(AudioPannerFilter))]
	public class AudioPannerFilterEditor : Editor
	{
		private EditorUndoManager undoManager;

		private AudioPannerFilter audioPannerFilter;

		private ComponentEditor componentEditor = new ComponentEditor();

		private void OnEnable()
		{
			audioPannerFilter = base.target as AudioPannerFilter;
			undoManager = new EditorUndoManager(audioPannerFilter, audioPannerFilter.name);
		}

		public override void OnInspectorGUI()
		{
			MenuBar.OnGUI("290569-audiopanner", box: true);
			GUILayout.BeginVertical("Box");
			undoManager.CheckUndo();
			GUILayout.BeginHorizontal();
			audioPannerFilter._FrontLeftChannel.SetValue(EditorGUILayout.Slider("1 (Front Left):", audioPannerFilter._FrontLeftChannel.GetTargetValue(), audioPannerFilter._FrontLeftChannel.Min, audioPannerFilter._FrontLeftChannel.Max));
			GUILayout.Label("(" + audioPannerFilter._FrontLeftChannel.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			audioPannerFilter._FrontRightChannel.SetValue(EditorGUILayout.Slider("2 (Front Right):", audioPannerFilter._FrontRightChannel.GetTargetValue(), audioPannerFilter._FrontRightChannel.Min, audioPannerFilter._FrontRightChannel.Max));
			GUILayout.Label("(" + audioPannerFilter._FrontRightChannel.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			audioPannerFilter._CenterChannel.SetValue(EditorGUILayout.Slider("3 (Center):", audioPannerFilter._CenterChannel.GetTargetValue(), audioPannerFilter._CenterChannel.Min, audioPannerFilter._CenterChannel.Max));
			GUILayout.Label("(" + audioPannerFilter._CenterChannel.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			audioPannerFilter._RearRightChannel.SetValue(EditorGUILayout.Slider("4 (LFE):", audioPannerFilter._LFEChannel.GetTargetValue(), audioPannerFilter._LFEChannel.Min, audioPannerFilter._LFEChannel.Max));
			GUILayout.Label("(" + audioPannerFilter._LFEChannel.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			audioPannerFilter._RearLeftChannel.SetValue(EditorGUILayout.Slider("5 (Rear Left):", audioPannerFilter._RearLeftChannel.GetTargetValue(), audioPannerFilter._RearLeftChannel.Min, audioPannerFilter._RearLeftChannel.Max));
			GUILayout.Label("(" + audioPannerFilter._RearLeftChannel.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			audioPannerFilter._RearRightChannel.SetValue(EditorGUILayout.Slider("6 (Rear Right):", audioPannerFilter._RearRightChannel.GetTargetValue(), audioPannerFilter._RearRightChannel.Min, audioPannerFilter._RearRightChannel.Max));
			GUILayout.Label("(" + audioPannerFilter._RearRightChannel.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			audioPannerFilter._SideLeftChannel.SetValue(EditorGUILayout.Slider("7 (Side Left):", audioPannerFilter._SideLeftChannel.GetTargetValue(), audioPannerFilter._RearLeftChannel.Min, audioPannerFilter._RearLeftChannel.Max));
			GUILayout.Label("(" + audioPannerFilter._SideLeftChannel.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			audioPannerFilter._SideRightChannel.SetValue(EditorGUILayout.Slider("8 (Side Right):", audioPannerFilter._SideRightChannel.GetTargetValue(), audioPannerFilter._RearRightChannel.Min, audioPannerFilter._RearRightChannel.Max));
			GUILayout.Label("(" + audioPannerFilter._SideRightChannel.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			undoManager.CheckDirty();
			GUILayout.EndVertical();
		}
	}
	[CustomEditor(typeof(EchoFilter))]
	public class EchoFilterEditor : Editor
	{
		private EditorUndoManager undoManager;

		private EchoFilter echoFilter;

		private ComponentEditor componentEditor = new ComponentEditor();

		private void OnEnable()
		{
			echoFilter = base.target as EchoFilter;
			undoManager = new EditorUndoManager(echoFilter, echoFilter.name);
		}

		public override void OnInspectorGUI()
		{
			MenuBar.OnGUI("290607-echo", box: true);
			GUILayout.BeginVertical("Box");
			undoManager.CheckUndo();
			GUILayout.BeginHorizontal();
			echoFilter._delay.SetValue(EditorGUILayout.IntSlider("Delay:", (int)echoFilter._delay.GetTargetValue(), (int)echoFilter._delay.Min, (int)echoFilter._delay.Max));
			GUILayout.Label("(" + (int)echoFilter._delay.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			echoFilter._decayRatio.SetValue(EditorGUILayout.Slider("Decay Ratio:", echoFilter._decayRatio.GetTargetValue(), echoFilter._decayRatio.Min, echoFilter._decayRatio.Max));
			GUILayout.Label("(" + echoFilter._decayRatio.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			echoFilter._dryMix.SetValue(EditorGUILayout.Slider("Dry Mix:", echoFilter._dryMix.GetTargetValue(), echoFilter._dryMix.Min, echoFilter._dryMix.Max));
			GUILayout.Label("(" + echoFilter._dryMix.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			echoFilter._wetMix.SetValue(EditorGUILayout.Slider("Wet Mix:", echoFilter._wetMix.GetTargetValue(), echoFilter._wetMix.Min, echoFilter._wetMix.Max));
			GUILayout.Label("(" + echoFilter._wetMix.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			undoManager.CheckDirty();
			GUILayout.EndVertical();
			GUIHelpers.CheckGUIHasChanged(echoFilter.gameObject);
		}
	}
	[CustomEditor(typeof(DistortionFilter))]
	public class DistortionFilterFilterEditor : Editor
	{
		private EditorUndoManager undoManager;

		private DistortionFilter distortionFilter;

		private ComponentEditor componentEditor = new ComponentEditor();

		private void OnEnable()
		{
			distortionFilter = base.target as DistortionFilter;
			undoManager = new EditorUndoManager(distortionFilter, distortionFilter.name);
		}

		public override void OnInspectorGUI()
		{
			MenuBar.OnGUI("290606-distortion", box: true);
			GUILayout.BeginVertical("Box");
			undoManager.CheckUndo();
			GUILayout.BeginHorizontal();
			distortionFilter._distortionLevel.SetValue(EditorGUILayout.FloatField("Distortion Level:", distortionFilter._distortionLevel.GetTargetValue()));
			GUILayout.Label("(" + distortionFilter._distortionLevel.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			undoManager.CheckDirty();
			GUILayout.EndVertical();
			GUIHelpers.CheckGUIHasChanged(distortionFilter.gameObject);
		}
	}
	[CustomEditor(typeof(HighPassFilter))]
	public class HighPassFilterEditor : Editor
	{
		private EditorUndoManager undoManager;

		private HighPassFilter highPassFilter;

		private ComponentEditor componentEditor = new ComponentEditor();

		private void OnEnable()
		{
			highPassFilter = base.target as HighPassFilter;
			undoManager = new EditorUndoManager(highPassFilter, highPassFilter.name);
		}

		public override void OnInspectorGUI()
		{
			MenuBar.OnGUI("290608-highpass", box: true);
			GUILayout.BeginVertical("Box");
			undoManager.CheckUndo();
			GUILayout.BeginHorizontal();
			highPassFilter._cutoffFrequency.SetValue(EditorGUILayout.IntSlider("Cutoff Frequency:", (int)highPassFilter._cutoffFrequency.GetTargetValue(), (int)highPassFilter._cutoffFrequency.Min, (int)highPassFilter._cutoffFrequency.Max));
			GUILayout.Label("(" + (int)highPassFilter._cutoffFrequency.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			highPassFilter._highpassResonaceQ.SetValue(EditorGUILayout.FloatField("Highpass Resonance Q:", highPassFilter._highpassResonaceQ.GetTargetValue()));
			GUILayout.Label("(" + highPassFilter._highpassResonaceQ.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			undoManager.CheckDirty();
			GUILayout.EndVertical();
			GUIHelpers.CheckGUIHasChanged(highPassFilter.gameObject);
		}
	}
	[CustomEditor(typeof(LowPassFilter))]
	public class LowPassFilterEditor : Editor
	{
		private EditorUndoManager undoManager;

		private LowPassFilter lowPassFilter;

		private ComponentEditor componentEditor = new ComponentEditor();

		private void OnEnable()
		{
			lowPassFilter = base.target as LowPassFilter;
			undoManager = new EditorUndoManager(lowPassFilter, lowPassFilter.name);
		}

		public override void OnInspectorGUI()
		{
			MenuBar.OnGUI("290609-lowpass", box: true);
			GUILayout.BeginVertical("Box");
			undoManager.CheckUndo();
			GUILayout.BeginHorizontal();
			lowPassFilter._cutoffFrequency.SetValue(EditorGUILayout.IntSlider("Cutoff Frequency:", (int)lowPassFilter._cutoffFrequency.GetTargetValue(), (int)lowPassFilter._cutoffFrequency.Min, (int)lowPassFilter._cutoffFrequency.Max));
			GUILayout.Label("(" + (int)lowPassFilter._cutoffFrequency.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			lowPassFilter._lowpassResonaceQ.SetValue(EditorGUILayout.FloatField("Lowpass Resonance Q:", lowPassFilter._lowpassResonaceQ.GetTargetValue()));
			GUILayout.Label("(" + lowPassFilter._lowpassResonaceQ.GetValue() + ")", GUILayout.Width(50f));
			GUILayout.EndHorizontal();
			undoManager.CheckDirty();
			GUILayout.EndVertical();
			GUIHelpers.CheckGUIHasChanged(lowPassFilter.gameObject);
		}
	}
	[CustomEditor(typeof(FabricManager))]
	public class FabricManagerEditor : Editor
	{
		private EditorUndoManager undoManager;

		private FabricManager fabricManager;

		private int selectedMusicSetting;

		private string musicSettingName = "";

		[MenuItem("Fabric/FabricManager")]
		private static void CreateGameObject()
		{
			if (!(GetFabricManager.Instance() != null))
			{
				GameObject gameObject = new GameObject("Audio");
				gameObject.AddComponent<FabricManager>();
				gameObject.AddComponent<EventManager>();
			}
		}

		private void OnEnable()
		{
			fabricManager = base.target as FabricManager;
			undoManager = new EditorUndoManager(fabricManager, fabricManager.name);
		}

		public static FabricManager GetManagerInstance()
		{
			if (FabricManager.Instance != null)
			{
				return FabricManager.Instance;
			}
			if (Selection.activeGameObject != null)
			{
				FabricManager result = null;
				GameObject activeGameObject = Selection.activeGameObject;
				#pragma warning disable CS0618
				PrefabType prefabType = PrefabUtility.GetPrefabType(activeGameObject);
				if (prefabType == PrefabType.Prefab)
				{
					GameObject gameObject = activeGameObject.transform.root.gameObject;
					if (gameObject != null)
					{
						result = (FabricManager.Instance = gameObject.GetComponent<FabricManager>());
					}
				}
				return result;
			}
			return null;
		}

		public static void DeleteAudioSources(GameObject go)
		{
			if (go == null)
			{
				return;
			}
			AudioSource[] componentsInChildren = go.GetComponentsInChildren<AudioSource>(includeInactive: true);
			foreach (AudioSource audioSource in componentsInChildren)
			{
				if (!audioSource.transform.parent.GetComponent<AudioSourcePool>())
				{
					UnityEngine.Object.DestroyImmediate(audioSource.gameObject);
				}
			}
		}

		public override void OnInspectorGUI()
		{
			MenuBar.OnGUI("288078-fabricmanager", box: true, fabricManager.gameObject);
			GetManagerInstance();
			undoManager.CheckUndo();
			GUILayout.BeginVertical("Box");
			fabricManager._dontDestroyOnLoad = EditorGUILayout.Toggle("Dont Destroy OnLoad: ", fabricManager._dontDestroyOnLoad);
			fabricManager._showAllInstances = EditorGUILayout.Toggle("Show All Instances: ", fabricManager._showAllInstances);
			fabricManager._enableVirtualization = EditorGUILayout.Toggle("AudioComponent Virtualization: ", fabricManager._enableVirtualization);
			GUILayout.BeginHorizontal();
			fabricManager._createEventListeners = EditorGUILayout.Toggle("Create Event Listeners: ", fabricManager._createEventListeners);
			if (fabricManager._createEventListeners)
			{
				fabricManager._useFullPathForEventListeners = EditorGUILayout.Toggle("Use full path for name:", fabricManager._useFullPathForEventListeners);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			fabricManager.enableTimelineAssetLoader = EditorGUILayout.Toggle("Timeline Loader: ", fabricManager.enableTimelineAssetLoader);
			if (fabricManager.enableTimelineAssetLoader && GUILayout.Button("Import ftp Project: "))
			{
				string text = EditorUtility.OpenFilePanel("import ftp Project", "", "ftp");
				if (text.Length != 0)
				{
					TimelineLoader.LoadFtpProject(text);
				}
			}
			GUILayout.EndHorizontal();
			int languageByIndex = EditorGUILayout.Popup("Language: ", fabricManager.GetLanguageIndex(), fabricManager.GetLanguageNames());
			fabricManager.SetLanguageByIndex(languageByIndex);
			bool flag = EditorGUILayout.Toggle("Bake Component Inst: ", fabricManager._bakeComponentInstances);
			if (flag != fabricManager._bakeComponentInstances)
			{
				if (flag)
				{
					fabricManager.BakeComponentInstances();
				}
				else
				{
					fabricManager.CleanBakedComponentInstances();
				}
			}
			fabricManager._bakeComponentInstances = flag;
			bool flag2 = EditorGUILayout.Toggle("Playmode Persistence: ", FabricEditorData.GetData()._playmodePersistance);
			if (flag2 != FabricEditorData.GetData()._playmodePersistance)
			{
				FabricEditorData.GetData()._playmodePersistance = flag2;
				FabricEditorData.ApplyChanges();
			}
			FabricEditorData.GetData()._enableHiearchyIcons = EditorGUILayout.Toggle("Show Component Icons: ", FabricEditorData.GetData()._enableHiearchyIcons);
			fabricManager._enableDebugLog = EditorGUILayout.Toggle("Debug Log: ", fabricManager._enableDebugLog);
			fabricManager._allowExternalGroupComponents = EditorGUILayout.Toggle("External Groups: ", fabricManager._allowExternalGroupComponents);
			fabricManager._transitionThreshold = EditorGUILayout.Slider("Transition Threshold (sec): ", (float)fabricManager._transitionThreshold, 0.01f, 1f);
			fabricManager._volumeThreshold = EditorGUILayout.Slider("Volume Threshold: ", (float)fabricManager._volumeThreshold, 0f, 1f);
			fabricManager._onApplicationPauseBehavior = (FabricManager.OnApplicationPauseBehavior)(object)EditorGUILayout.EnumPopup("On Application Pause: ", fabricManager._onApplicationPauseBehavior);
			GUILayout.EndVertical();
			GUILayout.Space(1f);
			GUILayout.BeginVertical("box");
			GUI.enabled = !EditorApplication.isPlaying;
			bool flag3 = EditorGUILayout.Toggle(new GUIContent("Editor Previewer: "), fabricManager._enableEditorPreviewer);
			if (flag3 != fabricManager._enableEditorPreviewer)
			{
				fabricManager._enableEditorPreviewer = flag3;
				ComponentPreviewer.Enable(flag3);
			}
			FabricEditorData.GetData()._previewerGUIUpdateRate = EditorGUILayout.Slider("Previewer Update Rate (ms): ", FabricEditorData.GetData()._previewerGUIUpdateRate, 0.05f, 1f);
			GUI.enabled = true;
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Editor Path: " + fabricManager._fabricEditorPath);
			if (GUILayout.Button("Set Path"))
			{
				fabricManager._fabricEditorPath = SetFabricEditorPath(fabricManager._fabricEditorPath);
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			AddRemoveAudioMixerDebugLogComponents();
			GUILayout.BeginHorizontal("Box");
			fabricManager._forceAxisVector = EditorGUILayout.Vector3Field("Force Axis: ", fabricManager._forceAxisVector);
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
			GUILayout.BeginVertical("Music Time Settings", "Box");
			GUILayout.Space(15f);
			GUILayout.BeginHorizontal();
			musicSettingName = EditorGUILayout.TextField("Name: ", musicSettingName);
			if (GUILayout.Button("Add"))
			{
				MusicTimeSittings musicTimeSittings = new MusicTimeSittings();
				musicTimeSittings._name = musicSettingName;
				fabricManager._musicTimeSignatures.Add(musicTimeSittings);
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
			int count = fabricManager._musicTimeSignatures.Count;
			if (count > 0)
			{
				string[] array = new string[count];
				for (int i = 0; i < count; i++)
				{
					array[i] = fabricManager._musicTimeSignatures[i]._name;
				}
				selectedMusicSetting = EditorGUILayout.Popup("Music Time Settings: ", selectedMusicSetting, array);
				MusicTimeSittings musicTimeSittings2 = fabricManager._musicTimeSignatures[selectedMusicSetting];
				musicTimeSittings2._name = EditorGUILayout.TextField("Name:", musicTimeSittings2._name);
				musicTimeSittings2._bpm = EditorGUILayout.FloatField("Tempo: ", musicTimeSittings2._bpm);
				musicTimeSittings2._syncType = (MusicSyncType)(object)EditorGUILayout.EnumPopup("Sync Type: ", musicTimeSittings2._syncType);
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Time Signature: ", GUILayout.MaxWidth(148f));
				string[] array2 = new string[17];
				for (int j = 1; j < 17; j++)
				{
					array2[j] = j.ToString();
				}
				musicTimeSittings2._timeSignatureLower = EditorGUILayout.Popup(musicTimeSittings2._timeSignatureLower, array2, GUILayout.MaxWidth(30f));
				EditorGUILayout.LabelField(" / ", GUILayout.MaxWidth(25f));
				string[] array3 = new string[3] { "2", "4", "8" };
				int selectedIndex = 0;
				for (int k = 0; k < array3.Length; k++)
				{
					if (array3[k] == musicTimeSittings2._timeSignatureUpper.ToString())
					{
						selectedIndex = k;
						break;
					}
				}
				selectedIndex = EditorGUILayout.Popup(selectedIndex, array3, GUILayout.MaxWidth(30f));
				musicTimeSittings2._timeSignatureUpper = int.Parse(array3[selectedIndex]);
				GUILayout.EndHorizontal();
				GUILayout.Space(5f);
				MusicSyncToTarget(musicTimeSittings2);
				GUILayout.Space(5f);
				if (selectedMusicSetting >= 0 && GUILayout.Button("Delete"))
				{
					fabricManager._musicTimeSignatures.RemoveAt(selectedMusicSetting);
					selectedMusicSetting = 0;
				}
			}
			GUILayout.Space(5f);
			GUILayout.EndVertical();
			GUILayout.Space(5f);
			GUILayout.BeginVertical("AudioSource Pool", "Box");
			GUILayout.Space(20f);
			int num = EditorGUILayout.IntField("Size: ", fabricManager._audioSourcePool);
			if (fabricManager._enableEditorPreviewer)
			{
				bool flag4 = false;
				if (num > 0 && fabricManager.AudioSourcePoolManager == null && !GUIHelpers.IsGameObjectPrefab(FabricManager.Instance.gameObject))
				{
					fabricManager.AudioSourcePoolManager = AudioSourcePool.Create();
					fabricManager.AudioSourcePoolManager.Initialise(num, fabricManager._audioSourcePoolFadeInTime, fabricManager._audioSourcePoolFadeOutTime);
					DeleteAudioSources(fabricManager.gameObject);
					flag4 = true;
				}
				else if (num == 0 && fabricManager.AudioSourcePoolManager != null)
				{
					AudioSourcePool.Destroy();
					flag4 = true;
				}
				else if (num != fabricManager._audioSourcePool)
				{
					if (fabricManager.AudioSourcePoolManager == null)
					{
						fabricManager.AudioSourcePoolManager = AudioSourcePool.Create();
					}
					fabricManager.AudioSourcePoolManager.Resize(num);
					flag4 = true;
				}
				if (flag4)
				{
					fabricManager.RestartPreviewer();
				}
			}
			fabricManager._audioSourcePool = num;
			AudioSourcePoolEditor.DisplayProperties(fabricManager);
			GUILayout.EndVertical();
			GUILayout.Space(5f);
			GUILayout.BeginVertical("Manager Info", "Box");
			GUILayout.Space(10f);
			GUILayout.Label("Total gameObjects used: " + fabricManager._totalNumberOfGameObjectsUsed);
			GUILayout.Space(5f);
			float num2 = (float)fabricManager._totalMemoryUsed / 1024f / 1024f;
			GUILayout.Label("Total memory used: " + num2 + " Mb");
			GUILayout.Space(10f);
			GUILayout.Label("CPU:" + fabricManager.profiler.percent.ToString("0.000") + "% - ms:" + fabricManager.profiler.msPerFrame.ToString("0.000"));
			GUILayout.EndVertical();
			GUIHelpers.CheckGUIHasChanged(fabricManager.gameObject);
			undoManager.CheckDirty();
		}

		private void AddRemoveAudioMixerDebugLogComponents()
		{
			GUILayout.Space(5f);
			GUILayout.BeginVertical("Box");
			GUILayout.BeginHorizontal();
			AudioMixer component = FabricManager.Instance.GetComponent<AudioMixer>();
			if (component == null)
			{
				Color backgroundColor = GUI.backgroundColor;
				GUI.backgroundColor = Color.red;
				if (GUILayout.Button("Add AudioMixer Manager"))
				{
					FabricManager.Instance.gameObject.AddComponent<AudioMixer>();
				}
				GUI.backgroundColor = backgroundColor;
			}
			else
			{
				Color backgroundColor2 = GUI.backgroundColor;
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("Remove AudioMixer Manager"))
				{
					component._destroy = true;
				}
				GUI.backgroundColor = backgroundColor2;
			}
			DebugLog component2 = FabricManager.Instance.GetComponent<DebugLog>();
			if (component2 == null)
			{
				Color backgroundColor3 = GUI.backgroundColor;
				GUI.backgroundColor = Color.red;
				if (GUILayout.Button("Add DebugLog"))
				{
					FabricManager.Instance.gameObject.AddComponent<DebugLog>();
				}
				GUI.backgroundColor = backgroundColor3;
			}
			else
			{
				Color backgroundColor4 = GUI.backgroundColor;
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("Remove DebugLog"))
				{
					component2._destroy = true;
				}
				GUI.backgroundColor = backgroundColor4;
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.Space(5f);
		}

		public static void MusicSyncToTarget(MusicTimeSittings timeSetting)
		{
			if (timeSetting != null)
			{
				GUILayout.BeginVertical();
				GUILayout.BeginHorizontal("Box");
				GUILayout.Label("Sync To Target: ", GUILayout.MaxWidth(100f));
				timeSetting._syncTarget = (Component)EditorGUILayout.ObjectField("", timeSetting._syncTarget, typeof(Component), true);
				GUILayout.EndHorizontal();
				if (timeSetting._syncTarget != null)
				{
					float num = (float)timeSetting._syncTarget.GetCurrentTime();
					float length = timeSetting._syncTarget.GetLength();
					float width = GUILayoutUtility.GetLastRect().width;
					MakeProgressBar("Time: ", width, 15f, num / length * width, Color.green);
				}
				GUILayout.EndVertical();
			}
		}

		private static void MakeProgressBar(string name, float width, float height, float fillWidth, Color col)
		{
			Texture2D texture2D = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, mipChain: false);
			float num = 1f / width;
			for (int i = 0; (float)i < width; i++)
			{
				float num2 = (float)i * num;
				Color color = ((!((float)i < fillWidth)) ? Color.black : ((1f - num2) * Color.green + num2 * Color.yellow));
				for (int j = 0; (float)j < height; j++)
				{
					texture2D.SetPixel(i, j, color);
				}
			}
			texture2D.Apply();
			GUILayout.BeginHorizontal("Box");
			GUILayout.Label(name, GUILayout.MaxWidth(100f));
			GUILayout.Box(texture2D, GUILayout.ExpandWidth(expand: true));
			GUILayout.EndHorizontal();
		}

		public static string SetFabricEditorPath(string path)
		{
			string text = EditorUtility.OpenFolderPanel("Fabric's editor folder ", "", "");
			if (text != null && text.Length > 0)
			{
				path = text;
			}
			else
			{
				EditorUtility.DisplayDialog("Fabric Warning", "A folder not selected, a default one will be created instead [Assets/Fabric/Editor]", "Ok");
			}
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			int num = path.LastIndexOf("Assets/");
			if (num >= 0)
			{
				path = path.Remove(0, num);
			}
			return path;
		}

		public static string GetFabricEditorPath()
		{
			string text = "Assets/Fabric/Editor";
			if (GetFabricManager.Instance() != null)
			{
				text = GetFabricManager.Instance()._fabricEditorPath;
				if (!Directory.Exists(text))
				{
					GetFabricManager.Instance()._fabricEditorPath = SetFabricEditorPath(text);
				}
			}
			return text;
		}
	}
	internal class GlobalUIs
	{
		public static void DrawMusicTimingSettings(MusicTimeSittings musicTimeSettings)
		{
			if (musicTimeSettings == null)
			{
				return;
			}
			if (musicTimeSettings._name.Length > 0)
			{
				musicTimeSettings._name = EditorGUILayout.TextField("Name:", musicTimeSettings._name);
			}
			musicTimeSettings._bpm = EditorGUILayout.FloatField("Tempo: ", musicTimeSettings._bpm);
			musicTimeSettings._syncType = (MusicSyncType)(object)EditorGUILayout.EnumPopup("Sync Type: ", musicTimeSettings._syncType);
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Time Signature: ", GUILayout.MaxWidth(148f));
			string[] array = new string[64];
			for (int i = 1; i < 64; i++)
			{
				array[i] = i.ToString();
			}
			musicTimeSettings._timeSignatureLower = EditorGUILayout.Popup(musicTimeSettings._timeSignatureLower, array, GUILayout.MaxWidth(30f));
			EditorGUILayout.LabelField(" / ", GUILayout.MaxWidth(25f));
			string[] array2 = new string[6] { "1", "2", "4", "8", "16", "32" };
			int selectedIndex = 0;
			for (int j = 0; j < array2.Length; j++)
			{
				if (array2[j] == musicTimeSettings._timeSignatureUpper.ToString())
				{
					selectedIndex = j;
					break;
				}
			}
			selectedIndex = EditorGUILayout.Popup(selectedIndex, array2, GUILayout.MaxWidth(30f));
			musicTimeSettings._timeSignatureUpper = int.Parse(array2[selectedIndex]);
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
		}

		public static int DrawMusicSelection(int selection)
		{
			GUILayout.BeginVertical();
			if (FabricManager.Instance == null)
			{
				GUIStyle gUIStyle = new GUIStyle();
				gUIStyle.normal.textColor = Color.red;
				gUIStyle.alignment = TextAnchor.MiddleCenter;
				GUILayout.Label("Music Time Settings not available, Fabric manager is not present", gUIStyle);
				GUILayout.EndVertical();
				return -1;
			}
			string[] array = new string[FabricManager.Instance._musicTimeSignatures.Count + 1];
			array[0] = "Custom";
			if (FabricManager.Instance != null)
			{
				for (int i = 0; i < FabricManager.Instance._musicTimeSignatures.Count; i++)
				{
					array[i + 1] = FabricManager.Instance._musicTimeSignatures[i]._name;
				}
			}
			selection = EditorGUILayout.Popup("Music Time Setting: ", selection, array);
			if (selection > 0 && selection < array.Length)
			{
				FabricManagerEditor.MusicSyncToTarget(FabricManager.Instance._musicTimeSignatures[selection - 1]);
			}
			GUILayout.EndVertical();
			return selection;
		}

		public static void DrawMusicTimingSettings(string name, ref float bpm, ref int timeSignatureLower, ref int timeSignatureUpper)
		{
			if (name != null)
			{
				name = EditorGUILayout.TextField("Name:", name);
			}
			bpm = EditorGUILayout.FloatField("Tempo: ", bpm);
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Time Signature: ", GUILayout.MaxWidth(148f));
			string[] array = new string[64];
			for (int i = 1; i < 64; i++)
			{
				array[i] = i.ToString();
			}
			timeSignatureLower = EditorGUILayout.Popup(timeSignatureLower, array, GUILayout.MaxWidth(30f));
			EditorGUILayout.LabelField(" / ", GUILayout.MaxWidth(25f));
			string[] array2 = new string[6] { "1", "2", "4", "8", "16", "32" };
			int selectedIndex = 0;
			for (int j = 0; j < array2.Length; j++)
			{
				if (array2[j] == timeSignatureUpper.ToString())
				{
					selectedIndex = j;
					break;
				}
			}
			selectedIndex = EditorGUILayout.Popup(selectedIndex, array2, GUILayout.MaxWidth(30f));
			timeSignatureUpper = int.Parse(array2[selectedIndex]);
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
		}
	}
	[CustomEditor(typeof(IntroLoopOutroComponent))]
	[CanEditMultipleObjects]
	public class IntroLoopOutroComponentComponentEditor : Editor
	{
		private IntroLoopOutroComponent introLoopOutroComponent;

		private ComponentEditor componentEditor = new ComponentEditor();

		private SerializedProperty transitionOffset;

		private SerializedProperty transitionOffsetRandomization;

		private bool _foldout = true;

		[MenuItem("Fabric/Components/IntroLoopOutroComponent")]
		private static void About()
		{
			GameObject activeGameObject = Selection.activeGameObject;
			if (!(activeGameObject == null))
			{
				GameObject gameObject = new GameObject("IntroLoopOutroComponent");
				gameObject.transform.parent = activeGameObject.transform;
				gameObject.AddComponent<IntroLoopOutroComponent>();
			}
		}

		private void OnEnable()
		{
			introLoopOutroComponent = base.target as IntroLoopOutroComponent;
			transitionOffset = base.serializedObject.FindProperty("_transitionOffset");
			transitionOffsetRandomization = base.serializedObject.FindProperty("_transitionOffsetRandomization");
			componentEditor.RegisterSerialisableObject(base.serializedObject);
			GUIHelpers.RegisterEditorUpdate(base.Repaint);
		}

		private void OnDisable()
		{
			GUIHelpers.UnregisterEditorUpdate(base.Repaint);
		}

		public override void OnInspectorGUI()
		{
			componentEditor.InspectorGUI(introLoopOutroComponent, "288047-introloopoutrocomponent");
			base.serializedObject.Update();
			_foldout = EditorGUILayout.Foldout(_foldout, "IntroLoopOutro Component Properties");
			if (_foldout)
			{
				GUILayout.BeginVertical("box");
				EditorGUILayout.Slider(transitionOffset, -60f, 60f, new GUIContent("Transition Offset (sec):"));
				EditorGUILayout.Slider(transitionOffsetRandomization, -60f, 60f, new GUIContent("Transition Rand (sec):"));
				GUILayout.Space(5f);
				introLoopOutroComponent._stages[0] = BuildSelection(IntroLoopOutroComponent.Stage.Intro.ToString(), introLoopOutroComponent._stages[0], introLoopOutroComponent.IsCurrentStageActive(0));
				GUILayout.BeginHorizontal();
				introLoopOutroComponent._stages[1] = BuildSelection(IntroLoopOutroComponent.Stage.Loop.ToString(), introLoopOutroComponent._stages[1], introLoopOutroComponent.IsCurrentStageActive(1));
				introLoopOutroComponent._playLoopToEnd = EditorGUILayout.Toggle("PlayLoopToEnd:", introLoopOutroComponent._playLoopToEnd);
				GUILayout.EndHorizontal();
				introLoopOutroComponent._stages[2] = BuildSelection(IntroLoopOutroComponent.Stage.Outro.ToString(), introLoopOutroComponent._stages[2], introLoopOutroComponent.IsCurrentStageActive(2));
				GUILayout.EndVertical();
			}
			GUIHelpers.CheckGUIHasChanged(base.serializedObject, introLoopOutroComponent);
		}

		private Component BuildSelection(string label, Component component, bool active)
		{
			Component[] childComponents = introLoopOutroComponent.GetChildComponents();
			string[] array = new string[childComponents.Length + 1];
			array[0] = "None";
			int selectedIndex = 0;
			for (int i = 0; i < childComponents.Length; i++)
			{
				array[i + 1] = childComponents[i].name;
				if (childComponents[i] == component)
				{
					selectedIndex = i + 1;
				}
			}
			GUILayout.BeginHorizontal();
			if (active)
			{
				GUILayout.Label(label + ": <--", GUILayout.Width(100f));
			}
			else
			{
				GUILayout.Label(label + ":", GUILayout.Width(100f));
			}
			selectedIndex = EditorGUILayout.Popup(selectedIndex, array);
			GUILayout.EndHorizontal();
			if (selectedIndex == 0)
			{
				return null;
			}
			return childComponents[selectedIndex - 1];
		}
	}
	[CanEditMultipleObjects]
	[CustomEditor(typeof(Preset))]
	public class PresetEditor : Editor
	{
		private Preset presetComponent;

		private EventManager eventManager;

		private SerializedProperty isPersistent;

		private SerializedProperty allowAutoGroupComponentUpdates;

		private SerializedProperty eventName;

		private SerializedProperty includeGameObject;

		private SerializedProperty parentGameObject;

		private SerializedProperty activationMode;

		private void OnEnable()
		{
			eventManager = (EventManager)UnityEngine.Object.FindObjectOfType(typeof(EventManager));
			presetComponent = base.target as Preset;
			isPersistent = base.serializedObject.FindProperty("_isPersistent");
			allowAutoGroupComponentUpdates = base.serializedObject.FindProperty("_allowAutoGroupComponentUpdates");
			eventName = base.serializedObject.FindProperty("_eventName");
			includeGameObject = base.serializedObject.FindProperty("_includeGameObject");
			parentGameObject = base.serializedObject.FindProperty("_parentGameObject");
			activationMode = base.serializedObject.FindProperty("_activationMode");
		}

		public override void OnInspectorGUI()
		{
			MenuBar.OnGUI("288104-preset", box: true);
			GUILayout.BeginVertical("Box");
			using (new FixedWidthLabel("Is Persistent:"))
			{
				EditorGUILayout.PropertyField(isPersistent, new GUIContent(""));
			}
			using (new FixedWidthLabel("Manage GroupComponents:"))
			{
				EditorGUILayout.PropertyField(allowAutoGroupComponentUpdates, new GUIContent(""));
			}
			GUILayout.EndVertical();
			GUILayout.Space(5f);
			GUILayout.BeginVertical("Box");
			EditorGUILayout.PropertyField(activationMode, new GUIContent("Activate On: "));
			if (activationMode.enumValueIndex == 1)
			{
				GUILayout.Space(5f);
				GUILayout.BeginHorizontal();
				GUILayout.Label("Event Name:");
				int num = eventManager.FindEventNameIndexByName(eventName.stringValue);
				string[] array = eventManager._eventList.ToArray();
				int num2 = EditorGUILayout.Popup(num, array, GUILayout.MinWidth(310f), GUILayout.MaxWidth(310f));
				if (num2 != num)
				{
					if (num2 == 0)
					{
						eventName.stringValue = null;
					}
					else
					{
						eventName.stringValue = array[num2];
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				EditorGUILayout.PropertyField(includeGameObject, new GUIContent("GameObject: "));
				if (includeGameObject.boolValue)
				{
					parentGameObject.objectReferenceValue = (GameObject)EditorGUILayout.ObjectField("", presetComponent._parentGameObject, typeof(GameObject), true);
				}
				else
				{
					parentGameObject.objectReferenceValue = null;
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
			if (GUI.changed)
			{
				base.serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(presetComponent);
			}
		}
	}
	[CanEditMultipleObjects]
	[CustomEditor(typeof(GroupPreset))]
	public class GroupPresetEditor : Editor
	{
		private GroupPreset groupPresetComponent;

		private SerializedProperty groupComponent;

		private SerializedProperty volume;

		private SerializedProperty pitch;

		private SerializedProperty fadeInTime;

		private SerializedProperty fadeInCurve;

		private SerializedProperty fadeOutTime;

		private SerializedProperty fadeOutCurve;

		private bool _foldout = true;

		private void OnEnable()
		{
			groupPresetComponent = base.target as GroupPreset;
			groupComponent = base.serializedObject.FindProperty("_groupComponent");
			volume = base.serializedObject.FindProperty("_volume");
			pitch = base.serializedObject.FindProperty("_pitch");
			fadeInTime = base.serializedObject.FindProperty("_fadeInTime");
			fadeInCurve = base.serializedObject.FindProperty("_fadeInCurve");
			fadeOutTime = base.serializedObject.FindProperty("_fadeOutTime");
			fadeOutCurve = base.serializedObject.FindProperty("_fadeOutCurve");
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			MenuBar.OnGUI("288105-grouppreset", box: true);
			_foldout = EditorGUILayout.Foldout(_foldout, "Group Preset Properties");
			if (_foldout)
			{
				groupPresetComponent.GroupComponent = (GroupComponent)EditorGUILayout.ObjectField("Group Component:", groupPresetComponent.GroupComponent, typeof(GroupComponent), true);
				EditorGUILayout.Slider(volume, -96f, 96f, new GUIContent("Volume:"));
				EditorGUILayout.Slider(pitch, -3f, 3f, new GUIContent("Pitch:"));
				EditorGUILayout.Slider(fadeInTime, 0f, 1000f, new GUIContent("FadeInTime:"));
				EditorGUILayout.Slider(fadeInCurve, 0f, 1f, new GUIContent("FadeInCurve:"));
				EditorGUILayout.Slider(fadeOutTime, 0f, 1000f, new GUIContent("FadeOutTime:"));
				EditorGUILayout.Slider(fadeOutCurve, 0f, 1f, new GUIContent("FadeOutCurve:"));
				groupPresetComponent.VolumeParameter = volume.floatValue;
				groupPresetComponent.PitchParameter = pitch.floatValue;
			}
			if (GUI.changed)
			{
				base.serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(groupPresetComponent);
			}
		}
	}
	[InitializeOnLoad]
	public class DynamicMixerHierarchyChangeDetector
	{
		static DynamicMixerHierarchyChangeDetector()
		{
			EditorApplication.hierarchyWindowChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.hierarchyWindowChanged, new EditorApplication.CallbackFunction(HierarchyChangedCallback));
		}

		private static void HierarchyChangedCallback()
		{
			if (EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode || !(GetDynamicMixer.Instance() != null) || !GetDynamicMixer.Instance()._enableHierarchyChangeDetector)
			{
				return;
			}
			FabricManager fabricManager = GetFabricManager.Instance();
			DynamicMixer dynamicMixer = GetDynamicMixer.Instance();
			if (fabricManager == null || dynamicMixer == null)
			{
				return;
			}
			GroupComponent[] componentsInChildren = fabricManager.GetComponentsInChildren<GroupComponent>();
			Preset[] componentsInChildren2 = dynamicMixer.gameObject.GetComponentsInChildren<Preset>();
			foreach (GroupComponent groupComponent in componentsInChildren)
			{
				foreach (Preset preset in componentsInChildren2)
				{
					if (!preset.HasGroupComponent(groupComponent) && preset._allowAutoGroupComponentUpdates)
					{
						preset.AddGroupComponent(groupComponent);
					}
				}
			}
			foreach (Preset preset2 in componentsInChildren2)
			{
				if (!preset2._allowAutoGroupComponentUpdates)
				{
					continue;
				}
				GroupPreset[] componentsInChildren3 = preset2.gameObject.GetComponentsInChildren<GroupPreset>();
				for (int l = 0; l < componentsInChildren3.Length; l++)
				{
					if (componentsInChildren3[l].GroupComponent == null)
					{
						UnityEngine.Object.DestroyImmediate(componentsInChildren3[l]);
					}
				}
			}
		}
	}
	[CustomEditor(typeof(DynamicMixer))]
	public class DynamicMixerEditor : Editor
	{
		private static DynamicMixer _dynamicMixer;

		private new static string name = "";

		private bool _foldout = true;

		private static int index = 0;

		[MenuItem("Fabric/Mixing/DynamicMixer")]
		private static void About()
		{
			GameObject activeGameObject = Selection.activeGameObject;
			if (!(activeGameObject == null))
			{
				GameObject gameObject = new GameObject("DynamicMixer");
				gameObject.transform.parent = activeGameObject.transform;
				gameObject.AddComponent<DynamicMixer>();
			}
		}

		private void OnEnable()
		{
			_dynamicMixer = base.target as DynamicMixer;
		}

		public static Preset DrawDynamicMixerEditor(DynamicMixer dynamicMixer, bool showPresetSelection = false)
		{
			Preset result = null;
			GUILayout.BeginVertical("box");
			_dynamicMixer._enableHierarchyChangeDetector = GUILayout.Toggle(_dynamicMixer._enableHierarchyChangeDetector, "Autodetect hierarchy changes");
			GUILayout.Space(5f);
			Preset[] presets = dynamicMixer.GetPresets();
			int num = presets.Length;
			if (showPresetSelection)
			{
				if (num > 0)
				{
					string[] array = new string[num];
					for (int i = 0; i < num; i++)
					{
						array[i] = presets[i].Name;
					}
					GUILayout.BeginHorizontal();
					GUILayout.Label("Preset to edit:", GUILayout.MinWidth(40f));
					GUILayout.BeginHorizontal("box");
					index = EditorGUILayout.Popup(index, array);
					GUILayout.EndHorizontal();
					GUILayout.EndHorizontal();
					result = presets[index];
				}
				GUILayout.Space(20f);
			}
			if (showPresetSelection)
			{
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.Label("Presets");
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.Space(5f);
			}
			GUILayout.BeginHorizontal();
			GUILayout.Label("Preset to create:", GUILayout.MaxWidth(100f));
			name = GUILayout.TextField(name, 48, GUILayout.MinWidth(80f));
			if (GUILayout.Button("Create") && name.Length > 0)
			{
				result = dynamicMixer.CreatePreset(name);
				name = "";
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Delete", GUILayout.MinWidth(60f), GUILayout.MaxWidth(60f));
			GUILayout.Label("Preset", GUILayout.MinWidth(80f));
			GUILayout.Label("Action", GUILayout.MinWidth(100f), GUILayout.MaxWidth(100f));
			if (showPresetSelection)
			{
				GUILayout.Label("Progress", GUILayout.Width(150f));
			}
			GUILayout.EndHorizontal();
			List<Preset> list = new List<Preset>();
			for (int j = 0; j < num; j++)
			{
				GUILayout.BeginVertical();
				Preset preset = presets[j];
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Del", GUILayout.MinWidth(60f), GUILayout.MaxWidth(60f)))
				{
					list.Add(preset);
				}
				preset.name = GUILayout.TextField(preset.name, 100, GUILayout.MinWidth(80f));
				bool flag = false;
				bool flag2 = false;
				if (preset.IsActive())
				{
					flag2 = GUILayout.Button("RemovePreset", GUILayout.MinWidth(100f), GUILayout.MaxWidth(100f));
				}
				else
				{
					flag = GUILayout.Button("AddPreset", GUILayout.MinWidth(100f), GUILayout.MaxWidth(100f));
				}
				if (flag)
				{
					dynamicMixer.AddPreset(preset);
				}
				if (flag2)
				{
					dynamicMixer.RemovePreset(preset);
				}
				if (showPresetSelection)
				{
					GUILayout.BeginHorizontal(GUILayout.Width(150f));
					GUILayout.Box(GUIContent.none, GUI.skin.box, GUILayout.MinHeight(5f), GUILayout.MaxHeight(5f), GUILayout.Width(150f * preset.GetProgress()));
					GUILayout.EndHorizontal();
				}
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}
			GUILayout.Space(10f);
			GUILayout.BeginVertical("Box");
			GUILayout.Space(5f);
			GUILayout.Label("Current switched preset: " + _dynamicMixer._currentSwitchedPreset);
			GUILayout.Space(5f);
			GUILayout.Label("Active Presets");
			for (int k = 0; k < _dynamicMixer.GetNumActivePresets(); k++)
			{
				Preset activePresetByIndex = _dynamicMixer.GetActivePresetByIndex(k);
				if (activePresetByIndex != null)
				{
					GUILayout.Label("|-- " + _dynamicMixer.GetActivePresetByIndex(k).Name);
				}
			}
			GUILayout.EndVertical();
			if (list.Count > 0)
			{
				result = null;
				index = 0;
				for (int l = 0; l < list.Count; l++)
				{
					dynamicMixer.DeletePreset(list[l]);
				}
			}
			GUILayout.EndVertical();
			return result;
		}

		public override void OnInspectorGUI()
		{
			MenuBar.OnGUI("288103-dynamicmixer", box: true);
			_foldout = EditorGUILayout.Foldout(_foldout, "DynamicMixer Presets");
			if (_foldout)
			{
				DrawDynamicMixerEditor(_dynamicMixer);
			}
		}
	}
	[CustomEditor(typeof(EventListener))]
	public class EventListenerEditor : Editor
	{
		private EventListener eventListener;

		private EventManager eventManager;

		private EventListComponent[] eventListComponents;

		private void OnEnable()
		{
			eventListener = base.target as EventListener;
			eventManager = EventManager.Instance;
			eventListComponents = UnityEngine.Object.FindObjectsOfType(typeof(EventListComponent)) as EventListComponent[];
		}

		public override void OnInspectorGUI()
		{
			MenuBar.OnGUI("288096-eventlistener", box: true);
			GUILayout.BeginVertical("box");
			eventListener._eventName = EventManagerEditor.BuildEventName(eventListener._eventName, EventManager.Instance, eventListComponents);
			GUILayout.Space(10f);
			eventListener._overrideEventTriggerAction = GUILayout.Toggle(eventListener._overrideEventTriggerAction, "Override Event Action");
			if (eventListener._overrideEventTriggerAction)
			{
				if (eventListener._overrideParameters == null)
				{
					eventListener._overrideParameters = new OverrideParameters();
				}
				GUILayout.BeginVertical("Box");
				eventListener._overrideParameters._overrideIncomingEventAction = GUILayout.Toggle(eventListener._overrideParameters._overrideIncomingEventAction, "Override On Specific Action");
				if (eventListener._overrideParameters._overrideIncomingEventAction)
				{
					eventListener._overrideParameters._incomingEventAction = (EventAction)(object)EditorGUILayout.EnumPopup("Old Event Action:", eventListener._overrideParameters._incomingEventAction);
				}
				GUILayout.EndVertical();
				GUILayout.Space(5f);
				GUILayout.BeginVertical("Box");
				eventListener._overrideParameters._overrideEventAction = (EventAction)(object)EditorGUILayout.EnumPopup("New Event Action:", eventListener._overrideParameters._overrideEventAction);
				OverrideEventActionProperties(eventListener);
				GUILayout.EndVertical();
				GUIHelpers.CheckGUIHasChanged(eventListener.gameObject);
			}
			GUILayout.EndVertical();
		}

		public static void OverrideEventActionProperties(EventListener eventListener)
		{
			if (eventListener._overrideParameters._overrideEventAction == EventAction.SetVolume)
			{
				eventListener._overrideParameters.FloatParameter = ComponentEditor.DisplayDecibelsVolumeSlider("Volume:", eventListener._overrideParameters.FloatParameter);
			}
			else if (eventListener._overrideParameters._overrideEventAction == EventAction.SetPitch)
			{
				eventListener._overrideParameters.FloatParameter = EditorGUILayout.Slider("Pitch:", eventListener._overrideParameters.FloatParameter, -5f, 5f);
			}
			else if (eventListener._overrideParameters._overrideEventAction == EventAction.SetPan)
			{
				eventListener._overrideParameters.FloatParameter = EditorGUILayout.Slider("Pan:", eventListener._overrideParameters.FloatParameter, -1f, 1f);
			}
			else if (eventListener._overrideParameters._overrideEventAction == EventAction.SetTime)
			{
				eventListener._overrideParameters.FloatParameter = EditorGUILayout.Slider("Time:", eventListener._overrideParameters.FloatParameter, -1000f, 1000f);
			}
			else if (eventListener._overrideParameters._overrideEventAction == EventAction.SetSwitch)
			{
				eventListener._overrideParameters.StringParameter = EditorGUILayout.TextField("SwitchTo:", eventListener._overrideParameters.StringParameter);
			}
			else if (eventListener._overrideParameters._overrideEventAction == EventAction.AddPreset || eventListener._overrideParameters._overrideEventAction == EventAction.RemovePreset)
			{
				eventListener._overrideParameters.StringParameter = EditorGUILayout.TextField("Preset:", eventListener._overrideParameters.StringParameter);
			}
			else if (eventListener._overrideParameters._overrideEventAction == EventAction.SwitchPreset)
			{
				if (eventListener._overrideParameters.SwitchPresetData == null)
				{
					eventListener._overrideParameters.SwitchPresetData = new SwitchPresetData();
				}
				eventListener._overrideParameters.SwitchPresetData._targetPreset = EditorGUILayout.TextField("SwitchTo:", eventListener._overrideParameters.SwitchPresetData._targetPreset);
			}
			else if (eventListener._overrideParameters._overrideEventAction == EventAction.TransitionToSnapshot)
			{
				if (eventListener._overrideParameters.TransitionToSnapshotData == null)
				{
					eventListener._overrideParameters.TransitionToSnapshotData = new TransitionToSnapshotData();
				}
				eventListener._overrideParameters.TransitionToSnapshotData._snapshot = EditorGUILayout.TextField("Transition To Snapshot: ", eventListener._overrideParameters.TransitionToSnapshotData._snapshot);
				eventListener._overrideParameters.TransitionToSnapshotData._timeToReach = EditorGUILayout.FloatField("Time To Reach: ", eventListener._overrideParameters.TransitionToSnapshotData._timeToReach);
			}
			else if (eventListener._overrideParameters._overrideEventAction == EventAction.SetDSPParameter)
			{
				if (eventListener._overrideParameters.DSPParameterData == null)
				{
					eventListener._overrideParameters.DSPParameterData = new DSPParameterData();
				}
				eventListener._overrideParameters.DSPParameterData = EventTriggerEditor.DSPParameterDataUI(eventListener._overrideParameters.DSPParameterData);
			}
			else if (eventListener._overrideParameters._overrideEventAction == EventAction.SetMarker)
			{
				eventListener._overrideParameters.StringParameter = EditorGUILayout.TextField(new GUIContent("Label"), eventListener._overrideParameters.StringParameter);
			}
			else
			{
				EditorGUILayout.LabelField("No override parameter data available for this event action");
			}
		}
	}
	[CustomEditor(typeof(EventManager))]
	public class EventManagerEditor : Editor
	{
		private EditorUndoManager undoManager;

		private EventManager eventManager;

		private int menuListIndex;

		private string _eventName = "";

		public static bool IsNullOrWhiteSpace(string value)
		{
			if (value == null)
			{
				return true;
			}
			for (int i = 0; i < value.Length; i++)
			{
				if (!char.IsWhiteSpace(value[i]))
				{
					return false;
				}
			}
			return true;
		}

		private void OnEnable()
		{
			eventManager = base.target as EventManager;
			undoManager = new EditorUndoManager(eventManager, eventManager.name);
		}

		private void DrawEventListMenu()
		{
			eventManager._eventMenuListFoldout = EditorGUILayout.Foldout(eventManager._eventMenuListFoldout, "Event Menu List");
			if (!eventManager._eventMenuListFoldout)
			{
				return;
			}
			GUILayout.BeginVertical("Box");
			if (eventManager._eventList.Count == 0)
			{
				eventManager._eventList.Add("_UnSet_");
			}
			GUILayout.BeginHorizontal();
			bool flag = GUILayout.Button("Add");
			GUILayout.Label("Event:");
			_eventName = EditorGUILayout.TextField("", _eventName, GUILayout.MinWidth(280f), GUILayout.MaxWidth(280f));
			GUILayout.EndHorizontal();
			if (flag && !IsNullOrWhiteSpace(_eventName))
			{
				_eventName.Trim();
				eventManager._eventList.Add(_eventName);
				_eventName = "";
				eventManager._eventList.Sort();
				Repaint();
			}
			EditorGUILayout.Space();
			string[] array = eventManager._eventList.ToArray();
			menuListIndex = EditorGUILayout.Popup(menuListIndex, array);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Del") && menuListIndex > 0)
			{
				eventManager._eventList.Remove(array[menuListIndex]);
				RenameEvent(array[menuListIndex], "");
				array = eventManager._eventList.ToArray();
				menuListIndex--;
			}
			if (array != null && array.Length > 0)
			{
				if (eventManager._eventList[0] == "UnSet")
				{
					eventManager._eventList[0] = "_UnSet_";
				}
				GUILayout.Label("Event:");
				string text = array[menuListIndex];
				text = EditorGUILayout.TextField(text, GUILayout.MinWidth(280f), GUILayout.MaxWidth(280f));
				if (text != eventManager._eventList[menuListIndex] && menuListIndex != 0)
				{
					RenameEvent(eventManager._eventList[menuListIndex], text);
					eventManager._eventList[menuListIndex] = text;
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
			GUILayout.BeginVertical("Box");
			GUILayout.BeginHorizontal();
			GameObject[] array2 = null;
			if (GUILayout.Button("Locate EventListeners"))
			{
				array2 = FindEventListeners(array[menuListIndex]);
			}
			if (GUILayout.Button("Locate EventTriggers"))
			{
				array2 = FindEventTriggers(array[menuListIndex]);
			}
			if (array2 != null && array2.Length > 0)
			{
				Selection.objects = array2;
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.Space(10f);
			GUILayout.BeginVertical("Box");
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Clear Event List") && EditorUtility.DisplayDialog("Event Manager List", "You are about to clear the entire event list, Are you sure you want to do this?!!", "Ok", "Cancel"))
			{
				ClearEventList();
			}
			if (GUILayout.Button("Clear All") && EditorUtility.DisplayDialog("Event Manager List", "You are about to clear the entire event list, event listeners and event triggers, Are you sure you want to do this?!!", "Ok", "Cancel"))
			{
				ClearAll();
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Load Events From File"))
			{
				string text2 = EditorUtility.OpenFilePanel("Load Event Names from txt file", "", "txt");
				if (text2 != null && text2.Length > 0)
				{
					string[] files = Directory.GetFiles(text2);
					for (int i = 0; i < files.Length; i++)
					{
						eventManager.AddEventNamesFromFile(files[i]);
						menuListIndex = 0;
					}
				}
			}
			if (GUILayout.Button("Save Events To File"))
			{
				string text3 = EditorUtility.SaveFilePanel("Save Event List to a txt file", "", "EventList.txt", "txt");
				if (text3 != null && text3.Length != 0)
				{
					eventManager.ExportEventNamesToFile(text3);
				}
			}
			if (GUILayout.Button("Unload Events From File"))
			{
				string text4 = EditorUtility.OpenFilePanel("Unload Event Names From txt file", "", "txt");
				if (text4 != null && text4.Length > 0)
				{
					string[] files2 = Directory.GetFiles(text4);
					for (int j = 0; j < files2.Length; j++)
					{
						eventManager.RemoveEventNamesFromFile(files2[j]);
						menuListIndex = 0;
					}
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.EndVertical();
		}

		public static GameObject[] FindEventListeners(string eventName)
		{
			List<GameObject> list = new List<GameObject>();
			EventListener[] array = UnityEngine.Object.FindObjectsByType(typeof(EventListener), FindObjectsSortMode.None) as EventListener[];
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i]._eventName == eventName || (array[i]._eventName.Length == 0 && eventName == "_UnSet_"))
				{
					list.Add(array[i].gameObject);
				}
			}
			return list.ToArray();
		}

		public static GameObject[] FindEventTriggers(string eventName)
		{
			List<GameObject> list = new List<GameObject>();
			UnityEngine.Object[] array = UnityEngine.Object.FindObjectsByType(typeof(GameObject), FindObjectsSortMode.None);
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = (GameObject)array[i];
				if (!(gameObject.transform.parent == null))
				{
					continue;
				}
				EventTrigger[] componentsInChildren = gameObject.GetComponentsInChildren<EventTrigger>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					if (componentsInChildren[j]._eventName == eventName || (componentsInChildren[j]._eventName.Length == 0 && eventName == "_UnSet_"))
					{
						list.Add(componentsInChildren[j].gameObject);
					}
				}
			}
			return list.ToArray();
		}

		private void DrawEventList()
		{
			if (eventManager._eventList.Count == 0)
			{
				eventManager._eventList.Add("_UnSet_");
			}
			GUILayout.BeginHorizontal();
			bool flag = GUILayout.Button("Add");
			GUILayout.Label("Event:");
			_eventName = EditorGUILayout.TextField("", _eventName, GUILayout.MinWidth(280f), GUILayout.MaxWidth(280f));
			GUILayout.EndHorizontal();
			if (flag && !IsNullOrWhiteSpace(_eventName))
			{
				_eventName.Trim();
				eventManager._eventList.Add(_eventName);
				_eventName = "";
				eventManager._eventList.Sort();
				Repaint();
			}
			EditorGUILayout.Space();
			List<string> list = new List<string>();
			for (int i = 1; i < eventManager._eventList.Count; i++)
			{
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Del"))
				{
					list.Add(eventManager._eventList[i]);
				}
				GUILayout.Label("Event:");
				string text = eventManager._eventList[i];
				text = EditorGUILayout.TextField(text, GUILayout.MinWidth(280f), GUILayout.MaxWidth(280f));
				if (text != eventManager._eventList[i])
				{
					RenameEvent(eventManager._eventList[i], text);
					eventManager._eventList[i] = text;
				}
				GUILayout.EndHorizontal();
			}
			for (int j = 0; j < list.Count; j++)
			{
				string text2 = list[j];
				eventManager._eventList.Remove(text2);
				RenameEvent(text2, "");
			}
		}

		public static string BuildEventName(string eventName, EventManager eventManager, EventListComponent[] eventListComponents, GroupComponent[] externalGroupComponents = null)
		{
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Event Name:");
			if (eventName == "DynamicMixer" || eventName == "AudioMixer")
			{
				eventName = "_UnSet_";
			}
			List<string> list = new List<string>();
			list.Add("_UnSet_");
			if (eventManager != null)
			{
				if (eventManager._eventList[0] == "UnSet")
				{
					eventManager._eventList[0] = "_UnSet_";
				}
				list = new List<string>(eventManager._eventList.ToArray());
			}
			if (eventListComponents != null)
			{
				for (int i = 0; i < eventListComponents.Length; i++)
				{
					for (int j = 0; j < eventListComponents[i]._eventList.Count; j++)
					{
						list.Add(eventListComponents[i]._eventList[j]);
					}
				}
			}
			if (externalGroupComponents != null)
			{
				foreach (GroupComponent groupComponent in externalGroupComponents)
				{
					if (!groupComponent.IsFabricHierarchyPresent())
					{
						for (int l = 0; l < groupComponent._eventEditor._eventNames.Count; l++)
						{
							list.Add(groupComponent._eventEditor._eventNames[l]);
						}
					}
				}
			}
			int num = -1;
			for (int m = 0; m < list.Count; m++)
			{
				if (list[m] == eventName)
				{
					num = m;
					break;
				}
			}
			if (num >= 0)
			{
				string[] array = list.ToArray();
				int num2 = EditorGUILayout.Popup(num, array);
				if (num2 != num)
				{
					eventName = array[num2];
				}
			}
			else
			{
				Color backgroundColor = GUI.backgroundColor;
				GUI.backgroundColor = Color.green;
				GUILayout.Label(eventName, "Box", GUILayout.ExpandWidth(expand: true));
				GUI.backgroundColor = backgroundColor;
				if (GUILayout.Button("Clear", GUILayout.MaxWidth(45f)))
				{
					eventName = "_UnSet_";
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.Space(5f);
			return eventName;
		}

		public static void RenameEvent(string eventName, string newEventName)
		{
			if (FabricManager.IsInitialised())
			{
				EventListener[] componentsInChildren = FabricManager.Instance.gameObject.GetComponentsInChildren<EventListener>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i]._eventName == eventName)
					{
						componentsInChildren[i]._eventName = newEventName;
					}
				}
			}
			new List<GameObject>();
			UnityEngine.Object[] array = UnityEngine.Object.FindObjectsByType(typeof(GameObject), FindObjectsSortMode.None);
			for (int j = 0; j < array.Length; j++)
			{
				GameObject gameObject = (GameObject)array[j];
				if (!(gameObject.transform.parent == null))
				{
					continue;
				}
				EventTrigger[] componentsInChildren2 = gameObject.GetComponentsInChildren<EventTrigger>();
				for (int k = 0; k < componentsInChildren2.Length; k++)
				{
					if (componentsInChildren2[k]._eventName == eventName)
					{
						componentsInChildren2[k]._eventName = newEventName;
					}
				}
			}
		}

		private void ClearAll()
		{
			if (FabricManager.IsInitialised())
			{
				EventListener[] componentsInChildren = FabricManager.Instance.gameObject.GetComponentsInChildren<EventListener>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i]._eventName = "_UnSet_";
				}
			}
			new List<GameObject>();
			UnityEngine.Object[] array = UnityEngine.Object.FindObjectsByType(typeof(GameObject), FindObjectsSortMode.None);
			for (int j = 0; j < array.Length; j++)
			{
				GameObject gameObject = (GameObject)array[j];
				if (gameObject.transform.parent == null)
				{
					EventTrigger[] componentsInChildren2 = gameObject.GetComponentsInChildren<EventTrigger>();
					for (int k = 0; k < componentsInChildren2.Length; k++)
					{
						componentsInChildren2[k]._eventName = "_UnSet_";
					}
				}
			}
			ClearEventList();
		}

		private void ClearEventList()
		{
			eventManager._eventList.Clear();
		}

		public override void OnInspectorGUI()
		{
			undoManager.CheckUndo();
			MenuBar.OnGUI("288079-eventmanager", box: true);
			if (!(eventManager == null))
			{
				GUILayout.BeginVertical("box");
				FabricEditorData.GetData()._logHistorySize = EditorGUILayout.IntField("Log History:", FabricEditorData.GetData()._logHistorySize);
				eventManager._forceQueueAllEvents = EditorGUILayout.Toggle("Force Queue All Events:", eventManager._forceQueueAllEvents);
				GUILayout.BeginHorizontal();
				GUILayout.Label("Num of Events in Queue: " + eventManager.NumOfEventsInQueue());
				GUILayout.EndHorizontal();
				GUILayout.Space(10f);
				GUILayout.Label("CPU:" + eventManager.profiler.percent.ToString("0.000") + "% - ms:" + eventManager.profiler.msPerFrame.ToString("0.000"));
				GUILayout.EndVertical();
				DrawEventListMenu();
				GUILayout.Space(10f);
				eventManager._eventListFoldout = EditorGUILayout.Foldout(eventManager._eventListFoldout, "Event List");
				if (eventManager._eventListFoldout)
				{
					GUILayout.BeginVertical("box");
					DrawEventList();
					GUILayout.EndVertical();
				}
				GUIHelpers.CheckGUIHasChanged(eventManager.gameObject);
				undoManager.CheckDirty();
			}
		}

		public static string DropDownEventNames(string eventName, bool collectExternalGroupComponents = false)
		{
			EventListComponent[] eventListComponents = UnityEngine.Object.FindObjectsByType(typeof(EventListComponent), FindObjectsSortMode.None) as EventListComponent[];
			List<GroupComponent> list = null;
			if (collectExternalGroupComponents)
			{
				list = new List<GroupComponent>();
				GroupComponent[] array = UnityEngine.Object.FindObjectsByType(typeof(GroupComponent), FindObjectsSortMode.None) as GroupComponent[];
				GroupComponent[] array2 = array;
				foreach (GroupComponent groupComponent in array2)
				{
					if (!groupComponent.IsFabricHierarchyPresent())
					{
						list.Add(groupComponent);
					}
				}
				return BuildEventName(eventName, EventManager.Instance, eventListComponents, list.ToArray());
			}
			return BuildEventName(eventName, EventManager.Instance, eventListComponents);
		}
	}
	public struct DSPParameterUI
	{
		public string _parameter;

		public float _min;

		public float _max;
	}
	public struct DSPParameterInfoUI
	{
		public DSPType _type;

		public DSPParameterUI[] dspParameters;

		public string _name;
	}
	[CanEditMultipleObjects]
	[CustomEditor(typeof(EventTrigger))]
	public class EventTriggerEditor : Editor
	{
		public static DSPParameterInfoUI[] _dspParameterInfo = new DSPParameterInfoUI[8]
		{
			new DSPParameterInfoUI
			{
				_type = DSPType.LowPass,
				dspParameters = new DSPParameterUI[2]
				{
					new DSPParameterUI
					{
						_parameter = "CutoffFrequency",
						_min = 0f,
						_max = 22000f
					},
					new DSPParameterUI
					{
						_parameter = "LowpassResonaceQ",
						_min = 0f,
						_max = 5f
					}
				}
			},
			new DSPParameterInfoUI
			{
				_type = DSPType.HighPass,
				dspParameters = new DSPParameterUI[2]
				{
					new DSPParameterUI
					{
						_parameter = "CutoffFrequency",
						_min = 0f,
						_max = 22000f
					},
					new DSPParameterUI
					{
						_parameter = "HighpassResonaceQ",
						_min = 0f,
						_max = 5f
					}
				}
			},
			new DSPParameterInfoUI
			{
				_type = DSPType.HighPass,
				dspParameters = new DSPParameterUI[4]
				{
					new DSPParameterUI
					{
						_parameter = "Delay",
						_min = 10f,
						_max = 5000f
					},
					new DSPParameterUI
					{
						_parameter = "DecayRatio",
						_min = 0f,
						_max = 1f
					},
					new DSPParameterUI
					{
						_parameter = "DryMix",
						_min = 0f,
						_max = 1f
					},
					new DSPParameterUI
					{
						_parameter = "WetMix",
						_min = 0f,
						_max = 1f
					}
				}
			},
			new DSPParameterInfoUI
			{
				_type = DSPType.Distorion,
				dspParameters = new DSPParameterUI[1]
				{
					new DSPParameterUI
					{
						_parameter = "DistortionLevel",
						_min = 0f,
						_max = 1f
					}
				}
			},
			new DSPParameterInfoUI
			{
				_type = DSPType.Chorus,
				dspParameters = new DSPParameterUI[8]
				{
					new DSPParameterUI
					{
						_parameter = "DryMix",
						_min = 0f,
						_max = 1f
					},
					new DSPParameterUI
					{
						_parameter = "WetMix1",
						_min = 0f,
						_max = 1f
					},
					new DSPParameterUI
					{
						_parameter = "WetMix2",
						_min = 0f,
						_max = 1f
					},
					new DSPParameterUI
					{
						_parameter = "WetMix3",
						_min = 0f,
						_max = 1f
					},
					new DSPParameterUI
					{
						_parameter = "Delay",
						_min = 0.1f,
						_max = 100f
					},
					new DSPParameterUI
					{
						_parameter = "Rate",
						_min = 0f,
						_max = 20f
					},
					new DSPParameterUI
					{
						_parameter = "Depth",
						_min = 0f,
						_max = 1f
					},
					new DSPParameterUI
					{
						_parameter = "Feedback",
						_min = 0f,
						_max = 1f
					}
				}
			},
			new DSPParameterInfoUI
			{
				_type = DSPType.Panner,
				dspParameters = new DSPParameterUI[6]
				{
					new DSPParameterUI
					{
						_parameter = "FrontLeft",
						_min = 0f,
						_max = 1f
					},
					new DSPParameterUI
					{
						_parameter = "FrontRight",
						_min = 0f,
						_max = 1f
					},
					new DSPParameterUI
					{
						_parameter = "Center",
						_min = 0f,
						_max = 1f
					},
					new DSPParameterUI
					{
						_parameter = "LFE",
						_min = 0f,
						_max = 1f
					},
					new DSPParameterUI
					{
						_parameter = "RearLeft",
						_min = 0f,
						_max = 1f
					},
					new DSPParameterUI
					{
						_parameter = "RearRight",
						_min = 0f,
						_max = 1f
					}
				}
			},
			new DSPParameterInfoUI
			{
				_type = DSPType.SamplePlayerPanner,
				dspParameters = new DSPParameterUI[6]
				{
					new DSPParameterUI
					{
						_parameter = "FrontLeft",
						_min = 0f,
						_max = 1f
					},
					new DSPParameterUI
					{
						_parameter = "FrontRight",
						_min = 0f,
						_max = 1f
					},
					new DSPParameterUI
					{
						_parameter = "Center",
						_min = 0f,
						_max = 1f
					},
					new DSPParameterUI
					{
						_parameter = "LFE",
						_min = 0f,
						_max = 1f
					},
					new DSPParameterUI
					{
						_parameter = "RearLeft",
						_min = 0f,
						_max = 1f
					},
					new DSPParameterUI
					{
						_parameter = "RearRight",
						_min = 0f,
						_max = 1f
					}
				}
			},
			new DSPParameterInfoUI
			{
				_type = DSPType.Reverb,
				dspParameters = new DSPParameterUI[1]
				{
					new DSPParameterUI
					{
						_parameter = "Properties Not Implemented",
						_min = 0f,
						_max = 1f
					}
				}
			}
		};

		private EventTrigger eventTrigger;

		private EventManager eventManager;

		private EventListComponent[] eventListComponents;

		private GroupComponent[] externalGroupComponents;

		private SerializedProperty eventTriggerName;

		private SerializedProperty eventTriggerType;

		private SerializedProperty eventTriggerAction;

		private SerializedProperty eventTriggerParameter;

		private SerializedProperty eventTriggerEventList;

		private SerializedProperty eventTriggerParameterName;

		private SerializedProperty eventTriggerParameterMax;

		private SerializedProperty eventTriggerParameterMin;

		private SerializedProperty eventTriggerValue;

		private SerializedProperty eventTriggerDelay;

		private SerializedProperty eventTriggerProbability;

		private SerializedProperty eventTriggerIgnoreGameObject;

		private SerializedProperty eventTriggerOverrideParentGameObject;

		private SerializedProperty eventTriggerAddToQueue;

		private SerializedProperty eventTriggerUseEventID;

		private SerializedProperty eventTriggerOnEnter;

		private SerializedProperty eventTriggerEnterTag;

		private SerializedProperty eventTriggerPostCount;

		public static void RegisterExternalDSPParameterInfo(DSPParameterInfoUI newDSPParameterInfoUI)
		{
			if (newDSPParameterInfoUI._type != DSPType.External)
			{
				return;
			}
			for (int i = 0; i < _dspParameterInfo.Length; i++)
			{
				if (_dspParameterInfo[i]._name == newDSPParameterInfoUI._name)
				{
					return;
				}
			}
			_dspParameterInfo = MyArray<DSPParameterInfoUI>.InsertAt(_dspParameterInfo, _dspParameterInfo.Length - 1, newDSPParameterInfoUI);
		}

		public static void UnegisterExternalDSPParameterInfo(DSPParameterInfoUI toRemoveDSPParameterInfoUI)
		{
			if (toRemoveDSPParameterInfoUI._type != DSPType.External)
			{
				return;
			}
			for (int i = 0; i < _dspParameterInfo.Length; i++)
			{
				if (_dspParameterInfo[i]._name == toRemoveDSPParameterInfoUI._name)
				{
					_dspParameterInfo = MyArray<DSPParameterInfoUI>.RemoveAt(_dspParameterInfo, i);
					break;
				}
			}
		}

		private void OnEnable()
		{
			eventTrigger = base.target as EventTrigger;
			eventManager = (EventManager)UnityEngine.Object.FindObjectOfType(typeof(EventManager));
			eventTriggerName = base.serializedObject.FindProperty("_eventName");
			eventTriggerType = base.serializedObject.FindProperty("_eventTriggerType");
			eventTriggerAction = base.serializedObject.FindProperty("_eventAction");
			eventTriggerParameter = base.serializedObject.FindProperty("_eventParameter");
			eventTriggerParameterName = base.serializedObject.FindProperty("_eventParameterName");
			eventTriggerParameterMax = base.serializedObject.FindProperty("_max");
			eventTriggerParameterMin = base.serializedObject.FindProperty("_min");
			eventTriggerValue = base.serializedObject.FindProperty("_eventValue");
			eventTriggerDelay = base.serializedObject.FindProperty("_delay");
			eventTriggerProbability = base.serializedObject.FindProperty("_probability");
			eventTriggerIgnoreGameObject = base.serializedObject.FindProperty("_ignoreGameObject");
			eventTriggerAddToQueue = base.serializedObject.FindProperty("_addToQueue");
			eventTriggerUseEventID = base.serializedObject.FindProperty("_useEventID");
			eventTriggerOnEnter = base.serializedObject.FindProperty("_eventTriggerOnEnter");
			eventTriggerEnterTag = base.serializedObject.FindProperty("_triggerEnterTag");
			eventTriggerPostCount = base.serializedObject.FindProperty("_postCountMax");
			if (eventTrigger._parentGameObject == null)
			{
				eventTrigger._parentGameObject = eventTrigger.gameObject;
			}
			eventTriggerOverrideParentGameObject = base.serializedObject.FindProperty("_overrideParentGameObject");
			if (eventManager != null)
			{
				SerializedObject serializedObject = new SerializedObject(eventManager);
				eventTriggerEventList = serializedObject.FindProperty("_eventList");
			}
			eventListComponents = UnityEngine.Object.FindObjectsOfType(typeof(EventListComponent)) as EventListComponent[];
			externalGroupComponents = UnityEngine.Object.FindObjectsOfType(typeof(GroupComponent)) as GroupComponent[];
			for (int i = 0; i < externalGroupComponents.Length; i++)
			{
				if (!externalGroupComponents[i].IsInstance && externalGroupComponents[i].IsFabricHierarchyPresent())
				{
					externalGroupComponents = MyArray<GroupComponent>.RemoveAt(externalGroupComponents, i);
				}
			}
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			MenuBar.OnGUI("288095-eventtrigger", box: true);
			GUILayout.BeginVertical("box");
			if (eventTrigger._eventAction != EventAction.AddPreset && eventTrigger._eventAction != EventAction.RemovePreset && eventTrigger._eventAction != EventAction.SwitchPreset && eventTrigger._eventAction != EventAction.ResetDynamicMixer && eventTrigger._eventAction != EventAction.SetGlobalParameter && eventTrigger._eventAction != EventAction.SetGlobalSwitch && eventTrigger._eventAction != EventAction.TransitionToSnapshot && eventTrigger._eventAction != EventAction.LoadAudioMixer && eventTrigger._eventAction != EventAction.UnloadAudioMixer)
			{
				eventTriggerName.stringValue = EventManagerEditor.BuildEventName(eventTriggerName.stringValue, eventManager, eventListComponents, externalGroupComponents);
			}
			EditorGUILayout.PropertyField(eventTriggerType, new GUIContent("TriggerOn:"));
			if (eventTriggerType.enumValueIndex == 5 || eventTriggerType.enumValueIndex == 6 || eventTriggerType.enumValueIndex == 15 || eventTriggerType.enumValueIndex == 16)
			{
				GUILayout.BeginVertical("Box");
				EditorGUILayout.PropertyField(eventTriggerOnEnter, new GUIContent("Mode:"));
				if (eventTriggerOnEnter.enumValueIndex == 2)
				{
					EditorGUILayout.PropertyField(eventTriggerEnterTag, new GUIContent("Tag:"));
				}
				GUILayout.EndVertical();
				GUILayout.Space(2f);
			}
			EditorGUILayout.PropertyField(eventTriggerAction, new GUIContent("Action:"));
			if (eventTrigger._eventAction == EventAction.SetPitch || eventTrigger._eventAction == EventAction.SetPitchProperty)
			{
				EditorGUILayout.Slider(eventTriggerParameter, -5f, 5f, new GUIContent("Pitch:"));
			}
			else if (eventTrigger._eventAction == EventAction.SetVolume || eventTrigger._eventAction == EventAction.SetVolumeProperty)
			{
				EditorGUILayout.Slider(eventTriggerParameter, 0f, 1f, new GUIContent("Volume:"));
			}
			else if (eventTrigger._eventAction == EventAction.SetPan)
			{
				EditorGUILayout.Slider(eventTriggerParameter, -1f, 1f, new GUIContent("Pan:"));
			}
			else if (eventTrigger._eventAction == EventAction.SetTime)
			{
				EditorGUILayout.Slider(eventTriggerParameter, -1000f, 1000f, new GUIContent("Time:"));
			}
			else if (eventTrigger._eventAction == EventAction.SetMarker)
			{
				EditorGUILayout.PropertyField(eventTriggerValue, new GUIContent("Label:"));
			}
			else if (eventTrigger._eventAction == EventAction.SetAudioClipReference)
			{
				EditorGUILayout.PropertyField(eventTriggerValue, new GUIContent("AudioClip Name:"));
			}
			else if (eventTrigger._eventAction == EventAction.SetParameter)
			{
				GUILayout.BeginHorizontal();
				EditorGUILayout.PropertyField(eventTriggerParameterName, new GUIContent("ParameterName:"));
				GUILayout.EndHorizontal();
				EditorGUILayout.Slider(eventTriggerParameter, eventTrigger._min, eventTrigger._max, new GUIContent("Parameter:"));
				EditorGUILayout.PropertyField(eventTriggerParameterMax, new GUIContent("Max:"));
				EditorGUILayout.PropertyField(eventTriggerParameterMin, new GUIContent("Min:"));
			}
			else if (eventTrigger._eventAction == EventAction.SetSwitch)
			{
				EditorGUILayout.PropertyField(eventTriggerValue, new GUIContent("SwitchTo:"));
			}
			else if (eventTrigger._eventAction == EventAction.AddPreset || eventTrigger._eventAction == EventAction.RemovePreset)
			{
				eventTrigger._eventName = "DynamicMixer";
				EditorGUILayout.PropertyField(eventTriggerValue, new GUIContent("Preset:"));
			}
			else if (eventTrigger._eventAction == EventAction.SwitchPreset)
			{
				eventTrigger._eventName = "DynamicMixer";
				if (eventTrigger.switchPresetData == null)
				{
					eventTrigger.switchPresetData = new SwitchPresetData();
				}
				GUILayout.BeginHorizontal();
				GUILayout.Label("SwitchTo:");
				eventTrigger.switchPresetData._targetPreset = GUILayout.TextField(eventTrigger.switchPresetData._targetPreset, GUILayout.MinWidth(300f), GUILayout.MaxWidth(300f));
				GUILayout.EndHorizontal();
			}
			else if (eventTrigger._eventAction == EventAction.SetGlobalParameter)
			{
				GlobalParameterManager.GlobalParametersFastList globalRTParameters = EventManager.Instance._globalParameterManager._globalRTParameters;
				eventTrigger._eventName = "GlobalParameter";
				if (eventTrigger.globalParameterData == null)
				{
					eventTrigger.globalParameterData = new GlobalParameterData();
				}
				string[] array = globalRTParameters.Keys();
				int num = globalRTParameters.GetIndexByKey(eventTrigger.globalParameterData._name);
				GUILayout.BeginHorizontal();
				bool flag = false;
				if (num < 0)
				{
					num = 0;
					flag = true;
				}
				int num2 = EditorGUILayout.Popup("Global Parameter:", num, array, GUILayout.MinWidth(240f));
				if ((num2 != num || flag) && num2 < array.Length)
				{
					eventTrigger.globalParameterData._name = array[num2];
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GlobalParameter globalParameter = globalRTParameters.FindItemByIndex(num2);
				if (globalParameter != null && eventTrigger.globalParameterData != null)
				{
					float value = eventTrigger.globalParameterData._value;
					value = EditorGUILayout.Slider("Value", value, globalParameter._min, globalParameter._max);
					eventTrigger.globalParameterData._value = value;
				}
				GUILayout.EndHorizontal();
			}
			else if (eventTrigger._eventAction == EventAction.SetGlobalSwitch)
			{
				GlobalParameterManager.GlobalSwitchFastList globalSwitches = EventManager.Instance._globalParameterManager._globalSwitches;
				if (globalSwitches != null && globalSwitches._keys.Count > 0)
				{
					eventTrigger._eventName = "GlobalParameter";
					if (eventTrigger.globalSwitchParameterData == null)
					{
						eventTrigger.globalSwitchParameterData = new GlobalSwitchParameterData();
						eventTrigger.globalSwitchParameterData._switch = "";
					}
					int num3 = -1;
					GUILayout.BeginHorizontal();
					string[] array2 = globalSwitches.Keys();
					int num4 = globalSwitches.GetIndexByKey(eventTrigger.globalSwitchParameterData._name);
					bool flag2 = false;
					if (num4 < 0)
					{
						num4 = 0;
						flag2 = true;
					}
					num3 = EditorGUILayout.Popup("Global Switch:", num4, array2, GUILayout.MinWidth(240f));
					if (num3 != num4 || flag2)
					{
						eventTrigger.globalSwitchParameterData._name = array2[num3];
					}
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
					GlobalSwitch globalSwitch = globalSwitches.FindItemByIndex(num3);
					List<string> list = new List<string>();
					for (int i = 0; i < globalSwitch._switches.Count; i++)
					{
						list.Add(globalSwitch._switches[i]._name);
					}
					int num5 = list.FindIndex((string x) => eventTrigger.globalSwitchParameterData._switch.Contains(x));
					bool flag3 = false;
					if (num5 < 0)
					{
						num5 = 0;
						flag3 = true;
					}
					int num6 = EditorGUILayout.Popup("Switch:", num5, list.ToArray(), GUILayout.MinWidth(240f));
					if ((num6 != num5 || flag3) && num6 >= 0)
					{
						eventTrigger.globalSwitchParameterData._switch = list[num6];
					}
					GUILayout.EndHorizontal();
				}
			}
			else if (eventTrigger._eventAction == EventAction.SetRegion || eventTrigger._eventAction == EventAction.QueueRegion)
			{
				EditorGUILayout.PropertyField(eventTriggerParameterName, new GUIContent("RegionName:"));
			}
			else if (eventTrigger._eventAction == EventAction.LoadAudioMixer)
			{
				eventTrigger._eventName = "AudioMixer";
				EditorGUILayout.PropertyField(eventTriggerParameterName, new GUIContent("AudioMixer:"));
			}
			else if (eventTrigger._eventAction == EventAction.UnloadAudioMixer)
			{
				eventTrigger._eventName = "AudioMixer";
				EditorGUILayout.PropertyField(eventTriggerParameterName, new GUIContent("AudioMixer:"));
			}
			else if (eventTrigger._eventAction == EventAction.TransitionToSnapshot)
			{
				eventTrigger._eventName = "AudioMixer";
				if (eventTrigger.transitionToSnapshotData == null)
				{
					eventTrigger.transitionToSnapshotData = new TransitionToSnapshotData();
				}
				GUILayout.BeginHorizontal();
				GUILayout.Label("Snapshot To Transition:");
				eventTrigger.transitionToSnapshotData._snapshot = GUILayout.TextField(eventTrigger.transitionToSnapshotData._snapshot, GUILayout.MinWidth(300f), GUILayout.MaxWidth(300f));
				GUILayout.EndHorizontal();
				eventTrigger.transitionToSnapshotData._timeToReach = EditorGUILayout.FloatField("Time To Reach: ", eventTrigger.transitionToSnapshotData._timeToReach);
			}
			else if (eventTrigger._eventAction == EventAction.SetDSPParameter)
			{
				GUILayout.Space(10f);
				eventTrigger._dspType = (DSPType)(object)EditorGUILayout.EnumPopup("DSP:", eventTrigger._dspType);
				string[] array3 = null;
				int selectedIndex = 0;
				if (eventTrigger._dspType == DSPType.External)
				{
					eventTrigger._eventParameterName = EditorGUILayout.TextField("Parameter: ", eventTrigger._eventParameterName);
					eventTrigger._eventParameter = EditorGUILayout.Slider("Value:", eventTrigger._eventParameter, eventTrigger._min, eventTrigger._max);
					eventTrigger._timeToTarget = EditorGUILayout.FloatField("Time: ", eventTrigger._timeToTarget);
					eventTrigger._curve = EditorGUILayout.Slider("Curve: ", eventTrigger._curve, 0f, 1f);
				}
				else
				{
					array3 = new string[_dspParameterInfo[(int)eventTrigger._dspType].dspParameters.Length];
					selectedIndex = 0;
					for (int num7 = 0; num7 < _dspParameterInfo[(int)eventTrigger._dspType].dspParameters.Length; num7++)
					{
						array3[num7] = _dspParameterInfo[(int)eventTrigger._dspType].dspParameters[num7]._parameter;
						if (array3[num7] == eventTrigger._eventParameterName)
						{
							selectedIndex = num7;
						}
					}
				}
				if (array3 != null)
				{
					selectedIndex = EditorGUILayout.Popup("Parameter:", selectedIndex, array3);
					eventTrigger._eventParameterName = array3[selectedIndex];
					eventTrigger._eventParameter = EditorGUILayout.Slider("Value:", eventTrigger._eventParameter, _dspParameterInfo[(int)eventTrigger._dspType].dspParameters[selectedIndex]._min, _dspParameterInfo[(int)eventTrigger._dspType].dspParameters[selectedIndex]._max);
					eventTrigger._timeToTarget = EditorGUILayout.FloatField("Time:", eventTrigger._timeToTarget);
					eventTrigger._curve = EditorGUILayout.Slider("Curve:", eventTrigger._curve, 0f, 1f);
				}
				GUILayout.Space(10f);
			}
			else if (eventTrigger._eventAction == EventAction.PlayScheduled || eventTrigger._eventAction == EventAction.StopScheduled)
			{
				eventTrigger._eventScheduleParameter = EditorGUILayout.DoubleField("Time: ", eventTrigger._eventScheduleParameter);
			}
			EditorGUILayout.Slider(eventTriggerDelay, 0f, 1000f, new GUIContent("Delay (secs):"));
			EditorGUILayout.IntSlider(eventTriggerProbability, 0, 100, new GUIContent("Probability:"));
			GUILayout.BeginHorizontal();
			EditorGUILayout.PropertyField(eventTriggerPostCount, new GUIContent("Post Event Limit:"));
			GUILayout.Label(" [ " + eventTrigger._postCount + " ]");
			GUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(eventTriggerIgnoreGameObject, new GUIContent("Ignore GameObject:"));
			EditorGUILayout.PropertyField(eventTriggerAddToQueue, new GUIContent("Add To Queue:"));
			EditorGUILayout.PropertyField(eventTriggerUseEventID, new GUIContent("Use EventID:"));
			EditorGUILayout.PropertyField(eventTriggerOverrideParentGameObject, new GUIContent("Override Parent GameObject: "));
			if (eventTriggerOverrideParentGameObject.boolValue)
			{
				eventTrigger._parentGameObject = (GameObject)EditorGUILayout.ObjectField("Parent GameObject: ", eventTrigger._parentGameObject, typeof(GameObject), true);
			}
			else
			{
				eventTrigger._parentGameObject = eventTrigger.gameObject;
			}
			if (GUILayout.Button("Post Event"))
			{
				eventTrigger.PostEvent();
			}
			GUILayout.EndVertical();
			GUIHelpers.CheckGUIHasChanged(base.serializedObject, eventTrigger.gameObject);
		}

		public static DSPParameterData DSPParameterDataUI(DSPParameterData dspParameter)
		{
			dspParameter._dspType = (DSPType)(object)EditorGUILayout.EnumPopup("DSP:", dspParameter._dspType);
			string[] array = new string[_dspParameterInfo[(int)dspParameter._dspType].dspParameters.Length];
			int selectedIndex = 0;
			for (int i = 0; i < _dspParameterInfo[(int)dspParameter._dspType].dspParameters.Length; i++)
			{
				array[i] = _dspParameterInfo[(int)dspParameter._dspType].dspParameters[i]._parameter;
				if (array[i] == dspParameter._parameter)
				{
					selectedIndex = i;
				}
			}
			selectedIndex = EditorGUILayout.Popup("Parameter:", selectedIndex, array);
			dspParameter._parameter = array[selectedIndex];
			dspParameter._value = EditorGUILayout.Slider("Value:", dspParameter._value, _dspParameterInfo[(int)dspParameter._dspType].dspParameters[selectedIndex]._min, _dspParameterInfo[(int)dspParameter._dspType].dspParameters[selectedIndex]._max);
			dspParameter._time = EditorGUILayout.FloatField("Time:", dspParameter._time);
			dspParameter._curve = EditorGUILayout.Slider("Curve:", dspParameter._curve, 0f, 1f);
			return dspParameter;
		}
	}
	[CustomEditor(typeof(SideChain))]
	public class SideChainEditor : Editor
	{
		private static int maxHeight = 10;

		private static int maxWidth = 200;

		public override void OnInspectorGUI()
		{
			SideChain sideChain = (SideChain)base.target;
			MenuBar.OnGUI("290568-sidechain", box: true);
			GUILayout.BeginVertical("Box");
			sideChain._useComponentIsPlayingFlag = EditorGUILayout.Toggle("Use Component: ", sideChain._useComponentIsPlayingFlag);
			if (sideChain._useComponentIsPlayingFlag)
			{
				sideChain._componentToListen = (Component)EditorGUILayout.ObjectField("Component:", sideChain._componentToListen, typeof(Component), true);
			}
			else
			{
				sideChain._volumeMeter = (VolumeMeter)EditorGUILayout.ObjectField("VolumeMeter:", sideChain._volumeMeter, typeof(VolumeMeter), true);
			}
			DrawSideChain(sideChain, maxWidth);
			float value = AudioTools.LinearToDB(sideChain.gain);
			value = EditorGUILayout.Slider("Gain (dB):", value, -96f, 12f);
			sideChain.gain = AudioTools.DBToLinear(value);
			GUILayout.Space(10f);
			sideChain.fadeUpRate = EditorGUILayout.Slider("Attack (secs):", sideChain.fadeUpRate, 0f, 2f);
			sideChain.fadeDownRate = EditorGUILayout.Slider("Release (secs):", sideChain.fadeDownRate, 0f, 2f);
			GUILayout.Space(10f);
			GUILayout.Label("CPU: " + sideChain.profiler.msPerFrame.ToString("0.000") + "(ms)");
			GUILayout.EndVertical();
		}

		public static void DrawSideChain(SideChain sideChain, float length)
		{
			GUILayout.BeginVertical("box");
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("SideChain");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			float db = AudioTools.LinearToDB(1f - sideChain._sideChainGain);
			float num = AudioTools.DBToNormalizedDB(db);
			GUILayout.BeginVertical("box");
			GUILayout.Label("Gain:" + db.ToString("N2") + " dB");
			GUILayout.Box(GUIContent.none, GUI.skin.box, GUILayout.MinHeight(maxHeight), GUILayout.MaxHeight(maxHeight), GUILayout.Width(length * num));
			GUILayout.EndVertical();
			GUILayout.EndVertical();
		}
	}
	[CustomEditor(typeof(VolumeMeter))]
	public class VolumeMeterEditor : Editor
	{
		private static int maxHeight = 10;

		private static int maxWidth = 500;

		private int selectedGlobalParameter;

		public override void OnInspectorGUI()
		{
			VolumeMeter volumeMeter = (VolumeMeter)base.target;
			MenuBar.OnGUI("290567-volumemeter", box: true);
			GUILayout.BeginVertical("Box");
			volumeMeter._is3D = GUILayout.Toggle(volumeMeter._is3D, "is3D");
			string[] array = new string[EventManager.Instance._globalParameterManager._globalRTParameters.GetCount() + 1];
			array[0] = "NONE";
			string[] array2 = EventManager.Instance._globalParameterManager._globalRTParameters.Keys();
			for (int i = 0; i < array2.Length; i++)
			{
				array[i + 1] = array2[i];
				if (volumeMeter._globalParameterName == array2[i])
				{
					selectedGlobalParameter = i + 1;
				}
			}
			selectedGlobalParameter = EditorGUILayout.Popup("Global Parameter: ", selectedGlobalParameter, array, GUILayout.MinWidth(240f));
			if (selectedGlobalParameter > 0)
			{
				volumeMeter._globalParameterName = array[selectedGlobalParameter];
			}
			else
			{
				volumeMeter._globalParameterName = null;
			}
			DrawMeters(volumeMeter, maxWidth);
			GUILayout.EndVertical();
		}

		public static void DrawMeters(VolumeMeter volumeMeter, float length)
		{
			GUILayout.BeginVertical("box");
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("VolumeMeter");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			length = GUILayoutUtility.GetLastRect().width;
			DrawMeter2("FL", volumeMeter.volumeMeterState.mPeaks.mChannels[0], 10, length, isVertical: true);
			GUILayout.Space(3f);
			DrawMeter2("FR", volumeMeter.volumeMeterState.mPeaks.mChannels[1], 10, length, isVertical: true);
			GUILayout.Space(3f);
			DrawMeter2("RMS", volumeMeter.volumeMeterState.mRMS, 10, length, isVertical: true);
			GUILayout.Space(10f);
			GUILayout.Label("CPU: " + volumeMeter.profiler.msPerFrame.ToString("0.000") + "(ms)");
			GUILayout.EndVertical();
		}

		private static void DrawMeter(string name, float value, float length, bool isVertical)
		{
			float db = AudioTools.LinearToDB(value);
			float num = AudioTools.DBToNormalizedDB(db);
			if (isVertical)
			{
				GUILayout.BeginVertical("box");
				GUILayout.Label(name + ":" + db.ToString("N2") + " dB");
				GUILayout.Box(GUIContent.none, GUI.skin.box, GUILayout.MinHeight(maxHeight), GUILayout.MaxHeight(maxHeight), GUILayout.Width(length * num));
				GUILayout.EndVertical();
			}
			else
			{
				GUILayout.BeginVertical("box", GUILayout.MinHeight(maxHeight));
				GUILayout.Label(name + ":" + db.ToString("N2") + " dB");
				GUILayout.Box(GUIContent.none, GUI.skin.box, GUILayout.Height(length * num), GUILayout.Width(maxWidth));
				GUILayout.EndVertical();
			}
		}

		public static void DrawMeter2(string name, float value, int height, float length, bool isVertical)
		{
			float db = AudioTools.LinearToDB(value);
			float num = AudioTools.DBToNormalizedDB(db);
			int num2 = (int)(length * num);
			Texture2D texture2D = new Texture2D(num2, height, TextureFormat.RGBA32, mipChain: false);
			float num3 = 1f / (float)num2;
			for (int i = 0; i < num2; i++)
			{
				float num4 = (float)i * num3;
				Color color = (1f - num4) * Color.green + num4 * Color.yellow;
				if (i % 10 == 0)
				{
					color = Color.black;
					i++;
				}
				for (int j = 0; j < height; j++)
				{
					texture2D.SetPixel(i, j, color);
				}
			}
			texture2D.Apply();
			GUILayout.BeginHorizontal();
			if (name.Length > 0)
			{
				GUILayout.Label(name + ":" + db.ToString("N1") + " dB", GUILayout.MaxWidth(90f));
			}
			GUIStyle gUIStyle = new GUIStyle();
			gUIStyle.alignment = TextAnchor.MiddleLeft;
			GUILayout.Box(texture2D, gUIStyle, GUILayout.ExpandWidth(expand: true));
			GUILayout.EndHorizontal();
		}
	}
	public class MultiEditAudioClipView : EditorWindow
	{
		private int compressionBitRate = 32;

		private bool applyCompressionBitRate;

		private bool hardwareDecoding;

		private bool applyHardwareDecoding;

		private bool threeDeeSound;

		private bool apply3DSound;

		private bool forceToMonoSetting;

		private bool applyForceToMonoSetting;

		private AudioClipLoadType loadType;

		private bool applyAudioFormat;

		private bool applyLoadType;

		[MenuItem("Window/Fabric/Multi Edit AudioClip", false, 20)]
		private static void init()
		{
			MultiEditAudioClipView multiEditAudioClipView = (MultiEditAudioClipView)EditorWindow.GetWindow(typeof(MultiEditAudioClipView));
			multiEditAudioClipView.titleContent = new GUIContent("Multi Edit AudioClip Window");
		}

		private void OnGUI()
		{
			MenuBar.OnGUI("288090-multieditaudioclip", box: true);
			GUILayout.Space(5f);
			GUILayout.BeginHorizontal();
			int num = 15;
			GUILayout.BeginVertical(GUILayout.MaxWidth(num));
			GUILayout.BeginHorizontal("Box");
			applyHardwareDecoding = EditorGUILayout.Toggle("", applyHardwareDecoding, GUILayout.MaxWidth(num));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal("Box");
			applyAudioFormat = EditorGUILayout.Toggle("", applyAudioFormat, GUILayout.MaxWidth(num));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal("Box");
			applyCompressionBitRate = EditorGUILayout.Toggle("", applyCompressionBitRate, GUILayout.MaxWidth(num));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal("Box");
			apply3DSound = EditorGUILayout.Toggle("", apply3DSound, GUILayout.MaxWidth(num));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal("Box");
			applyLoadType = EditorGUILayout.Toggle("", applyLoadType, GUILayout.MaxWidth(num));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal("Box");
			applyForceToMonoSetting = EditorGUILayout.Toggle("", applyForceToMonoSetting, GUILayout.MaxWidth(num));
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			int num2 = 3000;
			GUILayout.BeginVertical(GUILayout.MaxWidth(num2));
			GUILayout.BeginHorizontal("Box");
			hardwareDecoding = EditorGUILayout.Toggle("Hardware decoding:", hardwareDecoding);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal("Box");
			compressionBitRate = EditorGUILayout.IntSlider("Compression (kbps):", compressionBitRate, 32, 256);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal("Box");
			threeDeeSound = EditorGUILayout.Toggle("3D Sound:", threeDeeSound);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal("Box");
			loadType = (AudioClipLoadType)(object)EditorGUILayout.EnumPopup("Load type:", loadType);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal("Box");
			forceToMonoSetting = EditorGUILayout.Toggle("Force to mono:", forceToMonoSetting);
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			if (!GUILayout.Button("Apply"))
			{
				return;
			}
			UnityEngine.Object[] selectedAudioclips = GetSelectedAudioclips();
			if (selectedAudioclips.Length <= 1)
			{
				EditorUtility.DisplayDialog("Fabric MultiEditAudioClip", "You need to select more than one audio clips in order for the multi edit to work", "Ok");
				return;
			}
			UnityEngine.Object[] array = selectedAudioclips;
			for (int i = 0; i < array.Length; i++)
			{
				AudioClip assetObject = (AudioClip)array[i];
				string assetPath = AssetDatabase.GetAssetPath(assetObject);
				AudioImporter audioImporter = AssetImporter.GetAtPath(assetPath) as AudioImporter;
				if (applyForceToMonoSetting)
				{
					audioImporter.forceToMono = forceToMonoSetting;
				}
				AssetDatabase.ImportAsset(assetPath);
			}
		}

		private static UnityEngine.Object[] GetSelectedAudioclips()
		{
			return Selection.GetFiltered(typeof(AudioClip), SelectionMode.DeepAssets);
		}
	}
	[CustomEditor(typeof(SwitchComponent))]
	[CanEditMultipleObjects]
	public class SwitchComponentEditor : Editor
	{
		private SwitchComponent switchComponent;

		private ComponentEditor componentEditor = new ComponentEditor();

		private SerializedProperty startOnSwitch;

		private SerializedProperty switchComponentSwitchType;

		private SerializedProperty musicTimeSettingsindex;

		private SerializedProperty syncToMusicOnFirstPlay;

		private SerializedProperty _useGlobalSwitch;

		private bool _foldout = true;

		[MenuItem("Fabric/Components/SwitchComponent")]
		private static void About()
		{
			GameObject activeGameObject = Selection.activeGameObject;
			if (!(activeGameObject == null))
			{
				GameObject gameObject = new GameObject("SwitchComponent");
				gameObject.transform.parent = activeGameObject.transform;
				gameObject.AddComponent<SwitchComponent>();
			}
		}

		private void OnEnable()
		{
			switchComponent = base.target as SwitchComponent;
			componentEditor.RegisterSerialisableObject(base.serializedObject);
			startOnSwitch = base.serializedObject.FindProperty("_startOnSwitch");
			switchComponentSwitchType = base.serializedObject.FindProperty("_switchComponentSwitchType");
			musicTimeSettingsindex = base.serializedObject.FindProperty("_musicTimeSettingsIndex");
			syncToMusicOnFirstPlay = base.serializedObject.FindProperty("_syncToMusicOnFirstPlay");
			_useGlobalSwitch = base.serializedObject.FindProperty("_useGlobalSwitch");
		}

		private void OnDestroy()
		{
		}

		public override void OnInspectorGUI()
		{
			componentEditor.InspectorGUI(switchComponent, "288043-switchcomponent");
			base.serializedObject.Update();
			_foldout = EditorGUILayout.Foldout(_foldout, "Switch Component Properties");
			if (_foldout)
			{
				GUILayout.BeginVertical("box");
				if (switchComponentSwitchType.enumValueIndex == 3)
				{
					GUI.enabled = false;
				}
				EditorGUILayout.PropertyField(startOnSwitch, new GUIContent("Start On Switch:"));
				if (switchComponentSwitchType.enumValueIndex == 3)
				{
					GUI.enabled = true;
				}
				EditorGUILayout.PropertyField(switchComponentSwitchType, new GUIContent("Switch Type:"));
				if (switchComponent._switchComponentSwitchType == SwitchComponentSwitchType.OnMusicSync)
				{
					if (switchComponent._musicTimeResetOnPlay)
					{
						GUI.enabled = false;
					}
					EditorGUILayout.PropertyField(syncToMusicOnFirstPlay, new GUIContent("Sync On Play:"));
					GUI.enabled = true;
				}
				GUILayout.Space(5f);
				EditorGUILayout.PropertyField(_useGlobalSwitch, new GUIContent("Use Global Switch:"));
				GlobalParameterManager.GlobalSwitchFastList globalSwitches = EventManager.Instance._globalParameterManager._globalSwitches;
				if (_useGlobalSwitch.boolValue && globalSwitches.GetCount() > 0)
				{
					GUILayout.BeginVertical("Box");
					string[] array = globalSwitches.Keys();
					int indexByKey = globalSwitches.GetIndexByKey(switchComponent._globalSwitch);
					int num = EditorGUILayout.Popup(indexByKey, array, GUILayout.MinWidth(240f));
					if (num != indexByKey)
					{
						switchComponent._globalSwitch = array[num];
						switchComponent._globalSwitchMap.Clear();
						GlobalSwitch globalSwitch = globalSwitches.FindItem(switchComponent._globalSwitch);
						for (int i = 0; i < globalSwitch._switches.Count; i++)
						{
							SwitchComponent.GlobalSwitchContainer globalSwitchContainer = new SwitchComponent.GlobalSwitchContainer();
							globalSwitchContainer._switchName = globalSwitch._switches[i]._name;
							globalSwitchContainer._components.Add(null);
							switchComponent._globalSwitchMap.Add(globalSwitchContainer);
						}
					}
					for (int j = 0; j < switchComponent._globalSwitchMap.Count; j++)
					{
						GUILayout.BeginHorizontal();
						GUILayout.Label("    " + switchComponent._globalSwitchMap[j]._switchName, GUILayout.MinWidth(150f));
						switchComponent._globalSwitchMap[j]._components[0] = GUIHelpers.BuildComponentChildrenMenu(switchComponent, switchComponent._globalSwitchMap[j]._components[0]);
						GUILayout.EndHorizontal();
					}
					GUILayout.EndVertical();
				}
				else
				{
					switchComponent._selectedComponent = (Component)EditorGUILayout.ObjectField("Default Component:", switchComponent._selectedComponent, typeof(Component), true);
				}
				GUILayout.EndVertical();
			}
			GUIHelpers.CheckGUIHasChanged(base.serializedObject, switchComponent);
		}
	}
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SequenceComponent))]
	public class SequenceComponentEditor : Editor
	{
		private SequenceComponent sequenceComponent;

		private ComponentEditor componentEditor = new ComponentEditor();

		private SerializedProperty sequenceType;

		private SerializedProperty sequencePlayMode;

		private SerializedProperty sequenceAdvanceMode;

		private SerializedProperty resetOnFirstPlay;

		private SerializedProperty transitionThreshold;

		private SerializedProperty transitionOffset;

		private SerializedProperty transitionOffsetRandomization;

		private SerializedProperty syncToMusicOnFirstPlay;

		private SerializedProperty syncAdvanceBetweenInstances;

		private bool _foldout = true;

		private bool _playlistExpand = true;

		[MenuItem("Fabric/Components/SequenceComponent")]
		private static void About()
		{
			GameObject activeGameObject = Selection.activeGameObject;
			if (!(activeGameObject == null))
			{
				GameObject gameObject = new GameObject("SequenceComponent");
				gameObject.transform.parent = activeGameObject.transform;
				gameObject.AddComponent<SequenceComponent>();
			}
		}

		private void OnEnable()
		{
			sequenceComponent = base.target as SequenceComponent;
			componentEditor.RegisterSerialisableObject(base.serializedObject);
			sequenceType = base.serializedObject.FindProperty("_sequenceType");
			sequencePlayMode = base.serializedObject.FindProperty("_sequencePlayMode");
			sequenceAdvanceMode = base.serializedObject.FindProperty("_sequenceAdvanceMode");
			resetOnFirstPlay = base.serializedObject.FindProperty("_resetOnFirstPlay");
			transitionThreshold = base.serializedObject.FindProperty("_transitionThreshold");
			transitionOffset = base.serializedObject.FindProperty("_transitionOffset");
			transitionOffsetRandomization = base.serializedObject.FindProperty("_transitionOffsetRandomization");
			syncToMusicOnFirstPlay = base.serializedObject.FindProperty("_syncToMusicOnFirstPlay");
			syncAdvanceBetweenInstances = base.serializedObject.FindProperty("_syncAdvanceBetweenInstances");
			GUIHelpers.RegisterEditorUpdate(base.Repaint);
		}

		private void OnDisable()
		{
			GUIHelpers.UnregisterEditorUpdate(base.Repaint);
		}

		private void OnDestroy()
		{
		}

		private void DrawPlaylist(ref Component[] playList, int playingIndex, ref bool[] playlistPlayToEnd)
		{
			_playlistExpand = EditorGUILayout.Foldout(_playlistExpand, "Playlist");
			if (!_playlistExpand)
			{
				return;
			}
			if (playList == null)
			{
				playList = new Component[1];
			}
			if (playlistPlayToEnd == null)
			{
				playlistPlayToEnd = new bool[1];
			}
			int num = EditorGUILayout.IntField("Size:", playList.Length);
			if (num != playList.Length)
			{
				Component[] array = new Component[num];
				for (int i = 0; i < num; i++)
				{
					if (playList.Length > i)
					{
						array[i] = playList[i];
					}
				}
				playList = array;
			}
			if (num != playlistPlayToEnd.Length)
			{
				bool[] array2 = new bool[num];
				for (int j = 0; j < num; j++)
				{
					if (playlistPlayToEnd.Length > j)
					{
						array2[j] = playlistPlayToEnd[j];
					}
				}
				playlistPlayToEnd = array2;
			}
			for (int k = 0; k < playList.Length; k++)
			{
				GUILayout.BeginHorizontal();
				playList[k] = BuildSelection(k, playList[k], k == playingIndex - 1);
				if (sequenceComponent._sequenceType == SequenceComponentType.PlayOnAdvance && !sequenceComponent.IsMusicSyncEnabled())
				{
					playlistPlayToEnd[k] = GUILayout.Toggle(playlistPlayToEnd[k], "PlayToEnd");
				}
				GUILayout.EndHorizontal();
			}
		}

		public override void OnInspectorGUI()
		{
			if (componentEditor.InspectorGUI(sequenceComponent, "288044 - sequencecomponent"))
			{
				return;
			}
			base.serializedObject.Update();
			_foldout = EditorGUILayout.Foldout(_foldout, "Sequence Component Properties");
			if (_foldout)
			{
				GUILayout.BeginVertical("box");
				EditorGUILayout.PropertyField(sequenceType, new GUIContent("Sequence Type:"));
				if (sequenceComponent._sequenceType == SequenceComponentType.PlayOnAdvance)
				{
					EditorGUILayout.BeginVertical("Box");
					EditorGUILayout.PropertyField(sequenceAdvanceMode, new GUIContent("Advance On:"));
					if (sequenceComponent._sequenceAdvanceMode == SequenceComponentAdvanceMode.OnPlayEventAction)
					{
						EditorGUILayout.PropertyField(resetOnFirstPlay, new GUIContent("Reset On First Play:"));
					}
					EditorGUILayout.PropertyField(syncAdvanceBetweenInstances, new GUIContent("Sync All Instances:"));
					if (sequenceComponent._sequenceAdvanceMode == SequenceComponentAdvanceMode.OnMusicSync)
					{
						EditorGUILayout.PropertyField(syncToMusicOnFirstPlay, new GUIContent("Sync On Play:"));
					}
					EditorGUILayout.EndVertical();
					GUILayout.Space(5f);
				}
				EditorGUILayout.PropertyField(sequencePlayMode, new GUIContent("Play Mode:"));
				GUILayout.Space(5f);
				EditorGUILayout.Slider(transitionOffset, -60f, 60f, new GUIContent("Transition Offset (sec):"));
				EditorGUILayout.Slider(transitionOffsetRandomization, -60f, 60f, new GUIContent("Transition Rand (sec):"));
				DrawPlaylist(ref sequenceComponent._playlist, sequenceComponent._playingComponentIndex, ref sequenceComponent._playlistPlayToEnd);
				GUILayout.EndVertical();
			}
			GUIHelpers.CheckGUIHasChanged(base.serializedObject, sequenceComponent);
		}

		private Component BuildSelection(int x, Component component, bool active)
		{
			Component[] childComponents = sequenceComponent.GetChildComponents();
			if (childComponents == null || childComponents.Length == 0)
			{
				return null;
			}
			string[] array = new string[childComponents.Length];
			int selectedIndex = 0;
			for (int i = 0; i < childComponents.Length; i++)
			{
				array[i] = childComponents[i].name;
				if (childComponents[i] == component)
				{
					selectedIndex = i;
				}
			}
			GUILayout.BeginHorizontal();
			if (active)
			{
				GUILayout.Label(x + ": <--", GUILayout.MinWidth(50f));
			}
			else
			{
				GUILayout.Label(x + ":", GUILayout.MinWidth(50f));
			}
			selectedIndex = EditorGUILayout.Popup(selectedIndex, array);
			GUILayout.EndHorizontal();
			return childComponents[selectedIndex];
		}
	}
	[InitializeOnLoad]
	[CanEditMultipleObjects]
	[CustomEditor(typeof(RandomComponent))]
	public class RandomComponentEditor : Editor
	{
		private ComponentEditor componentEditor = new ComponentEditor();

		private RandomComponent randomComponent;

		private bool _foldout = true;

		private SerializedProperty playMode;

		private SerializedProperty triggerMode;

		private SerializedProperty shareRandomNoRepeatHistory;

		private SerializedProperty looped;

		private SerializedProperty delay;

		private SerializedProperty delayOnFirstPlay;

		[MenuItem("Fabric/Components/RandomComponent")]
		private static void About()
		{
			GameObject activeGameObject = Selection.activeGameObject;
			if (!(activeGameObject == null))
			{
				GameObject gameObject = new GameObject("RandomComponent");
				gameObject.transform.parent = activeGameObject.transform;
				gameObject.AddComponent<RandomComponent>();
			}
		}

		private void OnEnable()
		{
			randomComponent = base.target as RandomComponent;
			componentEditor.RegisterSerialisableObject(base.serializedObject);
			playMode = base.serializedObject.FindProperty("_playMode");
			triggerMode = base.serializedObject.FindProperty("_triggerMode");
			shareRandomNoRepeatHistory = base.serializedObject.FindProperty("_shareRandomNoRepeatHistory");
			looped = base.serializedObject.FindProperty("_looped");
			delay = base.serializedObject.FindProperty("_delay");
			delayOnFirstPlay = base.serializedObject.FindProperty("_delayOnFirstPlay");
			randomComponent.GetChildComponents();
			randomComponent.InitialiseWeights();
			EditorApplication.hierarchyWindowChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.hierarchyWindowChanged, new EditorApplication.CallbackFunction(OnHierarchyChange));
			GUIHelpers.RegisterEditorUpdate(base.Repaint);
		}

		private void OnDisable()
		{
			GUIHelpers.UnregisterEditorUpdate(base.Repaint);
		}

		private void OnDestroy()
		{
		}

		private void OnHierarchyChange()
		{
			if (randomComponent != null)
			{
				randomComponent.InitialiseWeights();
			}
		}

		public override void OnInspectorGUI()
		{
			if (componentEditor.InspectorGUI(randomComponent, "287975-randomcomponent"))
			{
				return;
			}
			base.serializedObject.Update();
			_foldout = EditorGUILayout.Foldout(_foldout, "Random Component Properties");
			if (_foldout)
			{
				GUILayout.BeginVertical("box");
				int enumValueIndex = playMode.enumValueIndex;
				EditorGUILayout.PropertyField(playMode, new GUIContent("Play Mode: "));
				if (playMode.enumValueIndex != enumValueIndex)
				{
					randomComponent.InitialiseRandomComponent((RandomComponentPlayMode)playMode.enumValueIndex);
				}
				GUILayout.Space(5f);
				if (randomComponent._playMode == RandomComponentPlayMode.Random)
				{
					Component[] childComponents = randomComponent.GetChildComponents();
					for (int i = 0; i < childComponents.Length; i++)
					{
						Component component = childComponents[i];
						if ((bool)component && randomComponent._randomWeights != null)
						{
							GUILayout.BeginHorizontal("Box");
							GUILayout.Label(component.name);
							GUILayout.BeginHorizontal();
							GUILayout.Label("Weight:", GUILayout.MaxWidth(50f));
							int num = EditorGUILayout.IntSlider("", randomComponent._randomWeights[i], 0, 100);
							if (randomComponent._randomWeights[i] != num)
							{
								randomComponent._randomWeights[i] = num;
								randomComponent.UpdateWeights();
							}
							GUILayout.EndHorizontal();
							GUILayout.EndHorizontal();
						}
					}
				}
				else
				{
					EditorGUILayout.PropertyField(shareRandomNoRepeatHistory, new GUIContent("Share History: "));
				}
				GUILayout.Space(5f);
				if (playMode.enumValueIndex == 0)
				{
					looped.boolValue = false;
					Color backgroundColor = GUI.backgroundColor;
					GUI.color = Color.red;
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					GUILayout.Label("Looped property is not supported in random play mode");
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
					GUI.color = backgroundColor;
					GUILayout.Space(5f);
					GUI.enabled = false;
				}
				EditorGUILayout.PropertyField(looped, new GUIContent("Looped: "));
				if (!looped.boolValue)
				{
					GUI.enabled = false;
				}
				EditorGUILayout.PropertyField(delayOnFirstPlay, new GUIContent("DelayOnFirstPlay: "));
				GUILayout.Space(5f);
				EditorGUILayout.PropertyField(triggerMode, new GUIContent("Trigger Mode: "));
				EditorGUILayout.Slider(delay, 0f, 60f, new GUIContent("Delay (secs)"));
				if (triggerMode.enumValueIndex == 1)
				{
					randomComponent._retriggerTime = delay.floatValue;
				}
				GUILayout.BeginHorizontal();
				EditorGUILayout.MinMaxSlider(new GUIContent("Delay Randomization: "), ref randomComponent._delayRandomization, ref randomComponent._delayMaxRandomization, 0f, 240f);
				randomComponent._delayRandomization = EditorGUILayout.FloatField("", randomComponent._delayRandomization, GUILayout.MaxWidth(64f));
				randomComponent._delayMaxRandomization = EditorGUILayout.FloatField("", randomComponent._delayMaxRandomization, GUILayout.MaxWidth(64f));
				GUILayout.EndHorizontal();
				if (!looped.boolValue)
				{
					GUI.enabled = true;
				}
				GUILayout.EndVertical();
			}
			GUIHelpers.CheckGUIHasChanged(base.serializedObject, randomComponent);
		}
	}
	internal class RTPManagerWidnow : EditorWindow
	{
		private class RTPTrack
		{
			public GUIGraph graphView;

			public RTPMarkersView markersView;

			public RTPParameterToProperty rtpParameterToProperty;

			public int tabIndex;

			public float min;

			public float max;

			public bool requestSorting;
		}

		private List<RTPTrack> rtpTracks = new List<RTPTrack>();

		private GUITimelineLayoutSettings settings = new GUITimelineLayoutSettings();

		private Vector2 layersScroll = Vector2.zero;

		private Component _component;

		private string[] propertyNames;

		private int selectedGlobalParameter;

		private List<RTPProperty> properties;

		private static RTPParameterToProperty parameterToPropertyCopied;

		private bool boundError;

		private float timerRepaint;

		[MenuItem("Window/Fabric/Runtime Parameters", false, 12)]
		public static void init()
		{
			RTPManagerWidnow rTPManagerWidnow = (RTPManagerWidnow)EditorWindow.GetWindow(typeof(RTPManagerWidnow));
			rTPManagerWidnow.title = "RTP Window";
		}

		private void OnEnable()
		{
			EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(OnSelectionChange));
			OnSelectionChange();
			Repaint();
			GUIHelpers.RegisterEditorUpdate(base.Repaint);
		}

		private void OnDisable()
		{
			GUIHelpers.UnregisterEditorUpdate(base.Repaint);
		}

		private void OnFocus()
		{
			OnSelectionChange();
			Repaint();
		}

		private void OnSelectionChange()
		{
			_component = null;
			rtpTracks.Clear();
			if (Selection.activeGameObject != null)
			{
				Component component = Selection.activeGameObject.GetComponent<Component>();
				if (component != null)
				{
					_component = component;
					IRTPPropertyListener component2 = _component;
					if (component2 != null)
					{
						properties = component2.CollectProperties();
						propertyNames = new string[properties.Count];
						for (int i = 0; i < propertyNames.Length; i++)
						{
							propertyNames[i] = properties[i]._name;
						}
					}
					for (int j = 0; j < _component.RTPManager.Parameters.Length; j++)
					{
						RTPParameterToProperty rTPParameterToProperty = _component.RTPManager.Parameters[j];
						RTPTrack rTPTrack = new RTPTrack();
						rTPTrack.rtpParameterToProperty = rTPParameterToProperty;
						rTPTrack.graphView = new GUIGraph();
						rTPTrack.markersView = new RTPMarkersView(rTPParameterToProperty._parameter);
						rTPTrack.graphView._OnGUI = rTPTrack.markersView.OnGraphGUI;
						rTPTrack.min = rTPParameterToProperty._parameter._min;
						rTPTrack.max = rTPParameterToProperty._parameter._max;
						rtpTracks.Add(rTPTrack);
					}
				}
			}
			Repaint();
		}

		public void AddPropertiesToParameters()
		{
			RTPParameterToProperty rTPParameterToProperty = _component.RTPManager.AddParameterToProperty();
			RTPTrack rTPTrack = new RTPTrack();
			rTPTrack.rtpParameterToProperty = rTPParameterToProperty;
			rTPTrack.graphView = new GUIGraph();
			rTPTrack.markersView = new RTPMarkersView(rTPParameterToProperty._parameter);
			rTPTrack.min = rTPParameterToProperty._parameter._min;
			rTPTrack.max = rTPParameterToProperty._parameter._max;
			rtpTracks.Add(rTPTrack);
			if (!EditorApplication.isPlaying)
			{
				rTPParameterToProperty._parameter.Init();
				_component.RTPManager.SetupParameterNames(_component);
			}
			EditorUtility.SetDirty(_component);
		}

		public void InsertPropertiesToParameters(object userData)
		{
			RTPParameterToProperty rTPParameterToProperty = _component.RTPManager.AddParameterToProperty();
			RTPTrack rTPTrack = new RTPTrack();
			rTPTrack.rtpParameterToProperty = rTPParameterToProperty;
			rTPTrack.graphView = new GUIGraph();
			rTPTrack.markersView = new RTPMarkersView(rTPParameterToProperty._parameter);
			rTPTrack.min = rTPParameterToProperty._parameter._min;
			rTPTrack.max = rTPParameterToProperty._parameter._max;
			rtpTracks.Add(rTPTrack);
			if (!EditorApplication.isPlaying)
			{
				rTPParameterToProperty._parameter.Init();
				_component.RTPManager.SetupParameterNames(_component);
			}
			EditorUtility.SetDirty(_component);
		}

		public void DeletePropertiesToParameters(object userData)
		{
			RTPParameterToProperty rTPParameterToProperty = (RTPParameterToProperty)userData;
			_component.RTPManager.DeleteParameterToProperty(rTPParameterToProperty);
			for (int i = 0; i < rtpTracks.Count; i++)
			{
				if (rtpTracks[i].rtpParameterToProperty == rTPParameterToProperty)
				{
					rtpTracks.RemoveAt(i);
					break;
				}
			}
			if (!EditorApplication.isPlaying)
			{
				_component.RTPManager.SetupParameterNames(_component);
			}
			EditorUtility.SetDirty(_component);
		}

		public void CopyProperty(object userData)
		{
			RTPParameterToProperty rTPParameterToProperty = (RTPParameterToProperty)userData;
			parameterToPropertyCopied = rTPParameterToProperty;
		}

		public void PasteProperty(object userData)
		{
			RTPParameterToProperty target = (RTPParameterToProperty)userData;
			_component.RTPManager.PasteParameterToProperty(parameterToPropertyCopied, target);
			EditorUtility.SetDirty(_component);
			Repaint();
		}

		private void SelectedGraph(GUIGraph selectedGraph)
		{
			for (int i = 0; i < rtpTracks.Count; i++)
			{
				RTPTrack rTPTrack = rtpTracks[i];
				if (rTPTrack.graphView != selectedGraph)
				{
					rTPTrack.graphView.isSelected = false;
				}
			}
		}

		private void OnGUI()
		{
			if (_component == null)
			{
				return;
			}
			if (boundError)
			{
				EditorUtility.DisplayDialog("Trying to change range outside locked point... Unlock point or use different range", "", "Ok");
				boundError = false;
			}
			settings.hackOffsetY = 0f;
			if (UnityEngine.Event.current != null && (UnityEngine.Event.current.type == EventType.MouseMove || UnityEngine.Event.current.type == EventType.MouseDrag))
			{
				Repaint();
			}
			GUILayout.BeginHorizontal("Box");
			Component obj = ((_component.RTPManager._reference != null) ? _component.RTPManager._reference : _component);
			GUILayout.Label("Using RTP's From:", GUILayout.MaxWidth(120f));
			Component component = (Component)EditorGUILayout.ObjectField("", obj, typeof(Component), true);
			if (component != _component && GUILayout.Button("Clear", GUILayout.MaxWidth(200f)))
			{
				_component._RTPManager = null;
				_component._RTPManager = new RTPManager();
				OnSelectionChange();
			}
			else if (component != _component)
			{
				_component._RTPManager = null;
				_component._RTPManager = component._RTPManager;
				_component.RTPManager._reference = component;
				OnSelectionChange();
			}
			MenuBar.OnGUI("287978-runtimeparameter");
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
			layersScroll = GUILayout.BeginScrollView(layersScroll);
			if (_component.RTPManager.Parameters.Length == 0)
			{
				GUILayout.BeginHorizontal("Box");
				GUILayout.Label("Right click to add property");
				if (GUILayout.Button("+", GUILayout.MaxWidth(40f)))
				{
					AddPropertiesToParameters();
				}
				GUILayout.EndHorizontal();
				if (GUILayoutUtility.GetLastRect().Contains(UnityEngine.Event.current.mousePosition) && UnityEngine.Event.current.type == EventType.ContextClick)
				{
					GenericMenu genericMenu = new GenericMenu();
					genericMenu.AddItem(new GUIContent("Add Property"), on: false, AddPropertiesToParameters);
					genericMenu.ShowAsContext();
					UnityEngine.Event.current.Use();
				}
			}
			else
			{
				for (int i = 0; i < rtpTracks.Count; i++)
				{
					RTPTrack rTPTrack = rtpTracks[i];
					RTPParameterToProperty rtpParameterToProperty = rTPTrack.rtpParameterToProperty;
					if (rtpParameterToProperty == null)
					{
						continue;
					}
					if (rtpParameterToProperty._property._property >= propertyNames.Length)
					{
						DeletePropertiesToParameters(rtpParameterToProperty);
						continue;
					}
					if (rTPTrack.requestSorting)
					{
						Array.Sort(rtpParameterToProperty._envelope._points, (Fabric.TimelineComponent.Point a, Fabric.TimelineComponent.Point b) => a._x.CompareTo(b._x));
						rTPTrack.requestSorting = false;
					}
					GUILayout.BeginVertical();
					rTPTrack.tabIndex = GUIHelpers.Tabs(new string[2] { "Parameters", "Markers" }, rTPTrack.tabIndex, 100f);
					GUILayout.BeginVertical();
					GUILayout.BeginHorizontal("Box");
					if (rTPTrack.tabIndex == 0)
					{
						GUILayout.BeginVertical();
						GUILayout.BeginHorizontal();
						int num = rtpParameterToProperty._envelope._selectedPoint;
						if (num >= rtpParameterToProperty._envelope._points.Length)
						{
							num = 0;
							rtpParameterToProperty._envelope._selectedPoint = 0;
						}
						if (properties != null && propertyNames.Length > 0)
						{
							GUILayout.BeginHorizontal();
							GUILayout.Label("Property: ", GUILayout.MaxWidth(60f));
							int property = rtpParameterToProperty._property._property;
							int num2 = EditorGUILayout.Popup(property, propertyNames, GUILayout.MinWidth(126f));
							GUILayout.EndHorizontal();
							if (property != num2)
							{
								rtpParameterToProperty._property = properties[num2];
							}
							if (num2 == 0 || num2 == 1)
							{
								string[] displayedOptions = new string[5] { "=", "*", "\\", "+", "-" };
								property = (int)rtpParameterToProperty._propertyType;
								GUILayout.BeginHorizontal();
								GUIStyle gUIStyle = new GUIStyle(GUI.skin.FindStyle("Popup"));
								gUIStyle.alignment = TextAnchor.MiddleCenter;
								gUIStyle.fixedHeight = 15f;
								rtpParameterToProperty._propertyType = (RTPPropertyType)EditorGUILayout.Popup(property, displayedOptions, gUIStyle, GUILayout.MaxWidth(40f));
								GUILayout.EndHorizontal();
							}
						}
						GUILayout.BeginHorizontal();
						GUILayout.Label("Parameter: ", GUILayout.MaxWidth(70f));
						if (rtpParameterToProperty._type == RTPParameterType.User_Defined)
						{
							rtpParameterToProperty._parameter.Name = EditorGUILayout.TextField("", rtpParameterToProperty._parameter.Name);
						}
						else if (rtpParameterToProperty._type == RTPParameterType.Volume_Meter)
						{
							rtpParameterToProperty._volumeMeter = (VolumeMeter)EditorGUILayout.ObjectField("", rtpParameterToProperty._volumeMeter, typeof(VolumeMeter), true);
						}
						else if (rtpParameterToProperty._type == RTPParameterType.Global_Parameter)
						{
							string[] array = EventManager.Instance._globalParameterManager._globalRTParameters.Keys();
							if (array == null || array.Length == 0)
							{
								array = new string[1] { "None" };
							}
							for (int num3 = 0; num3 < array.Length; num3++)
							{
								if (rtpParameterToProperty._globalParameterName == array[num3])
								{
									selectedGlobalParameter = num3;
								}
							}
							selectedGlobalParameter = EditorGUILayout.Popup(selectedGlobalParameter, array, GUILayout.MinWidth(240f));
							rtpParameterToProperty._globalParameterName = array[selectedGlobalParameter];
						}
						else if (rtpParameterToProperty._type == RTPParameterType.Modulator)
						{
							rtpParameterToProperty._rtpModulator = (RTPModulator)EditorGUILayout.ObjectField("", rtpParameterToProperty._rtpModulator, typeof(RTPModulator), true);
						}
						else if (rtpParameterToProperty._type == RTPParameterType.Random)
						{
							rtpParameterToProperty._parameter.Name = "Random";
						}
						rtpParameterToProperty._type = (RTPParameterType)(object)EditorGUILayout.EnumPopup("", rtpParameterToProperty._type, GUILayout.MaxWidth(140f));
						GUILayout.EndHorizontal();
						if (!IsInternalRTPParameter(rtpParameterToProperty))
						{
							GUILayout.BeginHorizontal(GUILayout.MaxWidth(40f));
							GUILayout.Label("Reset On Play:");
							rtpParameterToProperty._parameter._resetToDefaultValue = EditorGUILayout.Toggle("", rtpParameterToProperty._parameter._resetToDefaultValue, GUILayout.MaxWidth(12f));
							GUILayout.EndHorizontal();
						}
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal();
						if (rtpParameterToProperty._property._property < propertyNames.Length && propertyNames[rtpParameterToProperty._property._property] == "Volume")
						{
							float value = AudioTools.LinearToDB(rtpParameterToProperty._envelope._points[num]._y);
							GUILayout.BeginHorizontal();
							GUILayout.Label("dB: ", GUILayout.MaxWidth(48f));
							value = EditorGUILayout.Slider(value, -96f, 0f);
							GUILayout.EndHorizontal();
							rtpParameterToProperty._envelope._points[num]._y = AudioTools.DBToLinear(value);
						}
						else
						{
							GUILayout.BeginHorizontal(GUILayout.Width(500f));
							GUILayout.Label("Y: ", GUILayout.MaxWidth(48f));
							rtpParameterToProperty._envelope._points[num]._y = GUILayout.HorizontalSlider(rtpParameterToProperty._envelope._points[num]._y, 0f, 1f);
							float value2 = RTPManager.CalculateNewValueRange(rtpParameterToProperty._envelope._points[num]._y, rtpParameterToProperty._property._min, rtpParameterToProperty._property._max, 0f, 1f);
							value2 = EditorGUILayout.FloatField(value2, GUILayout.MaxWidth(150f));
							value2 = RTPManager.CalculateNewValueRange(value2, 0f, 1f, rtpParameterToProperty._property._min, rtpParameterToProperty._property._max);
							rtpParameterToProperty._envelope._points[num]._y = value2;
							GUILayout.EndHorizontal();
						}
						if (rtpParameterToProperty._parameter != null)
						{
							GUILayout.BeginHorizontal();
							GUILayout.Label("X: ", GUILayout.MaxWidth(20f));
							float value3 = RTPManager.CalculateNewValueRange(rtpParameterToProperty._envelope._points[num]._x, rtpParameterToProperty._parameter._min, rtpParameterToProperty._parameter._max, 0f, 1f);
							value3 = EditorGUILayout.Slider(value3, rtpParameterToProperty._parameter._min, rtpParameterToProperty._parameter._max);
							GUILayout.EndHorizontal();
							if (num == 0)
							{
								rtpParameterToProperty._envelope._points[num]._x = 0f;
							}
							else if (rtpParameterToProperty._envelope._selectedPoint == rtpParameterToProperty._envelope._points.Length - 1)
							{
								rtpParameterToProperty._envelope._points[num]._x = 1f;
							}
							else
							{
								rtpParameterToProperty._envelope._points[num]._x = RTPManager.CalculateNewValueRange(value3, 0f, 1f, rtpParameterToProperty._parameter._min, rtpParameterToProperty._parameter._max);
							}
							string[] array2 = new string[rtpParameterToProperty._envelope._points.Length];
							for (int num4 = 0; num4 < array2.Length; num4++)
							{
								array2[num4] = "Point_" + num4;
							}
							GUILayout.BeginHorizontal(GUILayout.MaxWidth(100f));
							GUILayout.Label("Points: ", GUILayout.MaxWidth(48f));
							num = EditorGUILayout.Popup("", num, array2);
							rtpParameterToProperty._envelope._selectedPoint = num;
							GUILayout.EndHorizontal();
						}
						GUILayout.EndHorizontal();
						GUILayout.Space(5f);
						GUILayout.BeginHorizontal();
						if (rtpParameterToProperty._type != RTPParameterType.Global_Parameter)
						{
							GUILayout.BeginHorizontal(GUILayout.MaxWidth(48f));
							GUILayout.Label("Seek:", GUILayout.MinWidth(48f));
							rtpParameterToProperty._parameter._seekSpeed = EditorGUILayout.FloatField("", rtpParameterToProperty._parameter._seekSpeed);
							GUILayout.EndHorizontal();
						}
						if (!IsInternalRTPParameter(rtpParameterToProperty))
						{
							GUILayout.BeginHorizontal(GUILayout.MaxWidth(48f));
							GUILayout.Label("Speed:", GUILayout.MinWidth(48f));
							rtpParameterToProperty._parameter._velocity = EditorGUILayout.FloatField("", rtpParameterToProperty._parameter._velocity);
							GUILayout.EndHorizontal();
							GUILayout.BeginHorizontal(GUILayout.MinWidth(100f));
							GUILayout.Label("Loop:", GUILayout.MinWidth(48f));
							rtpParameterToProperty._parameter._loopBehaviour = (ParameterLoopBehaviour)(object)EditorGUILayout.EnumPopup("", rtpParameterToProperty._parameter._loopBehaviour, "Popup");
							GUILayout.EndHorizontal();
						}
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal();
						if (rtpParameterToProperty._type == RTPParameterType.Global_Parameter)
						{
							GUI.enabled = false;
						}
						GUILayout.BeginHorizontal(GUILayout.MaxWidth(48f));
						GUILayout.Label("Min:", GUILayout.MinWidth(48f));
						rTPTrack.min = EditorGUILayout.FloatField("", rTPTrack.min);
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal(GUILayout.MaxWidth(48f));
						GUILayout.Label("Max:", GUILayout.MinWidth(48f));
						rTPTrack.max = EditorGUILayout.FloatField("", rTPTrack.max);
						GUILayout.EndHorizontal();
						GUI.enabled = true;
						if (UnityEngine.Event.current.keyCode == KeyCode.Return && ((rTPTrack.max > 0f && rTPTrack.max != rtpParameterToProperty._parameter._max) || rTPTrack.min != rtpParameterToProperty._parameter._min))
						{
							Fabric.TimelineComponent.Point[] points = rtpParameterToProperty._envelope._points;
							foreach (Fabric.TimelineComponent.Point point in points)
							{
								if (point._locked)
								{
									float num6 = point._x * rtpParameterToProperty._parameter._max;
									if (num6 > rTPTrack.max || num6 < rTPTrack.min)
									{
										boundError = true;
										break;
									}
									if (rTPTrack.max != rtpParameterToProperty._parameter._max)
									{
										point._x = num6 / rTPTrack.max;
										rTPTrack.requestSorting = true;
									}
								}
							}
							if (!boundError)
							{
								rtpParameterToProperty._parameter._max = rTPTrack.max;
								rtpParameterToProperty._parameter._min = rTPTrack.min;
							}
							else
							{
								rTPTrack.max = rtpParameterToProperty._parameter._max;
								rTPTrack.min = rtpParameterToProperty._parameter._min;
							}
						}
						GUILayout.BeginHorizontal();
						GUILayout.Label("X:", GUILayout.MaxWidth(20f));
						rtpParameterToProperty._parameter.SetValue(EditorGUILayout.FloatField("", rtpParameterToProperty._parameter.GetValue()));
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal();
						GUILayout.Label("Y:", GUILayout.MaxWidth(20f));
						float num7 = rtpParameterToProperty._envelope.Calculate_y(rtpParameterToProperty._parameter.GetNormalisedValue());
						EditorGUILayout.LabelField(((!(propertyNames[rtpParameterToProperty._property._property] == "Volume")) ? RTPManager.CalculateNewValueRange(num7, rtpParameterToProperty._property._min, rtpParameterToProperty._property._max, 0f, 1f) : AudioTools.LinearToDB(num7)).ToString(), GUILayout.MaxWidth(50f));
						GUILayout.EndHorizontal();
						GUILayout.EndVertical();
						GUILayout.BeginHorizontal();
						if (GUILayout.Button("+", GUILayout.MaxWidth(40f)))
						{
							InsertPropertiesToParameters(rtpParameterToProperty);
							return;
						}
						if (GUILayout.Button("-", GUILayout.MaxWidth(40f)))
						{
							DeletePropertiesToParameters(rtpParameterToProperty);
							return;
						}
						GUILayout.EndHorizontal();
						GUILayout.EndVertical();
					}
					else if (rTPTrack.markersView.OnGUI(base.position.width, 100f))
					{
						rTPTrack.graphView._OnGUI = rTPTrack.markersView.OnGraphGUI;
					}
					GUILayout.EndHorizontal();
					if (GUILayoutUtility.GetLastRect().Contains(UnityEngine.Event.current.mousePosition) && UnityEngine.Event.current.type == EventType.ContextClick)
					{
						GenericMenu genericMenu2 = new GenericMenu();
						genericMenu2.AddItem(new GUIContent("Add Property"), on: false, AddPropertiesToParameters);
						genericMenu2.AddItem(new GUIContent("Delete Property"), on: false, DeletePropertiesToParameters, rtpParameterToProperty);
						genericMenu2.AddItem(new GUIContent("Copy Property"), on: false, CopyProperty, rtpParameterToProperty);
						if (parameterToPropertyCopied != null)
						{
							genericMenu2.AddItem(new GUIContent("Paste Property"), on: false, PasteProperty, rtpParameterToProperty);
						}
						genericMenu2.ShowAsContext();
						UnityEngine.Event.current.Use();
						break;
					}
					GUILayout.BeginHorizontal("Box");
					float normalisedValue = rtpParameterToProperty._parameter.GetNormalisedValue();
					float normalisedCurrentValue = rtpParameterToProperty._parameter.GetNormalisedCurrentValue();
					settings.timelineWidth = (int)base.position.width;
					normalisedValue = rTPTrack.graphView.DrawGraph(1f, 100, 0.2f, rtpParameterToProperty._parameter._min, rtpParameterToProperty._parameter._max, settings, normalisedValue, normalisedCurrentValue, rtpParameterToProperty._envelope);
					rtpParameterToProperty._parameter.SetNormalisedValue(normalisedValue);
					if (rTPTrack.graphView.isSelected)
					{
						SelectedGraph(rTPTrack.graphView);
					}
					GUILayout.EndHorizontal();
					GUILayout.EndHorizontal();
					GUILayout.EndVertical();
					GUILayout.Space(2f);
				}
			}
			GUILayout.EndScrollView();
			GUIHelpers.CheckGUIHasChanged(_component);
		}

		private bool IsInternalRTPParameter(RTPParameterToProperty parameterToProperty)
		{
			if (parameterToProperty._type == RTPParameterType.User_Defined)
			{
				return false;
			}
			return true;
		}

		public void Update()
		{
			if (Application.isPlaying)
			{
				if (timerRepaint <= 0f)
				{
					Repaint();
					timerRepaint = 0.16f;
				}
				else
				{
					timerRepaint -= FabricTimer.GetRealtimeDelta();
				}
			}
		}
	}
	[CustomEditor(typeof(TimelineParameter))]
	public class TimelineParameterEditor : Editor
	{
		private TimelineParameter timelineParameter;

		private EditorUndoManager undoManager;

		private void OnEnable()
		{
			timelineParameter = base.target as TimelineParameter;
			undoManager = new EditorUndoManager(timelineParameter, timelineParameter._name);
		}

		public override void OnInspectorGUI()
		{
			undoManager.CheckUndo();
			EditorGUILayout.LabelField("Parameter:", timelineParameter._name);
			float value = timelineParameter.GetValue();
			value = EditorGUILayout.Slider("value", value, timelineParameter._min, timelineParameter._max);
			timelineParameter.SetValue(value);
			timelineParameter._min = EditorGUILayout.FloatField("Min", timelineParameter._min);
			timelineParameter._max = EditorGUILayout.FloatField("Max", timelineParameter._max);
			undoManager.CheckDirty();
		}
	}
	[CustomEditor(typeof(TimelineRegion))]
	public class TimelineRegionEditor : Editor
	{
		private TimelineRegion timellineRegion;

		private SerializedProperty regionLoop;

		private SerializedProperty regionRandomComponent;

		private void OnEnable()
		{
			timellineRegion = base.target as TimelineRegion;
			regionLoop = base.serializedObject.FindProperty("_loop");
			regionRandomComponent = base.serializedObject.FindProperty("_component");
		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.PropertyField(regionLoop, new GUIContent("Loop"));
			EditorGUILayout.PropertyField(regionRandomComponent, new GUIContent("RandomComponent"));
			timellineRegion.SetLoop(timellineRegion._loop);
		}
	}
	[CustomEditor(typeof(TimelineLayer))]
	public class TimelineLayerEditor : Editor
	{
		private TimelineLayer timellineLayer;

		private void OnEnable()
		{
			timellineLayer = base.target as TimelineLayer;
		}

		public override void OnInspectorGUI()
		{
		}
	}
	[CanEditMultipleObjects]
	[CustomEditor(typeof(Fabric.TimelineComponent.TimelineComponent))]
	public class TimelineComponentEditor : Editor
	{
		private Fabric.TimelineComponent.TimelineComponent timellineComponent;

		private ComponentEditor componentEditor = new ComponentEditor();

		private SerializedProperty oneShot;

		private bool _foldout = true;

		[MenuItem("Fabric/Components/TimelineComponent")]
		private static void About()
		{
			GameObject activeGameObject = Selection.activeGameObject;
			if (!(activeGameObject == null))
			{
				GameObject gameObject = new GameObject("TimelineComponent");
				gameObject.transform.parent = activeGameObject.transform;
				gameObject.AddComponent<Fabric.TimelineComponent.TimelineComponent>();
			}
		}

		private void OnEnable()
		{
			timellineComponent = base.target as Fabric.TimelineComponent.TimelineComponent;
			componentEditor.RegisterSerialisableObject(base.serializedObject);
			oneShot = base.serializedObject.FindProperty("_isOneShot");
		}

		public override void OnInspectorGUI()
		{
			componentEditor.InspectorGUI(timellineComponent, "288048-timelinecomponent");
			base.serializedObject.Update();
			_foldout = EditorGUILayout.Foldout(_foldout, "Timeline Component Properties");
			if (_foldout)
			{
				GUILayout.BeginVertical("box");
				EditorGUILayout.PropertyField(oneShot, new GUIContent("OneShot"));
				GUILayout.EndVertical();
			}
			GUIHelpers.CheckGUIHasChanged(base.serializedObject, timellineComponent.gameObject);
			if (FabricManager.IsInitialised() && FabricEditorData.GetData()._playmodePersistance)
			{
				PlaymodePersistance.SetGameObject(timellineComponent.gameObject);
			}
		}
	}
	public class TimelineLoader : AssetPostprocessor
	{
		public static bool ignoreLoading;

		private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
		{
			foreach (string text in importedAssets)
			{
				if (text.Contains(".ftp") && !text.Contains(".bak"))
				{
					LoadFtpProject(text);
				}
			}
		}

		private static GameObject FindOrCreateGroupComponent(string path)
		{
			GameObject gameObject = GameObject.Find(path);
			if (gameObject != null)
			{
				return gameObject;
			}
			FabricManager fabricManager = GetFabricManager.Instance();
			string[] array = path.Split('/');
			gameObject = fabricManager.gameObject;
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject2 = new GameObject(array[i]);
				gameObject2.AddComponent<GroupComponent>();
				gameObject2.transform.parent = gameObject.transform;
				gameObject = gameObject2;
			}
			return gameObject;
		}

		public static void LoadFtpProject(string filename)
		{
			FabricManager fabricManager = GetFabricManager.Instance();
			if (fabricManager == null || !fabricManager.enableTimelineAssetLoader)
			{
				return;
			}
			GameObject gameObject = fabricManager.gameObject;
			if (Selection.activeGameObject != null)
			{
				gameObject = Selection.activeGameObject;
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(filename);
			string text = "";
			GameObject gameObject2 = null;
			XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("Project");
			if (elementsByTagName.Count > 0)
			{
				XmlNode xmlNode = elementsByTagName[0];
				text = xmlNode["name"].InnerText;
				string name = "/" + fabricManager.gameObject.name + "/" + text;
				gameObject2 = GameObject.Find(name);
				if (gameObject2 == null)
				{
					gameObject2 = new GameObject(text);
					gameObject2.AddComponent<GroupComponent>();
					gameObject2.transform.parent = gameObject.transform;
				}
			}
			if (gameObject2 == null)
			{
				return;
			}
			RandomPoolManager componentInChildren = gameObject2.GetComponentInChildren<RandomPoolManager>();
			if (componentInChildren != null)
			{
				UnityEngine.Object.DestroyImmediate(componentInChildren.gameObject);
			}
			RandomPoolManager randomPoolManager = null;
			XmlNodeList elementsByTagName2 = xmlDocument.GetElementsByTagName("RandomComponentPool");
			if (elementsByTagName2.Count > 0)
			{
				XmlNode node = elementsByTagName2[0];
				GameObject gameObject3 = new GameObject("RandomComponentPool");
				gameObject3.transform.parent = gameObject2.transform;
				randomPoolManager = gameObject3.AddComponent<RandomPoolManager>();
				randomPoolManager.projectName = text;
				SerialiseRandomPoolManager(randomPoolManager, node);
			}
			XmlNodeList elementsByTagName3 = xmlDocument.GetElementsByTagName("TimelineComponent");
			for (int i = 0; i < elementsByTagName3.Count; i++)
			{
				XmlNode xmlNode2 = elementsByTagName3[i];
				string innerText = xmlNode2["name"].InnerText;
				_ = xmlNode2["group_component"].InnerText;
				GameObject gameObject4 = gameObject2;
				string name2 = "/" + fabricManager.gameObject.name + "/" + text + "/" + innerText;
				GameObject gameObject5 = GameObject.Find(name2);
				if (gameObject5 != null)
				{
					UnityEngine.Object.DestroyImmediate(gameObject5);
				}
				GameObject gameObject6 = new GameObject(innerText);
				gameObject6.transform.parent = gameObject4.transform;
				Fabric.TimelineComponent.TimelineComponent timeline = gameObject6.AddComponent<Fabric.TimelineComponent.TimelineComponent>();
				SerialiseTimeline(timeline, xmlNode2, randomPoolManager);
			}
			XmlNodeList elementsByTagName4 = xmlDocument.GetElementsByTagName("SimpleRandomComponent");
			for (int j = 0; j < elementsByTagName4.Count; j++)
			{
				XmlNode xmlNode3 = elementsByTagName4[j];
				string innerText2 = xmlNode3["name"].InnerText;
				_ = xmlNode3["group_component"].InnerText;
				GameObject gameObject7 = gameObject2;
				string name3 = "/" + fabricManager.gameObject.name + "/" + text + "/" + innerText2;
				GameObject gameObject8 = GameObject.Find(name3);
				if (gameObject8 != null)
				{
					UnityEngine.Object.DestroyImmediate(gameObject8);
				}
				GameObject gameObject9 = new GameObject(innerText2);
				gameObject9.transform.parent = gameObject7.transform;
				RandomComponent randomComponent = gameObject9.AddComponent<RandomComponent>();
				SerialiseRandomComponent(randomComponent, xmlNode3, text);
				EventListener eventListener = randomComponent.gameObject.AddComponent<EventListener>();
				eventListener._eventName = randomComponent.gameObject.name;
			}
		}

		private static void SerialiseRandomComponent(RandomComponent randomComponent, XmlNode node, string projectname)
		{
			float db = float.Parse(node["volume"].InnerText);
			randomComponent.Volume = AudioTools.DBToLinear(db);
			float db2 = float.Parse(node["volume_randomization"].InnerText);
			randomComponent.VolumeRandomization = 1f - AudioTools.DBToLinear(db2);
			randomComponent.Pitch = 1f + float.Parse(node["pitch"].InnerText);
			randomComponent.PitchRandomization = float.Parse(node["pitch_randomization"].InnerText);
			randomComponent.Priority = int.Parse(node["priority"].InnerText);
			randomComponent.MaxInstances = int.Parse(node["max_instances"].InnerText);
			switch (node["stealing_mode"].InnerText)
			{
			case "steal_oldest":
				randomComponent.StealingBehaviour = ComponentStealingBehaviour.Oldest;
				break;
			case "steal_newest":
				randomComponent.StealingBehaviour = ComponentStealingBehaviour.Newest;
				break;
			case "steal_farthest":
				randomComponent.StealingBehaviour = ComponentStealingBehaviour.Farthest;
				break;
			default:
				randomComponent.StealingBehaviour = ComponentStealingBehaviour.None;
				break;
			}
			randomComponent.MinDistance = int.Parse(node["min_distance"].InnerText);
			randomComponent.MaxDistance = int.Parse(node["max_distance"].InnerText);
			switch (node["rolloff_mode"].InnerText)
			{
			case "logarithmic":
				randomComponent.RolloffMode = AudioRolloffMode.Logarithmic;
				break;
			case "linear":
				randomComponent.RolloffMode = AudioRolloffMode.Linear;
				break;
			case "custom":
				randomComponent.RolloffMode = AudioRolloffMode.Custom;
				break;
			}
			int num = int.Parse(node["fade_in_time"].InnerText);
			if (num > 0)
			{
				randomComponent.FadeInTime = (float)num / 1000f;
			}
			int num2 = int.Parse(node["fade_out_time"].InnerText);
			if (num2 > 0)
			{
				randomComponent.FadeOutTime = (float)num2 / 1000f;
			}
			for (int i = 0; i < node.ChildNodes.Count; i++)
			{
				if (node.ChildNodes[i].Name == "AudioComponent")
				{
					GameObject gameObject = new GameObject();
					string text = (gameObject.name = node.ChildNodes[i].InnerText);
					gameObject.transform.parent = randomComponent.transform;
					AudioComponent audioComponent = gameObject.AddComponent<AudioComponent>();
					string text2 = text.Replace(".wav", "");
					audioComponent.AudioClip = Resources.Load("Audio/" + projectname + "/" + text2) as AudioClip;
				}
			}
		}

		private static void SerialiseTimeline(Fabric.TimelineComponent.TimelineComponent timeline, XmlNode node, RandomPoolManager RandomPoolManager)
		{
			string innerText = node["one_shot"].InnerText;
			if (innerText == "Yes")
			{
				timeline.SetOneShot(isOneShot: true);
			}
			else
			{
				timeline.SetOneShot(isOneShot: false);
			}
			float db = float.Parse(node["volume"].InnerText);
			timeline.Volume = AudioTools.DBToLinear(db);
			float db2 = float.Parse(node["volume_randomization"].InnerText);
			timeline.VolumeRandomization = 1f - AudioTools.DBToLinear(db2);
			timeline.Pitch = 1f + float.Parse(node["pitch"].InnerText);
			timeline.PitchRandomization = float.Parse(node["pitch_randomization"].InnerText);
			timeline.Priority = int.Parse(node["priority"].InnerText);
			timeline.MaxInstances = int.Parse(node["max_instances"].InnerText);
			switch (node["stealing_mode"].InnerText)
			{
			case "steal_oldest":
				timeline.StealingBehaviour = ComponentStealingBehaviour.Oldest;
				break;
			case "steal_newest":
				timeline.StealingBehaviour = ComponentStealingBehaviour.Newest;
				break;
			case "steal_farthest":
				timeline.StealingBehaviour = ComponentStealingBehaviour.Farthest;
				break;
			default:
				timeline.StealingBehaviour = ComponentStealingBehaviour.None;
				break;
			}
			timeline.MinDistance = int.Parse(node["min_distance"].InnerText);
			timeline.MaxDistance = int.Parse(node["max_distance"].InnerText);
			switch (node["rolloff_mode"].InnerText)
			{
			case "logarithmic":
				timeline.RolloffMode = AudioRolloffMode.Logarithmic;
				break;
			case "linear":
				timeline.RolloffMode = AudioRolloffMode.Linear;
				break;
			case "custom":
				timeline.RolloffMode = AudioRolloffMode.Custom;
				break;
			}
			int num = int.Parse(node["fade_in_time"].InnerText);
			if (num > 0)
			{
				timeline.FadeInTime = (float)num / 1000f;
			}
			int num2 = int.Parse(node["fade_out_time"].InnerText);
			if (num2 > 0)
			{
				timeline.FadeOutTime = (float)num2 / 1000f;
			}
			XmlNodeList xmlNodeList = node.SelectNodes("Parameter");
			if (xmlNodeList.Count > 0)
			{
				for (int i = 0; i < xmlNodeList.Count; i++)
				{
					XmlNode xmlNode = xmlNodeList[i];
					string text = "parameter";
					text = xmlNode["name"].InnerText;
					TimelineParameter timelineParameter = timeline.gameObject.AddComponent<TimelineParameter>();
					timelineParameter._name = text;
					SerialiseParameter(timelineParameter, xmlNode);
				}
			}
			XmlNodeList xmlNodeList2 = node.SelectNodes("Layer");
			if (xmlNodeList2.Count > 0)
			{
				for (int j = 0; j < xmlNodeList2.Count; j++)
				{
					XmlNode xmlNode2 = xmlNodeList2[j];
					string text2 = "layer";
					text2 = xmlNode2["name"].InnerText;
					GameObject gameObject = new GameObject(text2);
					gameObject.transform.parent = timeline.transform;
					TimelineLayer layer = gameObject.AddComponent<TimelineLayer>();
					SerialiseLayer(layer, xmlNode2, RandomPoolManager);
				}
				EventListener eventListener = timeline.gameObject.AddComponent<EventListener>();
				eventListener._eventName = timeline.gameObject.name;
			}
		}

		private static void SerialiseLayer(TimelineLayer layer, XmlNode node, RandomPoolManager RandomPoolManager)
		{
			TimelineParameter[] componentsInChildren = layer.gameObject.transform.parent.GetComponentsInChildren<TimelineParameter>();
			if (componentsInChildren.Length > 0)
			{
				string innerXml = node["control_parameter"].InnerXml;
				TimelineParameter[] array = componentsInChildren;
				foreach (TimelineParameter timelineParameter in array)
				{
					if (timelineParameter._name == innerXml)
					{
						layer._controlParameter = timelineParameter;
						break;
					}
				}
			}
			XmlNodeList xmlNodeList = node.SelectNodes("Envelope");
			layer._parameters = new ParameterToProperty[xmlNodeList.Count];
			for (int j = 0; j < xmlNodeList.Count; j++)
			{
				XmlNode xmlNode = xmlNodeList[j];
				ParameterToProperty parameterToProperty = new ParameterToProperty();
				string innerXml2 = xmlNode["property_name"].InnerXml;
				bool flag = false;
				if (innerXml2 == "volume")
				{
					parameterToProperty._property = Property.Volume;
					flag = true;
				}
				else if (innerXml2 == "pitch")
				{
					parameterToProperty._property = Property.Pitch;
					flag = true;
				}
				if (!flag)
				{
					continue;
				}
				string innerXml3 = xmlNode["parameter_name"].InnerXml;
				TimelineParameter[] array2 = componentsInChildren;
				foreach (TimelineParameter timelineParameter2 in array2)
				{
					if (timelineParameter2._name == innerXml3)
					{
						parameterToProperty._parameter = timelineParameter2;
						break;
					}
				}
				XmlNodeList xmlNodeList2 = xmlNode.SelectNodes("Point");
				parameterToProperty._envelope = new Fabric.TimelineComponent.Envelope();
				parameterToProperty._envelope._points = new Fabric.TimelineComponent.Point[xmlNodeList2.Count];
				for (int l = 0; l < xmlNodeList2.Count; l++)
				{
					XmlNode xmlNode2 = xmlNodeList2[l];
					string[] array3 = xmlNode2.InnerXml.Split(',');
					CurveTypes curveType = layer.ConvertEnvelopeTypes((EnvelopPointTypes)int.Parse(array3[2]));
					parameterToProperty._envelope._points[l] = Fabric.TimelineComponent.Point.Alloc(float.Parse(array3[0]), float.Parse(array3[1]), curveType);
				}
				layer._parameters[j] = parameterToProperty;
			}
			XmlNodeList xmlNodeList3 = node.SelectNodes("Region");
			layer._regions = new TimelineRegion[xmlNodeList3.Count];
			for (int m = 0; m < xmlNodeList3.Count; m++)
			{
				XmlNode xmlNode3 = xmlNodeList3[m];
				string text = "region";
				text = xmlNode3["name"].InnerText;
				text = text.Replace("/", "");
				GameObject gameObject = new GameObject(text);
				gameObject.transform.parent = layer.transform;
				TimelineRegion timelineRegion = gameObject.AddComponent<TimelineRegion>();
				if (timelineRegion != null)
				{
					SerialiseRegion(timelineRegion, xmlNode3, RandomPoolManager);
					layer._regions[m] = timelineRegion;
				}
			}
			layer.UpdateRegionEnvelopes();
		}

		private static void SerialiseRegion(TimelineRegion region, XmlNode node, RandomPoolManager RandomPoolManager)
		{
			string innerText = node["name"].InnerText;
			region._x = float.Parse(node["x"].InnerXml);
			region._width = float.Parse(node["width"].InnerText);
			region._autopitchenabled = Convert.ToBoolean(int.Parse(node["auto_pitch_enabled"].InnerText));
			region._autopitchreference = float.Parse(node["auto_pitch_reference"].InnerText);
			region._regionVolume = float.Parse(node["volume"].InnerText);
			int num = int.Parse(node["loop_mode"].InnerText);
			if (num == 1)
			{
				region._loop = false;
				region._loopMode = RegionLoopMode.PlayToEnd;
			}
			else
			{
				region._loop = true;
			}
			region._fadeInType = (CurveTypes)int.Parse(node["fade_in_type"].InnerText);
			region._fadeOutType = (CurveTypes)int.Parse(node["fade_out_type"].InnerText);
			region.ResetVolumeEnvelope();
			if (RandomPoolManager._definitionsTable.ContainsKey(innerText))
			{
				region._component = RandomPoolManager._definitionsTable[innerText];
			}
		}

		private static void SerialiseParameter(TimelineParameter parameter, XmlNode node)
		{
			switch (int.Parse(node["loop_mode"].InnerText))
			{
			case 0:
			case 1:
				parameter._loopBehaviour = ParameterLoopBehaviour.OneShot;
				break;
			case 2:
				parameter._loopBehaviour = ParameterLoopBehaviour.Loop;
				break;
			}
			parameter._velocity = float.Parse(node["speed"].InnerText);
			parameter._seekSpeed = float.Parse(node["seek"].InnerText);
			parameter._min = float.Parse(node["min"].InnerText);
			parameter._max = float.Parse(node["max"].InnerText);
		}

		private static void SerialiseRandomPoolManager(RandomPoolManager randomPoolManager, XmlNode node)
		{
			XmlNodeList xmlNodeList = node.SelectNodes("RandomComponent");
			for (int i = 0; i < xmlNodeList.Count; i++)
			{
				XmlNode xmlNode = xmlNodeList[i];
				string innerText = xmlNode["name"].InnerText;
				GameObject gameObject = new GameObject(innerText);
				gameObject.transform.parent = randomPoolManager.gameObject.transform;
				RandomComponent value = gameObject.AddComponent<RandomComponent>();
				randomPoolManager._definitionsTable.Add(innerText, value);
				XmlNodeList xmlNodeList2 = xmlNode.SelectNodes("Audio");
				for (int j = 0; j < xmlNodeList2.Count; j++)
				{
					XmlNode xmlNode2 = xmlNodeList2[j];
					string innerText2 = xmlNode2["filename"].InnerText;
					innerText2 = innerText2.Substring(innerText2.LastIndexOf('/') + 1);
					GameObject gameObject2 = new GameObject(innerText2);
					gameObject2.transform.parent = gameObject.transform;
					AudioComponent audioComponent = gameObject2.AddComponent<AudioComponent>();
					audioComponent.Loop = true;
					string innerText3 = xmlNode2["filename"].InnerText;
					innerText3 = innerText3.Replace(".wav", "");
					audioComponent.AudioClip = Resources.Load("Audio/" + randomPoolManager.projectName + "/" + innerText3) as AudioClip;
				}
			}
		}
	}
	[CanEditMultipleObjects]
	[CustomEditor(typeof(ExternalGroupComponent))]
	public class ExternalGroupComponentEditor : Editor
	{
		[MenuItem("Fabric/Utils/ExternalGroupComponent")]
		private static void Create()
		{
			GameObject gameObject = new GameObject("ExternalGroupComponent");
			gameObject.AddComponent<ExternalGroupComponent>();
			GameObject activeGameObject = Selection.activeGameObject;
			if (activeGameObject != null)
			{
				gameObject.transform.parent = activeGameObject.transform;
			}
		}
	}
	[CanEditMultipleObjects]
	[CustomEditor(typeof(GroupComponent))]
	public class GroupComponentEditor : DragAndDropArea
	{
		private GroupComponent groupComponent;

		private ComponentEditor componentEditor = new ComponentEditor();

		private SerializedProperty showInMixerView;

		private SerializedProperty ignoreUnloadUnusedAssets;

		private SerializedProperty notifyParentComponent;

		[MenuItem("Fabric/Components/GroupComponent")]
		private static void About()
		{
			GameObject gameObject = new GameObject("GroupComponent");
			GameObject activeGameObject = Selection.activeGameObject;
			if (activeGameObject != null)
			{
				gameObject.transform.parent = activeGameObject.transform;
				gameObject.AddComponent<GroupComponent>();
			}
		}

		[MenuItem("Fabric/GroupComponent (External)")]
		private static void Create()
		{
			GameObject gameObject = new GameObject("GroupComponent");
			gameObject.AddComponent<GroupComponent>();
			GameObject activeGameObject = Selection.activeGameObject;
			if (activeGameObject != null)
			{
				gameObject.transform.parent = activeGameObject.transform;
			}
		}

		private void OnEnable()
		{
			groupComponent = base.target as GroupComponent;
			componentEditor.RegisterSerialisableObject(base.serializedObject);
			showInMixerView = base.serializedObject.FindProperty("_showInMixerView");
			ignoreUnloadUnusedAssets = base.serializedObject.FindProperty("_ignoreUnloadUnusedAssets");
			notifyParentComponent = base.serializedObject.FindProperty("_notifyParentComponent");
		}

		private void OnDestroy()
		{
		}

		public override void OnInspectorGUI()
		{
			componentEditor.InspectorGUI(groupComponent, "288011-groupcomponent");
			Mixer.MixerSlot("", groupComponent, useFaderImages: false);
			GUILayout.BeginVertical("Box");
			EditorGUILayout.PropertyField(showInMixerView, new GUIContent("Show In Mixer View:"));
			EditorGUILayout.PropertyField(notifyParentComponent, new GUIContent("Notify Parent When: "));
			GUILayout.EndVertical();
			if (!groupComponent.IsFabricHierarchyPresent())
			{
				EditorGUILayout.PropertyField(ignoreUnloadUnusedAssets, new GUIContent("Ignore UnloadUnusedAssets:"));
				GUILayout.BeginVertical("Box");
				string label = "Drop Target GroupComponent";
				bool set = false;
				if (groupComponent._targetGroupComponentPath != null && groupComponent._targetGroupComponentPath.Length > 0)
				{
					label = groupComponent._targetGroupComponentPath;
					set = true;
				}
				GUILayout.BeginHorizontal();
				DragAndDropAudioClip(100f, 25f, label, set);
				if (GUILayout.Button("Clear", GUILayout.MaxWidth(60f)))
				{
					groupComponent._targetGroupComponentPath = "";
					groupComponent.UnregisterWithMainHierarchy();
					groupComponent.RegisterWithMainHierarchy();
				}
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}
			if (groupComponent._isRegisteredWithMainHierarchy)
			{
				GUILayout.BeginVertical("Box");
				Color backgroundColor = GUI.backgroundColor;
				GUI.color = Color.green;
				GUILayout.Label("Registered with main Hierarchy [ " + groupComponent._registeredWithMainRefCount + " ]");
				GUI.color = backgroundColor;
				GUILayout.EndVertical();
			}
			GUIHelpers.CheckGUIHasChanged(base.serializedObject, groupComponent);
		}

		protected override void OnDrop(UnityEngine.Object[] dragged_objects)
		{
			GameObject gameObject = dragged_objects[0] as GameObject;
			if (gameObject != null)
			{
				Component component = gameObject.GetComponent<Component>();
				if (component != null)
				{
					string path = "";
					int depth = 0;
					component.BuildComponentEventPathName(ref path, depth);
					groupComponent._targetGroupComponentPath = FabricManager.Instance.name + "_" + path.Replace("/", "_");
					groupComponent.UnregisterWithMainHierarchy();
					groupComponent.RegisterWithMainHierarchy();
				}
			}
		}
	}
	[Serializable]
	public class EventLogEntry
	{
		[SerializeField]
		public EventAction _eventAction;

		[SerializeField]
		public object _value;

		[SerializeField]
		public float _triggerTime;

		[SerializeField]
		public string _descritpion;

		[SerializeField]
		public GameObject _gameObject;

		[SerializeField]
		public string _gameObjectName;

		[SerializeField]
		public Event _audioEvent;
	}
	[InitializeOnLoad]
	internal class EventViewerHandlerRegisration
	{
		static EventViewerHandlerRegisration()
		{
			EventManager.OnLogEvent += OnLogEvent;
			EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(PlayModeChanged));
		}

		private static void PlayModeChanged()
		{
			if (FabricEditorData.GetData()._resetEventLogOnPlayModeChange)
			{
				FabricEditorData.GetData()._audioEventHistory.Clear();
			}
		}

		public static T DeepCopy<T>(T other)
		{
			using MemoryStream memoryStream = new MemoryStream();
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, other);
			memoryStream.Position = 0L;
			return (T)binaryFormatter.Deserialize(memoryStream);
		}

		public static bool IsSerializable(object obj)
		{
			Type type = obj.GetType();
			return type.IsSerializable;
		}

		private static void OnLogEvent(Event postedEvent)
		{
			if (FabricEditorData.GetData()._logHistorySize > 0)
			{
				EventLogEntry eventLogEntry = new EventLogEntry();
				Event obj = new Event();
				obj.Copy(postedEvent);
				eventLogEntry._audioEvent = obj;
				eventLogEntry._descritpion = obj._eventName;
				eventLogEntry._eventAction = obj.EventAction;
				if (obj._parameter != null && IsSerializable(obj._parameter))
				{
					eventLogEntry._value = DeepCopy(obj._parameter);
				}
				else
				{
					eventLogEntry._value = obj._parameter;
				}
				eventLogEntry._triggerTime = FabricTimer.Get();
				eventLogEntry._gameObject = obj.parentGameObject;
				if (eventLogEntry._gameObject != null)
				{
					eventLogEntry._gameObjectName = eventLogEntry._gameObject.name;
				}
				else
				{
					eventLogEntry._gameObjectName = "Undefined";
				}
				if (FabricEditorData.GetData()._audioEventHistory.Count > FabricEditorData.GetData()._logHistorySize)
				{
					FabricEditorData.GetData()._audioEventHistory.Dequeue();
				}
				FabricEditorData.GetData()._audioEventHistory.Enqueue(eventLogEntry);
			}
		}
	}
	internal class EventViewerFilterSelection : EditorWindow
	{
		private Vector2 filterScrollPosition = default(Vector2);

		public static EventViewerFilterSelection window;

		public static void Open(float x, float y)
		{
			if (window == null)
			{
				window = ScriptableObject.CreateInstance<EventViewerFilterSelection>();
				window.position = new Rect(x, y, 280f, 200f);
				window.name = "Log Filter";
				window.Show();
			}
		}

		private void OnGUI()
		{
			filterScrollPosition = GUILayout.BeginScrollView(filterScrollPosition, "Box");
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Check All"))
			{
				for (int i = 0; i < FabricEditorData.GetData()._eventLogFilterEventActions.Length; i++)
				{
					FabricEditorData.GetData()._eventLogFilterEventActions[i] = true;
				}
			}
			if (GUILayout.Button("Uncheck All"))
			{
				for (int j = 0; j < FabricEditorData.GetData()._eventLogFilterEventActions.Length; j++)
				{
					FabricEditorData.GetData()._eventLogFilterEventActions[j] = false;
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			GUILayout.BeginVertical();
			string[] names = Enum.GetNames(typeof(EventAction));
			for (int k = 0; k < FabricEditorData.GetData()._eventLogFilterEventActions.Length; k++)
			{
				if (k < names.Length)
				{
					FabricEditorData.GetData()._eventLogFilterEventActions[k] = GUILayout.Toggle(FabricEditorData.GetData()._eventLogFilterEventActions[k], names[k]);
				}
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			if (GUILayout.Button("Close"))
			{
				Close();
			}
		}
	}
	public class EventViewerEditor : EditorWindow
	{
		private enum Widths
		{
			TIMESTAMP = 100,
			EVENT_NAME = 150,
			EVENT_ACTION = 200,
			VALUE = 200,
			DESCRIPTION = 200,
			COMPONENT = 125,
			GAMEOBJECT = 125,
			STATUS = 150
		}

		private Vector2 scrollPosition = default(Vector2);

		private bool pause;

		private bool current = true;

		private string filter = "";

		public GUIStyle normalCenteredStyle = new GUIStyle();

		[MenuItem("Window/Fabric/Event Log", false, 5)]
		private static void Init()
		{
			EventViewerEditor eventViewerEditor = (EventViewerEditor)EditorWindow.GetWindow(typeof(EventViewerEditor));
			eventViewerEditor.title = "EventLog";
		}

		private bool WriteHeaderText(string text, float width = 125f)
		{
			int fontSize = normalCenteredStyle.fontSize;
			normalCenteredStyle.fontSize = 22;
			normalCenteredStyle.alignment = TextAnchor.LowerCenter;
			normalCenteredStyle.normal.textColor = Color.grey;
			GUILayout.Label(text, normalCenteredStyle, GUILayout.MinWidth(width));
			bool result = false;
			normalCenteredStyle.fontSize = fontSize;
			return result;
		}

		private void WriteEventText(string text, float width = 125f)
		{
			int fontSize = normalCenteredStyle.fontSize;
			normalCenteredStyle.fontSize = 16;
			normalCenteredStyle.alignment = TextAnchor.LowerCenter;
			normalCenteredStyle.normal.textColor = FabricEditorData.GetData()._eventLogColor;
			GUILayout.Label(text, normalCenteredStyle, GUILayout.MinWidth(width));
			normalCenteredStyle.fontSize = fontSize;
		}

		private void Open()
		{
		}

		private void DisplayEvents()
		{
			GUILayout.Space(5f);
			GUIStyle gUIStyle = new GUIStyle(GUI.skin.label);
			gUIStyle.alignment = TextAnchor.MiddleCenter;
			GUILayout.BeginHorizontal("Box");
			WriteHeaderText("Timestamp", 100f);
			WriteHeaderText("EventAction", 200f);
			WriteHeaderText("Value", 200f);
			WriteHeaderText("Description", 200f);
			WriteHeaderText("GameObject");
			WriteHeaderText("Status", 150f);
			GUILayout.EndHorizontal();
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			GUILayout.BeginVertical();
			Rect rect = new Rect(0f, 0f, 5000f, 100f);
			rect.y = 0f;
			object[] array = FabricEditorData.GetData()._audioEventHistory.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				EventLogEntry eventLogEntry = (EventLogEntry)array[i];
				if (eventLogEntry == null)
				{
					continue;
				}
				bool flag = true;
				if ((int)eventLogEntry._eventAction < FabricEditorData.GetData()._eventLogFilterEventActions.Length)
				{
					flag = FabricEditorData.GetData()._eventLogFilterEventActions[(int)eventLogEntry._eventAction];
				}
				if ((filter.Length > 0 && !eventLogEntry._descritpion.Contains(filter)) || !flag)
				{
					continue;
				}
				GUILayout.BeginHorizontal("Box");
				WriteEventText(eventLogEntry._triggerTime.ToString("F2"), 100f);
				WriteEventText(eventLogEntry._eventAction.ToString("F"), 200f);
				if (eventLogEntry._value != null)
				{
					if (EventAction.SetParameter == eventLogEntry._eventAction)
					{
						ParameterData parameterData = (ParameterData)eventLogEntry._value;
						WriteEventText(parameterData._value.ToString("F"), 200f);
					}
					else if (EventAction.SetDSPParameter == eventLogEntry._eventAction)
					{
						DSPParameterData dSPParameterData = (DSPParameterData)eventLogEntry._value;
						WriteEventText(dSPParameterData._value.ToString("F"), 200f);
					}
					else if (EventAction.SetGlobalParameter == eventLogEntry._eventAction)
					{
						GlobalParameterData globalParameterData = (GlobalParameterData)eventLogEntry._value;
						WriteEventText("'" + globalParameterData._name + "' Set to '" + globalParameterData._value.ToString("F") + "'", 200f);
					}
					else if (EventAction.SetGlobalSwitch == eventLogEntry._eventAction)
					{
						GlobalSwitchParameterData globalSwitchParameterData = (GlobalSwitchParameterData)eventLogEntry._value;
						WriteEventText("'" + globalSwitchParameterData._name + "' Switch to '" + globalSwitchParameterData._switch.ToString() + "'", 200f);
					}
					else if (EventAction.RegisterGameObject == eventLogEntry._eventAction || EventAction.SetGameObject == eventLogEntry._eventAction)
					{
						EditorGUILayout.ObjectField(eventLogEntry._gameObject, typeof(GameObject), true, GUILayout.Width(125f));
					}
					else if (EventAction.SetVolume == eventLogEntry._eventAction || EventAction.SetPitch == eventLogEntry._eventAction || EventAction.SetPan == eventLogEntry._eventAction || EventAction.SetFadeIn == eventLogEntry._eventAction || EventAction.SetFadeOut == eventLogEntry._eventAction || EventAction.SetTime == eventLogEntry._eventAction)
					{
						WriteEventText(((float)eventLogEntry._value).ToString("F"), 200f);
					}
					else if (EventAction.AddPreset == eventLogEntry._eventAction || EventAction.RemovePreset == eventLogEntry._eventAction || EventAction.SetSwitch == eventLogEntry._eventAction)
					{
						WriteEventText(((string)eventLogEntry._value).ToString(), 200f);
					}
					else
					{
						WriteEventText("-", 200f);
					}
				}
				else
				{
					WriteEventText("-", 200f);
				}
				WriteEventText(eventLogEntry._descritpion, 200f);
				if ((bool)eventLogEntry._gameObject)
				{
					EditorGUILayout.ObjectField(eventLogEntry._gameObject, typeof(GameObject), true, GUILayout.Width(125f));
				}
				else
				{
					WriteEventText("-");
				}
				if (eventLogEntry._audioEvent == null)
				{
					WriteEventText("Handled(Destroyed)", 150f);
				}
				else
				{
					WriteEventText(eventLogEntry._audioEvent.eventStatus.ToString(), 150f);
				}
				GUILayout.EndHorizontal();
				rect.y += 20f;
			}
			if (current)
			{
				scrollPosition.y = rect.y;
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
		}

		private void OnGUI()
		{
			if (!(EventManager.Instance == null))
			{
				GUILayout.BeginVertical("box");
				GUILayout.BeginHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Text Color: ", GUILayout.MaxWidth(80f));
				FabricEditorData.GetData()._eventLogColor = EditorGUILayout.ColorField(FabricEditorData.GetData()._eventLogColor);
				GUILayout.EndHorizontal();
				pause = GUILayout.Toggle(pause, "Pause", "button", GUILayout.MinWidth(200f));
				if (pause)
				{
					UnityEngine.Debug.Break();
				}
				if (GUILayout.Button("Clear", GUILayout.MinWidth(200f)))
				{
					FabricEditorData.GetData()._audioEventHistory.Clear();
				}
				current = GUILayout.Toggle(current, "Current");
				FabricEditorData.GetData()._resetEventLogOnPlayModeChange = GUILayout.Toggle(FabricEditorData.GetData()._resetEventLogOnPlayModeChange, "Clear On Playmode change");
				MenuBar.OnGUI("288093-eventlog");
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Filter", GUILayout.MaxWidth(70f));
				filter = GUILayout.TextField(filter, GUILayout.MinWidth(180f));
				if (GUILayout.Button("Filter"))
				{
					EventViewerFilterSelection.Open(UnityEngine.Event.current.mousePosition.x, UnityEngine.Event.current.mousePosition.y);
				}
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
				DisplayEvents();
				FabricEditorData.ApplyChanges();
			}
		}

		private void OnInspectorUpdate()
		{
			Repaint();
		}
	}
	public class Drawing
	{
		public static Texture2D aaLineTex;

		public static Texture2D lineTex;

		public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width, bool antiAlias)
		{
			Color color2 = Handles.color;
			if (UnityEngine.Event.current.type.Equals(EventType.Repaint))
			{
				Handles.color = color;
				for (float num = 0f; num < width; num += 1f)
				{
					float num2 = num * 0.2f;
					pointA.Set(pointA.x + num2, pointA.y);
					pointB.Set(pointB.x + num2, pointB.y);
					Handles.DrawLine(pointA, pointB);
				}
				Handles.color = color2;
			}
		}

		public static void DrawBox(Rect rect, Color color, float width, bool antiAlias)
		{
			DrawLine(new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMax, rect.yMin), color, width, antiAlias);
			DrawLine(new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMin, rect.yMax), color, width, antiAlias);
			DrawLine(new Vector2(rect.xMax, rect.yMin), new Vector2(rect.xMax, rect.yMax), color, width, antiAlias);
			DrawLine(new Vector2(rect.xMin, rect.yMax), new Vector2(rect.xMax, rect.yMax), color, width, antiAlias);
		}

		public static void bezierLine(Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, float width, bool antiAlias, int segments)
		{
			Handles.DrawBezier(start, end, startTangent, endTangent, color, null, width);
		}

		private static Vector2 cubeBezier(Vector2 s, Vector2 st, Vector2 e, Vector2 et, float t)
		{
			float num = 1f - t;
			float num2 = num * t;
			return num * num * num * s + 3f * num * num2 * st + 3f * num2 * t * et + t * t * t * e;
		}

		private static Matrix4x4 translationMatrix(Vector3 v)
		{
			return Matrix4x4.TRS(v, Quaternion.identity, Vector3.one);
		}

		public static void DrawBox(Texture2D a_Texture, int x1, int y1, int x2, int y2, Color a_Color)
		{
		}

		public static void DrawLine(Texture2D a_Texture, float x1, float y1, float x2, float y2, Color a_Color)
		{
			DrawLine(a_Texture, (int)x1, (int)y1, (int)x2, (int)y2, a_Color);
		}

		public static void DrawLine(Texture2D a_Texture, int x1, int y1, int x2, int y2, Color a_Color)
		{
			float num = x2 - x1;
			float num2 = y2 - y1;
			float num3 = Mathf.Abs(num);
			if (Mathf.Abs(num2) > num3)
			{
				num3 = Mathf.Abs(num2);
			}
			int num4 = (int)num3;
			float num5 = num / num3;
			float num6 = num2 / num3;
			for (int i = 0; i <= num4; i++)
			{
				a_Texture.SetPixel(x1, y1, a_Color);
				x1 += (int)num5;
				y1 += (int)num6;
			}
		}
	}
}
public class GLDraw
{
	protected static bool clippingEnabled;

	protected static Rect clippingBounds;

	public static Material lineMaterial = null;

	protected static bool clip_test(float p, float q, ref float u1, ref float u2)
	{
		bool result = true;
		if ((double)p < 0.0)
		{
			float num = q / p;
			if (num > u2)
			{
				result = false;
			}
			else if (num > u1)
			{
				u1 = num;
			}
		}
		else if ((double)p > 0.0)
		{
			float num = q / p;
			if (num < u1)
			{
				result = false;
			}
			else if (num < u2)
			{
				u2 = num;
			}
		}
		else if ((double)q < 0.0)
		{
			result = false;
		}
		return result;
	}

	protected static bool segment_rect_intersection(Rect bounds, ref Vector2 p1, ref Vector2 p2)
	{
		float u = 0f;
		float u2 = 1f;
		float num = p2.x - p1.x;
		if (clip_test(0f - num, p1.x - bounds.xMin, ref u, ref u2) && clip_test(num, bounds.xMax - p1.x, ref u, ref u2))
		{
			float num2 = p2.y - p1.y;
			if (clip_test(0f - num2, p1.y - bounds.yMin, ref u, ref u2) && clip_test(num2, bounds.yMax - p1.y, ref u, ref u2))
			{
				if ((double)u2 < 1.0)
				{
					p2.x = p1.x + u2 * num;
					p2.y = p1.y + u2 * num2;
				}
				if ((double)u > 0.0)
				{
					p1.x += u * num;
					p1.y += u * num2;
				}
				return true;
			}
		}
		return false;
	}

	public static void BeginGroup(Rect position)
	{
		clippingEnabled = true;
		clippingBounds = new Rect(0f, 0f, position.width, position.height);
		GUI.BeginGroup(position);
	}

	public static void EndGroup()
	{
		GUI.EndGroup();
		clippingBounds = new Rect(0f, 0f, Screen.width, Screen.height);
		clippingEnabled = false;
	}

	public static void CreateMaterial()
	{
		if (!(lineMaterial != null))
		{
			Shader shader = Shader.Find("Hidden/Internal-Colored");
			if (shader == null)
			{
				return;
			}
			lineMaterial = new Material(shader);
			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
			lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
			lineMaterial.SetInt("_ZWrite", 0);
		}
	}

	public static void DrawLine(Vector2 start, Vector2 end, Color color, float width)
	{
		if (Event.current != null && Event.current.type == EventType.Repaint && (!clippingEnabled || segment_rect_intersection(clippingBounds, ref start, ref end)))
		{
			CreateMaterial();
			lineMaterial.SetPass(0);
			if (width == 1f)
			{
				GL.Begin(1);
				GL.Color(color);
				Vector3 v = new Vector3(start.x, start.y, 0f);
				Vector3 v2 = new Vector3(end.x, end.y, 0f);
				GL.Vertex(v);
				GL.Vertex(v2);
			}
			else
			{
				GL.Begin(7);
				GL.Color(color);
				Vector3 v = new Vector3(end.y, start.x, 0f);
				Vector3 v2 = new Vector3(start.y, end.x, 0f);
				Vector3 vector = (v - v2).normalized * width;
				Vector3 vector2 = new Vector3(start.x, start.y, 0f);
				Vector3 vector3 = new Vector3(end.x, end.y, 0f);
				GL.Vertex(vector2 - vector);
				GL.Vertex(vector2 + vector);
				GL.Vertex(vector3 + vector);
				GL.Vertex(vector3 - vector);
			}
			GL.End();
		}
	}

	public static void DrawBox(Rect box, Color color, float width)
	{
		Vector2 vector = new Vector2(box.xMin, box.yMin);
		Vector2 vector2 = new Vector2(box.xMax, box.yMin);
		Vector2 vector3 = new Vector2(box.xMax, box.yMax);
		Vector2 vector4 = new Vector2(box.xMin, box.yMax);
		DrawLine(vector, vector2, color, width);
		DrawLine(vector2, vector3, color, width);
		DrawLine(vector3, vector4, color, width);
		DrawLine(vector4, vector, color, width);
	}

	public static void DrawBox(Vector2 topLeftCorner, Vector2 bottomRightCorner, Color color, float width)
	{
		Rect box = new Rect(topLeftCorner.x, topLeftCorner.y, bottomRightCorner.x - topLeftCorner.x, bottomRightCorner.y - topLeftCorner.y);
		DrawBox(box, color, width);
	}

	public static void DrawRoundedBox(Rect box, float radius, Color color, float width)
	{
		Vector2 vector = new Vector2(box.xMin + radius, box.yMin);
		Vector2 vector2 = new Vector2(box.xMax - radius, box.yMin);
		Vector2 vector3 = new Vector2(box.xMax, box.yMin + radius);
		Vector2 vector4 = new Vector2(box.xMax, box.yMax - radius);
		Vector2 vector5 = new Vector2(box.xMax - radius, box.yMax);
		Vector2 vector6 = new Vector2(box.xMin + radius, box.yMax);
		Vector2 vector7 = new Vector2(box.xMin, box.yMax - radius);
		Vector2 vector8 = new Vector2(box.xMin, box.yMin + radius);
		DrawLine(vector, vector2, color, width);
		DrawLine(vector3, vector4, color, width);
		DrawLine(vector5, vector6, color, width);
		DrawLine(vector7, vector8, color, width);
		float num = radius / 2f;
		DrawBezier(startTangent: new Vector2(vector8.x, vector8.y + num), endTangent: new Vector2(vector.x - num, vector.y), start: vector8, end: vector, color: color, width: width);
		DrawBezier(startTangent: new Vector2(vector2.x + num, vector2.y), endTangent: new Vector2(vector3.x, vector3.y - num), start: vector2, end: vector3, color: color, width: width);
		DrawBezier(startTangent: new Vector2(vector4.x, vector4.y + num), endTangent: new Vector2(vector5.x + num, vector5.y), start: vector4, end: vector5, color: color, width: width);
		DrawBezier(startTangent: new Vector2(vector6.x - num, vector6.y), endTangent: new Vector2(vector7.x, vector7.y + num), start: vector6, end: vector7, color: color, width: width);
	}

	public static void DrawConnectingCurve(Vector2 start, Vector2 end, Color color, float width)
	{
		Vector2 vector = start - end;
		Vector2 startTangent = start;
		startTangent.x -= (vector / 2f).x;
		Vector2 endTangent = end;
		endTangent.x += (vector / 2f).x;
		int segments = Mathf.FloorToInt(vector.magnitude / 20f * 3f);
		DrawBezier(start, startTangent, end, endTangent, color, width, segments);
	}

	public static void DrawBezier(Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, float width)
	{
		int segments = Mathf.FloorToInt((start - end).magnitude / 20f) * 3;
		DrawBezier(start, startTangent, end, endTangent, color, width, segments);
	}

	public static void DrawBezier(Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, float width, int segments)
	{
		Vector2 start2 = CubeBezier(start, startTangent, end, endTangent, 0f);
		for (int i = 1; i <= segments; i++)
		{
			Vector2 vector = CubeBezier(start, startTangent, end, endTangent, (float)i / (float)segments);
			DrawLine(start2, vector, color, width);
			start2 = vector;
		}
	}

	private static Vector2 CubeBezier(Vector2 s, Vector2 st, Vector2 e, Vector2 et, float t)
	{
		float num = 1f - t;
		float num2 = num * t;
		return num * num * num * s + 3f * num * num2 * st + 3f * num2 * t * et + t * t * t * e;
	}
}
namespace Fabric
{
	public class GraphViewer : EditorWindow
	{
		public Vector2 scrollPosition = default(Vector2);

		private int componentId;

		private int offset = 25;

		private Dictionary<int, Component> _components = new Dictionary<int, Component>();

		private float graphWidth = 10000f;

		private float graphHeight = 10000f;

		private int numOfActiveComponents;

		private Component[] componentList;

		public float xValue;

		public float yValue;

		[MenuItem("Window/Fabric/Graph View", false, 17)]
		private static void init()
		{
			GraphViewer graphViewer = (GraphViewer)EditorWindow.GetWindow(typeof(GraphViewer));
			graphViewer.title = "GraphView";
		}

		private void doWindow(int id)
		{
			if (!_components.ContainsKey(id))
			{
				return;
			}
			Component component = _components[id];
			if (component != null)
			{
				if (component.IsPlaying())
				{
					numOfActiveComponents++;
				}
				GUI.Label(new Rect(2f, 20f, 100f, 20f), "IsPlaying:" + component.IsPlaying());
				GUI.Label(new Rect(2f, 40f, 100f, 20f), "Instances:" + component.GetNumActiveComponentInstances());
				GUI.Label(new Rect(2f, 60f, 100f, 20f), "Vol:" + component.UpdateContext._volume);
				GUI.Label(new Rect(2f, 80f, 100f, 20f), "Vol Rnd:" + component.VolumeOffset);
				GUI.Label(new Rect(2f, 100f, 100f, 20f), "Pitch:" + component.UpdateContext._pitch);
				GUI.Label(new Rect(2f, 120f, 100f, 20f), "Pitch Rnd:" + component.PitchOffset);
				GUI.Label(new Rect(2f, 140f, 150f, 20f), "CPU:" + component.profiler.percent.ToString("0.000") + "% - ms:" + component.profiler.msPerFrame.ToString("0.000"));
			}
		}

		private void curveFromTo(Rect wr, Rect wr2, Color color, Color shadow)
		{
			Drawing.bezierLine(new Vector2(wr.x + wr.width, wr.y + (float)offset), new Vector2(wr.x + wr.width + Mathf.Abs(wr2.x - (wr.x + wr.width)) / 2f, wr.y + (float)offset), new Vector2(wr2.x, wr2.y + (float)offset), new Vector2(wr2.x - Mathf.Abs(wr2.x - (wr.x + wr.width)) / 2f, wr2.y + 25f), color, 2f, antiAlias: true, 20);
		}

		private void OnEnable()
		{
			if (FabricManager.IsInitialised())
			{
				componentList = FabricManager.Instance.gameObject.GetComponentsInChildren<Component>();
			}
			GUIHelpers.RegisterEditorUpdate(base.Repaint);
		}

		private void OnDisable()
		{
			GUIHelpers.UnregisterEditorUpdate(base.Repaint);
		}

		private void OnGUI()
		{
			numOfActiveComponents = 0;
			MenuBar.OnGUI("288087-graphview", box: true);
			DrawComponents();
		}

		private void DrawConnection(Rect from, Rect to)
		{
			float num = base.position.height - 50f;
			if ((!(from.y > num) || !(to.y > num)) && (!(from.y < 0f) || !(to.y < 0f)))
			{
				if (to.y > num)
				{
					to.y = num;
				}
				if (to.y < 0f)
				{
					to.y = 0f;
				}
				if (from.y > num)
				{
					from.y = num;
				}
				if (from.y < 0f)
				{
					from.y = 0f;
				}
				curveFromTo(shadow: new Color(0.4f, 0.4f, 0.5f), wr: from, wr2: to, color: new Color(0.3f, 0.7f, 0.4f));
			}
		}

		private void DrawComponent(Component component, float x, ref float y)
		{
			Component[] childComponents = component.GetChildComponents();
			int num = component.ToString().LastIndexOf(".");
			string text = component.name + "(" + component.ToString().Remove(0, num + 1);
			int num2 = text.Length * 10;
			GUI.Window(componentId, new Rect(x, y, num2, 160f), doWindow, text);
			_components.Add(componentId++, component);
			float x2 = x + (float)num2 + 20f;
			float y2 = y;
			foreach (Component component2 in childComponents)
			{
				if (component2 != null)
				{
					DrawConnection(new Rect(x, y2, 100f, 100f), new Rect(x2, y, 100f, 100f));
					DrawComponent(component2, x2, ref y);
					y += 180f;
				}
			}
		}

		private void DrawComponents()
		{
			if (!FabricManager.IsInitialised())
			{
				return;
			}
			float num = base.position.width - 20f;
			float num2 = base.position.height - 20f;
			GUILayout.BeginArea(new Rect(num, 0f, 50f, num2));
			yValue = GUILayout.VerticalScrollbar(yValue, 1f, 0f, graphHeight, GUILayout.Width(50f), GUILayout.Height(num2));
			GUILayout.EndArea();
			GUILayout.BeginArea(new Rect(20f, num2, num, 50f));
			xValue = GUILayout.HorizontalScrollbar(xValue, 1f, 0f, graphWidth, GUILayout.Width(num - 20f), GUILayout.Height(50f));
			GUILayout.EndArea();
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(num), GUILayout.Height(num2));
			GUI.BeginGroup(new Rect(1f, 1f, num, num2));
			BeginWindows();
			GameObject activeGameObject = Selection.activeGameObject;
			bool flag = false;
			if (activeGameObject != null)
			{
				for (int i = 0; i < componentList.Length; i++)
				{
					Component component = componentList[i];
					if (component.gameObject == activeGameObject)
					{
						float y = 20f - yValue;
						float x = 20f - xValue;
						DrawComponent(component, x, ref y);
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				Component[] components = FabricManager.Instance.GetComponents();
				float y2 = 20f - yValue;
				float x2 = 20f - xValue;
				foreach (Component component2 in components)
				{
					if (component2 != null)
					{
						DrawComponent(component2, x2, ref y2);
					}
				}
			}
			EndWindows();
			GUI.EndGroup();
			GUILayout.EndScrollView();
		}

		private void Update()
		{
			Repaint();
		}
	}
	[CustomEditor(typeof(AudioMixer))]
	public class AudioMixerEditor : Editor
	{
		private AudioMixer _audioMixer;

		private void OnEnable()
		{
			_audioMixer = base.target as AudioMixer;
		}

		public override void OnInspectorGUI()
		{
			if (_audioMixer != null)
			{
				DrawDefaultInspector();
			}
			if (_audioMixer._destroy)
			{
				UnityEngine.Object.DestroyImmediate(_audioMixer);
			}
		}
	}
	public class Mixer : EditorWindow
	{
		private List<GroupComponent> groupComponents = new List<GroupComponent>();

		private Vector2 scrollPosition = default(Vector2);

		private float width;

		private float slotHeight = 500f;

		private static float faderHeight = 250f;

		private static GUIStyle thumbStyle = new GUIStyle();

		private static GUIStyle backgroundStyle = new GUIStyle();

		private static List<GroupComponent> _soloComponents = new List<GroupComponent>();

		[MenuItem("Window/Fabric/Mixer", false, 10)]
		private static void init()
		{
			Mixer mixer = (Mixer)EditorWindow.GetWindow(typeof(Mixer));
			mixer.title = "Mixer";
		}

		public void PlaymodeCallback()
		{
			groupComponents.Clear();
			GroupComponent[] array = UnityEngine.Object.FindObjectsOfType(typeof(GroupComponent)) as GroupComponent[];
			for (int i = 0; i < array.Length; i++)
			{
				groupComponents.Add(array[i]);
			}
			GroupComponentProxy[] array2 = UnityEngine.Object.FindObjectsOfType(typeof(GroupComponentProxy)) as GroupComponentProxy[];
			for (int j = 0; j < array2.Length; j++)
			{
				groupComponents.Add(array2[j]._groupComponent);
			}
			Repaint();
		}

		private void OnEnable()
		{
			EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(PlaymodeCallback));
			EditorApplication.hierarchyWindowChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.hierarchyWindowChanged, new EditorApplication.CallbackFunction(PlaymodeCallback));
			PlaymodeCallback();
		}

		private static void LoadFaderBackgrounds()
		{
			if (thumbStyle.normal.background == null)
			{
				thumbStyle.normal.background = GUIHelpers.LoadImage("Fader");
				thumbStyle.fixedWidth = 16f;
				thumbStyle.fixedHeight = 32f;
			}
			if (backgroundStyle.normal.background == null)
			{
				backgroundStyle.normal.background = GUIHelpers.LoadImage("FaderBackground");
				backgroundStyle.fixedWidth = 16f;
				backgroundStyle.stretchHeight = true;
			}
		}

		private void OnGUI()
		{
			MenuBar.OnGUI("288083-mixer", box: true);
			DrawComponents();
			GUIHelpers.CheckGUIHasChanged(Selection.activeGameObject);
		}

		private static void DisplayCenterText(string text)
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(text, GUILayout.MinHeight(30f));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawVUMeters(GroupComponent component, float length)
		{
			VolumeMeter component2 = component.GetComponent<VolumeMeter>();
			if (component2 != null)
			{
				VolumeMeterEditor.DrawMeters(component2, length);
			}
		}

		private void DrawSideChain(GroupComponent component, float length)
		{
			SideChain component2 = component.GetComponent<SideChain>();
			if (component2 != null)
			{
				SideChainEditor.DrawSideChain(component2, length);
			}
		}

		public static void UnmuteParentComponent(Component component)
		{
			if (!(component.transform.parent == null))
			{
				Component component2 = component.transform.parent.GetComponent<Component>();
				if (component2 != null)
				{
					UnmuteParentComponent(component2);
					component2.Mute = false;
				}
			}
		}

		public static void MuteChildComponent(Component component, Component ignoreComponent = null)
		{
			Component[] componentsInChildren = component.GetComponentsInChildren<Component>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (ignoreComponent != null && ignoreComponent == componentsInChildren[i])
				{
					componentsInChildren[i].Mute = false;
				}
				else
				{
					componentsInChildren[i].Mute = true;
				}
			}
		}

		public static void MixerSlot(string title, GroupComponent component, bool useFaderImages = true)
		{
			LoadFaderBackgrounds();
			GUILayout.BeginVertical();
			GUILayout.BeginVertical();
			DisplayCenterText("Pitch");
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			component.Pitch = GUILayout.HorizontalSlider(component.Pitch, -4f, 4f, GUILayout.MinWidth(50f));
			GUILayout.Label(component.Pitch.ToString("F"));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.BeginHorizontal(GUILayout.MinHeight(50f));
			GUILayout.FlexibleSpace();
			component.Mute = GUILayout.Toggle(component.Mute, "Mute", "button");
			bool solo = component.Solo;
			if (useFaderImages)
			{
				component.Solo = GUILayout.Toggle(component.Solo, "Solo", "button");
			}
			if (component.Solo)
			{
				for (int i = 0; i < _soloComponents.Count; i++)
				{
					if (_soloComponents[i] != null && _soloComponents[i] != component)
					{
						_soloComponents[i].Mute = true;
					}
				}
				Component[] componentsInChildren = component.GetComponentsInChildren<Component>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					componentsInChildren[j].Mute = false;
				}
				UnmuteParentComponent(component);
			}
			else if (solo != component.Solo)
			{
				for (int k = 0; k < _soloComponents.Count; k++)
				{
					if (_soloComponents[k] != null && _soloComponents[k] != component)
					{
						_soloComponents[k].Mute = false;
					}
				}
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginVertical();
			DisplayCenterText("Volume");
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (useFaderImages)
			{
				if (Application.isPlaying && GetDynamicMixer.Instance() != null)
				{
					GUILayout.VerticalSlider(component.Volume * component.MixerVolume, 1f, 0f, backgroundStyle, thumbStyle, GUILayout.MinHeight(faderHeight), GUILayout.MaxHeight(faderHeight));
				}
				else
				{
					component.Volume = GUILayout.VerticalSlider(component.Volume * component.MixerVolume, 1f, 0f, backgroundStyle, thumbStyle, GUILayout.MinHeight(faderHeight), GUILayout.MaxHeight(faderHeight));
				}
			}
			else if (Application.isPlaying && GetDynamicMixer.Instance() != null)
			{
				GUILayout.VerticalSlider(component.Volume * component.MixerVolume, 1f, 0f, GUILayout.MinHeight(faderHeight), GUILayout.MaxHeight(faderHeight));
			}
			else
			{
				component.Volume = GUILayout.VerticalSlider(component.Volume, 1f, 0f, GUILayout.MinHeight(faderHeight), GUILayout.MaxHeight(faderHeight));
			}
			GUI.skin = null;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			DisplayCenterText(AudioTools.LinearToDB(component.Volume * component.MixerVolume).ToString("F2") + " dB");
			if (component.IsComponentActive())
			{
				DisplayCenterText("Active");
			}
			else
			{
				DisplayCenterText("InActive");
			}
			GUILayout.EndVertical();
			GUILayout.EndVertical();
			if (GUILayoutUtility.GetLastRect().Contains(UnityEngine.Event.current.mousePosition))
			{
				Selection.activeGameObject = component.gameObject;
			}
		}

		private bool HasSideChain(Component component)
		{
			SideChain component2 = component.GetComponent<SideChain>();
			if (!component2)
			{
				return false;
			}
			return true;
		}

		private bool HasVolumeMeter(Component component)
		{
			VolumeMeter component2 = component.GetComponent<VolumeMeter>();
			if (!component2)
			{
				return false;
			}
			return true;
		}

		private void DrawComponent(Component component, ref float x, float y)
		{
			if (!component.ToString().Contains("GroupComponent"))
			{
				return;
			}
			GroupComponent groupComponent = (GroupComponent)component;
			if (groupComponent._showInMixerView)
			{
				if (!_soloComponents.Contains(groupComponent))
				{
					_soloComponents.Add(groupComponent);
				}
				component.ToString().LastIndexOf(".");
				string text = component.name;
				int num = text.Length * 10;
				float num2 = slotHeight;
				if (num < 120)
				{
					num = 120;
				}
				if (HasSideChain(groupComponent))
				{
					num2 += 50f;
				}
				if (HasVolumeMeter(groupComponent))
				{
					num2 += 120f;
				}
				GUILayout.BeginArea(new Rect(x, y, num, num2), text, GUI.skin.window);
				MixerSlot(text, groupComponent);
				DrawVUMeters(groupComponent, num);
				DrawSideChain(groupComponent, num - 30);
				GUILayout.EndArea();
				x += num + 20;
			}
		}

		private void DrawComponents()
		{
			scrollPosition = GUI.BeginScrollView(new Rect(0f, 0f, base.position.width, base.position.height), scrollPosition, new Rect(0f, 0f, width, slotHeight));
			Component[] array = groupComponents.ToArray();
			float y = 35f;
			float x = 20f;
			foreach (Component component in array)
			{
				if (component != null)
				{
					DrawComponent(component, ref x, y);
				}
			}
			width = x;
			GUI.EndScrollView();
			if (Application.isPlaying && GetDynamicMixer.Instance() != null)
			{
				GUILayout.Label("NOTE: Runtime editting of mixer faders disabled when dynamic mixer is active");
			}
		}

		private void OnInspectorUpdate()
		{
			Repaint();
		}
	}
	public class GUIGraph
	{
		public delegate void OnGUI(Rect rect);

		private class DraggingData
		{
			public int _index;

			public float _x;

			public float _y;

			public DraggingData(int index)
			{
				_index = index;
			}
		}

		private class AddDeleteEnvelopePoint
		{
			public int index;

			public Fabric.TimelineComponent.Point point;
		}

		private float timeLength;

		private float unitInterval = 1f;

		private Fabric.TimelineComponent.Envelope _envelope;

		private int height;

		private bool pointSelected;

		private float minRange;

		private float maxRange = 1f;

		public bool hideMouseCursor;

		public bool isSelected;

		private GUITimelineScroller timelineScroller = new GUITimelineScroller();

		private GUITimelineLayoutSettings layoutSettings;

		private GUIStyle _unitStyle;

		private DraggingData dragging;

		protected Texture2D pointTex;

		protected Texture2D pointSelectedTex;

		protected Texture2D pointLockedTex;

		protected GUIStyle pointSelectedStyle;

		protected GUIStyle pointLockedStyle;

		protected GUIStyle pointStyle;

		public OnGUI _OnGUI;

		private GUIStyle unitStyle
		{
			get
			{
				if (_unitStyle == null)
				{
					_unitStyle = new GUIStyle("Label");
					_unitStyle.alignment = TextAnchor.LowerCenter;
				}
				return _unitStyle;
			}
		}

		public float DrawGraph(float zTimeLength, int zHeight, float zUnitInterval, float zMinRange, float zMaxRange, GUITimelineLayoutSettings zLayoutSettings, float parameter, float targetParameter, Fabric.TimelineComponent.Envelope envelope)
		{
			layoutSettings = zLayoutSettings;
			timeLength = zTimeLength;
			unitInterval = zUnitInterval;
			minRange = zMinRange;
			maxRange = zMaxRange;
			_envelope = envelope;
			height = zHeight;
			hideMouseCursor = false;
			Handles.BeginGUI();
			Rect rect = GUILayoutUtility.GetRect(layoutSettings.timelineWidth, zHeight);
			rect.y += layoutSettings.hackOffsetY;
			GUI.BeginGroup(rect, "", "Box");
			bool flag = false;
			pointSelected = false;
			Rect area = new Rect(0f, 0f, layoutSettings.timelineWidth, zHeight);
			if (area.Contains(UnityEngine.Event.current.mousePosition))
			{
				flag = true;
				isSelected = true;
			}
			parameter = timelineScroller.SetupScrollerLine(1f, parameter, targetParameter, layoutSettings.timelineWidth, height);
			if (flag && isSelected)
			{
				PerformEnvelopePointDragging();
			}
			if (HasPointDragged())
			{
				ApplyPointDragging(area);
			}
			DrawHorizontalValues(area);
			DrawUnitValues(area);
			Color green = Color.green;
			if (envelope != null && envelope._points != null)
			{
				Vector2 pointA = default(Vector2);
				for (float num = 0f; num < 1f; num += 0.01f)
				{
					float num2 = YAxisToPixels(_envelope.Calculate_y(num));
					float num3 = XAxisToPixels(num);
					if (num == 0f)
					{
						pointA.x = num3;
						pointA.y = num2;
					}
					if (flag)
					{
						float num4 = UnityEngine.Event.current.mousePosition.x - area.x;
						float num5 = UnityEngine.Event.current.mousePosition.y - area.y;
						if (Mathf.Abs(num3 - num4) < 8f && Mathf.Abs(num2 - num5) < 8f)
						{
							GenericMenu genericMenu = new GenericMenu();
							if (UnityEngine.Event.current.type == EventType.ContextClick && !pointSelected)
							{
								float x = UnityEngine.Event.current.mousePosition.x;
								float y = UnityEngine.Event.current.mousePosition.y;
								AddDeleteEnvelopePoint addDeleteEnvelopePoint = new AddDeleteEnvelopePoint();
								addDeleteEnvelopePoint.index = _envelope.GetPointIndex(num);
								addDeleteEnvelopePoint.point = Fabric.TimelineComponent.Point.Alloc(PixelsToXAxis(x), PixelsToYAxis((float)height - y));
								genericMenu.AddItem(new GUIContent("Add Point"), on: false, AddEnvelopePointCallback, addDeleteEnvelopePoint);
								genericMenu.AddSeparator("");
								int num6 = _envelope.GetPointIndex(num) - 1;
								genericMenu.AddItem(new GUIContent("Linear Curve"), on: false, SelectCurveTypeLinear, num6);
								genericMenu.AddItem(new GUIContent("Flat Ended Curve"), on: false, SelectCurveTypeFlatEnded, num6);
								genericMenu.AddItem(new GUIContent("Log Curve"), on: false, SelectCurveTypeLog, num6);
								genericMenu.ShowAsContext();
								UnityEngine.Event.current.Use();
								pointSelected = true;
							}
						}
					}
					Drawing.DrawLine(pointA, new Vector2(num3, num2), green, 2f, antiAlias: false);
					pointA.x = num3;
					pointA.y = num2;
				}
				for (int i = 0; i < envelope._points.Length; i++)
				{
					Fabric.TimelineComponent.Point point = envelope._points[i];
					Rect drawRect = new Rect((float)layoutSettings.timelineWidth * point._x - 5f, (float)zHeight - point._y * (float)zHeight - 5f, 10f, 10f);
					DrawPoint(drawRect, (i == _envelope._selectedPoint) ? true : false, point._locked);
				}
			}
			timelineScroller.DrawScrollerLine();
			if (_OnGUI != null)
			{
				_OnGUI(new Rect(0f, 0f, layoutSettings.timelineWidth, zHeight));
			}
			GUI.EndGroup();
			Handles.EndGUI();
			return parameter;
		}

		private void DrawPoint(Rect drawRect, bool isSelected = false, bool isLocked = false)
		{
			if (pointStyle == null)
			{
				pointStyle = new GUIStyle(GUI.skin.GetStyle("Box"));
				pointTex = GUIHelpers.LoadImage("FabricPoint");
				pointStyle.normal.background = pointTex;
				pointSelectedStyle = new GUIStyle(GUI.skin.GetStyle("Box"));
				pointSelectedTex = GUIHelpers.LoadImage("FabricPoint_Selected");
				pointSelectedStyle.normal.background = pointSelectedTex;
				pointLockedStyle = new GUIStyle(GUI.skin.GetStyle("Box"));
				pointLockedTex = GUIHelpers.LoadImage("FabricPoint_Locked");
				pointLockedStyle.normal.background = pointLockedTex;
			}
			GUIStyle gUIStyle = pointStyle;
			GUILayout.BeginArea(style: isSelected ? pointSelectedStyle : ((!isLocked) ? pointStyle : pointLockedStyle), screenRect: drawRect);
			GUILayout.EndArea();
			GUIStyle gUIStyle2 = new GUIStyle();
			gUIStyle2.fontSize = 9;
			gUIStyle2.normal.textColor = Color.red;
			float num = -20f;
			if (drawRect.x < 20f)
			{
				num *= -1f;
			}
			float num2 = -10f;
			if (drawRect.y < 10f)
			{
				num2 *= -1f;
			}
			Handles.Label(new Vector3(drawRect.x + num, drawRect.y + num2, 0f), "[" + drawRect.x.ToString("N0") + " - " + drawRect.y.ToString("N0") + " ]", gUIStyle2);
		}

		private void DrawHorizontalValues(Rect area)
		{
			int num = (int)area.height / 4;
			for (int i = 1; i < num; i++)
			{
				Drawing.DrawLine(new Vector2(0f, i * num), new Vector2(area.width, i * num), Color.gray, 0.5f, antiAlias: false);
			}
		}

		private void DrawUnitValues(Rect area)
		{
			float num = XAxisToPixels(unitInterval);
			for (int i = 0; (float)i * num < area.width; i++)
			{
				float num2 = num / 2f;
				Rect position = new Rect(area.xMin + (float)i * num - num2, area.yMin, num, area.height);
				Drawing.DrawLine(new Vector2(position.x + num2, 0f), new Vector2(position.x + num2, position.height), Color.gray, 1f, antiAlias: false);
				GUI.Box(position, ((float)i * unitInterval * (maxRange - minRange) + minRange).ToString(), unitStyle);
			}
		}

		private void PerformEnvelopePointDragging()
		{
			if (_envelope == null || _envelope._points.Length < 2 || UnityEngine.Event.current == null || (!UnityEngine.Event.current.isMouse && UnityEngine.Event.current.type != EventType.ContextClick))
			{
				return;
			}
			if (UnityEngine.Event.current.type == EventType.MouseDown || UnityEngine.Event.current.type == EventType.ContextClick)
			{
				for (int i = 0; i < _envelope._points.Length; i++)
				{
					Fabric.TimelineComponent.Point point = _envelope._points[i];
					RectOffset rectOffset = new RectOffset(10, 10, 10, 10);
					Rect rect = new Rect((float)layoutSettings.timelineWidth * point._x, (float)height - point._y * (float)height, 1f, 1f);
					if (!rectOffset.Add(rect).Contains(UnityEngine.Event.current.mousePosition))
					{
						continue;
					}
					if (UnityEngine.Event.current.type == EventType.ContextClick && i != 0 && i != _envelope._points.Length)
					{
						GenericMenu genericMenu = new GenericMenu();
						AddDeleteEnvelopePoint addDeleteEnvelopePoint = new AddDeleteEnvelopePoint();
						addDeleteEnvelopePoint.index = i;
						genericMenu.AddItem(new GUIContent("Delete Point"), on: false, DeleteEnvelopePointCallback, addDeleteEnvelopePoint);
						if (!point._locked)
						{
							genericMenu.AddItem(new GUIContent("Lock Point"), on: false, LockEnvelopePointCallback, addDeleteEnvelopePoint);
						}
						else
						{
							genericMenu.AddItem(new GUIContent("Unlock Point"), on: false, UnlockEnvelopePointCallback, addDeleteEnvelopePoint);
						}
						genericMenu.ShowAsContext();
						UnityEngine.Event.current.Use();
					}
					else if (UnityEngine.Event.current.button == 0)
					{
						dragging = new DraggingData(i);
						_envelope._selectedPoint = i;
						dragging._x = UnityEngine.Event.current.mousePosition.x;
						dragging._y = _envelope._points[i]._y;
						UnityEngine.Event.current.Use();
					}
					pointSelected = true;
					break;
				}
			}
			else if (UnityEngine.Event.current.type == EventType.MouseDrag && dragging != null)
			{
				dragging._x = UnityEngine.Event.current.mousePosition.x;
				dragging._y = UnityEngine.Event.current.delta.y;
				UnityEngine.Event.current.Use();
			}
			else
			{
				dragging = null;
			}
		}

		private bool HasPointDragged()
		{
			if (UnityEngine.Event.current != null && UnityEngine.Event.current.type == EventType.Layout)
			{
				return dragging != null;
			}
			return false;
		}

		private void ApplyPointDragging(Rect area)
		{
			if (dragging != null)
			{
				float x = PixelsToXAxis(dragging._x);
				float num = PixelsToYAxis(dragging._y);
				if (dragging._index == 0)
				{
					_envelope._points[dragging._index]._x = 0f;
				}
				else if (dragging._index == _envelope._points.Length - 1)
				{
					_envelope._points[dragging._index]._x = 1f;
				}
				else
				{
					_envelope._points[dragging._index]._x = x;
				}
				_envelope._points[dragging._index]._y -= num;
				if (dragging._index == 0)
				{
					ClipValues(null, _envelope._points[0], ref _envelope._points[dragging._index]);
				}
				else if (dragging._index == _envelope._points.Length - 1)
				{
					ClipValues(_envelope._points[_envelope._points.Length - 1], null, ref _envelope._points[dragging._index]);
				}
				else
				{
					ClipValues(_envelope._points[dragging._index - 1], _envelope._points[dragging._index + 1], ref _envelope._points[dragging._index]);
				}
			}
		}

		public void AddEnvelopePointCallback(object userData)
		{
			AddDeleteEnvelopePoint addDeleteEnvelopePoint = (AddDeleteEnvelopePoint)userData;
			_envelope._points = MyArray<Fabric.TimelineComponent.Point>.InsertAt(_envelope._points, addDeleteEnvelopePoint.index, addDeleteEnvelopePoint.point);
		}

		public void DeleteEnvelopePointCallback(object userData)
		{
			AddDeleteEnvelopePoint addDeleteEnvelopePoint = (AddDeleteEnvelopePoint)userData;
			_envelope._points = MyArray<Fabric.TimelineComponent.Point>.RemoveAt(_envelope._points, addDeleteEnvelopePoint.index);
		}

		public void LockEnvelopePointCallback(object userData)
		{
			AddDeleteEnvelopePoint addDeleteEnvelopePoint = (AddDeleteEnvelopePoint)userData;
			_envelope._points[addDeleteEnvelopePoint.index]._locked = true;
		}

		public void UnlockEnvelopePointCallback(object userData)
		{
			AddDeleteEnvelopePoint addDeleteEnvelopePoint = (AddDeleteEnvelopePoint)userData;
			_envelope._points[addDeleteEnvelopePoint.index]._locked = false;
		}

		public void SelectCurveTypeLinear(object userData)
		{
			int num = (int)userData;
			_envelope._points[num]._curveType = CurveTypes.Linear;
		}

		public void SelectCurveTypeFlatEnded(object userData)
		{
			int num = (int)userData;
			_envelope._points[num]._curveType = CurveTypes.Bezier;
		}

		public void SelectCurveTypeFlatMiddle(object userData)
		{
			int num = (int)userData;
			_envelope._points[num]._curveType = CurveTypes.Flat;
		}

		public void SelectCurveTypeLog(object userData)
		{
			int num = (int)userData;
			_envelope._points[num]._curveType = CurveTypes.Log;
		}

		private float PixelsToXAxis(float x)
		{
			return timeLength * x / (float)layoutSettings.timelineWidth;
		}

		private float PixelsToYAxis(float y)
		{
			return y / (float)layoutSettings.laneHeight;
		}

		private float XAxisToPixels(float x)
		{
			return (float)layoutSettings.timelineWidth * x / timeLength;
		}

		private float YAxisToPixels(float y)
		{
			return (float)height - y * (float)height;
		}

		public static void ClipValues(Fabric.TimelineComponent.Point Left, Fabric.TimelineComponent.Point Right, ref Fabric.TimelineComponent.Point point)
		{
			if (Left == null && Right != null)
			{
				if (point._x < 0f)
				{
					point._x = 0f;
				}
				if (point._x > Right._x)
				{
					point._x = Right._x;
				}
			}
			else if (Left != null && Right == null)
			{
				if (point._x > 1f)
				{
					point._x = 1f;
				}
				if (point._x < Left._x)
				{
					point._x = Left._x;
				}
			}
			else if (Left != null && Right != null)
			{
				if (point._x > Right._x)
				{
					point._x = Right._x;
				}
				if (point._x < Left._x)
				{
					point._x = Left._x;
				}
			}
			if (point._y < 0f)
			{
				point._y = 0f;
			}
			if (point._y > 1f)
			{
				point._y = 1f;
			}
		}
	}
	public class GUIFoldoutPanel
	{
		private string name;

		[SerializeField]
		private bool status;

		private int width;

		private int height;

		private static readonly float vPadding;

		private static readonly float hPadding;

		public GUIFoldoutPanel(string zName, bool zStatus, int zWidth = -1, int zHeight = -1)
		{
			name = zName;
			status = zStatus;
			width = zWidth;
			height = zHeight;
		}

		public bool Begin()
		{
			status = EditorGUILayout.Foldout(status, name);
			if (status)
			{
				GUILayout.Space(vPadding);
				if (width > 0 && height > 0)
				{
					GUILayout.BeginVertical(GUILayout.MaxWidth(width), GUILayout.MaxHeight(height));
				}
				if (width > 0 && height < 0)
				{
					GUILayout.BeginVertical(GUILayout.MaxWidth(width));
				}
				else if (width < 0 && height > 0)
				{
					GUILayout.BeginVertical(GUILayout.MaxHeight(height));
				}
				else
				{
					GUILayout.BeginVertical();
				}
				GUILayout.Space(hPadding);
			}
			return status;
		}

		public void End()
		{
			if (status)
			{
				GUILayout.FlexibleSpace();
				GUILayout.EndVertical();
				GUILayout.Space(vPadding);
			}
		}
	}
	public class GUISmartFloatField
	{
		private float originalValue;

		private float currentValue;

		private string fieldName;

		private float width;

		public GUISmartFloatField(float zWidth)
		{
			width = zWidth;
			fieldName = Guid.NewGuid().ToString();
		}

		public float FloatField(float value)
		{
			if (!value.Equals(originalValue))
			{
				originalValue = value;
				currentValue = value;
			}
			GUI.SetNextControlName(fieldName);
			currentValue = EditorGUILayout.FloatField(currentValue, GUILayout.Width(width));
			if (GUI.GetNameOfFocusedControl() != fieldName && !currentValue.Equals(originalValue))
			{
				float result = currentValue;
				currentValue = originalValue;
				return result;
			}
			return originalValue;
		}
	}
	public class GUITimeline
	{
		public delegate void OnGUI(Rect rect);

		public class LaneData
		{
			public float start;

			public float duration;

			public bool clicked;

			public Rect rect;
		}

		private enum DraggingType
		{
			ResizeLeft,
			Move,
			ResizeRight,
			TimeScroller
		}

		private class DraggingData
		{
			public DraggingType type;

			public float delta;

			public int laneIndex;

			public DraggingData(DraggingType zType, float zDelta, int zLaneIndex)
			{
				type = zType;
				delta = zDelta;
				laneIndex = zLaneIndex;
			}
		}

		private GUITimelineScroller timelineScroller = new GUITimelineScroller();

		private Vector2 globalScroll = Vector2.zero;

		private GUITimelineLayoutSettings layoutSettings;

		private GUIStyle _unitStyle;

		private GUITimelineRectLayout headerLayout;

		private GUIStyle timeScrollerStyle;

		private DraggingData dragging;

		private float timeLength;

		private int laneCounter;

		private float unitInterval = 1f;

		private float unitRange = 1f;

		private float currentTime = -1f;

		private float timelineWidth;

		public OnGUI _OnGUI;

		private GUIStyle unitStyle
		{
			get
			{
				if (_unitStyle == null)
				{
					_unitStyle = new GUIStyle("Label");
					_unitStyle.alignment = TextAnchor.LowerCenter;
				}
				return _unitStyle;
			}
		}

		public float width => timelineWidth;

		public Rect Begin(float zTimeLength, float zCurrentTime, float zTargetTime, GUITimelineLayoutSettings zLayoutSettings, GUIStyle zTimeScrollerStyle, float zUnitInterval, float zUnitRange)
		{
			layoutSettings = zLayoutSettings;
			timeLength = zTimeLength;
			currentTime = zCurrentTime;
			timeScrollerStyle = zTimeScrollerStyle;
			unitInterval = zUnitInterval;
			unitRange = zUnitRange;
			if (unitInterval <= 0f)
			{
				unitInterval = 1f;
			}
			GUILayout.BeginVertical(GUILayout.Width(timelineWidth));
			globalScroll = GUILayout.BeginScrollView(globalScroll, GUILayout.MinHeight(layoutSettings.laneHeight + 5));
			Rect rect = GUILayoutUtility.GetRect(layoutSettings.timelineWidth, (float)layoutSettings.laneHeight + layoutSettings.hackOffsetY);
			rect.y += layoutSettings.hackOffsetY;
			GUI.BeginGroup(rect, "", "Box");
			headerLayout = new GUITimelineRectLayout(0, 0f, 1f, layoutSettings);
			timelineWidth = headerLayout.bounding.xMax;
			DrawUnitValues(headerLayout.timeline);
			laneCounter = 0;
			if (_OnGUI != null)
			{
				_OnGUI(new Rect(0f, 0f, layoutSettings.timelineWidth, layoutSettings.laneHeight));
			}
			currentTime = timelineScroller.SetupScrollerLine(timeLength, currentTime, zTargetTime, layoutSettings.timelineWidth, layoutSettings.laneHeight);
			return rect;
		}

		public float End()
		{
			timelineScroller.DrawScrollerLine();
			if (_OnGUI != null)
			{
				_OnGUI(new Rect(0f, 0f, layoutSettings.timelineWidth, layoutSettings.laneHeight));
			}
			GUI.EndGroup();
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			return currentTime;
		}

		public LaneData Lane(string name, string label, float start, float duration, GUIStyle occupationStyle, GUIStyle laneStyle, Fabric.TimelineComponent.Envelope envelope = null)
		{
			GUITimelineRectLayout gUITimelineRectLayout = new GUITimelineRectLayout(1, start / timeLength, duration / timeLength, layoutSettings);
			LaneData output = new LaneData();
			output.start = start;
			output.duration = duration;
			output.rect = gUITimelineRectLayout.occupation;
			if (PerformLaneMouseDragging(laneCounter, gUITimelineRectLayout))
			{
				output.clicked = true;
			}
			if (HasLaneDragged(laneCounter))
			{
				ApplyLaneDragging(start, duration, ref output);
			}
			ClampValuesToTimeline(ref output);
			if (occupationStyle != null)
			{
				new GUIStyle();
				GUI.Box(gUITimelineRectLayout.occupation, new GUIContent(label, label), occupationStyle);
			}
			else
			{
				GUI.Box(gUITimelineRectLayout.occupation, new GUIContent(label, label), "Box");
			}
			DrawEnvelope(gUITimelineRectLayout, envelope, start, duration);
			GUI.Box(gUITimelineRectLayout.resizeLeftButton, "", "Box");
			GUI.Box(gUITimelineRectLayout.resizeRightButton, "", "Box");
			laneCounter++;
			return output;
		}

		private void DrawUnitValues(Rect area)
		{
			float num = TimeToPixels(unitInterval);
			for (int i = 0; (float)i * num < (float)layoutSettings.timelineWidth; i++)
			{
				float num2 = num / 2f;
				Rect position = new Rect(area.xMin + (float)i * num - num / 2f, area.yMin, num, area.height);
				Drawing.DrawLine(new Vector2(position.x + num2, 0f), new Vector2(position.x + num2, position.height), Color.gray, 1f, antiAlias: false);
				GUI.Box(position, ((float)i * unitInterval * unitRange).ToString(), unitStyle);
			}
		}

		private void DrawEnvelope(GUITimelineRectLayout layout, Fabric.TimelineComponent.Envelope envelope, float start, float duration)
		{
			if (envelope == null || envelope._points == null)
			{
				return;
			}
			Vector2 pointA = new Vector2(0f, 0f);
			for (float num = 0f; num <= 1f; num += 0.0025f)
			{
				float y = layout.timeline.height - envelope.Calculate_y(num) * layout.timeline.height;
				float num2 = TimeToPixels(num);
				if ((num2 > TimeToPixels(envelope._points[0]._x) && num2 < TimeToPixels(envelope._points[1]._x)) || (num2 > TimeToPixels(envelope._points[2]._x) && num2 < TimeToPixels(envelope._points[3]._x)))
				{
					Drawing.DrawLine(pointA, new Vector2(num2, y), Color.red, 2f, antiAlias: false);
				}
				pointA.x = num2;
				pointA.y = y;
			}
		}

		private bool PerformLaneMouseDragging(int laneIndex, GUITimelineRectLayout layout)
		{
			bool result = false;
			if (UnityEngine.Event.current != null && UnityEngine.Event.current.isMouse && UnityEngine.Event.current.button == 0)
			{
				if (UnityEngine.Event.current.type == EventType.MouseDown)
				{
					if (layout.bounding.Contains(UnityEngine.Event.current.mousePosition))
					{
						result = true;
					}
					if (layout.resizeLeftButton.Contains(UnityEngine.Event.current.mousePosition))
					{
						dragging = new DraggingData(DraggingType.ResizeLeft, 0f, laneIndex);
						UnityEngine.Event.current.Use();
					}
					else if (layout.resizeRightButton.Contains(UnityEngine.Event.current.mousePosition))
					{
						dragging = new DraggingData(DraggingType.ResizeRight, 0f, laneIndex);
						UnityEngine.Event.current.Use();
					}
					else if (layout.occupation.Contains(UnityEngine.Event.current.mousePosition))
					{
						dragging = new DraggingData(DraggingType.Move, 0f, laneIndex);
						UnityEngine.Event.current.Use();
					}
				}
				else if (UnityEngine.Event.current.type == EventType.MouseDrag && dragging != null)
				{
					dragging.delta = UnityEngine.Event.current.delta.x;
				}
				else
				{
					dragging = null;
				}
			}
			return result;
		}

		private bool HasLaneDragged(int laneIndex)
		{
			if (UnityEngine.Event.current != null && UnityEngine.Event.current.type == EventType.Layout && dragging != null)
			{
				return dragging.laneIndex == laneIndex;
			}
			return false;
		}

		private void ApplyLaneDragging(float start, float duration, ref LaneData output)
		{
			if (dragging != null)
			{
				float num = PixelsToTime(dragging.delta);
				dragging.delta = 0f;
				switch (dragging.type)
				{
				case DraggingType.ResizeLeft:
					output.start = start + num;
					output.duration = duration - num;
					break;
				case DraggingType.Move:
					output.start = start + num;
					output.duration = duration;
					break;
				case DraggingType.ResizeRight:
					output.start = start;
					output.duration = duration + num;
					break;
				}
			}
		}

		public float PixelsToTime(float pixels)
		{
			return timeLength * pixels / (float)layoutSettings.timelineWidth;
		}

		public float TimeToPixels(float time)
		{
			return (float)layoutSettings.timelineWidth * time / timeLength;
		}

		private void ClampValuesToTimeline(ref LaneData output)
		{
			if (output.start < 0f)
			{
				output.start = 0f;
			}
			if (output.start > timeLength)
			{
				output.start = timeLength;
			}
			if (output.duration < 0f)
			{
				output.duration = 0f;
			}
			if (output.start + output.duration > timeLength)
			{
				float num = output.start + output.duration - timeLength;
				if (output.start > num)
				{
					output.start -= num;
					return;
				}
				output.start = 0f;
				output.duration = timeLength;
			}
		}
	}
	[Serializable]
	public class GUITimelineLayoutSettings
	{
		public int laneHeight = 150;

		public int laneVerticalSpacing;

		public int laneHorizontalSpacing;

		public int labelWidth = 100;

		public int timelineWidth = 800;

		public int resizeButtonWidth = 10;

		public int timeScrollerWidth = 10;

		public int timeScrollerHeight = 20;

		public int timeScrollerLineWidth = 3;

		public float hackOffsetY = 4f;
	}
	public class GUITimelineRectLayout
	{
		public Rect bounding { get; protected set; }

		public Rect label { get; protected set; }

		public Rect timeline { get; protected set; }

		public Rect occupation { get; protected set; }

		public Rect resizeLeftButton { get; protected set; }

		public Rect resizeRightButton { get; protected set; }

		public GUITimelineRectLayout(int lineNumber, float normalizedStart, float normalizedDuration, GUITimelineLayoutSettings settings)
		{
			int laneVerticalSpacing = settings.laneVerticalSpacing;
			timeline = new Rect(settings.laneHorizontalSpacing, laneVerticalSpacing, settings.timelineWidth, settings.laneHeight);
			occupation = new Rect((float)(int)(normalizedStart * (float)settings.timelineWidth) + timeline.x, timeline.y, (int)(normalizedDuration * (float)settings.timelineWidth), settings.laneHeight);
			resizeLeftButton = new Rect(occupation.x, occupation.y, settings.resizeButtonWidth, occupation.height);
			resizeRightButton = new Rect(occupation.width - (float)settings.resizeButtonWidth + occupation.x, occupation.y, settings.resizeButtonWidth, occupation.height);
			bounding = new Rect(0f, laneVerticalSpacing, timeline.xMax, settings.laneHeight);
		}
	}
	internal class GUITimelineScroller
	{
		private class DraggingData
		{
			public float delta;
		}

		private GUITimelineRectLayout headerLayout;

		private DraggingData dragging;

		private float timeLength;

		private float currentTime = -1f;

		private float targetTime = -1f;

		private int scrollWidth = 10;

		private int scrollLineWidth = 4;

		private Vector2 globalScroll = Vector2.zero;

		private float timelineHeight;

		private float timelineWidth;

		private Rect scrollerRect;

		private bool isClicked;

		private bool IsClicked()
		{
			return isClicked;
		}

		public float SetupScrollerLine(float zTimeLength, float zCurrentTime, float zTargetTime, float width, float height)
		{
			timeLength = zTimeLength;
			timelineWidth = width;
			timelineHeight = height;
			currentTime = zCurrentTime;
			targetTime = zTargetTime;
			isClicked = false;
			currentTime = ManageTimeScroller(zCurrentTime);
			return currentTime;
		}

		public void DrawScrollerLine()
		{
			if (currentTime >= 0f)
			{
				float x = TimeToPixels(currentTime);
				scrollerRect = new Rect(x, 0f, scrollWidth, timelineHeight);
				Drawing.DrawLine(new Vector2(x, 0f), new Vector2(x, timelineHeight), Color.blue, scrollLineWidth, antiAlias: false);
				scrollerRect.xMin -= 10f;
			}
			if (targetTime >= 0f && targetTime != currentTime && Application.isPlaying)
			{
				float x2 = TimeToPixels(targetTime);
				Drawing.DrawLine(new Vector2(x2, 0f), new Vector2(x2, timelineHeight), Color.red, scrollLineWidth, antiAlias: false);
			}
		}

		private float ManageTimeScroller(float currentTime)
		{
			if (currentTime >= 0f)
			{
				Rect rect = new Rect(0f, 0f, timelineWidth, timelineHeight);
				if (UnityEngine.Event.current != null && UnityEngine.Event.current.type == EventType.MouseDown && UnityEngine.Event.current.button == 0 && scrollerRect.Contains(UnityEngine.Event.current.mousePosition) && !rect.Contains(UnityEngine.Event.current.mousePosition))
				{
					float x = UnityEngine.Event.current.mousePosition.x;
					x -= headerLayout.timeline.x;
					currentTime = PixelsToTime(x);
					dragging = new DraggingData();
					UnityEngine.Event.current.Use();
				}
				PerformTimeScrollerDragging(scrollerRect);
				if (HasTimeScrollerDragged())
				{
					currentTime += PixelsToTime(dragging.delta);
					dragging.delta = 0f;
					if (currentTime < 0f)
					{
						currentTime = 0f;
					}
					if (currentTime > timeLength)
					{
						currentTime = timeLength;
					}
					isClicked = true;
				}
			}
			return currentTime;
		}

		private void PerformTimeScrollerDragging(Rect timeScrollerRect)
		{
			if (UnityEngine.Event.current == null || !UnityEngine.Event.current.isMouse || UnityEngine.Event.current.button != 0)
			{
				return;
			}
			if (UnityEngine.Event.current.type == EventType.MouseDown)
			{
				if (timeScrollerRect.Contains(UnityEngine.Event.current.mousePosition))
				{
					dragging = new DraggingData();
					UnityEngine.Event.current.Use();
				}
			}
			else if (UnityEngine.Event.current.type == EventType.MouseDrag && dragging != null)
			{
				dragging.delta = UnityEngine.Event.current.delta.x;
				UnityEngine.Event.current.Use();
			}
			else
			{
				dragging = null;
			}
		}

		private bool HasTimeScrollerDragged()
		{
			if (UnityEngine.Event.current != null && UnityEngine.Event.current.type == EventType.Layout)
			{
				return dragging != null;
			}
			return false;
		}

		private float PixelsToTime(float pixels)
		{
			return timeLength * pixels / timelineWidth;
		}

		public float TimeToPixels(float time)
		{
			return timelineWidth * time / timeLength;
		}
	}
	internal class TimelineUIEditor : EditorWindow
	{
		private class AddDeleteRegionData
		{
			public TimelineRegion region;

			public TimelineLayer layer;

			public float position;
		}

		private class AddDeletePropertiesData
		{
			public ParameterToProperty property;

			public TimelineLayer layer;
		}

		private GUISmartFloatField timelineLengthField = new GUISmartFloatField(floatFieldWidth);

		private GUISmartFloatField multipleStartField = new GUISmartFloatField(floatFieldWidth);

		private GUISmartFloatField multipleDurationField = new GUISmartFloatField(floatFieldWidth);

		private GUISmartFloatField multipleEndField = new GUISmartFloatField(floatFieldWidth);

		private GUIFoldoutPanel parametersPanel = new GUIFoldoutPanel("Parameters", zStatus: true);

		private GUIFoldoutPanel layerPropertiesPanel = new GUIFoldoutPanel("Layer Properties", zStatus: true);

		private GUIFoldoutPanel regionPropertiesPanel = new GUIFoldoutPanel("Timeline Properties", zStatus: true);

		private Vector2 parametersScroll = Vector2.zero;

		private Vector2 layersScroll = Vector2.zero;

		private GUITimelineLayoutSettings settings = new GUITimelineLayoutSettings();

		[SerializeField]
		private TimelineEditorGUIStyles _styles;

		[SerializeField]
		private TimelineUIEditorState state;

		public static bool windowEnabled = false;

		public static TimelineUIEditor window = null;

		private float[] lastMinParameter = new float[32];

		private float[] lastMaxParameter = new float[32];

		private float unitSize = 0.1f;

		private string parameterName = "";

		private static readonly int floatFieldWidth = 70;

		public int parametersHeight = 120;

		public static int parameterToPropertyWidth = 400;

		public static int parameterToPropertyHeight = 50;

		public int layerPropertiesWidth = 400;

		private ParameterToProperty parameterToPropertyCopied;

		private TimelineUIEditorState currentState
		{
			get
			{
				if (state == null)
				{
					state = new TimelineUIEditorState();
					currentState.UpdateSelection();
				}
				return state;
			}
		}

		private TimelineEditorGUIStyles styles
		{
			get
			{
				if (_styles == null || _styles.nonSelectedStyle.normal.background == null)
				{
					_styles = new TimelineEditorGUIStyles();
				}
				return _styles;
			}
		}

		[MenuItem("Window/Fabric/Timeline", false, 14)]
		public static void Init()
		{
			window = null;
			window = (TimelineUIEditor)EditorWindow.GetWindow(typeof(TimelineUIEditor));
			window.title = "Timeline";
		}

		private void OnEnable()
		{
			base.wantsMouseMove = true;
			base.autoRepaintOnSceneChange = true;
			windowEnabled = true;
			for (int i = 0; i < lastMaxParameter.Length; i++)
			{
				lastMinParameter[i] = -1f;
				lastMaxParameter[i] = -1f;
			}
			GUIHelpers.RegisterEditorUpdate(base.Repaint);
		}

		private void OnDisable()
		{
			windowEnabled = false;
			GUIHelpers.UnregisterEditorUpdate(base.Repaint);
		}

		private void OnSelectionChange()
		{
			state = null;
			currentState.UpdateSelection();
			Repaint();
		}

		public void DeleteRegionCallback(object userData)
		{
			AddDeleteRegionData addDeleteRegionData = (AddDeleteRegionData)userData;
			addDeleteRegionData.layer.RemoveRegion(addDeleteRegionData.region);
			currentState.UpdateSelection();
		}

		public void AddRegionCallback(object userData)
		{
			AddDeleteRegionData addDeleteRegionData = (AddDeleteRegionData)userData;
			TimelineRegion timelineRegion = addDeleteRegionData.layer.AddRegion("Region");
			timelineRegion._x = addDeleteRegionData.position;
			timelineRegion._width = 0.2f;
			currentState.UpdateSelection();
		}

		public void DeleteRegionComponent(object userData)
		{
			AddDeleteRegionData addDeleteRegionData = (AddDeleteRegionData)userData;
			for (int i = 0; i < addDeleteRegionData.region.transform.childCount; i++)
			{
				Component component = addDeleteRegionData.region.transform.GetChild(i).GetComponent<Component>();
				if (component != null)
				{
					UnityEngine.Object.DestroyImmediate(component.gameObject);
				}
			}
		}

		public void ShowRegionInProjectView(object userData)
		{
			AddDeleteRegionData addDeleteRegionData = (AddDeleteRegionData)userData;
			Component componentInChildren = addDeleteRegionData.region.GetComponentInChildren<Component>();
			if (componentInChildren != null)
			{
				Selection.activeGameObject = componentInChildren.gameObject;
			}
		}

		public void AddLayer()
		{
			TimelineLayer timelineLayer = currentState.timeline.AddLayer("layer");
			TimelineParameter[] parameterList = currentState.timeline.GetParameterList();
			if (parameterList.Length > 0)
			{
				timelineLayer._controlParameter = parameterList[0];
			}
			currentState.UpdateSelection();
		}

		public void DeleteLayer(object userData)
		{
			TimelineLayer timelineLayer = (TimelineLayer)userData;
			UnityEngine.Object.DestroyImmediate(timelineLayer.gameObject);
			currentState.UpdateSelection();
		}

		public void AddPropertiesToParameters(object userData)
		{
			AddDeletePropertiesData addDeletePropertiesData = (AddDeletePropertiesData)userData;
			addDeletePropertiesData.layer.AddParameterToProperty();
			currentState.UpdateSelection();
		}

		public void DeletePropertiesToParameters(object userData)
		{
			AddDeletePropertiesData addDeletePropertiesData = (AddDeletePropertiesData)userData;
			addDeletePropertiesData.layer.DeleteParameterToProperty(addDeletePropertiesData.property);
			currentState.UpdateSelection();
		}

		public void CopyPropertiesToParameters(object userData)
		{
			AddDeletePropertiesData addDeletePropertiesData = (AddDeletePropertiesData)userData;
			parameterToPropertyCopied = addDeletePropertiesData.property;
		}

		public void PastePropertiesToParameters(object userData)
		{
			AddDeletePropertiesData addDeletePropertiesData = (AddDeletePropertiesData)userData;
			addDeletePropertiesData.layer.PasteParameterToProperty(parameterToPropertyCopied, addDeletePropertiesData.property);
		}

		public void UpdateWindow()
		{
			Repaint();
		}

		private void OnGUI()
		{
			if (!(currentState.timeline != null))
			{
				return;
			}
			TimelineParameter[] parameterList = currentState.timeline.GetParameterList();
			if (parameterList.Length == 0)
			{
				currentState.timeline.AddParameter("Default");
			}
			if (UnityEngine.Event.current != null && (UnityEngine.Event.current.type == EventType.MouseMove || UnityEngine.Event.current.type == EventType.MouseDrag))
			{
				Repaint();
			}
			if (currentState.timeline == null || currentState.timelineUILayers == null)
			{
				currentState.UpdateSelection();
				if (currentState.timeline == null || currentState.timelineUILayers == null)
				{
					return;
				}
			}
			MenuBar.OnGUI("288085-timeline", box: true);
			OnGUI_ParameterList();
			layersScroll = EditorGUILayout.BeginScrollView(layersScroll);
			GUILayout.Space(10f);
			GUILayout.BeginVertical();
			for (int i = 0; i < currentState.timelineUILayers.Length; i++)
			{
				TimelineUILayer timelineUILayer = currentState.timelineUILayers[i];
				if (timelineUILayer != null)
				{
					GUILayout.BeginVertical();
					GUILayout.BeginHorizontal(GUILayout.Height(settings.laneHeight));
					OnGUI_LayerProperties(timelineUILayer);
					GUILayout.Space(2f);
					OnGUI_Timeline(timelineUILayer);
					GUILayout.EndHorizontal();
					OnGUI_ParameterToProperty(timelineUILayer);
					GUILayout.EndVertical();
					GUILayout.Space(2f);
				}
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			if (UnityEngine.Event.current.type == EventType.ContextClick)
			{
				GenericMenu genericMenu = new GenericMenu();
				genericMenu.AddItem(new GUIContent("Add Layer"), on: false, AddLayer);
				genericMenu.ShowAsContext();
				UnityEngine.Event.current.Use();
			}
			GUIHelpers.CheckGUIHasChanged(currentState.timeline);
		}

		private void OnGUI_TimelineWindow()
		{
			GUILayout.BeginHorizontal(GUILayout.Width(layerPropertiesWidth + settings.timelineWidth));
			GUILayout.BeginHorizontal(GUILayout.MinWidth(layerPropertiesWidth));
			GUILayout.Label("");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal("Box", GUILayout.MinWidth(settings.timelineWidth));
			Rect rect = GUILayoutUtility.GetRect(settings.timelineWidth, 25f);
			GUI.BeginGroup(rect, "", "Box");
			GUI.EndGroup();
			GUILayout.EndHorizontal();
			GUILayout.EndHorizontal();
		}

		private void OnGUI_TimelineProperties()
		{
			GUILayout.BeginVertical("Box", GUILayout.MinWidth(parameterToPropertyWidth), GUILayout.MinHeight(parametersHeight));
			GUILayout.Label("");
			GUILayout.EndVertical();
		}

		private void OnGUI_ParameterList()
		{
			parametersScroll = GUILayout.BeginScrollView(parametersScroll, GUILayout.Height(parametersHeight));
			GUILayout.BeginVertical("Box");
			CenteredText("Parameters", horizontal: true);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Add", GUILayout.Width(50f)))
			{
				currentState.timeline.AddParameter(parameterName);
				parameterName = "";
			}
			parameterName = GUILayout.TextField(parameterName, 48);
			GUILayout.EndHorizontal();
			TimelineParameter[] parameterList = currentState.timeline.GetParameterList();
			List<TimelineParameter> list = new List<TimelineParameter>();
			foreach (TimelineParameter timelineParameter in parameterList)
			{
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Del", GUILayout.Width(50f)))
				{
					list.Add(timelineParameter);
					continue;
				}
				GUILayout.BeginHorizontal(GUILayout.MinWidth(100f));
				GUILayout.Label("Name:");
				timelineParameter._name = EditorGUILayout.TextField("", timelineParameter._name);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Reset On Play:", GUILayout.MaxWidth(100f));
				timelineParameter._resetToDefaultValue = EditorGUILayout.Toggle("", timelineParameter._resetToDefaultValue, GUILayout.MaxWidth(30f));
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(GUILayout.MinWidth(100f));
				GUILayout.Label("Loop:");
				timelineParameter._loopBehaviour = (ParameterLoopBehaviour)(object)EditorGUILayout.EnumPopup("", timelineParameter._loopBehaviour, "Popup");
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(GUILayout.MinWidth(100f));
				GUILayout.Label("Min:");
				timelineParameter._min = EditorGUILayout.FloatField("", timelineParameter._min, GUILayout.MinWidth(64f));
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(GUILayout.MinWidth(100f));
				GUILayout.Label("Max:");
				timelineParameter._max = EditorGUILayout.FloatField("", timelineParameter._max, GUILayout.MinWidth(64f));
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(GUILayout.MinWidth(100f));
				GUILayout.Label("Seek:");
				timelineParameter.SetSeekSpeed(EditorGUILayout.FloatField("", timelineParameter.GetSeekSpeed(), GUILayout.MinWidth(100f)));
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(GUILayout.MinWidth(100f));
				GUILayout.Label("Speed:");
				float value = timelineParameter._velocity * timelineParameter._max;
				value = EditorGUILayout.FloatField("", value, GUILayout.MinWidth(100f));
				timelineParameter._velocity = value / timelineParameter._max;
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(GUILayout.MinWidth(100f));
				GUILayout.Label("Value:");
				EditorGUILayout.LabelField("", timelineParameter.GetCurrentValue().ToString(), GUILayout.MinWidth(100f));
				GUILayout.EndHorizontal();
				GUILayout.EndHorizontal();
			}
			for (int j = 0; j < list.Count; j++)
			{
				currentState.timeline.DeleteParameter(list[j]);
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
		}

		private bool CheckRange(float origValue, ref float cacheValue, float min, float max, bool minCheck = false)
		{
			bool flag = false;
			if (UnityEngine.Event.current.Equals(UnityEngine.Event.KeyboardEvent("return")))
			{
				flag = true;
				UnityEngine.Event.current.Use();
			}
			if (cacheValue == -1f)
			{
				cacheValue = origValue;
			}
			GUI.SetNextControlName("range");
			cacheValue = EditorGUILayout.FloatField("", cacheValue, GUILayout.MaxWidth(64f));
			bool result = false;
			if (flag && GUI.GetNameOfFocusedControl() == "range")
			{
				switch (EditorUtility.DisplayDialogComplex("Changing the parameter range", "Please choose one of the following options.", "Strech", "Preserve X", "Cancel"))
				{
				case 0:
				{
					float num = max - min;
					float num2 = (minCheck ? (max - cacheValue) : (cacheValue - min));
					float num3 = num2 / num;
					for (int i = 0; i < currentState.timelineUILayers.Length; i++)
					{
						TimelineUILayer timelineUILayer = currentState.timelineUILayers[i];
						TimelineRegion[] regions = timelineUILayer.layer.Regions;
						for (int j = 0; j < regions.Length; j++)
						{
							regions[j]._x *= num3;
							AudioTools.Limit(ref regions[j]._x, 0f, 1f);
							regions[j]._width *= num3;
							AudioTools.Limit(ref regions[j]._width, 0f, 1f);
						}
						ParameterToProperty[] parameters = timelineUILayer.layer.Parameters;
						for (int k = 0; k < parameters.Length; k++)
						{
							parameters[k]._envelope.StretchX(num3);
						}
						timelineUILayer.layer.UpdateRegionEnvelopes();
					}
					result = true;
					break;
				}
				case 1:
					result = true;
					break;
				}
			}
			return result;
		}

		private void OnGUI_LayerProperties(TimelineUILayer UIlayer)
		{
			TimelineLayer layer = UIlayer.layer;
			if (layer == null)
			{
				return;
			}
			GUILayout.BeginVertical("Box", GUILayout.Width(layerPropertiesWidth), GUILayout.Height(settings.laneHeight));
			UIlayer.tabIndex = GUIHelpers.Tabs(new string[2] { "Layer Properties", "Parameter Markers" }, UIlayer.tabIndex, 120f);
			if (UIlayer.tabIndex == 0)
			{
				GUILayout.Space(10f);
				GUILayout.BeginHorizontal(GUILayout.MinWidth(150f));
				GUILayout.Label("Name:");
				layer.name = EditorGUILayout.TextField("", layer.name);
				GUILayout.EndHorizontal();
				GUILayout.Space(10f);
				GUILayout.BeginHorizontal();
				GUILayout.BeginHorizontal(GUILayout.MinWidth(100f));
				GUILayout.Label("Volume:");
				layer.Volume = EditorGUILayout.Slider("", layer.Volume, 0f, 1f);
				GUILayout.EndHorizontal();
				layer.Mute = GUILayout.Toggle(layer.Mute, "Mute", "Button", GUILayout.MaxWidth(50f));
				GUILayout.EndHorizontal();
				GUILayout.Space(10f);
				GUILayout.BeginHorizontal();
				int num = -1;
				TimelineParameter[] parameterList = currentState.timeline.GetParameterList();
				string[] array = new string[parameterList.Length];
				for (int i = 0; i < array.Length; i++)
				{
					if (layer._controlParameter == parameterList[i])
					{
						num = i;
					}
					array[i] = parameterList[i]._name;
				}
				int num2 = EditorGUILayout.Popup("Control Parameter:", num, array, "Popup", GUILayout.Width(300f));
				if (num != num2)
				{
					layer._controlParameter = parameterList[num2];
					if ((bool)layer._controlParameter)
					{
						UIlayer.markersView.SetRTPMarkers(layer._controlParameter._markers);
					}
				}
				if ((bool)layer._controlParameter)
				{
					UIlayer.markersView.SetRTPMarkers(layer._controlParameter._markers);
				}
				GUILayout.EndHorizontal();
			}
			else if (UIlayer.tabIndex == 1)
			{
				UIlayer.markersView.OnGUI(parameterToPropertyWidth, 100f);
			}
			GUILayout.EndVertical();
			if (GUILayoutUtility.GetLastRect().Contains(UnityEngine.Event.current.mousePosition) && UnityEngine.Event.current.type == EventType.ContextClick)
			{
				GenericMenu genericMenu = new GenericMenu();
				genericMenu.AddItem(new GUIContent("Delete Layer"), on: false, DeleteLayer, layer);
				genericMenu.ShowAsContext();
				UnityEngine.Event.current.Use();
			}
		}

		private void OnGUI_ParameterToProperty(TimelineUILayer timelineUILayer)
		{
			TimelineParameter[] parameterList = currentState.timeline.GetParameterList();
			string[] array = new string[parameterList.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = parameterList[i]._name;
			}
			ParameterToProperty[] parameters = timelineUILayer.layer.Parameters;
			if (parameters.Length == 0)
			{
				GUILayout.BeginHorizontal("", "Box", GUILayout.Width(parameterToPropertyWidth), GUILayout.Height(parameterToPropertyHeight));
				GUILayout.Label("");
				GUILayout.EndHorizontal();
				if (GUILayoutUtility.GetLastRect().Contains(UnityEngine.Event.current.mousePosition) && UnityEngine.Event.current.type == EventType.ContextClick)
				{
					AddDeletePropertiesData addDeletePropertiesData = new AddDeletePropertiesData();
					addDeletePropertiesData.layer = timelineUILayer.layer;
					GenericMenu genericMenu = new GenericMenu();
					genericMenu.AddItem(new GUIContent("Add Property"), on: false, AddPropertiesToParameters, addDeletePropertiesData);
					genericMenu.ShowAsContext();
					UnityEngine.Event.current.Use();
				}
				return;
			}
			bool flag = false;
			for (int j = 0; j < parameters.Length; j++)
			{
				ParameterToProperty parameterToProperty = parameters[j];
				if (parameterToProperty == null || parameterToProperty._envelope == null || parameterToProperty._envelope._points.Length <= 0)
				{
					continue;
				}
				GUILayout.BeginHorizontal();
				GUILayout.BeginHorizontal("", "Box", GUILayout.MaxWidth(parameterToPropertyWidth), GUILayout.Height(parameterToPropertyHeight));
				GUILayout.FlexibleSpace();
				int selectedPoint = parameterToProperty._envelope._selectedPoint;
				Fabric.TimelineComponent.Point point = new Fabric.TimelineComponent.Point();
				if (selectedPoint < parameterToProperty._envelope._points.Length)
				{
					point = parameterToProperty._envelope._points[selectedPoint];
				}
				GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Property:");
				parameterToProperty._property = (Property)(object)EditorGUILayout.EnumPopup("", parameterToProperty._property, "Popup", GUILayout.MaxWidth(126f));
				GUILayout.EndHorizontal();
				if (parameterToProperty._property == Property.Volume)
				{
					float value = AudioTools.LinearToDB(point._y);
					value = EditorGUILayout.Slider(value, -96f, 0f);
					point._y = AudioTools.DBToLinear(value);
				}
				else if (Property.Pitch == parameterToProperty._property)
				{
					point._y = EditorGUILayout.Slider(point._y, 0f, 3f);
				}
				else
				{
					point._y = EditorGUILayout.Slider(point._y, 0f, 1f);
				}
				if (selectedPoint == 0)
				{
					GUIGraph.ClipValues(null, parameterToProperty._envelope._points[0], ref point);
				}
				else if (parameterToProperty._envelope._selectedPoint == parameterToProperty._envelope._points.Length - 1)
				{
					GUIGraph.ClipValues(parameterToProperty._envelope._points[parameterToProperty._envelope._points.Length - 1], null, ref point);
				}
				else
				{
					GUIGraph.ClipValues(parameterToProperty._envelope._points[selectedPoint - 1], parameterToProperty._envelope._points[selectedPoint + 1], ref point);
				}
				GUILayout.EndVertical();
				int indexByParameter = currentState.timeline.GetIndexByParameter(parameterToProperty._parameter);
				GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Parameter:");
				int num = EditorGUILayout.Popup("", indexByParameter, array, "Popup", GUILayout.MaxWidth(125f));
				GUILayout.EndHorizontal();
				if (num != indexByParameter)
				{
					parameterToProperty._parameter = currentState.timeline.GetParameterByIndex(num);
				}
				if (parameterToProperty._parameter != null)
				{
					GUILayout.BeginHorizontal();
					float num2 = parameterToProperty._parameter._max - parameterToProperty._parameter._min;
					float num3 = EditorGUILayout.Slider(point._x * num2, parameterToProperty._parameter._min, parameterToProperty._parameter._max);
					if (selectedPoint == 0)
					{
						point._x = 0f;
					}
					else if (parameterToProperty._envelope._selectedPoint == parameterToProperty._envelope._points.Length - 1)
					{
						point._x = 1f;
					}
					else
					{
						point._x = 1f / num2 * num3;
					}
					string[] array2 = new string[parameterToProperty._envelope._points.Length];
					for (int k = 0; k < array2.Length; k++)
					{
						array2[k] = "Point_" + k;
					}
					parameterToProperty._envelope._selectedPoint = EditorGUILayout.Popup("", parameterToProperty._envelope._selectedPoint, array2, GUILayout.MaxWidth(60f));
					GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				if (GUILayoutUtility.GetLastRect().Contains(UnityEngine.Event.current.mousePosition) && UnityEngine.Event.current.type == EventType.ContextClick)
				{
					AddDeletePropertiesData addDeletePropertiesData2 = new AddDeletePropertiesData();
					addDeletePropertiesData2.layer = timelineUILayer.layer;
					addDeletePropertiesData2.property = parameterToProperty;
					GenericMenu genericMenu2 = new GenericMenu();
					genericMenu2.AddItem(new GUIContent("Add Property"), on: false, AddPropertiesToParameters, addDeletePropertiesData2);
					genericMenu2.AddItem(new GUIContent("Delete Property"), on: false, DeletePropertiesToParameters, addDeletePropertiesData2);
					genericMenu2.AddItem(new GUIContent("Copy Property"), on: false, CopyPropertiesToParameters, addDeletePropertiesData2);
					if (parameterToPropertyCopied != null)
					{
						genericMenu2.AddItem(new GUIContent("Paste Property"), on: false, PastePropertiesToParameters, addDeletePropertiesData2);
					}
					genericMenu2.ShowAsContext();
					UnityEngine.Event.current.Use();
					break;
				}
				float num4 = -0f;
				float targetParameter = -0f;
				float zMinRange = 0f;
				float zMaxRange = 1f;
				if ((bool)parameterToProperty._parameter)
				{
					num4 = parameterToProperty._parameter.GetNormalisedValue();
					targetParameter = parameterToProperty._parameter.GetNormalisedCurrentValue();
					zMinRange = parameterToProperty._parameter._min;
					zMaxRange = parameterToProperty._parameter._max;
				}
				if (j < timelineUILayer.graphs.Count)
				{
					num4 = timelineUILayer.graphs[j].DrawGraph(1f, parameterToPropertyHeight, 0.2f, zMinRange, zMaxRange, settings, num4, targetParameter, parameterToProperty._envelope);
					if (timelineUILayer.graphs[j].isSelected)
					{
						timelineUILayer.SelectedGraph(timelineUILayer.graphs[j]);
					}
					flag |= timelineUILayer.graphs[j].hideMouseCursor;
				}
				if ((bool)parameterToProperty._parameter)
				{
					parameterToProperty._parameter.SetNormalisedValue(num4);
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(2f);
			}
		}

		private void OnGUI_Timeline(TimelineUILayer timelineUILayer)
		{
			TimelineLayer layer = timelineUILayer.layer;
			if (layer == null)
			{
				return;
			}
			Rect rect;
			if (layer._controlParameter != null)
			{
				TimelineParameter controlParameter = layer._controlParameter;
				timelineUILayer.guiTimeline._OnGUI = timelineUILayer.markersView.OnGraphGUI;
				rect = timelineUILayer.guiTimeline.Begin(1f, controlParameter.GetNormalisedValue(), controlParameter.GetNormalisedCurrentValue(), settings, styles.timeScrollerStyle, unitSize, controlParameter._max);
			}
			else
			{
				rect = timelineUILayer.guiTimeline.Begin(1f, -1f, -1f, settings, styles.timeScrollerStyle, unitSize, 1f);
			}
			if ((bool)timelineUILayer.selectedRegion)
			{
				TimelineRegion selectedRegion = timelineUILayer.selectedRegion;
				GUIStyle nonSelectedStyle = styles.nonSelectedStyle;
				GUITimeline.LaneData laneData = timelineUILayer.guiTimeline.Lane("", selectedRegion.gameObject.name, selectedRegion._x, selectedRegion._width, styles.timelineStyle, nonSelectedStyle, selectedRegion._volumeEnvelope);
				if (selectedRegion._x != laneData.start || selectedRegion._width != laneData.duration)
				{
					Undo.RegisterCompleteObjectUndo(selectedRegion, "Effect Change");
					selectedRegion._x = laneData.start;
					selectedRegion._width = laneData.duration;
					layer.UpdateRegionEnvelopes();
				}
			}
			bool flag = false;
			for (int i = 0; i < layer.Regions.Length; i++)
			{
				TimelineRegion timelineRegion = layer.Regions[i];
				if (!(timelineRegion != null) && !(timelineRegion != timelineUILayer.selectedRegion))
				{
					continue;
				}
				GUIStyle nonSelectedStyle2 = styles.nonSelectedStyle;
				GUITimeline.LaneData laneData2 = timelineUILayer.guiTimeline.Lane("", timelineRegion.gameObject.name, timelineRegion._x, timelineRegion._width, styles.timelineStyle, nonSelectedStyle2, timelineRegion._volumeEnvelope);
				if (timelineRegion._x != laneData2.start || timelineRegion._width != laneData2.duration)
				{
					Undo.RegisterCompleteObjectUndo(timelineRegion, "Effect Change");
					timelineRegion._x = laneData2.start;
					timelineRegion._width = laneData2.duration;
					layer.UpdateRegionEnvelopes();
				}
				if (laneData2.clicked)
				{
					timelineUILayer.selectedRegion = timelineRegion;
				}
				if (layer.Regions.Length >= 2 && i < layer.Regions.Length - 1 && layer.Regions[i] != null && layer.Regions[i + 1] != null)
				{
					float num = timelineUILayer.guiTimeline.TimeToPixels(layer.Regions[i]._volumeEnvelope._points[2]._x);
					float num2 = timelineUILayer.guiTimeline.TimeToPixels(layer.Regions[i + 1]._volumeEnvelope._points[1]._x);
					float num3 = timelineUILayer.guiTimeline.TimeToPixels(layer.Regions[i]._volumeEnvelope._points[3]._x);
					float num4 = timelineUILayer.guiTimeline.TimeToPixels(layer.Regions[i + 1]._volumeEnvelope._points[0]._x);
					if (num3 > num4)
					{
						float x = num;
						float num5 = num2 - num;
						Rect rect2 = new Rect(x, 0f, num5, rect.height);
						if (num5 > 0f && rect2.Contains(UnityEngine.Event.current.mousePosition) && UnityEngine.Event.current.type == EventType.ContextClick)
						{
							GenericMenu genericMenu = new GenericMenu();
							genericMenu.AddItem(new GUIContent("Crossfade Linear"), on: false, timelineUILayer.CrossfadeTypeLinear, i);
							genericMenu.AddItem(new GUIContent("Crossfade Log"), on: false, timelineUILayer.CrossfadeTypeLog, i);
							genericMenu.AddItem(new GUIContent("Crossfade Raised"), on: false, timelineUILayer.CrossfadeTypeRaised, i);
							genericMenu.ShowAsContext();
							UnityEngine.Event.current.Use();
						}
					}
				}
				if (UnityEngine.Event.current.type == EventType.ContextClick && laneData2.rect.Contains(UnityEngine.Event.current.mousePosition))
				{
					GenericMenu genericMenu2 = new GenericMenu();
					AddDeleteRegionData addDeleteRegionData = new AddDeleteRegionData();
					addDeleteRegionData.layer = layer;
					addDeleteRegionData.region = timelineRegion;
					genericMenu2.AddItem(new GUIContent("Delete Region"), on: false, DeleteRegionCallback, addDeleteRegionData);
					genericMenu2.AddSeparator("");
					if (timelineRegion.GetComponentInChildren<AudioComponent>() != null || timelineRegion.GetComponentInChildren<RandomComponent>() != null || timelineRegion.GetComponentInChildren<SequenceComponent>() != null || timelineRegion.GetComponentInChildren<SwitchComponent>() != null || timelineRegion.GetComponentInChildren<GroupComponent>() != null)
					{
						genericMenu2.AddItem(new GUIContent("Delete Component"), on: false, DeleteRegionComponent, addDeleteRegionData);
						genericMenu2.AddItem(new GUIContent("Show Component in project view"), on: false, ShowRegionInProjectView, addDeleteRegionData);
					}
					else
					{
						genericMenu2.AddItem(new GUIContent("New Child Component"), on: false, ComponentMenuBar.AddNewChildComponent, timelineRegion);
					}
					genericMenu2.ShowAsContext();
					UnityEngine.Event.current.Use();
					flag = true;
					break;
				}
			}
			if (UnityEngine.Event.current.type == EventType.ContextClick && !flag && rect.Contains(UnityEngine.Event.current.mousePosition))
			{
				GenericMenu genericMenu3 = new GenericMenu();
				AddDeleteRegionData addDeleteRegionData2 = new AddDeleteRegionData();
				addDeleteRegionData2.layer = layer;
				addDeleteRegionData2.position = timelineUILayer.guiTimeline.PixelsToTime(UnityEngine.Event.current.mousePosition.x);
				genericMenu3.AddItem(new GUIContent("Add Region"), on: false, AddRegionCallback, addDeleteRegionData2);
				genericMenu3.ShowAsContext();
				UnityEngine.Event.current.Use();
			}
			if (layer._controlParameter != null)
			{
				layer._controlParameter.SetNormalisedValue(timelineUILayer.guiTimeline.End());
			}
			else
			{
				timelineUILayer.guiTimeline.End();
			}
			if (timelineUILayer.selectedRegion == null && layer.Regions.Length > 0)
			{
				timelineUILayer.selectedRegion = layer.Regions[0];
			}
			if (timelineUILayer.selectedRegion != null)
			{
				OnGUI_RegionProperties(timelineUILayer, timelineUILayer.selectedRegion);
			}
		}

		private float ConvertRangeFromControlParameter(float value, string label, TimelineParameter parameter)
		{
			if (parameter == null)
			{
				return value;
			}
			float num = parameter._max - parameter._min;
			float value2 = Mathf.Round(value * num * 100f) / 100f;
			value2 = EditorGUILayout.FloatField(label, value2);
			return 1f / num * value2;
		}

		private void OnGUI_RegionProperties(TimelineUILayer timelineUILayer, TimelineRegion region)
		{
			GUILayout.BeginVertical("Box", GUILayout.Height(settings.laneHeight), GUILayout.Width(300f));
			if (region != null)
			{
				region.name = EditorGUILayout.TextField("Name:", region.name);
				region._regionVolume = EditorGUILayout.Slider("Volume:", region._regionVolume, 0f, 1f);
				region._x = ConvertRangeFromControlParameter(region._x, "X:", timelineUILayer.layer._controlParameter);
				region._width = ConvertRangeFromControlParameter(region._width, "Width:", timelineUILayer.layer._controlParameter);
				region._autopitchenabled = EditorGUILayout.Toggle("Autopitch", region._autopitchenabled);
				region._autopitchreference = ConvertRangeFromControlParameter(region._autopitchreference, "Autopitch Base:", timelineUILayer.layer._controlParameter);
				bool loop = EditorGUILayout.Toggle("Loop", region._loop);
				region.SetLoop(loop);
				region._loopMode = (RegionLoopMode)(object)EditorGUILayout.EnumPopup("On Exit:", region._loopMode, "Popup");
			}
			GUILayout.EndVertical();
		}

		private void CenteredText(string text, bool horizontal)
		{
			if (horizontal)
			{
				GUILayout.BeginHorizontal();
			}
			else
			{
				GUILayout.BeginVertical();
			}
			GUILayout.FlexibleSpace();
			GUILayout.Label(text);
			GUILayout.FlexibleSpace();
			if (horizontal)
			{
				GUILayout.EndHorizontal();
			}
			else
			{
				GUILayout.EndVertical();
			}
		}
	}
	public class TimelineUILayer
	{
		public int tabIndex;

		public RTPMarkersView markersView = new RTPMarkersView();

		public GUITimeline guiTimeline = new GUITimeline();

		public TimelineLayer layer;

		public TimelineRegion selectedRegion;

		public Vector2 parameterToPropertyScroll = Vector2.zero;

		public List<GUIGraph> graphs = new List<GUIGraph>();

		public GUIFoldoutPanel propertiesToParametersPanel = new GUIFoldoutPanel("Properties", zStatus: true, TimelineUIEditor.parameterToPropertyWidth, TimelineUIEditor.parameterToPropertyHeight);

		public void SelectedGraph(GUIGraph selectedGraph)
		{
			for (int i = 0; i < graphs.Count; i++)
			{
				GUIGraph gUIGraph = graphs[i];
				if (gUIGraph != selectedGraph)
				{
					gUIGraph.isSelected = false;
				}
			}
		}

		public void CrossfadeTypeLinear(object userData)
		{
			int num = (int)userData;
			layer.Regions[num]._volumeEnvelope._points[2]._curveType = CurveTypes.Linear;
			layer.Regions[num + 1]._volumeEnvelope._points[0]._curveType = CurveTypes.Linear;
		}

		public void CrossfadeTypeRaised(object userData)
		{
			int num = (int)userData;
			layer.Regions[num]._volumeEnvelope._points[2]._curveType = CurveTypes.Raised;
			layer.Regions[num + 1]._volumeEnvelope._points[0]._curveType = CurveTypes.Raised;
		}

		public void CrossfadeTypeLog(object userData)
		{
			int num = (int)userData;
			layer.Regions[num]._volumeEnvelope._points[2]._curveType = CurveTypes.Log;
			layer.Regions[num + 1]._volumeEnvelope._points[0]._curveType = CurveTypes.Log;
		}
	}
	[Serializable]
	public class TimelineUIEditorState
	{
		public Fabric.TimelineComponent.TimelineComponent timeline;

		public TimelineRegion regionSelection;

		public TimelineUILayer[] timelineUILayers;

		public void UpdateSelection()
		{
			timeline = GetSelectedTimeline();
			regionSelection = null;
			if (Selection.gameObjects.Length == 1 && Selection.activeGameObject != null)
			{
				regionSelection = Selection.activeGameObject.GetComponent<TimelineRegion>();
			}
			if (!(timeline != null))
			{
				return;
			}
			TimelineLayer[] array;
			if (EditorApplication.isPlaying)
			{
				List<TimelineLayer> list = new List<TimelineLayer>();
				int childCount = timeline.transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					TimelineLayer component = timeline.transform.GetChild(i).GetComponent<TimelineLayer>();
					if ((bool)component)
					{
						list.Add(component);
					}
				}
				array = list.ToArray();
			}
			else
			{
				array = timeline.gameObject.GetComponentsInChildren<TimelineLayer>();
			}
			if (array == null)
			{
				return;
			}
			if (timelineUILayers == null || array.Length != timelineUILayers.Length)
			{
				timelineUILayers = new TimelineUILayer[array.Length];
			}
			for (int j = 0; j < array.Length; j++)
			{
				TimelineUILayer timelineUILayer = timelineUILayers[j];
				TimelineLayer timelineLayer = array[j];
				if (timelineUILayer == null)
				{
					timelineUILayer = new TimelineUILayer();
					timelineUILayer.graphs.Clear();
					for (int k = 0; k < timelineLayer.Parameters.Length; k++)
					{
						timelineUILayer.graphs.Add(new GUIGraph());
					}
					timelineUILayers[j] = timelineUILayer;
					timelineUILayers[j].layer = timelineLayer;
				}
				else if (timelineUILayer.graphs.Count != timelineLayer.Parameters.Length)
				{
					timelineUILayer.graphs.Clear();
					for (int l = 0; l < timelineLayer.Parameters.Length; l++)
					{
						timelineUILayer.graphs.Add(new GUIGraph());
					}
				}
			}
		}

		private Fabric.TimelineComponent.TimelineComponent GetSelectedTimeline()
		{
			GameObject activeGameObject = Selection.activeGameObject;
			if (activeGameObject == null)
			{
				return null;
			}
			Fabric.TimelineComponent.TimelineComponent component = activeGameObject.GetComponent<Fabric.TimelineComponent.TimelineComponent>();
			if (component != null)
			{
				return component;
			}
			return null;
		}
	}
	public class TimelineEditorGUIStyles
	{
		public GUIStyle timeScrollerStyle;

		public GUIStyle selectedStyle;

		public GUIStyle nonSelectedStyle;

		public GUIStyle timelineStyle;

		private static readonly Color timeScrollerColor = new Color(0.8f, 0f, 0f, 0.6f);

		private static readonly Color nonSelectedColor = new Color(0.25f, 0.25f, 0.25f, 1f);

		private static readonly Color selectedColor = new Color(0.3f, 0.3f, 0.3f, 1f);

		private static readonly Color defaultColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

		private static readonly Color meshColor = new Color(0.44f, 0.68f, 0.87f, 0.5f);

		private static readonly Color particleColor = new Color(0.87f, 0.84f, 0.45f, 0.5f);

		private static readonly Color timelineColor = new Color(0.87f, 0.5f, 0.44f, 0.5f);

		private static readonly Color soundColor = new Color(0.86f, 0.47f, 1f, 0.5f);

		private static readonly Color lightColor = new Color(0.73f, 0.87f, 0.44f, 0.5f);

		private static readonly Color cameraColor = new Color(0.44f, 0.44f, 0.87f, 0.5f);

		private static readonly Color skinnedMeshColor = new Color(0.44f, 0.87f, 0.77f, 0.5f);

		public TimelineEditorGUIStyles()
		{
			RectOffset border = new RectOffset(0, 0, 0, 0);
			timeScrollerStyle = new GUIStyle("Box");
			timeScrollerStyle.normal.background = CreateBackground(timeScrollerColor);
			timeScrollerStyle.border = border;
			selectedStyle = new GUIStyle("Box");
			selectedStyle.normal.background = CreateBackground(selectedColor);
			selectedStyle.border = border;
			nonSelectedStyle = new GUIStyle("Box");
			nonSelectedStyle.normal.background = CreateBackground(nonSelectedColor);
			nonSelectedStyle.border = border;
			timelineStyle = new GUIStyle("Box");
			timelineStyle.normal.background = CreateBackground(defaultColor);
			timelineStyle.border = border;
		}

		private Texture2D CreateBackground(Color color)
		{
			Texture2D texture2D = new Texture2D(1, 1);
			texture2D.SetPixel(1, 1, color);
			texture2D.Apply();
			return texture2D;
		}
	}
}
public class EditorUndoManager
{
	private UnityEngine.Object defTarget;

	private string defName;

	private bool autoSetDirty;

	private bool listeningForGuiChanges;

	public EditorUndoManager(UnityEngine.Object p_target, string p_name)
		: this(p_target, p_name, p_autoSetDirty: true)
	{
	}

	public EditorUndoManager(UnityEngine.Object p_target, string p_name, bool p_autoSetDirty)
	{
		defTarget = p_target;
		defName = p_name;
		autoSetDirty = p_autoSetDirty;
	}

	public void CheckUndo()
	{
		CheckUndo(defTarget, defName);
	}

	public void CheckUndo(UnityEngine.Object p_target)
	{
		CheckUndo(p_target, defName);
	}

	public void CheckUndo(UnityEngine.Object p_target, string p_name)
	{
		Event current = Event.current;
		if ((current.type == EventType.MouseDown && current.button == 0) || (current.type == EventType.KeyUp && current.keyCode == KeyCode.Tab))
		{
			Undo.RecordObject(p_target, p_name);
			listeningForGuiChanges = true;
		}
	}

	public void CheckDirty()
	{
		CheckDirty(defTarget, defName);
	}

	public void CheckDirty(UnityEngine.Object p_target)
	{
		CheckDirty(p_target, defName);
	}

	public void CheckDirty(UnityEngine.Object p_target, string p_name)
	{
		if (listeningForGuiChanges && GUI.changed)
		{
			Undo.RecordObject(p_target, p_name);
			if (autoSetDirty)
			{
				EditorUtility.SetDirty(p_target);
			}
			listeningForGuiChanges = false;
		}
	}

	public static void RegisterUndo(UnityEngine.Object obj, string comment)
	{
		Undo.RecordObject(obj, comment);
	}

	public static void RegisterCreateUndo(UnityEngine.Object obj, string comment)
	{
		Undo.RegisterCreatedObjectUndo(obj, comment);
	}

	public static void RegisterDestroyImmediateUndo(UnityEngine.Object obj, string comment)
	{
		Undo.DestroyObjectImmediate(obj);
	}

	public static void RegisterMoveUndo(GameObject obj, GameObject newObj, string comment)
	{
		Undo.SetTransformParent(obj.transform, newObj.transform, comment);
	}
}
