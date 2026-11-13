using System.Collections.Generic;
using UnityEngine;

namespace MixAvatar
{
	public class Slot
	{
		public readonly string SlotName;

		private List<EquipmentPart> parts;

		private EquipmentPart secondaryAdditionPart;

		public SkinnedMeshRenderer DefaultSkinnedMeshRenderer { get; private set; }

		public List<SkinnedMeshRenderer> VisibleSkinnedMeshRenderers { get; private set; }

		public Slot(SkinnedMeshRenderer defaultSkinnedMeshRenderer, string slotName)
		{
			SlotName = slotName;
			DefaultSkinnedMeshRenderer = defaultSkinnedMeshRenderer;
			VisibleSkinnedMeshRenderers = new List<SkinnedMeshRenderer>();
			VisibleSkinnedMeshRenderers.Add(defaultSkinnedMeshRenderer);
			parts = new List<EquipmentPart>();
		}

		public void ApplyPart(EquipmentPart equipmentPart)
		{
			if (!parts.Contains(equipmentPart))
			{
				switch (equipmentPart.PartType)
				{
				case EquipmentPartType.BaseMeshReplacement:
					setBaseMeshReplacement(equipmentPart);
					break;
				case EquipmentPartType.BaseMeshAddition:
					setBaseMeshAddition(equipmentPart);
					break;
				case EquipmentPartType.SecondaryMeshAddition:
					addSecondaryMeshPart(equipmentPart);
					break;
				}
			}
		}

		private void setBaseMeshReplacement(EquipmentPart baseMeshReplacementPart)
		{
			EjectAllParts();
			VisibleSkinnedMeshRenderers.Remove(DefaultSkinnedMeshRenderer);
			VisibleSkinnedMeshRenderers.Add(baseMeshReplacementPart.SkinnedMeshRenderer);
			parts.Add(baseMeshReplacementPart);
			baseMeshReplacementPart.OccupiedSlot = this;
		}

		private void setBaseMeshAddition(EquipmentPart baseMeshAdditionPart)
		{
			EjectAllParts();
			VisibleSkinnedMeshRenderers.Add(DefaultSkinnedMeshRenderer);
			VisibleSkinnedMeshRenderers.Add(baseMeshAdditionPart.SkinnedMeshRenderer);
			parts.Add(baseMeshAdditionPart);
			baseMeshAdditionPart.OccupiedSlot = this;
		}

		private void addSecondaryMeshPart(EquipmentPart secondaryMeshAdditionPart)
		{
			if (!parts.Contains(secondaryMeshAdditionPart))
			{
				if (secondaryAdditionPart != null)
				{
					RemovePart(secondaryAdditionPart);
					secondaryAdditionPart.Eject();
					secondaryAdditionPart = null;
				}
				VisibleSkinnedMeshRenderers.Add(secondaryMeshAdditionPart.SkinnedMeshRenderer);
				parts.Add(secondaryMeshAdditionPart);
				secondaryAdditionPart = secondaryMeshAdditionPart;
				secondaryMeshAdditionPart.OccupiedSlot = this;
			}
		}

		public void RemovePart(EquipmentPart equipmentPart)
		{
			if (parts.Contains(equipmentPart))
			{
				switch (equipmentPart.PartType)
				{
				case EquipmentPartType.BaseMeshReplacement:
					EjectAllParts();
					break;
				case EquipmentPartType.BaseMeshAddition:
					EjectAllParts();
					break;
				case EquipmentPartType.SecondaryMeshAddition:
					ejectSecondaryAdditionPart();
					break;
				}
			}
		}

		public void EjectAllParts()
		{
			List<EquipmentPart> list = resetPartsAndMeshRenderers();
			if (list.Count > 0)
			{
				for (int num = list.Count - 1; num >= 0; num--)
				{
					list[num].Eject();
				}
			}
			VisibleSkinnedMeshRenderers.Add(DefaultSkinnedMeshRenderer);
		}

		private void ejectSecondaryAdditionPart()
		{
			if (secondaryAdditionPart != null)
			{
				secondaryAdditionPart.OccupiedSlot = null;
				parts.Remove(secondaryAdditionPart);
				VisibleSkinnedMeshRenderers.Remove(secondaryAdditionPart.SkinnedMeshRenderer);
				secondaryAdditionPart.Eject();
			}
		}

		private List<EquipmentPart> resetPartsAndMeshRenderers()
		{
			List<EquipmentPart> list = parts;
			for (int i = 0; i < list.Count; i++)
			{
				if (parts[i].PartType != EquipmentPartType.SecondaryMeshAddition)
				{
					parts[i].OccupiedSlot = null;
				}
			}
			VisibleSkinnedMeshRenderers.Clear();
			parts = new List<EquipmentPart>();
			if (secondaryAdditionPart != null)
			{
				parts.Add(secondaryAdditionPart);
				VisibleSkinnedMeshRenderers.Add(secondaryAdditionPart.SkinnedMeshRenderer);
				list.Remove(secondaryAdditionPart);
			}
			return list;
		}
	}
}
