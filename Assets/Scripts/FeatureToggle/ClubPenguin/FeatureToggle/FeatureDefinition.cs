using ClubPenguin.Core.StaticGameData;
using System;
using UnityEngine;

namespace ClubPenguin.FeatureToggle
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/Feature")]
	public class FeatureDefinition : StaticGameDataDefinition
	{
		[StaticGameDataDefinitionId]
		public string Id;

		[TextArea]
		public string Description;
	}
}
