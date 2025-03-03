using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Props
{
	[DisallowMultipleComponent]
	public class PropDisableControls : MonoBehaviour
	{
		public enum DisableTrigger
		{
			Start,
			Use
		}

		public DisableTrigger Trigger;

		public bool DisableJoystick;

		public bool HideJoystick;

		public bool DisableJump;

		public bool DisableTube;

		public bool DisableSnowball;

		public bool DisableMainNav;

		private PropUser propUser;

		private EventDispatcher dispatcher;

		private bool isLocalPlayer;

		private IEnumerator Start()
		{
			dispatcher = Service.Get<EventDispatcher>();
			propUser = GetComponentInParent<PropUser>();
			GameObject localPlayer = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			isLocalPlayer = (localPlayer == propUser.gameObject);
			yield return null;
			if (isLocalPlayer)
			{
				switch (Trigger)
				{
				case DisableTrigger.Start:
					disableControls();
					break;
				case DisableTrigger.Use:
					addListenerToPropUse();
					break;
				}
			}
			else
			{
				Object.Destroy(this);
			}
		}

		private void OnDestroy()
		{
			if (isLocalPlayer)
			{
				enableControls();
				propUser.EPropUseStarted -= onPropUseStarted;
			}
		}

		private void addListenerToPropUse()
		{
			propUser.EPropUseStarted += onPropUseStarted;
		}

		private void onPropUseStarted(Prop prop)
		{
			disableControls();
		}

		private void disableControls()
		{
			if (DisableJoystick)
			{
				dispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElement("Joystick", HideJoystick));
			}
			if (DisableJump)
			{
				dispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElement("JumpButton"));
			}
			if (DisableTube)
			{
				dispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElement("ControlsButton2"));
			}
			if (DisableSnowball)
			{
				dispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElement("ControlsButton1"));
			}
			if (DisableMainNav)
			{
				dispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElementGroup("MainNavButtons"));
				dispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElement("ChatButtons"));
			}
		}

		private void enableControls()
		{
			if (DisableJoystick)
			{
				dispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElement("Joystick"));
			}
			if (DisableJump)
			{
				dispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElement("JumpButton"));
			}
			if (DisableTube)
			{
				dispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElement("ControlsButton2"));
			}
			if (DisableSnowball)
			{
				dispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElement("ControlsButton1"));
			}
			if (DisableMainNav)
			{
				dispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElementGroup("MainNavButtons"));
				dispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElement("ChatButtons"));
			}
		}
	}
}
