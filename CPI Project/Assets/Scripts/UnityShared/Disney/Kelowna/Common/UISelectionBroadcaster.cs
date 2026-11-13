using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Disney.Kelowna.Common
{
	public class UISelectionBroadcaster : MonoBehaviour, ISelectHandler, IEventSystemHandler
	{
		public event Action<GameObject> onSelected;

		public void OnSelect(BaseEventData eventData)
		{
			if (this.onSelected != null)
			{
				this.onSelected(base.gameObject);
			}
		}
	}
}
