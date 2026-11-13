using ClubPenguin.Core.StaticGameData;
using Disney.Kelowna.Common;
using Newtonsoft.Json;
using UnityEngine;

namespace ClubPenguin
{
	[CreateAssetMenu(menuName = "Definition/Status/TemporaryHeadStatus")]
	public class TemporaryHeadStatusDefinition : StaticGameDataDefinition
	{
		[StaticGameDataDefinitionId]
		public string Id;

		[JsonProperty]
		public TemporaryHeadStatusType Type;

		[SerializeField]
		[JsonIgnore]
		public PrefabContentKey EffectsContentKey;
	}
}
