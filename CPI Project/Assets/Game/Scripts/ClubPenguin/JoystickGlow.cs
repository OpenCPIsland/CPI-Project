using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	public class JoystickGlow : MonoBehaviour
	{
		private SpriteSelector spriteSelector;

		private EventDispatcher eventDispatcher;

		private void Awake()
		{
			spriteSelector = GetComponent<SpriteSelector>();
			eventDispatcher = Service.Get<EventDispatcher>();
		}

		private void OnEnable()
		{
			eventDispatcher.AddListener<VirtualJoystickEvents.JoystickActivated>(activateGlow);
			eventDispatcher.AddListener<VirtualJoystickEvents.JoystickDeactivated>(deactivateGlow);
		}

		private void OnDisable()
		{
			eventDispatcher.RemoveListener<VirtualJoystickEvents.JoystickActivated>(activateGlow);
			eventDispatcher.RemoveListener<VirtualJoystickEvents.JoystickDeactivated>(deactivateGlow);
		}

		private bool activateGlow(VirtualJoystickEvents.JoystickActivated evt)
		{
			spriteSelector.SelectSprite(1);
			return false;
		}

		private bool deactivateGlow(VirtualJoystickEvents.JoystickDeactivated evt)
		{
			spriteSelector.SelectSprite(0);
			return false;
		}
	}
}
