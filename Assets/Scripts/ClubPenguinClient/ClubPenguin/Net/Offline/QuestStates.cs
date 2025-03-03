using ClubPenguin.Net.Domain;
using System;
using System.Collections.Generic;

namespace ClubPenguin.Net.Offline
{
	public struct QuestStates : IOfflineData
	{
		public class QuestState : ClubPenguin.Net.Domain.QuestState
		{
			public DateTime completedTime;
		}

		public List<QuestState> Quests;

		public void Init()
		{
			Quests = new List<QuestState>();
		}
	}
}
