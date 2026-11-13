using ClubPenguin.Core;
using ClubPenguin.Net;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin
{
	public class OneIdLoginCheckStateHandler : AbstractAccountStateHandler
	{
		public string LoggedInEvent;

		public string MigrationNeededEvent;

		public string NotLoggedInEvent;

		public void OnStateChanged(string state)
		{
			if (!(state == HandledState) || !(rootStateMachine != null))
			{
				return;
			}
			if (Service.Get<SessionManager>().HasSession)
			{
				CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
				if (cPDataEntityCollection == null || cPDataEntityCollection.LocalPlayerHandle.IsNull)
				{
					throw new Exception("Unable to resolve data entity collection");
				}
				ProfileData component;
				if (!cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
				{
					throw new MissingReferenceException("No profile data found for local player");
				}
				if (component.IsMigratedPlayer)
				{
					rootStateMachine.SendEvent(LoggedInEvent);
				}
				else
				{
					rootStateMachine.SendEvent(MigrationNeededEvent);
				}
			}
			else
			{
				Service.Get<MembershipService>().LoginViaMembership = false;
				rootStateMachine.SendEvent(NotLoggedInEvent);
			}
		}
	}
}
