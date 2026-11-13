using ClubPenguin.Core;
using ClubPenguin.Net;
using Disney.Kelowna.Common.SEDFSM;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class SettingsMain : MonoBehaviour
	{
		private enum InitialStateEvent
		{
			member,
			nonmember,
			allaccesseventmember,
			loggedout
		}

		private const string LOADING_STATE = "Loading";

		private void Start()
		{
			GetComponent<ScrollRect>().content = base.transform.GetChild(0).GetComponent<RectTransform>();
		}

		public void OnStateChanged(string stateName)
		{
			if (stateName.Equals("Loading"))
			{
				InitialStateEvent initialStateEvent = getInitialStateEvent();
				GetComponent<StateMachine>().SendEvent(initialStateEvent.ToString());
			}
		}

		private InitialStateEvent getInitialStateEvent()
		{
			InitialStateEvent result = InitialStateEvent.loggedout;
			if (Service.Get<SessionManager>().HasSession)
			{
				result = InitialStateEvent.nonmember;
				CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
				MembershipData component;
				if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
				{
					switch (component.MembershipType)
					{
					case MembershipType.Member:
						result = InitialStateEvent.member;
						break;
					case MembershipType.AllAccessEventMember:
						result = InitialStateEvent.allaccesseventmember;
						break;
					}
				}
			}
			return result;
		}
	}
}
