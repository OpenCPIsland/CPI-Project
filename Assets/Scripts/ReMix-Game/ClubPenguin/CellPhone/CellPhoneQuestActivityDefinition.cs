using ClubPenguin.Adventure;
using System;
using UnityEngine;

namespace ClubPenguin.CellPhone
{
	[Serializable]
	public class CellPhoneQuestActivityDefinition : CellPhoneActivityDefinition
	{
		[HideInInspector]
		public QuestDefinition Quest;
	}
}
