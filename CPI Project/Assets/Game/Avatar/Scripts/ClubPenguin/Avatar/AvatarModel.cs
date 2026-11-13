using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ClubPenguin.Avatar
{
	[DisallowMultipleComponent]
	public class AvatarModel : MonoBehaviour
	{
		public class Part
		{
			public readonly EquipmentModelDefinition Equipment;

			public readonly int Index;

			public readonly DCustomEquipmentDecal[] Decals;

			public Part(EquipmentModelDefinition equipment, int index, DCustomEquipmentDecal[] decals)
			{
				Equipment = equipment;
				Index = index;
				Decals = decals;
			}
		}

		public class ApplyResult
		{
			public DCustomEquipment CustomEquipmentApplied;

			public EquipmentModelDefinition EquipmentDefinitionApplied;

			public HashSet<Part> PartsAdded;

			public HashSet<Part> PartsEjected;

			public HashSet<EquipmentModelDefinition> EquipmentEjected;
		}

		public int LodLevel;

		public string AvatarDefinitionName;

		private AvatarDefinition definition;

		private Color beakColor;

		private Color bellyColor;

		private Color bodyColor;

		private Part[,] parts;

		private readonly HashSet<EquipmentModelDefinition> equipmentList = new HashSet<EquipmentModelDefinition>();

		public readonly int ColumnMax = Enum.GetValues(typeof(EquipmentPartType)).Length;

		public AvatarDefinition Definition
		{
			get
			{
				if (definition == null)
				{
					definition = Service.Get<AvatarService>().GetDefinitionByName(AvatarDefinitionName);
				}
				return definition;
			}
		}

		public Color BeakColor
		{
			get
			{
				return beakColor;
			}
			set
			{
				if (beakColor != value)
				{
					beakColor = value;
					if (this.ColorChanged != null)
					{
						this.ColorChanged();
					}
				}
			}
		}

		public Color BellyColor
		{
			get
			{
				return bellyColor;
			}
			set
			{
				if (bellyColor != value)
				{
					bellyColor = value;
					if (this.ColorChanged != null)
					{
						this.ColorChanged();
					}
				}
			}
		}

		public Color BodyColor
		{
			get
			{
				return bodyColor;
			}
			set
			{
				if (bodyColor != value)
				{
					bodyColor = value;
					if (this.ColorChanged != null)
					{
						this.ColorChanged();
					}
				}
			}
		}

		public int RowMax
		{
			get
			{
				return Definition.Slots.Length;
			}
		}

		public Part this[int slotIndex, int partIndex]
		{
			get
			{
				return parts[slotIndex, partIndex];
			}
		}

		public IEnumerable<EquipmentModelDefinition> Equipment
		{
			get
			{
				return equipmentList;
			}
		}

		public event Action<int, int, Part, Part> PartChanged;

		public event Action<IEnumerable<ApplyResult>> OutfitSet;

		public event System.Action ColorChanged;

		public event Action<EquipmentModelDefinition> EquipmentEjected;

		public Part GetPart(int slotIndex, EquipmentPartType partType)
		{
			return parts[slotIndex, (int)partType];
		}

		private Part setPart(int slotIndex, EquipmentPartType partType, Part newPart)
		{
			Part part = GetPart(slotIndex, partType);
			parts[slotIndex, (int)partType] = newPart;
			return part;
		}

		public bool TryGetEquippedPart(int slotIndex, EquipmentPartType partType, out Part outPart)
		{
			outPart = GetPart(slotIndex, partType);
			return outPart != null && outPart.Equipment != null;
		}

		public EquipmentModelDefinition.Part GetPartDefinition(Part part)
		{
			return part.Equipment.Parts[part.Index];
		}

		public bool IsRequiredPart(Part part)
		{
			return GetPartDefinition(part).Required;
		}

		public IEnumerable<Part> IterateParts()
		{
			for (int i = 0; i < RowMax; i++)
			{
				for (int j = 0; j < ColumnMax; j++)
				{
					Part part = parts[i, j];
					if (part != null)
					{
						yield return part;
					}
				}
			}
		}

		public IEnumerable<Part> GetConnectedParts(Part modelPart)
		{
			EquipmentModelDefinition equipment = modelPart.Equipment;
			EquipmentModelDefinition.Part[] source = equipment.Parts;
			return from p in source
				select GetPart(p.SlotIndex, p.PartType) into p
				where p.Equipment != null && p != modelPart
				select p;
		}

		public void Awake()
		{
			parts = new Part[RowMax, ColumnMax];
			for (int i = 0; i < RowMax; i++)
			{
				parts[i, 0] = new Part(null, i, null);
			}
			bodyColor = Definition.BodyColor.BodyColor;
			beakColor = Definition.BodyColor.BeakColor;
			bellyColor = Definition.BodyColor.BellyColor;
		}

		public IEnumerable<ApplyResult> ApplyOutfit(DCustomOutfit outfit)
		{
			List<ApplyResult> list = new List<ApplyResult>();
			DCustomEquipment[] equipment = outfit.Equipment;
			foreach (DCustomEquipment customEqToApply in equipment)
			{
				list.Add(ApplyEquipment(customEqToApply));
			}
			if (this.OutfitSet != null)
			{
				this.OutfitSet(list);
			}
			return list;
		}

		public ApplyResult ApplyEquipment(DCustomEquipment customEqToApply)
		{
			EquipmentModelDefinition equipmentDefinition = Definition.GetEquipmentDefinition(customEqToApply);
			ApplyResult applyResult = new ApplyResult();
			applyResult.CustomEquipmentApplied = customEqToApply;
			applyResult.EquipmentDefinitionApplied = equipmentDefinition;
			applyResult.PartsAdded = new HashSet<Part>();
			if (equipmentDefinition != null)
			{
				ejectReplacedEquipment(equipmentDefinition, applyResult);
				applyParts(equipmentDefinition, customEqToApply, applyResult.PartsAdded);
				ejectRemainingParts(applyResult.PartsEjected);
				equipmentList.Add(equipmentDefinition);
			}
			return applyResult;
		}

		private void ejectReplacedEquipment(EquipmentModelDefinition eqDefToApply, ApplyResult result)
		{
			HashSet<Part> ejectedParts = null;
			HashSet<EquipmentModelDefinition> ejectedEquipment = null;
			if (GetEquipmentToEject(eqDefToApply, out ejectedParts, out ejectedEquipment))
			{
				foreach (EquipmentModelDefinition item in ejectedEquipment)
				{
					equipmentList.Remove(item);
					if (this.EquipmentEjected != null)
					{
						try
						{
							this.EquipmentEjected(item);
						}
						catch (Exception ex)
						{
							Log.LogException(this, ex);
						}
					}
				}
			}
			result.PartsEjected = ejectedParts;
			result.EquipmentEjected = ejectedEquipment;
		}

		private void applyParts(EquipmentModelDefinition eqDefToApply, DCustomEquipment customEqToApply, HashSet<Part> partsAdded)
		{
			for (int i = 0; i < eqDefToApply.Parts.Length; i++)
			{
				DCustomEquipmentDecal[] decals = null;
				EquipmentModelDefinition.Part newPartDef = eqDefToApply.Parts[i];
				if (customEqToApply.Parts != null)
				{
					for (int j = 0; j < customEqToApply.Parts.Length; j++)
					{
						if (customEqToApply.Parts[j].SlotIndex == newPartDef.SlotIndex)
						{
							decals = customEqToApply.Parts[j].Decals;
							break;
						}
					}
				}
				Part part = new Part(eqDefToApply, i, decals);
				changePart(newPartDef, part);
				partsAdded.Add(part);
			}
		}

		private void ejectRemainingParts(HashSet<Part> partsToEject)
		{
			foreach (Part item in partsToEject)
			{
				EquipmentModelDefinition.Part partDefinition = GetPartDefinition(item);
				Part part = GetPart(partDefinition.SlotIndex, partDefinition.PartType);
				if (part == item)
				{
					changePart(partDefinition, null);
				}
			}
		}

		public bool GetEquipmentToEject(EquipmentModelDefinition eqToApply, out HashSet<Part> ejectedParts, out HashSet<EquipmentModelDefinition> ejectedEquipment)
		{
			ejectedParts = new HashSet<Part>();
			ejectedEquipment = new HashSet<EquipmentModelDefinition>();
			EquipmentModelDefinition.Part[] array = eqToApply.Parts;
			for (int i = 0; i < array.Length; i++)
			{
				EquipmentModelDefinition.Part part = array[i];
				Part outPart = null;
				bool flag = TryGetEquippedPart(part.SlotIndex, part.PartType, out outPart);
				if (!flag)
				{
					flag = (part.PartType == EquipmentPartType.BaseMeshAddition && TryGetEquippedPart(part.SlotIndex, EquipmentPartType.BaseMeshReplacement, out outPart));
				}
				if (!flag)
				{
					flag = (part.PartType == EquipmentPartType.BaseMeshReplacement && TryGetEquippedPart(part.SlotIndex, EquipmentPartType.BaseMeshAddition, out outPart));
				}
				if (flag)
				{
					ejectPart(ejectedParts, ejectedEquipment, outPart);
				}
			}
			return ejectedParts.Count > 0;
		}

		private void ejectPart(HashSet<Part> ejectedParts, HashSet<EquipmentModelDefinition> ejectedEquipment, Part modelPart)
		{
			ejectedParts.Add(modelPart);
			IEnumerable<Part> connectedParts = GetConnectedParts(modelPart);
			if (IsRequiredPart(modelPart))
			{
				foreach (Part item in connectedParts)
				{
					ejectedParts.Add(item);
				}
			}
			if (areAllPartsRemoved(connectedParts, ejectedParts))
			{
				ejectedEquipment.Add(modelPart.Equipment);
			}
		}

		private static bool areAllPartsRemoved(IEnumerable<Part> connectedModelParts, HashSet<Part> ejectedParts)
		{
			bool result = true;
			foreach (Part connectedModelPart in connectedModelParts)
			{
				if (!ejectedParts.Contains(connectedModelPart))
				{
					result = false;
					break;
				}
			}
			return result;
		}

		public bool RemoveEquipment(string equipmentName)
		{
			bool result = false;
			EquipmentModelDefinition equipmentDefinition = Definition.GetEquipmentDefinition(equipmentName);
			if (equipmentDefinition != null)
			{
				if (equipmentList.Contains(equipmentDefinition))
				{
					removeEquipment(equipmentDefinition);
					result = true;
				}
			}
			else
			{
				Log.LogErrorFormatted(this, "Trying to remove unknown equipment '{0}' from avatar model {1}", equipmentName, base.name);
			}
			return result;
		}

		public void ClearAllEquipment()
		{
			equipmentList.Clear();
			for (int i = 0; i < RowMax; i++)
			{
				changePart(i, 0, new Part(null, i, null));
				for (int j = 1; j < ColumnMax; j++)
				{
					changePart(i, j, null);
				}
			}
		}

		private void removeEquipment(EquipmentModelDefinition eq)
		{
			equipmentList.Remove(eq);
			for (int i = 0; i < eq.Parts.Length; i++)
			{
				int slotIndex = eq.Parts[i].SlotIndex;
				int partType = (int)eq.Parts[i].PartType;
				Part part = parts[slotIndex, partType];
				if (part != null && part.Equipment == eq)
				{
					changePart(slotIndex, partType, null);
				}
			}
		}

		private void changePart(EquipmentModelDefinition.Part newPartDef, Part newPart)
		{
			int slotIndex = newPartDef.SlotIndex;
			int partType = (int)newPartDef.PartType;
			changePart(slotIndex, partType, newPart);
		}

		private void changePart(int slotIndex, int partIndex, Part newPart)
		{
			Part arg = setPart(slotIndex, (EquipmentPartType)partIndex, newPart ?? ((partIndex == 0) ? new Part(null, slotIndex, null) : null));
			if (this.PartChanged != null)
			{
				try
				{
					this.PartChanged(slotIndex, partIndex, arg, GetPart(slotIndex, (EquipmentPartType)partIndex));
				}
				catch (Exception ex)
				{
					Log.LogException(this, ex);
					throw;
				}
			}
		}
	}
}
