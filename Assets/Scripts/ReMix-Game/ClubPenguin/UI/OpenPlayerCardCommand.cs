using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class OpenPlayerCardCommand
	{
		private DataEntityHandle handle;

		private CPDataEntityCollection dataEntityCollection;

		private PlayerCardData playerCardDataData;

		private PrefabContentKey PlayerCardContentKey = new PrefabContentKey("PlayerCardPrefabs/PlayerCard");

		private GameObject modalBackground;

		public OpenPlayerCardCommand(DataEntityHandle handle)
		{
			this.handle = handle;
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle entityByType = dataEntityCollection.GetEntityByType<PlayerCardData>();
			playerCardDataData = dataEntityCollection.GetComponent<PlayerCardData>(entityByType);
		}

		public void Execute()
		{
			if (playerCardDataData.Enabled && !playerCardDataData.IsPlayerCardShowing && dataEntityCollection.HasComponent<DisplayNameData>(handle))
			{
				playerCardDataData.IsPlayerCardShowing = true;
				modalBackground = new GameObject("ModalBackground", typeof(ModalBackground));
				Content.LoadAsync(onPlayerCardLoaded, PlayerCardContentKey);
			}
		}

		private void onPlayerCardLoaded(string path, GameObject playerCardPrefab)
		{
			Object.Destroy(modalBackground);
			if (!handle.IsNull)
			{
				PlayerCardController component = Object.Instantiate(playerCardPrefab).GetComponent<PlayerCardController>();
				component.SetUpPlayerCard(handle);
				Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowTopPopup(component.gameObject, false, true, "Accessibility.Popup.Title.PlayerCard"));
			}
		}
	}
}
