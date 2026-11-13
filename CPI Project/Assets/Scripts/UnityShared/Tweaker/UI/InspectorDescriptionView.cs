using System;
using UnityEngine;
using TMPro;

namespace Tweaker.UI
{
	public class InspectorDescriptionView : MonoBehaviour, IInspectorContentView
	{
		public TextMeshProUGUI DescriptionText;

		public event Action Destroyed;

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
		}
	}
}
