using System;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	[RequireComponent(typeof(RectTransform))]
	public class UILayoutSystemBroadcaster : MonoBehaviour
	{
		public event Action<GameObject> onLayoutUpdate;

		public void OnRectTransformDimensionsChange()
		{
			if (this.onLayoutUpdate != null)
			{
				this.onLayoutUpdate(base.gameObject);
			}
		}
	}
}
