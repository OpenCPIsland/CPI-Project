using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/Mixing/DynamicMixer/Preset")]
	public class Preset : MonoBehaviour
	{
		[SerializeField]
		[HideInInspector]
		public bool _isPersistent;

		[HideInInspector]
		[SerializeField]
		public bool _allowAutoGroupComponentUpdates = true;

		[HideInInspector]
		[SerializeField]
		public string _eventName;

		[SerializeField]
		[HideInInspector]
		public PresetActivationMode _activationMode;

		[SerializeField]
		[HideInInspector]
		public bool _includeGameObject;

		[HideInInspector]
		public GameObject _parentGameObject;

		private bool _isDeactivating;

		private GroupPreset[] _groupPreset;

		[NonSerialized]
		private Dictionary<int, GroupPreset> _groupPresetTable = new Dictionary<int, GroupPreset>();

		private string _name = "";

		[HideInInspector]
		public bool IsDeactivating
		{
			get
			{
				return _isDeactivating;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
		}

		private void Start()
		{
			_groupPreset = GetComponents<GroupPreset>();
			if (_eventName != null && _parentGameObject == null && _includeGameObject)
			{
				_parentGameObject = FabricManager.Instance.gameObject;
			}
			for (int i = 0; i < _groupPreset.Length; i++)
			{
				int instanceID = _groupPreset[i].GroupComponent.GetInstanceID();
				if (!_groupPresetTable.ContainsKey(instanceID))
				{
					_groupPresetTable.Add(instanceID, _groupPreset[i]);
				}
			}
			_name = base.name;
		}

		public void Init(GroupComponent[] groupComponents, string name)
		{
			base.gameObject.name = name;
			for (int i = 0; i < groupComponents.Length; i++)
			{
				AddGroupComponent(groupComponents[i]);
			}
			_groupPreset = GetComponents<GroupPreset>();
		}

		public void Activate()
		{
			if (_groupPreset != null)
			{
				for (int i = 0; i < _groupPreset.Length; i++)
				{
					_groupPreset[i].Activate();
				}
			}
			_isDeactivating = false;
		}

		public void Deactivate()
		{
			if (_groupPreset != null)
			{
				for (int i = 0; i < _groupPreset.Length; i++)
				{
					_groupPreset[i].Deactivate();
				}
			}
			_isDeactivating = true;
		}

		public void SwitchFromPreset(Preset sourcePreset)
		{
			if (_groupPreset == null)
			{
				return;
			}
			for (int i = 0; i < sourcePreset._groupPreset.Length; i++)
			{
				GroupPreset groupComponentByID = GetGroupComponentByID(sourcePreset._groupPreset[i].GroupComponent.GetInstanceID());
				if (groupComponentByID != null)
				{
					groupComponentByID.SwitchFromPreset(sourcePreset._groupPreset[i]);
				}
			}
			sourcePreset.Reset();
		}

		public void Reset()
		{
			if (_groupPreset != null)
			{
				for (int i = 0; i < _groupPreset.Length; i++)
				{
					_groupPreset[i].Reset();
				}
			}
		}

		public bool HasEventNameSet()
		{
			if (_activationMode == PresetActivationMode.EventName && _eventName != null)
			{
				return true;
			}
			return false;
		}

		public bool IsEventActive()
		{
			if ((_eventName != null && EventManager.Instance.IsEventActive(_eventName, _parentGameObject)) || _activationMode == PresetActivationMode.EventAction)
			{
				return true;
			}
			return false;
		}

		public bool IsActive()
		{
			if (_groupPreset != null)
			{
				for (int i = 0; i < _groupPreset.Length; i++)
				{
					if (_groupPreset[i].IsActive())
					{
						return true;
					}
				}
			}
			_isDeactivating = false;
			return false;
		}

		public void AddGroupComponent(GroupComponent groupComponent)
		{
			GroupPreset groupPreset = base.gameObject.AddComponent<GroupPreset>();
			groupPreset.Init(groupComponent);
		}

		public void RemoveGroupComponent(GroupComponent groupComponent)
		{
			_groupPreset = GetComponents<GroupPreset>();
			if (_groupPreset == null)
			{
				return;
			}
			int num = 0;
			while (true)
			{
				if (num < _groupPreset.Length)
				{
					if (_groupPreset[num].GroupComponent != null && _groupPreset[num].GroupComponent.Name == groupComponent.Name)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			UnityEngine.Object.DestroyImmediate(_groupPreset[num]);
		}

		public GroupPreset GetGroupComponentByID(int id)
		{
			if (_groupPresetTable.ContainsKey(id))
			{
				return _groupPresetTable[id];
			}
			return null;
		}

		public GroupPreset GetGroupComponentByName(string Name)
		{
			for (int i = 0; i < _groupPreset.Length; i++)
			{
				GroupPreset groupPreset = _groupPreset[i];
				if (groupPreset.GroupComponent != null && Name == groupPreset.Name)
				{
					return groupPreset;
				}
			}
			return null;
		}

		public GroupPreset GetGroupComponentByNameSlow(string Name)
		{
			_groupPreset = GetComponents<GroupPreset>();
			for (int i = 0; i < _groupPreset.Length; i++)
			{
				GroupPreset groupPreset = _groupPreset[i];
				if ((bool)groupPreset.GroupComponent && groupPreset.GroupComponent.name == Name)
				{
					return groupPreset;
				}
			}
			return null;
		}

		public bool HasGroupComponent(GroupComponent groupComponent)
		{
			GroupPreset[] componentsInChildren = base.gameObject.GetComponentsInChildren<GroupPreset>();
			foreach (GroupPreset groupPreset in componentsInChildren)
			{
				if (groupPreset.GroupComponent != null && groupPreset.GroupComponent == groupComponent)
				{
					return true;
				}
			}
			return false;
		}

		public float GetProgress()
		{
			float num = 1f;
			if (_groupPreset != null)
			{
				for (int i = 0; i < _groupPreset.Length; i++)
				{
					num *= _groupPreset[i].Progress();
				}
			}
			return num;
		}
	}
}
