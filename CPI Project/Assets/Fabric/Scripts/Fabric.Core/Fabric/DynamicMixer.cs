using System.Collections.Generic;
using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/Mixing/DynamicMixer")]
	public class DynamicMixer : MonoBehaviour, IEventListener
	{
		private bool _isInitialised;

		private GroupComponent[] _groupComponents;

		private List<Preset> _activePresets;

		private Preset[] _presetList;

		[SerializeField]
		[HideInInspector]
		public string _currentSwitchedPreset = "";

		[HideInInspector]
		[SerializeField]
		public bool _enableHierarchyChangeDetector = true;

		bool IEventListener.IsDestroyed
		{
			get
			{
				return this == null;
			}
		}

		private void Start()
		{
			EventManager.Instance.RegisterListener(this, "DynamicMixer");
			_presetList = GetPresets();
			_activePresets = new List<Preset>();
		}

		public Preset CreatePreset(string name)
		{
			if (GetFabricManager.Instance() == null)
			{
				return null;
			}
			_groupComponents = GetFabricManager.Instance().gameObject.GetComponentsInChildren<GroupComponent>();
			_presetList = base.gameObject.GetComponentsInChildren<Preset>();
			for (int i = 0; i < _presetList.Length; i++)
			{
				if (_presetList[i].Name == name)
				{
					return null;
				}
			}
			GameObject gameObject = new GameObject(name);
			Preset preset = gameObject.AddComponent<Preset>();
			preset.transform.parent = base.transform;
			preset.Init(_groupComponents, name);
			_presetList = base.gameObject.GetComponentsInChildren<Preset>();
			return preset;
		}

		public int GetNumActivePresets()
		{
			if (_activePresets == null)
			{
				return 0;
			}
			return _activePresets.Count;
		}

		public Preset GetActivePresetByIndex(int index)
		{
			if (index >= _activePresets.Count)
			{
				return null;
			}
			return _activePresets[index];
		}

		public void DeletePreset(string name)
		{
			Preset presetByName = GetPresetByName(name);
			if (presetByName != null)
			{
				Object.DestroyImmediate(presetByName.gameObject);
				_presetList = base.gameObject.GetComponentsInChildren<Preset>();
			}
		}

		public void DeletePreset(Preset preset)
		{
			if (preset != null)
			{
				Object.DestroyImmediate(preset.gameObject);
				_presetList = base.gameObject.GetComponentsInChildren<Preset>();
			}
		}

		public Preset[] GetPresets()
		{
			return base.gameObject.GetComponentsInChildren<Preset>();
		}

		public Preset GetPresetByName(string name)
		{
			_presetList = GetPresets();
			for (int i = 0; i < _presetList.Length; i++)
			{
				Preset preset = _presetList[i];
				if (preset.Name == name)
				{
					return preset;
				}
			}
			return null;
		}

		public void AddPreset(string name)
		{
			Preset presetByName = GetPresetByName(name);
			if (presetByName != null && !presetByName.HasEventNameSet())
			{
				AddPreset(presetByName);
			}
		}

		public void AddPreset(Preset preset)
		{
			if (preset != null && !preset.IsActive())
			{
				if (!_activePresets.Contains(preset))
				{
					_activePresets.Add(preset);
				}
				preset.Activate();
				_currentSwitchedPreset = preset.Name;
			}
		}

		public void RemovePreset(string name)
		{
			Preset presetByName = GetPresetByName(name);
			if (presetByName != null)
			{
				for (int i = 0; i < _activePresets.Count; i++)
				{
					if (_activePresets[i] == presetByName)
					{
						presetByName.Deactivate();
						return;
					}
				}
			}
			if (_activePresets.Count == 0)
			{
				_currentSwitchedPreset = null;
			}
		}

		public void SwitchPreset(string targetPreset)
		{
			SwitchPreset(_currentSwitchedPreset, targetPreset);
		}

		public void SwitchPreset(string sourcePreset, string targetPreset)
		{
			Preset presetByName = GetPresetByName(sourcePreset);
			Preset presetByName2 = GetPresetByName(targetPreset);
			if (presetByName == null)
			{
				AddPreset(targetPreset);
			}
			else if (presetByName2 != null && targetPreset != sourcePreset)
			{
				presetByName2.SwitchFromPreset(presetByName);
				_activePresets.Remove(presetByName);
				_activePresets.Add(presetByName2);
				_currentSwitchedPreset = targetPreset;
			}
		}

		public void RemovePreset(Preset preset)
		{
			if (!(preset != null))
			{
				return;
			}
			int num = 0;
			while (true)
			{
				if (num < _activePresets.Count)
				{
					if (_activePresets[num] == preset)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			preset.Deactivate();
		}

		public void SetPresetEventName(string presetName, string eventName)
		{
			Preset presetByName = GetPresetByName(presetName);
			if (presetByName != null)
			{
				presetByName._eventName = eventName;
			}
		}

		public void SetPresetGameObject(string presetName, GameObject go)
		{
			Preset presetByName = GetPresetByName(presetName);
			if (presetByName != null)
			{
				presetByName._parentGameObject = go;
			}
		}

		EventStatus IEventListener.Process(Event zEvent)
		{
			switch (zEvent.EventAction)
			{
			case EventAction.AddPreset:
				AddPreset((string)zEvent._parameter);
				break;
			case EventAction.RemovePreset:
				RemovePreset((string)zEvent._parameter);
				break;
			case EventAction.ResetDynamicMixer:
				Reset();
				break;
			case EventAction.SwitchPreset:
			{
				SwitchPresetData switchPresetData = (SwitchPresetData)zEvent._parameter;
				if (switchPresetData != null)
				{
					if (switchPresetData._sourcePreset.Length > 0)
					{
						SwitchPreset(switchPresetData._sourcePreset, switchPresetData._targetPreset);
					}
					else
					{
						SwitchPreset(_currentSwitchedPreset, switchPresetData._targetPreset);
					}
				}
				break;
			}
			}
			return EventStatus.Handled;
		}

		bool IEventListener.IsActive(GameObject parentGameObject)
		{
			return false;
		}

		bool IEventListener.GetEventInfo(GameObject parentGameObject, ref EventInfo eventInfo)
		{
			return false;
		}

		bool IEventListener.GetEventListeners(string eventName, List<EventListener> listeners)
		{
			return false;
		}

		bool IEventListener.GetEventListeners(int eventID, List<EventListener> listeners)
		{
			return false;
		}

		public void Reset()
		{
			if (_activePresets == null)
			{
				return;
			}
			for (int i = 0; i < _activePresets.Count; i++)
			{
				Preset preset = _activePresets[i];
				if (!preset._isPersistent)
				{
					preset.Reset();
					_activePresets.Remove(preset);
				}
			}
		}

		private void Update()
		{
			if (!_isInitialised)
			{
				_groupComponents = FabricManager.Instance.gameObject.GetComponentsInChildren<GroupComponent>();
				_isInitialised = true;
			}
			if (_presetList != null)
			{
				for (int i = 0; i < _presetList.Length; i++)
				{
					Preset preset = _presetList[i];
					if (preset.HasEventNameSet() && preset.IsEventActive())
					{
						AddPreset(preset);
					}
				}
			}
			if (_groupComponents == null)
			{
				return;
			}
			for (int j = 0; j < _groupComponents.Length; j++)
			{
				GroupComponent groupComponent = _groupComponents[j];
				float num = 0f;
				float num2 = 1f;
				for (int k = 0; k < _activePresets.Count; k++)
				{
					Preset preset2 = _activePresets[k];
					if (preset2.HasEventNameSet() && !preset2.IsEventActive() && !preset2.IsDeactivating)
					{
						preset2.Deactivate();
					}
					if (preset2.IsActive())
					{
						GroupPreset groupComponentByID = preset2.GetGroupComponentByID(groupComponent.GetInstanceID());
						if (groupComponentByID != null)
						{
							num += groupComponentByID.CalculateVolume();
							num2 *= groupComponentByID.CalculatePitch();
						}
					}
					else
					{
						_activePresets.Remove(preset2);
					}
				}
				float value = AudioTools.DBToLinear(num);
				AudioTools.Limit(ref value, 0f, 1f);
				groupComponent.MixerVolume = value;
				groupComponent.MixerPitch = num2;
			}
		}
	}
}
