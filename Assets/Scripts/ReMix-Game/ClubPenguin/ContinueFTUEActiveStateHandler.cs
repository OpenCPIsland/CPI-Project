using Disney.Kelowna.Common.SEDFSM;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace ClubPenguin
{
	public class ContinueFTUEActiveStateHandler : ActiveStateHandler
	{
		public override string HandleStateChange()
		{
			Service.Get<GameStateController>().DoFTUECheckOnZoneChange = true;
			if (SceneManager.GetActiveScene().name == Service.Get<GameStateController>().SceneConfig.HomeSceneName)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add(SceneTransitionService.SceneArgs.ShowAvailableMarketingLoadingScreen.ToString(), true);
				Service.Get<ZoneTransitionService>().LoadZone(Service.Get<GameStateController>().GetZoneToLoad(), Service.Get<GameStateController>().SceneConfig.TransitionSceneName, null, null, dictionary);
			}
			else
			{
				Service.Get<ZoneTransitionService>().ConnectToZone(Service.Get<GameStateController>().GetZoneToLoad());
			}
			return Service.Get<GameStateController>().ZoneConnectingEvent;
		}
	}
}
