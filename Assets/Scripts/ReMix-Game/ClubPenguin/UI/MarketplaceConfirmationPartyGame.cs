using ClubPenguin.Core;
using ClubPenguin.Game.PartyGames;
using ClubPenguin.Props;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class MarketplaceConfirmationPartyGame : MarketplaceConfirmation
	{
		private const string NUMBER_OF_PLAYERS_TOKEN = "Activity.Games.NumberPlayers.Range";

		private const string NUMBER_OF_PLAYERS_SINGLE_TOKEN = "Activity.Games.NumberPlayers";

		public Text NumberOfPlayersText;

		private ManifestContentKey launchersManifestContentKey = new ManifestContentKey("PartyGames/PartyGameLaunchers.Manifest");

		public override void SetItem(PropDefinition propDefinition, Texture icon, MarketplaceScreenController marketplaceController, RectTransform itemTransform, RectTransform scrollRectTransform)
		{
			base.SetItem(propDefinition, icon, marketplaceController, itemTransform, scrollRectTransform);
			Content.LoadAsync(onLaunchersManifestLoaded, launchersManifestContentKey);
		}

		private void onLaunchersManifestLoaded(string path, Manifest manifest)
		{
			int num = 1;
			int num2 = 1;
			ScriptableObject[] assets = manifest.Assets;
			foreach (ScriptableObject scriptableObject in assets)
			{
				PartyGameLauncherDefinition partyGameLauncherDefinition = (PartyGameLauncherDefinition)scriptableObject;
				if (partyGameLauncherDefinition.TriggerProp.NameOnServer == prop.NameOnServer)
				{
					num = partyGameLauncherDefinition.PartyGame.MinPlayerCount;
					num2 = partyGameLauncherDefinition.PartyGame.MaxPlayerCount;
					break;
				}
			}
			string text = "";
			text = ((num != num2) ? string.Format(Service.Get<Localizer>().GetTokenTranslation("Activity.Games.NumberPlayers.Range"), num, num2) : string.Format(Service.Get<Localizer>().GetTokenTranslation("Activity.Games.NumberPlayers"), num));
			NumberOfPlayersText.text = text;
		}
	}
}
