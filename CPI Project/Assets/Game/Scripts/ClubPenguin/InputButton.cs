using ClubPenguin.Core;
using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClubPenguin
{
	[RequireComponent(typeof(Button))]
	public class InputButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		public InputEvents.Actions Action;

		private EventDispatcher dispatcher;

		private void Awake()
		{
			dispatcher = Service.Get<EventDispatcher>();
		}

		public void OnPointerDown(PointerEventData data)
		{
			if (GetComponent<Button>().IsInteractable())
			{
				dispatcher.DispatchEvent(new InputEvents.ActionEvent(Action));
			}
		}

		public void OnButtonStateChanged(TrayInputButton.ButtonState buttonState)
		{
			dispatcher.DispatchEvent(new InputEvents.ActionEnabledEvent(Action, buttonState != TrayInputButton.ButtonState.Disabled));
		}
	}
}
