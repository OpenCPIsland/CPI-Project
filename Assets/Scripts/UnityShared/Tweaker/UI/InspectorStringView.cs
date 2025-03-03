using System;
using UnityEngine;
using UnityEngine.UI;

namespace Tweaker.UI
{
	public class InspectorStringView : MonoBehaviour, IInspectorContentView
	{
		public InputField InputText;

		public event Action<string> ValueChanged;

		public event Action Destroyed;

		public void Awake()
		{
			InputText.onEndEdit.AddListener(OnEndEdit);
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
			InputText.onEndEdit.RemoveAllListeners();
			this.ValueChanged = null;
		}

		private void OnEndEdit(string value)
		{
			if (this.ValueChanged != null)
			{
				this.ValueChanged(value);
			}
		}
	}
}
