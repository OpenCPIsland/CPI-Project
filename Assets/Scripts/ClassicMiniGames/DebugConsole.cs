using DisneyMobile.CoreUnitySystems;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class DebugConsole
{
	public static bool showTextEdit;

	public static string typedString;

	public static GUIStyle textAreaStyle;

	public static GUIStyle buttonStyle;

	protected static HashSet<string> memberStringsToHide;

	private static HashSet<string> availableCommands;

	private static HashSet<string> gameSpecificCommands;

	private static float plusWidth;

	public static Type instanceType;

	protected static DebugConsole _instance;

	public static DebugConsole Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = (Activator.CreateInstance(instanceType) as DebugConsole);
				DisneyMobile.CoreUnitySystems.Logger.LogDebug(null, "creating new instance of Debug Console");
			}
			DisneyMobile.CoreUnitySystems.Logger.LogDebug(null, "DebugConsole.Instance returning " + _instance.ToString());
			return _instance;
		}
	}

	protected Type MyType()
	{
		return _instance.GetType();
	}

	static DebugConsole()
	{
		showTextEdit = false;
		typedString = "";
		textAreaStyle = null;
		buttonStyle = null;
		memberStringsToHide = new HashSet<string>();
		plusWidth = 50f;
		instanceType = typeof(DebugConsole);
		DisneyMobile.CoreUnitySystems.Logger.LogDebug(null, "start of DebugConsole");
		memberStringsToHide.Add(".cctor");
		memberStringsToHide.Add("notify");
		memberStringsToHide.Add("memberStringsToHide");
		memberStringsToHide.Add("_instance");
		memberStringsToHide.Add("availableCommands");
		memberStringsToHide.Add("gameSpecificCommands");
		if (plusWidth < (float)Screen.width * 0.08f)
		{
			plusWidth = (float)Screen.width * 0.08f;
		}
	}

	public static void OnGUI()
	{
		GUILayout.BeginArea(new Rect(0f, 0f, Screen.width, Screen.height));
		if (showTextEdit)
		{
			if (textAreaStyle == null)
			{
				textAreaStyle = new GUIStyle(GUI.skin.textArea);
				buttonStyle = new GUIStyle(GUI.skin.button);
			}
			typedString = GUILayout.TextField(typedString, textAreaStyle, GUILayout.MinWidth(Screen.width), GUILayout.MinHeight(150f));
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Close", buttonStyle, GUILayout.MinHeight(50f), GUILayout.MinWidth(100f)))
			{
				showTextEdit = false;
			}
			if (GUILayout.Button("Clear", buttonStyle, GUILayout.MinHeight(50f), GUILayout.MinWidth(100f)))
			{
				typedString = "";
			}
			if (GUILayout.Button("Run Command", buttonStyle, GUILayout.MinHeight(50f)))
			{
				runCommand(typedString);
			}
		}
		else if (GUILayout.Button("+", GUILayout.ExpandWidth(false), GUILayout.MinWidth(plusWidth), GUILayout.MinHeight(plusWidth)))
		{
			showTextEdit = true;
		}
		GUILayout.EndArea();
	}

	public static void runCommand(string cmd, bool isLanguage = false)
	{
		if (cmd == "")
		{
			typedString = "try typing 'help'";
			return;
		}
		string text = cmd.Trim();
		try
		{
			string[] array = text.Split(' ');
			if (array.Length > 0)
			{
				Type type = Instance.MyType();
				DisneyMobile.CoreUnitySystems.Logger.LogDebug(null, "calledType = " + type.Name);
				string text2 = array[0].ToLower();
				if (isLanguage)
				{
					DisneyMobile.CoreUnitySystems.Logger.LogDebug(null, "call language function");
					type.InvokeMember("language", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod, null, null, new object[1]
					{
						text
					});
				}
				else if (GetGameSpecificCommands().Contains(text2))
				{
					DisneyMobile.CoreUnitySystems.Logger.LogDebug(null, "found a method called " + text2);
					type.InvokeMember(text2, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod, null, null, new object[1]
					{
						text
					});
				}
				else
				{
					DisneyMobile.CoreUnitySystems.Logger.LogDebug(null, "Did not find  a method called " + text2);
				}
			}
		}
		catch (Exception arg)
		{
			typedString = string.Concat(arg, " Exception caught.");
		}
	}

	public static HashSet<string> GetAvailableCommands()
	{
		if (availableCommands == null)
		{
			availableCommands = new HashSet<string>();
			if (Instance.MyType() != typeof(DebugConsole))
			{
				Type typeFromHandle = typeof(DebugConsole);
				MemberInfo[] members = typeFromHandle.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
				MemberInfo[] array = members;
				foreach (MemberInfo memberInfo in array)
				{
					if (!memberStringsToHide.Contains(memberInfo.Name))
					{
						availableCommands.Add(memberInfo.Name);
					}
				}
			}
			availableCommands.UnionWith(GetGameSpecificCommands());
		}
		return availableCommands;
	}

	public static HashSet<string> GetGameSpecificCommands()
	{
		if (gameSpecificCommands == null)
		{
			gameSpecificCommands = new HashSet<string>();
			Type type = Instance.MyType();
			MemberInfo[] members = type.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			MemberInfo[] array = members;
			foreach (MemberInfo memberInfo in array)
			{
				if (!memberStringsToHide.Contains(memberInfo.Name))
				{
					gameSpecificCommands.Add(memberInfo.Name);
				}
			}
		}
		return gameSpecificCommands;
	}

	private static bool help(string cmd)
	{
		typedString = "commands available= ";
		foreach (string availableCommand in GetAvailableCommands())
		{
			typedString = typedString + availableCommand + " ";
		}
		return true;
	}

	private static bool ts(string cmd)
	{
		bool result = true;
		string[] array = cmd.Split(' ');
		if (array.Length < 2)
		{
			typedString = "Time Scale usage : ts <1|0>";
			result = false;
		}
		else
		{
			float result2 = 0f;
			if (float.TryParse(array[1], out result2))
			{
				Time.timeScale = result2;
			}
			else
			{
				typedString = "couldn't parse " + array[1] + " as a float";
				result = false;
			}
		}
		return result;
	}

	private static bool gc(string cmd)
	{
		GC.Collect();
		Resources.UnloadUnusedAssets();
		typedString = "called System.GC.Collect() and Resources.UnloadUnusedAssets()";
		return true;
	}

	private static bool dl(string cmd)
	{
		bool result = true;
		string[] array = cmd.Split(' ');
		if (array.Length < 2)
		{
			typedString = "Directional Light usage : dl  <on|off>";
			result = false;
		}
		else if (array[1] == "on")
		{
			GameObject gameObject = GameObject.Find("DebugLight");
			if ((bool)gameObject)
			{
				Light component = gameObject.GetComponent<Light>();
				component.enabled = true;
			}
			else
			{
				gameObject = new GameObject("DebugLight");
				gameObject.transform.localEulerAngles = new Vector3(50f, -30f, 0f);
				Light component = gameObject.AddComponent<Light>();
				component.type = LightType.Directional;
				component.intensity = 0.5f;
			}
		}
		else if (array[1] == "off")
		{
			GameObject gameObject = GameObject.Find("DebugLight");
			if ((bool)gameObject)
			{
				Light component = gameObject.GetComponent<Light>();
				component.enabled = false;
			}
			else
			{
				typedString = "No DebugLight to turn off";
			}
		}
		else
		{
			typedString = "parameter must be 'on' or 'off' ";
			result = false;
		}
		return result;
	}

	public static bool DebugDirectionalLight(bool on)
	{
		if (on)
		{
			return dl("dl on");
		}
		return dl("dl off");
	}
}
