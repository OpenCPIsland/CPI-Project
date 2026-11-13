using ClubPenguin.UI;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;

namespace ClubPenguin
{
	public class StartGameActiveStateHandler : ActiveStateHandler
	{
		public override string HandleStateChange()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerCardEvents.SetEnablePlayerCard(true));
			Service.Get<GameStateController>().DoFTUECheckOnZoneChange = false;
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add(SceneTransitionService.SceneArgs.ShowAvailableMarketingLoadingScreen.ToString(), true);
			Service.Get<ZoneTransitionService>().LoadZone(Service.Get<GameStateController>().GetZoneToLoad(), null, null, null, dictionary);
			return Service.Get<GameStateController>().ZoneConnectingEvent;
		}
	}
}
