using System.Collections.Generic;
using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/Utils/InGameMixer")]
	public class InGameMixer : MonoBehaviour
	{
		private Vector2 scrollPosition = default(Vector2);

		private float width;

		private float slotHeight = 500f;

		private static GUIStyle thumbStyle = new GUIStyle();

		private static GUIStyle backgroundStyle = new GUIStyle();

		public float scale = 1f;

		private static List<GroupComponent> _soloComponents = new List<GroupComponent>();

		private bool toggleMixer;

		private void OnGUI()
		{
			if (toggleMixer)
			{
				DrawComponents();
			}
		}

		private float GetActualSize(float value)
		{
			return value * scale;
		}

		private void DisplayCenterText(string text)
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(text, GUILayout.MaxHeight(GetActualSize(30f)));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawSideChain(SideChain sideChain, float length)
		{
			GUILayout.BeginVertical("box");
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("SideChain");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			float db = AudioTools.LinearToDB(1f - sideChain._sideChainGain);
			AudioTools.DBToNormalizedDB(db);
			GUILayout.BeginVertical("box");
			GUILayout.Label("Gain:" + db.ToString("N0") + " dB");
			GUILayout.EndVertical();
			GUILayout.EndVertical();
		}

		private void DrawVolumeMeters(VolumeMeter volumeMeter, float length)
		{
			GUILayout.BeginVertical("box");
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("VolumeMeter");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			DrawMeter("FL", volumeMeter.volumeMeterState.mPeaks.mChannels[0], length);
			DrawMeter("FR", volumeMeter.volumeMeterState.mPeaks.mChannels[1], length);
			DrawMeter("RMS", volumeMeter.volumeMeterState.mRMS, length);
			GUILayout.EndVertical();
		}

		private static void DrawMeter(string name, float value, float length)
		{
			float db = AudioTools.LinearToDB(value);
			AudioTools.DBToNormalizedDB(db);
			GUILayout.BeginVertical("box");
			GUILayout.Label(name + ":" + db.ToString("N0") + " dB");
			GUILayout.EndVertical();
		}

		public void MixerSlot(string title, GroupComponent component)
		{
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
			GUILayout.BeginHorizontal(GUILayout.MaxHeight(GetActualSize(40f)));
			GUILayout.FlexibleSpace();
			component.Mute = GUILayout.Toggle(component.Mute, "Mute", "button");
			bool solo = component.Solo;
			component.Solo = GUILayout.Toggle(component.Solo, "Solo", "button");
			if (component.Solo)
			{
				for (int i = 0; i < _soloComponents.Count; i++)
				{
					if (_soloComponents[i] != null && _soloComponents[i] != component)
					{
						_soloComponents[i].Mute = true;
					}
				}
			}
			else if (solo != component.Solo)
			{
				for (int j = 0; j < _soloComponents.Count; j++)
				{
					if (_soloComponents[j] != null && _soloComponents[j] != component)
					{
						_soloComponents[j].Mute = false;
					}
				}
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginVertical();
			DisplayCenterText("Volume");
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			component.Volume = GUILayout.VerticalSlider(component.Volume, 1f, 0f, GUILayout.MinHeight(100f));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			DisplayCenterText(AudioTools.LinearToDB(component.Volume).ToString("F") + "dB");
			GUILayout.EndVertical();
			GUILayout.EndVertical();
		}

		private void DrawComponent(Component component, ref float x, float y)
		{
			if (component.ToString().Contains("GroupComponent"))
			{
				GroupComponent groupComponent = (GroupComponent)component;
				if (!_soloComponents.Contains(groupComponent))
				{
					_soloComponents.Add(groupComponent);
				}
				component.ToString().LastIndexOf(".");
				string name = component.name;
				int num = name.Length * 10;
				float num2 = slotHeight;
				if (num < 120)
				{
					num = 120;
				}
				VolumeMeter component2 = groupComponent.GetComponent<VolumeMeter>();
				if ((bool)component2)
				{
					num2 += 120f;
				}
				SideChain component3 = groupComponent.GetComponent<SideChain>();
				if ((bool)component3)
				{
					num2 += 120f;
				}
				GUILayout.BeginArea(new Rect(x, y, num, GetActualSize(num2)), name, GUI.skin.window);
				MixerSlot(name, groupComponent);
				if ((bool)component2)
				{
					DrawVolumeMeters(component2, GetActualSize(num - 10));
				}
				if ((bool)component3)
				{
					DrawSideChain(component3, GetActualSize(num - 30));
				}
				GUILayout.EndArea();
				x += num + 20;
			}
		}

		private void DrawComponents()
		{
			if (!FabricManager.IsInitialised())
			{
				return;
			}
			float value = slotHeight + 500f;
			scrollPosition = GUI.BeginScrollView(new Rect(0f, 0f, 700f, GetActualSize(value) + 20f), scrollPosition, new Rect(0f, 0f, width, GetActualSize(value)));
			Component[] componentsInChildren = FabricManager.Instance.gameObject.GetComponentsInChildren<GroupComponent>();
			float y = 20f;
			float x = 20f;
			foreach (Component component in componentsInChildren)
			{
				if (component != null)
				{
					DrawComponent(component, ref x, y);
				}
			}
			width = x;
			GUI.EndScrollView();
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.M))
			{
				if (toggleMixer)
				{
					toggleMixer = false;
					Debug.Log("Mixer off");
				}
				else
				{
					toggleMixer = true;
					Debug.Log("Mixer On");
				}
				Debug.Log("Mixer toggled");
			}
		}
	}
}
