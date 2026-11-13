using System;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace ClubPenguin.Avatar
{
	[Serializable]
	public struct DCustomOutfit
	{
		public int LodLevel;

		public DCustomEquipment[] Equipment;

		public override string ToString()
		{
			return string.Format("[DCustomOutfit] LodLevel: {0} #Equipment: {1} Full hash: 0x{2:x8} Resource hash: 0x{3:x8}", LodLevel, Equipment.Length, GetFullHash(), GetResourceHash());
		}

		public int GetResourceHash()
		{
			StructHash sh = default(StructHash);
			sh.Combine(LodLevel);
			for (int i = 0; i < Equipment.Length; i++)
			{
				sh.Combine(Equipment[i].GetResourceHash());
			}
			return sh;
		}

		public int GetFullHash()
		{
			StructHash sh = default(StructHash);
			sh.Combine(LodLevel);
			for (int i = 0; i < Equipment.Length; i++)
			{
				sh.Combine(Equipment[i].GetFullHash());
			}
			return sh;
		}

		public bool Validate()
		{
			bool flag = true;
			flag &= (Equipment != null);
			int num = 0;
			while (flag && num < Equipment.Length)
			{
				flag &= Equipment[num].Validate();
				num++;
			}
			return flag;
		}

		[Conditional("DUMP_OUTFIT")]
		public void Dump()
		{
			StringBuilder stringBuilder = new StringBuilder("Outfit: ");
			stringBuilder.AppendLine(ToString());
			for (int i = 0; i < Equipment.Length; i++)
			{
				DCustomEquipment dCustomEquipment = Equipment[i];
				stringBuilder.Append("- ");
				stringBuilder.Append(dCustomEquipment.Name);
				stringBuilder.Append(", DefId:");
				stringBuilder.Append(dCustomEquipment.DefinitionId);
				stringBuilder.Append(", Id:");
				stringBuilder.Append(dCustomEquipment.Id);
				stringBuilder.AppendLine();
				for (int j = 0; j < dCustomEquipment.Parts.Length; j++)
				{
					DCustomEquipmentPart dCustomEquipmentPart = dCustomEquipment.Parts[j];
					stringBuilder.Append("\t- SlotIndex: ");
					stringBuilder.AppendLine(dCustomEquipmentPart.SlotIndex.ToString());
					for (int k = 0; k < dCustomEquipmentPart.Decals.Length; k++)
					{
						DCustomEquipmentDecal dCustomEquipmentDecal = dCustomEquipmentPart.Decals[k];
						stringBuilder.Append("\t\t- Type: ");
						stringBuilder.Append(dCustomEquipmentDecal.Type);
						stringBuilder.Append(", TexName: ");
						stringBuilder.Append(dCustomEquipmentDecal.TextureName);
						stringBuilder.Append(", DefId: ");
						stringBuilder.Append(dCustomEquipmentDecal.DefinitionId);
						stringBuilder.Append(", Index: ");
						stringBuilder.Append(dCustomEquipmentDecal.Index);
						stringBuilder.Append(", Scale: ");
						stringBuilder.Append(dCustomEquipmentDecal.Scale);
						stringBuilder.Append(", uOffset: ");
						stringBuilder.Append(dCustomEquipmentDecal.Uoffset);
						stringBuilder.Append(", vOffset: ");
						stringBuilder.Append(dCustomEquipmentDecal.Voffset);
						stringBuilder.Append(", Rotation: ");
						stringBuilder.Append(dCustomEquipmentDecal.Rotation);
						stringBuilder.Append(", Repeat: ");
						stringBuilder.Append(dCustomEquipmentDecal.Repeat);
						stringBuilder.AppendLine();
					}
				}
			}
			UnityEngine.Debug.Log(stringBuilder.ToString());
		}
	}
}
