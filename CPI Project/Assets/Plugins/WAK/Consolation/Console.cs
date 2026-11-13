using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Consolation
{
	internal class Console : MonoBehaviour
	{
		private struct Log
		{
			public string message;
			public string stackTrace;
			public LogType type;
		}

		public Key toggleKey = Key.Backquote; // Unity Input System Key
		public bool shakeToOpen = true;
		public float shakeAcceleration = 3f;

		private readonly List<Log> logs = new List<Log>();
		private Vector2 scrollPosition;
		private bool visible;
		private bool collapse;
		private float visibilityChanged = 0f;

		private static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>
		{
			{ LogType.Assert, Color.white },
			{ LogType.Error, Color.red },
			{ LogType.Exception, Color.red },
			{ LogType.Log, Color.white },
			{ LogType.Warning, Color.yellow }
		};

		private const string windowTitle = "Console";
		private const int margin = 20;
		private static readonly GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
		private static readonly GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");
		private readonly Rect titleBarRect = new Rect(0f, 0f, 10000f, 20f);
		private Rect windowRect = new Rect(20f, 20f, Screen.width - 40, Screen.height - 40);

		private void OnEnable()
		{
#if UNITY_2020_1_OR_NEWER
			Application.logMessageReceived += HandleLog;
#else
			Application.RegisterLogCallback(HandleLog);
#endif
		}

		private void OnDisable()
		{
#if UNITY_2020_1_OR_NEWER
			Application.logMessageReceived -= HandleLog;
#else
			Application.RegisterLogCallback(null);
#endif
		}

		private void Update()
		{
			visibilityChanged += Time.deltaTime;

			// Use the new Input System for toggle key
			if (Keyboard.current != null && Keyboard.current[toggleKey].wasPressedThisFrame)
			{
				visible = !visible;
			}
#if UNITY_ANDROID || UNITY_IOS
			if (shakeToOpen && Input.acceleration.sqrMagnitude > shakeAcceleration && visibilityChanged > 2f)
			{
				visible = !visible;
				visibilityChanged = 0f;
			}
#endif
		}

		private void OnGUI()
		{
			if (visible)
			{
				windowRect = GUILayout.Window(123456, windowRect, ConsoleWindow, windowTitle);
			}
		}

		private void ConsoleWindow(int windowID)
		{
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			for (int i = 0; i < logs.Count; i++)
			{
				Log log = logs[i];
				if (collapse && i > 0 && log.message == logs[i - 1].message)
				{
					continue;
				}
				GUI.contentColor = logTypeColors[log.type];
				GUILayout.Label(log.message);
			}
			GUILayout.EndScrollView();
			GUI.contentColor = Color.white;
			GUILayout.BeginHorizontal();
			if (GUILayout.Button(clearLabel))
			{
				logs.Clear();
			}
			collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));
			GUILayout.EndHorizontal();
			GUI.DragWindow(titleBarRect);
		}

#if UNITY_2020_1_OR_NEWER
		private void HandleLog(string message, string stackTrace, LogType type)
		{
			logs.Add(new Log
			{
				message = message,
				stackTrace = stackTrace,
				type = type
			});
		}
#else
		private void HandleLog(string message, string stackTrace, LogType type)
		{
			logs.Add(new Log
			{
				message = message,
				stackTrace = stackTrace,
				type = type
			});
		}
#endif
	}
}