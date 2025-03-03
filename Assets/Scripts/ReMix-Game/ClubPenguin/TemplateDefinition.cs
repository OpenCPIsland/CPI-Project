using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/Template")]
	public class TemplateDefinition : StaticGameDataDefinition, IMemberLocked
	{
		[StaticGameDataDefinitionId]
		public int Id;

		public string AssetName;

		public bool WorkInProgress;

		[Header("Use Devon Token here.")]
		public string Name;

		[Header("Use Devon Token here.")]
		public string Description;

		public int Cost;

		[Header("Category this template will sort to in Customizer/Inventory")]
		public EquipmentCategoryDefinitionContentKey CategoryKey;

		[Header("Can the user edit this template?")]
		public bool IsEditable;

		[Header("Is this template locked so that only members can create an instance of it?")]
		public bool IsMemberOnlyCreatable;

		[Header("Camera Settings")]
		public float ZoomOffset;

		[Header("The Default Rotation for Mannequin on Y axis")]
		public float RotationYOffset;

		public TagDefinition[] Tags;

		[JsonProperty]
		[SerializeField]
		private bool isMemberOnly;

		[JsonIgnore]
		[Header("- Script created using render placement tool -")]
		public TemplateRenderDataContentKey RenderDataKey;

		public bool IsMemberOnly
		{
			get
			{
				return isMemberOnly;
			}
		}
	}
}
