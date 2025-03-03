using ClubPenguin.Analytics;
using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClubPenguin.UI
{
	public class InteractIndicator : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		public OnOffSpriteSelector BackgroundSprite;

		public OnOffSpriteSelector IconSprite;

		public OnOffTintSelector Arrow;

		public GameObject LockIcon;

		public Animator IndicatorAnimator;

		private bool isMemberLocked;

		public void SetMemberLockedStatus(bool isMemberLocked)
		{
			this.isMemberLocked = isMemberLocked;
			BackgroundSprite.IsOn = !isMemberLocked;
			IconSprite.IsOn = !isMemberLocked;
			Arrow.IsOn = !isMemberLocked;
			LockIcon.SetActive(isMemberLocked);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (isMemberLocked)
			{
				string trigger = base.gameObject.name.Replace("(Clone)", "");
				Service.Get<GameStateController>().ShowAccountSystemMembership(trigger);
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(new InputEvents.ActionEvent(InputEvents.Actions.Interact));
			}
			Service.Get<ICPSwrveService>().ActionSingular("InputHotkeyActionon_screen", "inworld_interact", "on_screen");
		}

		public void Show()
		{
			base.gameObject.SetActive(true);
			IndicatorAnimator.ResetTrigger("Close");
		}

		public void Hide()
		{
			if (base.gameObject.activeSelf)
			{
				IndicatorAnimator.SetTrigger("Close");
			}
		}

		public void OnAnimationComplete()
		{
			base.gameObject.SetActive(false);
		}
	}
}
