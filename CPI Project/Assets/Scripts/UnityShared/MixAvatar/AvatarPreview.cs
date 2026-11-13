using System;
using System.Collections.Generic;
using Disney.LaunchPadFramework;
using UnityEngine;

namespace MixAvatar
{
	public class AvatarPreview : MonoBehaviour
	{
		[HideInInspector]
		public bool AvatarPreviewReady = false;

		private Transform equipmentContainerTransform;

		public AvatarMeshCombineData MeshCombineData { get; private set; }

		private void Start()
		{
			MeshCombineData = new AvatarMeshCombineData(base.gameObject);
			GameObject gameObject = new GameObject();
			gameObject.name = "EquipmentInstances";
			equipmentContainerTransform = gameObject.transform;
			equipmentContainerTransform.SetParent(base.transform, false);
			AvatarPreviewReady = true;
			refreshPenguinShaders();
		}

		private void refreshPenguinShaders()
		{
			SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].material.shader = componentsInChildren[i].material.shader;
			}
		}

		private void revertEjectedPartBonesAndUpdateVisibility(List<Equipment> equipmentWithEjectedParts)
		{
			for (int i = 0; i < equipmentWithEjectedParts.Count; i++)
			{
				AvatarUtils.RevertEquipmentPartBonesToOriginals(equipmentWithEjectedParts[i].EjectedParts);
				if (equipmentWithEjectedParts[i].AllPartsEjected)
				{
					equipmentWithEjectedParts[i].transform.SetParent(null);
					continue;
				}
				for (int j = 0; j < equipmentWithEjectedParts[i].EjectedParts.Count; j++)
				{
					equipmentWithEjectedParts[i].EjectedParts[j].gameObject.SetActive(false);
				}
			}
			AvatarUtils.UpdateSlotVisibilities(MeshCombineData);
		}

		public List<Equipment> ApplyEquipment(Equipment equipment)
		{
			if (MeshCombineData.AppliedEquipment.Contains(equipment))
			{
				return new List<Equipment>();
			}
			for (int i = 0; i < equipment.Parts.Length; i++)
			{
				equipment.Parts[i].gameObject.SetActive(true);
			}
			List<Equipment> list = null;
			try
			{
				list = MeshCombineData.ApplyEquipment(equipment);
				equipment.transform.SetParent(equipmentContainerTransform, false);
				AvatarUtils.ReplaceAndMergeEquipmentBonesWithAvatarBones(equipment, MeshCombineData);
				revertEjectedPartBonesAndUpdateVisibility(list);
			}
			catch (InvalidSlotTypeException ex)
			{
				Log.LogException(this, ex);
			}
			catch (IndexOutOfRangeException ex2)
			{
				Log.LogException(this, ex2);
			}
			return list;
		}

		public List<Equipment> RemoveEquipment(Equipment equipment)
		{
			List<Equipment> list = MeshCombineData.RemoveEquipment(equipment);
			revertEjectedPartBonesAndUpdateVisibility(list);
			return list;
		}

		public List<Equipment> RemoveEquipmentByName(string equipmentGameObjectName)
		{
			List<Equipment> list = MeshCombineData.RemoveEquipmentByName(equipmentGameObjectName);
			revertEjectedPartBonesAndUpdateVisibility(list);
			return list;
		}

		public List<Equipment> RemoveAllEquipment()
		{
			List<Equipment> list = MeshCombineData.RemoveAllEquipment();
			revertEjectedPartBonesAndUpdateVisibility(list);
			return list;
		}
	}
}
