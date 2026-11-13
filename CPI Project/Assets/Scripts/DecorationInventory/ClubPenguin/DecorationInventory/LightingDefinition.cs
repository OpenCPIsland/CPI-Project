using ClubPenguin.Core.StaticGameData;
using Disney.Kelowna.Common;
using System;
using UnityEngine;

namespace ClubPenguin.DecorationInventory
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/Igloo/Lighting")]
	public class LightingDefinition : IglooAssetDefinition<int>
	{
		[StaticGameDataDefinitionId]
		public int Id;

		[Header("The name used to identify the item in Axis and other internal tools")]
		public string InternalName;

		public Color AmbientSkyColor;

		public Color AmbientEquatorColor;

		public Color AmbientGroundColor;

		[Tooltip("Optional skybox")]
		public MaterialContentKey SkyboxMaterialKey;

		public override int GetId()
		{
			return Id;
		}
	}
}
