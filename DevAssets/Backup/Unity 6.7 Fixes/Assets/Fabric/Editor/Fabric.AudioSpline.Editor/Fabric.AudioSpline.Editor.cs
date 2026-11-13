using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

[assembly: AssemblyProduct("Fabric.AudioSpline.Editor")]
[assembly: AssemblyCopyright("Copyright © TazMan-Audio 2026")]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Tazman-Audio")]
[assembly: ComVisible(false)]
[assembly: CompilationRelaxations(8)]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyTitle("Fabric.AudioSpline.Editor")]
[assembly: Guid("2bf2dc8d-1d42-4603-9a10-0d9d0fd143eb")]
[assembly: AssemblyFileVersion("4.0.1.0")]
[assembly: AssemblyVersion("4.0.1.0")]
namespace Fabric
{
	public class DefaultHandles
	{
		public static bool Hidden
		{
			get
			{
				Type typeFromHandle = typeof(Tools);
				FieldInfo field = typeFromHandle.GetField("s_Hidden", BindingFlags.Static | BindingFlags.NonPublic);
				return (bool)field.GetValue(null);
			}
			set
			{
				Type typeFromHandle = typeof(Tools);
				FieldInfo field = typeFromHandle.GetField("s_Hidden", BindingFlags.Static | BindingFlags.NonPublic);
				field.SetValue(null, value);
			}
		}
	}
	[CustomEditor(typeof(AudioSpline))]
	public class PolyLineEditor : Editor
	{
		private AudioSpline _audioSpline;

		private SerializedProperty resolution;

		private SerializedProperty splineColor;

		private SerializedProperty audioSplineSource;

		private int curPointIndex = -1;

		private bool hideDefaultHandle = true;

		[MenuItem("Fabric/Extensions/AudioSpline")]
		private static void AudioSplineMenuItem()
		{
			GameObject activeGameObject = Selection.activeGameObject;
			if (!(activeGameObject == null))
			{
				GameObject gameObject = new GameObject("AudioSpline");
				gameObject.transform.parent = activeGameObject.transform;
				AudioSpline audioSpline = gameObject.AddComponent<AudioSpline>();
				audioSpline.Initialise();
			}
		}

		private void OnEnable()
		{
			_audioSpline = base.target as AudioSpline;
			resolution = base.serializedObject.FindProperty("_resolution");
			splineColor = base.serializedObject.FindProperty("_splineColor");
			audioSplineSource = base.serializedObject.FindProperty("_audioSplineSource");
			_audioSpline.Initialise();
			DefaultHandles.Hidden = hideDefaultHandle;
		}

		private void OnDisable()
		{
			DefaultHandles.Hidden = false;
		}

		private void DoMyWindow(int windowID)
		{
			if (curPointIndex != -1)
			{
				GUILayout.Label("Selected Point: " + _audioSpline._CRSpline.pts[curPointIndex].name);
				GUILayout.Space(10f);
				GUILayout.BeginHorizontal();
				if (curPointIndex < _audioSpline._CRSpline.pts.Length - 2 && GUILayout.Button("Insert After"))
				{
					_audioSpline.AddPoint(curPointIndex);
					curPointIndex++;
				}
				if (curPointIndex > 1 && curPointIndex < _audioSpline._CRSpline.pts.Length - 2 && GUILayout.Button("Delete"))
				{
					_audioSpline.RemovePoint(curPointIndex);
				}
				if (curPointIndex > 1 && GUILayout.Button("Insert Before"))
				{
					_audioSpline.AddPoint(curPointIndex - 1);
				}
				GUILayout.EndHorizontal();
			}
			else
			{
				GUILayout.BeginHorizontal();
				if (_audioSpline._CRSpline.pts.Length == 0)
				{
					if (GUILayout.Button("Insert"))
					{
						_audioSpline.AddPoint(0);
					}
				}
				else
				{
					if (GUILayout.Button("Insert First"))
					{
						_audioSpline.AddPoint(0);
						curPointIndex = 0;
					}
					if (GUILayout.Button("Insert Last"))
					{
						_audioSpline.AddPoint(-1);
						curPointIndex = _audioSpline._CRSpline.pts.Length - 1;
					}
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.Space(10f);
			if (hideDefaultHandle)
			{
				if (GUILayout.Button("Show Main Transform"))
				{
					hideDefaultHandle = false;
					DefaultHandles.Hidden = hideDefaultHandle;
				}
			}
			else if (GUILayout.Button("Hide Main Transform"))
			{
				hideDefaultHandle = true;
				DefaultHandles.Hidden = hideDefaultHandle;
			}
		}

		private void OnSceneGUI()
		{
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Expected O, but got Unknown
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Expected O, but got Unknown
			int hashCode = GetHashCode();
			for (int i = 0; i < _audioSpline._CRSpline.pts.Length; i++)
			{
				Vector3 vector = _audioSpline._CRSpline.pts[i].transform.position;
				Handles.Label(vector, _audioSpline._CRSpline.pts[i].name);
				float handleSize = HandleUtility.GetHandleSize(vector);
				int controlID = GUIUtility.GetControlID(hashCode, FocusType.Passive);
				bool flag = UnityEngine.Event.current.type == EventType.Used;
				Handles.CapFunction val = Handles.SphereHandleCap;
				if (i == 0 || i == _audioSpline._CRSpline.pts.Length - 1)
				{
					val = Handles.ConeHandleCap;
				}
				Handles.ScaleValueHandle(0f, vector, Quaternion.identity, handleSize, val, 0f);
				if (curPointIndex == i)
				{
					vector = Handles.PositionHandle(vector, Quaternion.identity);
				}
				int controlID2 = GUIUtility.GetControlID(hashCode, FocusType.Passive);
				bool flag2 = !flag && UnityEngine.Event.current.type == EventType.Used;
				if ((controlID < GUIUtility.hotControl && GUIUtility.hotControl < controlID2) || flag2)
				{
					curPointIndex = i;
				}
				_audioSpline._CRSpline.pts[i].transform.position = vector;
			}
			Handles.BeginGUI();
			Rect rect = new Rect(0f, 0f, 300f, 125f);
			GUI.Window(0, new Rect((float)Screen.width - rect.width - 10f, (float)Screen.height - rect.height - 50f, rect.width, rect.height), DoMyWindow, "AudioSpline Tool Bar");
			Handles.EndGUI();
		}

		public override void OnInspectorGUI()
		{
			GUILayout.BeginHorizontal("Box");
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("?", GUILayout.MaxHeight(15f), GUILayout.MaxWidth(20f)))
			{
				Application.OpenURL("http://tazman-audio.screenstepslive.com/s/Docs/m/Fabric/l/290618-audiospline");
			}
			GUILayout.EndHorizontal();
			if (!(_audioSpline == null))
			{
				GUILayout.BeginVertical("Box");
				EditorGUILayout.Slider(resolution, 0.001f, 1f, new GUIContent("Resolution: "));
				EditorGUILayout.PropertyField(splineColor, new GUIContent("SplineColor: "));
				if (GUILayout.Button("Update Points"))
				{
					_audioSpline.UpdateSplinePoints();
				}
				GUILayout.EndVertical();
				if (GUI.changed)
				{
					base.serializedObject.ApplyModifiedProperties();
					_audioSpline.UpdateSplinePoints();
					EditorUtility.SetDirty(_audioSpline);
				}
			}
		}
	}
}
