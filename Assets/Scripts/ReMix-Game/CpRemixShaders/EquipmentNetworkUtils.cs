using ClubPenguin;
using ClubPenguin.Avatar;
using ClubPenguin.ClothingDesigner.ItemCustomizer;
using System.Collections.Generic;

namespace CpRemixShaders
{
	public static class EquipmentNetworkUtils
	{
		public static DCustomEquipment GetModelFromCustomizerChildList(List<CustomizerChild> customizerChildList, TemplateDefinition templateDefinition, IDictionary<string, int> decalDefinitionIds)
		{
			DCustomEquipment result = default(DCustomEquipment);
			result.Name = templateDefinition.AssetName;
			result.DefinitionId = templateDefinition.Id;
			List<DCustomEquipmentPart> list = new List<DCustomEquipmentPart>();
			for (int i = 0; i < customizerChildList.Count; i++)
			{
				if (customizerChildList[i].gameObject.activeSelf && customizerChildList[i].HasEquipment)
				{
					DCustomEquipmentPart item = default(DCustomEquipmentPart);
					item.SlotIndex = customizerChildList[i].SlotIndex;
					EquipmentShaderParams equipmentShaderParams = EquipmentShaderParams.FromMaterial(customizerChildList[i].SharedMaterial);
					List<DCustomEquipmentDecal> list2 = new List<DCustomEquipmentDecal>();
					if (equipmentShaderParams.DecalRed1Texture != null && !isIgnoredTexture(equipmentShaderParams.DecalRed1Texture.name))
					{
						DCustomEquipmentDecal item2 = default(DCustomEquipmentDecal);
						item2.TextureName = equipmentShaderParams.DecalRed1Texture.name;
						item2.DefinitionId = decalDefinitionIds[item2.TextureName];
						item2.Scale = equipmentShaderParams.DecalRed1Scale;
						item2.Uoffset = equipmentShaderParams.DecalRed1UOffset;
						item2.Voffset = equipmentShaderParams.DecalRed1VOffset;
						item2.Rotation = equipmentShaderParams.DecalRed1RotationRads;
						item2.Repeat = equipmentShaderParams.DecalRed1Repeat;
						item2.Type = EquipmentDecalType.FABRIC;
						item2.Index = 0;
						list2.Add(item2);
					}
					if (equipmentShaderParams.DecalGreen2Texture != null && !isIgnoredTexture(equipmentShaderParams.DecalGreen2Texture.name))
					{
						DCustomEquipmentDecal item3 = default(DCustomEquipmentDecal);
						item3.TextureName = equipmentShaderParams.DecalGreen2Texture.name;
						item3.DefinitionId = decalDefinitionIds[item3.TextureName];
						item3.Scale = equipmentShaderParams.DecalGreen2Scale;
						item3.Uoffset = equipmentShaderParams.DecalGreen2UOffset;
						item3.Voffset = equipmentShaderParams.DecalGreen2VOffset;
						item3.Rotation = equipmentShaderParams.DecalGreen2RotationRads;
						item3.Repeat = equipmentShaderParams.DecalGreen2Repeat;
						item3.Type = EquipmentDecalType.FABRIC;
						item3.Index = 1;
						list2.Add(item3);
					}
					if (equipmentShaderParams.DecalBlue3Texture != null && !isIgnoredTexture(equipmentShaderParams.DecalBlue3Texture.name))
					{
						DCustomEquipmentDecal item4 = default(DCustomEquipmentDecal);
						item4.TextureName = equipmentShaderParams.DecalBlue3Texture.name;
						item4.DefinitionId = decalDefinitionIds[item4.TextureName];
						item4.Scale = equipmentShaderParams.DecalBlue3Scale;
						item4.Uoffset = equipmentShaderParams.DecalBlue3UOffset;
						item4.Voffset = equipmentShaderParams.DecalBlue3VOffset;
						item4.Rotation = equipmentShaderParams.DecalBlue3RotationRads;
						item4.Repeat = equipmentShaderParams.DecalBlue3Repeat;
						item4.Type = EquipmentDecalType.FABRIC;
						item4.Index = 2;
						list2.Add(item4);
					}
					if (equipmentShaderParams.DecalRed4Texture != null && !isIgnoredTexture(equipmentShaderParams.DecalRed4Texture.name))
					{
						DCustomEquipmentDecal item5 = default(DCustomEquipmentDecal);
						item5.TextureName = equipmentShaderParams.DecalRed4Texture.name;
						item5.DefinitionId = decalDefinitionIds[item5.TextureName];
						item5.Scale = equipmentShaderParams.DecalRed4Scale;
						item5.Uoffset = equipmentShaderParams.DecalRed4UOffset;
						item5.Voffset = equipmentShaderParams.DecalRed4VOffset;
						item5.Rotation = equipmentShaderParams.DecalRed4RotationRads;
						item5.Repeat = equipmentShaderParams.DecalRed4Repeat;
						item5.Type = EquipmentDecalType.DECAL;
						item5.Index = 3;
						list2.Add(item5);
					}
					if (equipmentShaderParams.DecalGreen5Texture != null && !isIgnoredTexture(equipmentShaderParams.DecalGreen5Texture.name))
					{
						DCustomEquipmentDecal item6 = default(DCustomEquipmentDecal);
						item6.TextureName = equipmentShaderParams.DecalGreen5Texture.name;
						item6.DefinitionId = decalDefinitionIds[item6.TextureName];
						item6.Scale = equipmentShaderParams.DecalGreen5Scale;
						item6.Uoffset = equipmentShaderParams.DecalGreen5UOffset;
						item6.Voffset = equipmentShaderParams.DecalGreen5VOffset;
						item6.Rotation = equipmentShaderParams.DecalGreen5RotationRads;
						item6.Repeat = equipmentShaderParams.DecalGreen5Repeat;
						item6.Type = EquipmentDecalType.DECAL;
						item6.Index = 4;
						list2.Add(item6);
					}
					if (equipmentShaderParams.DecalBlue6Texture != null && !isIgnoredTexture(equipmentShaderParams.DecalBlue6Texture.name))
					{
						DCustomEquipmentDecal item7 = default(DCustomEquipmentDecal);
						item7.TextureName = equipmentShaderParams.DecalBlue6Texture.name;
						item7.DefinitionId = decalDefinitionIds[item7.TextureName];
						item7.Scale = equipmentShaderParams.DecalBlue6Scale;
						item7.Uoffset = equipmentShaderParams.DecalBlue6UOffset;
						item7.Voffset = equipmentShaderParams.DecalBlue6VOffset;
						item7.Rotation = equipmentShaderParams.DecalBlue6RotationRads;
						item7.Repeat = equipmentShaderParams.DecalBlue6Repeat;
						item7.Type = EquipmentDecalType.DECAL;
						item7.Index = 5;
						list2.Add(item7);
					}
					item.Decals = list2.ToArray();
					list.Add(item);
				}
			}
			result.Parts = list.ToArray();
			return result;
		}

		private static bool isIgnoredTexture(string textureName)
		{
			return textureName.Contains("DefaultSwatch");
		}
	}
}
