using System;
using UnityEngine;
using UnityEngine.UI;

namespace Tweaker.UI
{
	public class InspectorSliderView : MonoBehaviour, IInspectorContentView
	{
		public Slider Slider;

		public event Action<float> ValueChanged;

		public event Action Destroyed;

		public void Awake()
		{
			Slider.onValueChanged.AddListener(OnValueChanged);
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
			Slider.onValueChanged.RemoveAllListeners();
		}

		private void OnValueChanged(float value)
		{
			if (this.ValueChanged != null)
			{
				this.ValueChanged(value);
			}
		}
	}
}
