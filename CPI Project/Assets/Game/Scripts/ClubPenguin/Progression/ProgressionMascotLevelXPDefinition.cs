using ClubPenguin.Adventure;
using ClubPenguin.Core.StaticGameData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Progression
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/Progression/MascotLevelXP")]
	public class ProgressionMascotLevelXPDefinition : StaticGameDataDefinition
	{
		public MascotDefinition Mascot;

		public List<int> Levels = new List<int>();
	}
}
