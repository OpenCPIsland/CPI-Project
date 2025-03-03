using ClubPenguin.Locomotion;
using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	[RequireComponent(typeof(InputButtonMapper))]
	public class ScavengerHuntHideButton : MonoBehaviour
	{
		private const string BUTTON_ID = "ActionButton";

		private Animator animator;

		private TrayInputButtonDisabler buttonDisabler;

		private bool isDisabledByInAir = false;

		private bool isDisabledByScavengerHunt = false;

		private void Start()
		{
			animator = getPenguinAnimator();
			buttonDisabler = getButtonDisabler();
			checkInitialButtonState();
			Service.Get<EventDispatcher>().AddListener<UIDisablerEvents.DisableUIElement>(onUiElementDisabled);
			Service.Get<EventDispatcher>().AddListener<UIDisablerEvents.EnableUIElement>(onUiElementEnabled);
		}

		private void checkInitialButtonState()
		{
			if (!buttonDisabler.IsEnabled)
			{
				isDisabledByScavengerHunt = true;
			}
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<UIDisablerEvents.DisableUIElement>(onUiElementDisabled);
			Service.Get<EventDispatcher>().RemoveListener<UIDisablerEvents.EnableUIElement>(onUiElementEnabled);
		}

		private void Update()
		{
			if (animator != null && !isDisabledByScavengerHunt)
			{
				bool flag = LocomotionUtils.IsInAir(LocomotionUtils.GetAnimatorStateInfo(animator));
				if (flag && !isDisabledByInAir)
				{
					disableButton();
				}
				else if (!flag && isDisabledByInAir)
				{
					enableButton();
				}
			}
		}

		private bool onUiElementDisabled(UIDisablerEvents.DisableUIElement evt)
		{
			if (evt.ElementID == "ActionButton")
			{
				isDisabledByScavengerHunt = true;
			}
			return false;
		}

		private bool onUiElementEnabled(UIDisablerEvents.EnableUIElement evt)
		{
			if (evt.ElementID == "ActionButton")
			{
				isDisabledByScavengerHunt = false;
			}
			return false;
		}

		private void disableButton()
		{
			buttonDisabler.DisableElement(false);
			isDisabledByInAir = true;
		}

		private void enableButton()
		{
			buttonDisabler.EnableElement();
			isDisabledByInAir = false;
		}

		private TrayInputButtonDisabler getButtonDisabler()
		{
			return GetComponentInParent<TrayInputButtonDisabler>();
		}

		private Animator getPenguinAnimator()
		{
			if (SceneRefs.ZoneLocalPlayerManager == null || SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject == null)
			{
				return null;
			}
			return SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<Animator>();
		}
	}
}
