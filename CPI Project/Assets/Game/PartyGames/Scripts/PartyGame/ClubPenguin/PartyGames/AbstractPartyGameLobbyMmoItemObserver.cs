using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;

namespace ClubPenguin.PartyGames
{
	public abstract class AbstractPartyGameLobbyMmoItemObserver : AbstractMmoItemObserver<PartygameLobbyMmoItem>
	{
		public PartyGameDefinition PartyGame;

		protected abstract void onLobbyItemAdded(PartygameLobbyMmoItem item);

		protected abstract void onLobbyItemUpdated(PartygameLobbyMmoItem item, PartyGamePlayerCollection players);

		protected abstract void onLobbyItemRemoved(PartygameLobbyMmoItem item);

		protected override void onItemAdded(PartygameLobbyMmoItem item)
		{
			if (item.GetGameTemplateId() == PartyGame.Id)
			{
				onLobbyItemAdded(item);
			}
		}

		protected override void onItemUpdated(PartygameLobbyMmoItem item)
		{
			if (item.GetGameTemplateId() == PartyGame.Id)
			{
				onLobbyItemUpdated(item, getplayerDataFromMmoItem(item));
			}
		}

		protected override void onItemRemoved(PartygameLobbyMmoItem item)
		{
			if (item.GetGameTemplateId() == PartyGame.Id)
			{
				onLobbyItemRemoved(item);
			}
		}

		private PartyGamePlayerCollection getplayerDataFromMmoItem(PartygameLobbyMmoItem item)
		{
			return Service.Get<JsonService>().Deserialize<PartyGamePlayerCollection>(item.GetPlayerData());
		}
	}
}
