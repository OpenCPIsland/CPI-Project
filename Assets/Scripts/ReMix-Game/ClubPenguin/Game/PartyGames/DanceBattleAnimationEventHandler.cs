using System;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	public class DanceBattleAnimationEventHandler : MonoBehaviour
	{
		public Action OnStartTurnSequence;

		public void StartTurnSequence()
		{
			if (OnStartTurnSequence != null)
			{
				OnStartTurnSequence();
			}
		}
	}
}
