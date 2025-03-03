using ClubPenguin.Net.Domain;
using ClubPenguin.PartyGames;
using Disney.Kelowna.Common;
using System;

namespace ClubPenguin.Game.PartyGames
{
	public class DanceBattleLobbyMmoItemObserver : AbstractPartyGameLobbyMmoItemObserver
	{
		public Action<long> LobbyStartedAction;

		public Action LobbyEndedAction;

		public Action<PartyGamePlayerCollection> LobbyPlayersUpdatedAction;

		private PartygameLobbyMmoItem mmoItem;

		public bool MmoItemExists
		{
			get
			{
				return mmoItem != null;
			}
		}

		public CPMMOItemId MmoItemId
		{
			get
			{
				return mmoItem.Id;
			}
		}

		protected override void onLobbyItemAdded(PartygameLobbyMmoItem item)
		{
			mmoItem = item;
			long arg = item.GetTimeStartedInSecondsSinceEpoc() + item.GetTimeToLiveInSeconds();
			LobbyStartedAction.InvokeSafe(arg);
		}

		protected override void onLobbyItemUpdated(PartygameLobbyMmoItem item, PartyGamePlayerCollection players)
		{
			mmoItem = item;
			LobbyPlayersUpdatedAction.InvokeSafe(players);
		}

		protected override void onLobbyItemRemoved(PartygameLobbyMmoItem item)
		{
			mmoItem = null;
			LobbyEndedAction.InvokeSafe();
		}

		private void OnDestroy()
		{
			LobbyStartedAction = null;
			LobbyEndedAction = null;
			LobbyPlayersUpdatedAction = null;
		}
	}
}
