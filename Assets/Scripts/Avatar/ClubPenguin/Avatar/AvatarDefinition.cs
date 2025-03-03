using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Avatar
{
	public class AvatarDefinition : ScriptableObject
	{
		public const string DEFINITION_NAME_PATTERN = "{0}_{1}_{2}_{3}LOD";

		public static readonly AssetContentKey DEFINITION_KEYPATTERN = new AssetContentKey("definitions/equipment/*");

		public static readonly string[] PartTypeStrings = new string[3]
		{
			"rep",
			"add",
			"sec"
		};

		public readonly Dictionary<string, int> EquipmentIndexLookup = new Dictionary<string, int>();

		public readonly Dictionary<string, int> SlotIndexLookup = new Dictionary<string, int>();

		public readonly Dictionary<string, int> BoneIndexLookup = new Dictionary<string, int>();

		public AvatarSlot[] Slots = new AvatarSlot[0];

		public EquipmentList EquipmentList;

		public RigDefinition RigDefinition;

		public string[] BoneNames = new string[0];

		public Matrix4x4[] BindPose;

		public BodyColorMaterialProperties BodyColor;

		public UnityEngine.Avatar UnityAvatar;

		public RendererProperties RenderProperties;

		public EquipmentModelDefinition GetEquipmentDefinition(DCustomEquipment customEq)
		{
			return GetEquipmentDefinition(customEq.Name);
		}

		public EquipmentModelDefinition GetEquipmentDefinition(string customEqName)
		{
			int value;
			if (EquipmentIndexLookup.TryGetValue(customEqName.ToLower(), out value))
			{
				return EquipmentList.Equipment[value];
			}
			Log.LogError(this, "Could not find equipment definition for " + customEqName);
			return null;
		}

		public void OnEnable()
		{
			UpdateLookups();
		}

		public void UpdateLookups()
		{
			EquipmentIndexLookup.Clear();
			if (EquipmentList != null)
			{
				for (int i = 0; i < EquipmentList.Equipment.Length; i++)
				{
					EquipmentIndexLookup[EquipmentList.Equipment[i].Name.ToLower()] = i;
				}
			}
			SlotIndexLookup.Clear();
			for (int i = 0; i < Slots.Length; i++)
			{
				SlotIndexLookup[Slots[i].Name] = i;
			}
			BoneIndexLookup.Clear();
			for (int i = 0; i < BoneNames.Length; i++)
			{
				BoneIndexLookup[BoneNames[i]] = i;
			}
		}

		public TypedAssetContentKey<EquipmentViewDefinition> CreatePartKey(EquipmentModelDefinition equipment, EquipmentModelDefinition.Part eqPart, int lodLevel)
		{
			string text = string.Format("{0}_{1}_{2}_{3}LOD", equipment.Name, PartTypeStrings[(int)eqPart.PartType], Slots[eqPart.SlotIndex].Name, lodLevel);
			return new TypedAssetContentKey<EquipmentViewDefinition>(DEFINITION_KEYPATTERN, text);
		}
	}
}
