using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Extensions/Extensions Toggle Group")]
	[DisallowMultipleComponent]
	public class ExtensionsToggleGroup : UIBehaviour
	{
		[Serializable]
		public class ToggleGroupEvent : UnityEvent<bool>
		{
		}

		[SerializeField]
		private bool m_AllowSwitchOff = false;

		private List<ExtensionsToggle> m_Toggles = new List<ExtensionsToggle>();

		public ToggleGroupEvent onToggleGroupChanged = new ToggleGroupEvent();

		public ToggleGroupEvent onToggleGroupToggleChanged = new ToggleGroupEvent();

		public bool AllowSwitchOff
		{
			get
			{
				return m_AllowSwitchOff;
			}
			set
			{
				m_AllowSwitchOff = value;
			}
		}

		public ExtensionsToggle SelectedToggle
		{
			get;
			private set;
		}

		protected ExtensionsToggleGroup()
		{
		}

		private void ValidateToggleIsInGroup(ExtensionsToggle toggle)
		{
			if (toggle == null || !m_Toggles.Contains(toggle))
			{
				throw new ArgumentException(string.Format("Toggle {0} is not part of ToggleGroup {1}", new object[2]
				{
					toggle,
					this
				}));
			}
		}

		public void NotifyToggleOn(ExtensionsToggle toggle)
		{
			ValidateToggleIsInGroup(toggle);
			for (int i = 0; i < m_Toggles.Count; i++)
			{
				if (m_Toggles[i] == toggle)
				{
					SelectedToggle = toggle;
				}
				else
				{
					m_Toggles[i].IsOn = false;
				}
			}
			onToggleGroupChanged.Invoke(AnyTogglesOn());
		}

		public void UnregisterToggle(ExtensionsToggle toggle)
		{
			if (m_Toggles.Contains(toggle))
			{
				m_Toggles.Remove(toggle);
				toggle.onValueChanged.RemoveListener(NotifyToggleChanged);
			}
		}

		private void NotifyToggleChanged(bool isOn)
		{
			onToggleGroupToggleChanged.Invoke(isOn);
		}

		public void RegisterToggle(ExtensionsToggle toggle)
		{
			if (!m_Toggles.Contains(toggle))
			{
				m_Toggles.Add(toggle);
				toggle.onValueChanged.AddListener(NotifyToggleChanged);
			}
		}

		public bool AnyTogglesOn()
		{
			return m_Toggles.Find((ExtensionsToggle x) => x.IsOn) != null;
		}

		public IEnumerable<ExtensionsToggle> ActiveToggles()
		{
			return m_Toggles.Where((ExtensionsToggle x) => x.IsOn);
		}

		public void SetAllTogglesOff()
		{
			bool allowSwitchOff = m_AllowSwitchOff;
			m_AllowSwitchOff = true;
			for (int i = 0; i < m_Toggles.Count; i++)
			{
				m_Toggles[i].IsOn = false;
			}
			m_AllowSwitchOff = allowSwitchOff;
		}

		public void HasTheGroupToggle(bool value)
		{
			Debug.Log("Testing, the group has toggled [" + value + "]");
		}

		public void HasAToggleFlipped(bool value)
		{
			Debug.Log("Testing, a toggle has toggled [" + value + "]");
		}
	}
}
