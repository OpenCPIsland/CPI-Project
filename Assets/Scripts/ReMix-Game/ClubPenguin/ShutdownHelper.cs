using ClubPenguin.Analytics;
using ClubPenguin.Mix;
using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	public class ShutdownHelper
	{
		private MonoBehaviour owner;

		public ShutdownHelper(MonoBehaviour owner)
		{
			this.owner = owner;
		}

		public void Shutdown()
		{
			Service.Get<ICPSwrveService>().Action("desktop", "exit_game");
			try
			{
				AsynchOnFinishedManifold asynchOnFinishedManifold = new AsynchOnFinishedManifold(quitWhenSafe);
				asynchOnFinishedManifold.MainStart();
				if (Service.Get<SessionManager>().HasSession)
				{
					Service.Get<SessionManager>().Logout(asynchOnFinishedManifold);
				}
				else
				{
					Service.Get<RememberMeService>().ResetCurrentUsername();
					Service.Get<MixLoginCreateService>().LogoutLastSession(asynchOnFinishedManifold);
				}
				asynchOnFinishedManifold.MainFinished();
			}
			catch (Exception ex)
			{
				Log.LogException(this, ex);
				quitWhenSafe();
			}
		}

		private void quitWhenSafe()
		{
			owner.StartCoroutine(waitAndQuit());
		}

		private IEnumerator waitAndQuit()
		{
			yield return new WaitForSeconds(1.5f);
			owner = null;
			Application.Quit();
		}
	}
}
