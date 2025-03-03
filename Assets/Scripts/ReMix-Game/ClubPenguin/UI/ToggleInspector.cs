using Disney.Kelowna.Common;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Toggle))]
	public class ToggleInspector : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		public string ToggleId;

		public Toggle Toggle
		{
			get;
			set;
		}

		public event Action<string, bool, Vector2> ToggleClicked;

		private void Start()
		{
			Toggle = GetComponent<Toggle>();
		}

		private void OnDestroy()
		{
			this.ToggleClicked = null;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			this.ToggleClicked.InvokeSafe(ToggleId, Toggle.isOn, eventData.position);
		}
	}
}
