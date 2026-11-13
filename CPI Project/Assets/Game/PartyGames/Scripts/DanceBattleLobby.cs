using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	public class DanceBattleLobby : MonoBehaviour
	{
		public GameObject PlayerPositionParent;

		private List<Transform> playerPositions;

		private int numPlayersPerTeam;

		private void Awake()
		{
			playerPositions = (List<Transform>)PlayerPositionParent.GetChildren();
		}

		public void Init(int numPlayersPerTeam)
		{
			this.numPlayersPerTeam = numPlayersPerTeam;
		}

		public Transform GetPlayerPosition(int teamId, int playerId)
		{
			int num = playerId;
			if (teamId == 1)
			{
				num += numPlayersPerTeam;
			}
			if (num >= 0 && num < playerPositions.Count)
			{
				return playerPositions[num];
			}
			return null;
		}
	}
}
