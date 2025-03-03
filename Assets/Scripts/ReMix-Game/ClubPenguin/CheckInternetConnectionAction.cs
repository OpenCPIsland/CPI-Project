using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitCoreServicesAction))]
	[RequireComponent(typeof(InitLocalizerSetupAction))]
	public class CheckInternetConnectionAction : InitActionComponent
	{
		private const string titleToken = "GlobalUI.ErrorMessages.NetworkConnectionError";

		private const string bodyToken = "GlobalUI.ErrorMessages.CheckNetworkConnection";

		public Sprite Image;

		public float RetryTime;

		public string Url;

		private GameObject prompt;

		private bool isConnected;

		public override bool HasSecondPass
		{
			get
			{
				return false;
			}
		}

		public override bool HasCompletedPass
		{
			get
			{
				return false;
			}
		}

		public override IEnumerator PerformFirstPass()
		{
			if (!Service.Get<GameSettings>().OfflineMode)
			{
				yield return checkInternetConnection(Url);
				if (!isConnected)
				{
					DPrompt data = new DPrompt("GlobalUI.ErrorMessages.NetworkConnectionError", "GlobalUI.ErrorMessages.CheckNetworkConnection", DPrompt.ButtonFlags.None, Image);
					prompt = Service.Get<PromptManager>().ShowPrompt(data, null);
				}
				while (!isConnected)
				{
					yield return new WaitForSeconds(RetryTime);
					yield return checkInternetConnection(Url);
				}
				if (prompt != null)
				{
					Object.Destroy(prompt);
				}
			}
		}

		private IEnumerator checkInternetConnection(string url)
		{
			WWW www = new WWW(url);
			yield return www;
			isConnected = (www.error == null);
		}
	}
}
