#define UNITY_ASSERTIONS
using ClubPenguin.Adventure;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace ClubPenguin
{
	public class MemberAccess : MonoBehaviour
	{
		public bool isMemberOnly = true;

		public string[] QuestNameExclusions;

		public string[] QuestObjectiveExclusions;

		public bool IsMemberLocked
		{
			get
			{
				return isMemberOnly && !isMembershipIgnoredForActiveQuestObjective();
			}
		}

		public void OnValidate()
		{
		}

		private bool isMembershipIgnoredForActiveQuestObjective()
		{
			try
			{
				if (QuestNameExclusions != null && QuestNameExclusions.Length > 0)
				{
					Quest activeQuest = Service.Get<QuestService>().ActiveQuest;
					if (activeQuest != null)
					{
						for (int i = 0; i < QuestNameExclusions.Length; i++)
						{
							Assert.IsTrue(!string.IsNullOrEmpty(QuestNameExclusions[i]), "Quest name cannot be empty");
							Assert.IsTrue(!string.IsNullOrEmpty(QuestObjectiveExclusions[i]), "Quest objective cannot be empty");
							Assert.IsTrue(!string.IsNullOrEmpty(activeQuest.Id), "Quest id cannot be empty");
							Assert.IsTrue(!string.IsNullOrEmpty(activeQuest.CurrentObjectiveName), "Quest objective cannot be empty");
							if (activeQuest.Id == QuestNameExclusions[i] && activeQuest.CurrentObjectiveName == QuestObjectiveExclusions[i])
							{
								return true;
							}
						}
					}
				}
			}
			catch (Exception)
			{
				Log.LogError(this, "Failed to query quest service. Membership line enforced by default.");
			}
			return false;
		}
	}
}
