using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tweaker.UI
{
	public class InspectorToggleValueView : MonoBehaviour, IInspectorContentView
	{
		public Toggle Toggle;

		public TextMeshProUGUI ToggleText;

		public event Action<bool> ValueChanged;

		public event Action Destroyed;

		public bool Value
		{
			get => Toggle != null && Toggle.isOn;
			private set => SetValue(value, notify: false);
		}

		public void Awake()
		{
			if (Toggle != null)
			{
				Toggle.onValueChanged.AddListener(OnValueChanged);
			}
		}

		public void SetValue(bool value, bool notify = true)
		{
			if (Toggle == null)
			{
				return;
			}

			if (Toggle.isOn != value)
			{
				Toggle.isOn = value;
			}

			if (notify)
			{
				OnValueChanged(value);
			}
		}

		public void SetLabel(string label)
		{
			if (ToggleText != null)
			{
				ToggleText.text = label;
			}
		}

		public void DestroySelf()
		{
			Destroy(gameObject);
		}

		public void OnDestroy()
		{
			if (Destroyed != null)
			{
				Destroyed();
				Destroyed = null;
			}

			if (Toggle != null)
			{
				Toggle.onValueChanged.RemoveAllListeners();
			}

			ValueChanged = null;
		}

		private void OnValueChanged(bool value)
		{
			ValueChanged?.Invoke(value);
		}
	}
}
