using System;
using UnityEngine;
using UnityEngine.UI;

namespace Tweaker.UI
{
	public class InspectorToggleGroupView : MonoBehaviour, IInspectorContentView
	{
		public ToggleGroup ToggleGroup;

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
