using ClubPenguin.Avatar;
using ClubPenguin.Net.Domain;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;

namespace ClubPenguin
{
	public class CustomEquipmentResponseAdaptor
	{
		public static DCustomEquipment[] ConvertResponseToOutfit(List<CustomEquipment> outfit)
		{
			if (outfit == null)
			{
				return new DCustomEquipment[0];
			}
			return ConvertResponseToOutfit(outfit.ToArray());
		}

		public static DCustomEquipment[] ConvertResponseToOutfit(CustomEquipment[] outfit)
		{
			if (outfit == null)
			{
				return new DCustomEquipment[0];
			}
			DCustomEquipment[] array = new DCustomEquipment[outfit.Length];
			int num = 0;
			for (int i = 0; i < outfit.Length; i++)
			{
				try
				{
					array[num++] = ConvertResponseToCustomEquipment(outfit[i]);
				}
				catch (KeyNotFoundException)
				{
				}
			}
			if (num != array.Length)
			{
				DCustomEquipment[] array2 = new DCustomEquipment[num];
				Array.Copy(array, array2, num);
				return array2;
			}
			return array;
		}

		public static DCustomEquipment ConvertResponseToCustomEquipment(CustomEquipment data)
		{
			Dictionary<int, TemplateDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>();
			DCustomEquipment result = default(DCustomEquipment);
			result.Id = data.equipmentId;
			result.DefinitionId = data.definitionId;
			result.DateTimeCreated = data.dateTimeCreated;
			if (!dictionary.ContainsKey(data.definitionId))
			{
				Log.LogErrorFormatted(typeof(CustomEquipmentResponseAdaptor), "No known equipment template definition for id {0}", data.definitionId);
				throw new KeyNotFoundException();
			}
			result.Name = dictionary[data.definitionId].AssetName;
			if (data.parts != null)
			{
				DCustomEquipmentPart[] array = new DCustomEquipmentPart[data.parts.Length];
				for (int i = 0; i < data.parts.Length; i++)
				{
					DCustomEquipmentPart dCustomEquipmentPart = default(DCustomEquipmentPart);
					dCustomEquipmentPart.SlotIndex = data.parts[i].slotIndex;
					if (data.parts[i].customizations != null)
					{
						Dictionary<int, FabricDefinition> dictionary2 = Service.Get<GameData>().Get<Dictionary<int, FabricDefinition>>();
						Dictionary<int, DecalDefinition> dictionary3 = Service.Get<GameData>().Get<Dictionary<int, DecalDefinition>>();
						DCustomEquipmentDecal[] array2 = new DCustomEquipmentDecal[data.parts[i].customizations.Length];
						for (int j = 0; j < array2.Length; j++)
						{
							CustomEquipmentCustomization customEquipmentCustomization = data.parts[i].customizations[j];
							DCustomEquipmentDecal dCustomEquipmentDecal = default(DCustomEquipmentDecal);
							dCustomEquipmentDecal.DefinitionId = customEquipmentCustomization.definitionId;
							dCustomEquipmentDecal.Index = customEquipmentCustomization.index;
							dCustomEquipmentDecal.Scale = customEquipmentCustomization.scale;
							dCustomEquipmentDecal.Rotation = customEquipmentCustomization.rotation;
							dCustomEquipmentDecal.Uoffset = customEquipmentCustomization.uoffset;
							dCustomEquipmentDecal.Voffset = customEquipmentCustomization.voffset;
							switch (customEquipmentCustomization.type)
							{
							case EquipmentCustomizationType.DECAL:
								if (!dictionary3.ContainsKey(customEquipmentCustomization.definitionId))
								{
									Log.LogErrorFormatted(typeof(CustomEquipmentResponseAdaptor), "No known decal template definition for id {0}", customEquipmentCustomization.definitionId);
									continue;
								}
								dCustomEquipmentDecal.TextureName = dictionary3[customEquipmentCustomization.definitionId].AssetName;
								dCustomEquipmentDecal.Repeat = customEquipmentCustomization.repeat;
								dCustomEquipmentDecal.Type = EquipmentDecalType.DECAL;
								break;
							case EquipmentCustomizationType.FABRIC:
								if (!dictionary2.ContainsKey(customEquipmentCustomization.definitionId))
								{
									Log.LogErrorFormatted(typeof(CustomEquipmentResponseAdaptor), "No known fabric template definition for id {0}", customEquipmentCustomization.definitionId);
									continue;
								}
								dCustomEquipmentDecal.TextureName = dictionary2[customEquipmentCustomization.definitionId].AssetName;
								dCustomEquipmentDecal.Type = EquipmentDecalType.FABRIC;
								dCustomEquipmentDecal.Repeat = true;
								break;
							}
							array2[j] = dCustomEquipmentDecal;
						}
						dCustomEquipmentPart.Decals = array2;
					}
					else
					{
						dCustomEquipmentPart.Decals = new DCustomEquipmentDecal[0];
					}
					array[i] = dCustomEquipmentPart;
				}
				result.Parts = array;
			}
			else
			{
				result.Parts = new DCustomEquipmentPart[0];
			}
			return result;
		}

		public static CustomEquipment ConvertCustomEquipmentToRequest(DCustomEquipment equipment)
		{
			CustomEquipment result = default(CustomEquipment);
			Dictionary<int, TemplateDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>();
			bool flag = false;
			foreach (TemplateDefinition value in dictionary.Values)
			{
				if (value.AssetName.Equals(equipment.Name))
				{
					result.definitionId = value.Id;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				Log.LogFatalFormatted(typeof(CustomEquipmentResponseAdaptor), "No known equipment template definition for asset named {0}", equipment.Name);
			}
			CustomEquipmentPart[] array = new CustomEquipmentPart[equipment.Parts.Length];
			for (int i = 0; i < equipment.Parts.Length; i++)
			{
				CustomEquipmentPart customEquipmentPart = default(CustomEquipmentPart);
				customEquipmentPart.slotIndex = equipment.Parts[i].SlotIndex;
				if (equipment.Parts[i].Decals != null)
				{
					Dictionary<int, FabricDefinition> dictionary2 = Service.Get<GameData>().Get<Dictionary<int, FabricDefinition>>();
					Dictionary<int, DecalDefinition> dictionary3 = Service.Get<GameData>().Get<Dictionary<int, DecalDefinition>>();
					CustomEquipmentCustomization[] array2 = new CustomEquipmentCustomization[equipment.Parts[i].Decals.Length];
					for (int j = 0; j < equipment.Parts[i].Decals.Length; j++)
					{
						DCustomEquipmentDecal dCustomEquipmentDecal = equipment.Parts[i].Decals[j];
						CustomEquipmentCustomization customEquipmentCustomization = default(CustomEquipmentCustomization);
						customEquipmentCustomization.index = dCustomEquipmentDecal.Index;
						customEquipmentCustomization.scale = dCustomEquipmentDecal.Scale;
						customEquipmentCustomization.rotation = dCustomEquipmentDecal.Rotation;
						customEquipmentCustomization.uoffset = dCustomEquipmentDecal.Uoffset;
						customEquipmentCustomization.voffset = dCustomEquipmentDecal.Voffset;
						flag = false;
						if (dCustomEquipmentDecal.Type == EquipmentDecalType.FABRIC)
						{
							foreach (FabricDefinition value2 in dictionary2.Values)
							{
								if (value2.AssetName.Equals(dCustomEquipmentDecal.TextureName))
								{
									customEquipmentCustomization.type = EquipmentCustomizationType.FABRIC;
									customEquipmentCustomization.definitionId = value2.Id;
									flag = true;
									break;
								}
							}
						}
						else if (dCustomEquipmentDecal.Type == EquipmentDecalType.DECAL)
						{
							foreach (DecalDefinition value3 in dictionary3.Values)
							{
								if (value3.AssetName.Equals(dCustomEquipmentDecal.TextureName))
								{
									customEquipmentCustomization.type = EquipmentCustomizationType.DECAL;
									customEquipmentCustomization.definitionId = value3.Id;
									customEquipmentCustomization.repeat = dCustomEquipmentDecal.Repeat;
									flag = true;
									break;
								}
							}
						}
						if (!flag)
						{
							Log.LogFatalFormatted(typeof(CustomEquipmentResponseAdaptor), "No known customization definition for texture {0}", dCustomEquipmentDecal.TextureName);
						}
						array2[j] = customEquipmentCustomization;
					}
					customEquipmentPart.customizations = array2;
				}
				array[i] = customEquipmentPart;
			}
			result.parts = array;
			return result;
		}
	}
}
