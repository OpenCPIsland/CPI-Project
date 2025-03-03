using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClubPenguin
{
	[RequireComponent(typeof(Button))]
	public class LocomotionStateInputButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		public LocomotionState LocomotionState;

		private EventDispatcher dispatcher;

		private Button myButton;

		public void Awake()
		{
			myButton = GetComponent<Button>();
		}

		public void Start()
		{
			dispatcher = Service.Get<EventDispatcher>();
		}

		public void OnPointerDown(PointerEventData data)
		{
			if (myButton.IsInteractable())
			{
				dispatcher.DispatchEvent(new InputEvents.LocomotionStateEvent(LocomotionState));
			}
		}
	}
}
