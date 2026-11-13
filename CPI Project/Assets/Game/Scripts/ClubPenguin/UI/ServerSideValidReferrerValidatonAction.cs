using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System.Collections;
using System.Diagnostics;

namespace ClubPenguin.UI
{
	public class ServerSideValidReferrerValidatonAction : InputFieldValidatonAction
	{
		private bool isBaseValidationDone;

		private bool isWaiting = false;

		private Stopwatch sw;

		private int maxTime;

		public override IEnumerator Execute(ScriptableActionPlayer player)
		{
			if (!isWaiting)
			{
				setup(player);
				isBaseValidationDone = false;
			}
			if (string.IsNullOrEmpty(inputString))
			{
				HasError = false;
				yield break;
			}
			ILocalUser localUser = Service.Get<SessionManager>().LocalUser;
			localUser.FindUser(inputString, onValidationComplete);
			isWaiting = true;
			sw = new Stopwatch();
			sw.Start();
			maxTime = 30000;
			while (!isBaseValidationDone && sw.ElapsedMilliseconds < maxTime)
			{
				yield return null;
			}
			if (!isBaseValidationDone)
			{
				HasError = false;
			}
			isWaiting = false;
		}

		private void onValidationComplete(IFindUserResult result)
		{
			HasError = !result.Success;
			isBaseValidationDone = true;
		}
	}
}
