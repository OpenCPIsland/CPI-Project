using ClubPenguin.Core;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class MainNavBarBackButtonEventToFSMEvent : MonoBehaviour
	{
		public string Target;

		public string Event;

		private void Start()
		{
			Service.Get<EventDispatcher>().AddListener<MainNavBarBackButton.MainNavBarBackButtonClicked>(onMainNavBarBackButtonClicked);
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<MainNavBarBackButton.MainNavBarBackButtonClicked>(onMainNavBarBackButtonClicked);
		}

		private bool onMainNavBarBackButtonClicked(MainNavBarBackButton.MainNavBarBackButtonClicked evt)
		{
			if (base.enabled)
			{
				StateMachineContext componentInChildren = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root).GetComponentInChildren<StateMachineContext>();
				componentInChildren.SendEvent(new ExternalEvent(Target, Event));
			}
			return false;
		}
	}
}
