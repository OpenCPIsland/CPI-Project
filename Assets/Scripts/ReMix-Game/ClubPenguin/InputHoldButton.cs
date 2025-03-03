using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class InputHoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
	{
		public InputEvents.ChargeActions Action;

		private Button button;

		private EventDispatcher dispatcher;

		private float holdTime;

		private bool buttonDown;

		private bool prevInteractable = false;

		private void Awake()
		{
			dispatcher = Service.Get<EventDispatcher>();
			button = GetComponent<Button>();
			prevInteractable = button.IsInteractable();
		}

		public void OnDestroy()
		{
			if (buttonDown)
			{
				release(holdTime);
			}
		}

		public void OnPointerDown(PointerEventData data)
		{
			if (button.IsInteractable() && !buttonDown)
			{
				holdTime = 0f;
				buttonDown = true;
				dispatcher.DispatchEvent(new InputEvents.ChargeActionEvent(Action, true, holdTime));
			}
		}

		public void OnPointerUp(PointerEventData data)
		{
			if (buttonDown)
			{
				release(holdTime);
			}
		}

		private void release(float _holdTime)
		{
			dispatcher.DispatchEvent(new InputEvents.ChargeActionEvent(Action, false, _holdTime));
			buttonDown = false;
		}

		public void Update()
		{
			if (buttonDown)
			{
				holdTime += Time.deltaTime;
			}
			if (prevInteractable != button.IsInteractable())
			{
				prevInteractable = button.IsInteractable();
				if (!button.IsInteractable() && buttonDown)
				{
					release(holdTime);
				}
			}
		}
	}
}
