using System;
using UnityEngine;
using UnityEngine.UI;

namespace Tweaker.UI
{
	public class InspectorBoolView : MonoBehaviour, IInspectorContentView
	{
		public Toggle Toggle;

		public Text ToggleText;

		public event Action<bool> ValueChanged;

		public event Action Destroyed;

		public void Awake()
		{
			Toggle.onValueChanged.AddListener(OnValueChanged);
		}

		public void DestroySelf()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}

		public void OnDestroy()
		{
			if (this.Destroyed != null)
			{
				this.Destroyed();
				this.Destroyed = null;
			}
			Toggle.onValueChanged.RemoveAllListeners();
			this.ValueChanged = null;
		}

		private void OnValueChanged(bool value)
		{
			if (this.ValueChanged != null)
			{
				this.ValueChanged(value);
			}
		}
	}
}
