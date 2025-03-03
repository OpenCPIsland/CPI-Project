using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace ClubPenguin
{
	public class PlayerCardService : AbstractDataModelService
	{
		private EventChannel eventChannel;

		private PlayerCardData playerCardDataData;

		private void Start()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<PlayerCardEvents.SetEnablePlayerCard>(onEnablePlayerCard);
			DataEntityHandle handle = dataEntityCollection.AddEntity("PlayerCardData");
			playerCardDataData = dataEntityCollection.AddComponent<PlayerCardData>(handle);
		}

		private void OnDestroy()
		{
			eventChannel.RemoveAllListeners();
		}

		private bool onEnablePlayerCard(PlayerCardEvents.SetEnablePlayerCard evt)
		{
			playerCardDataData.Enabled = evt.Enabled;
			return true;
		}
	}
}
