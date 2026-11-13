using System;
using System.Collections.Generic;
using UnityEngine;

namespace MixAvatar
{
	public class AvatarMeshCombineData
	{
		private AvatarBonesData _bonesData;

		public Dictionary<string, Slot> SlotNamesToModels { get; private set; }

		public GameObject View { get; private set; }

		public List<Equipment> AppliedEquipment { get; private set; }

		public bool IsReferencingBones { get; private set; }

		public AvatarBonesData BonesData
		{
			get
			{
				if (_bonesData == null)
				{
					if (IsReferencingBones)
					{
						throw new Exception("Bones data does not exist as IsReferencingBones is true.  Data must be manually set before accessing.");
					}
					throw new Exception("Internal error: Bones data does not exist and IsReferencingBones is false.");
				}
				return _bonesData;
			}
			set
			{
				if (!IsReferencingBones)
				{
					throw new Exception("You must specify 'referenceBonesOnly' as true in Constructor if you wish to change AvatarBonesData.");
				}
				if (_bonesData != value)
				{
					_bonesData = value;
				}
			}
		}

		public AvatarMeshCombineData(GameObject view, bool referenceBonesOnly = false)
		{
			View = view;
			AppliedEquipment = new List<Equipment>();
			List<SkinnedMeshRenderer> skinnedMeshRenderers;
			Transform rootBone;
			getSkinnedMeshRenderersAndRootBone(out skinnedMeshRenderers, out rootBone);
			IsReferencingBones = referenceBonesOnly;
			_bonesData = ((!IsReferencingBones) ? new AvatarBonesData(rootBone) : null);
			setupSlots(skinnedMeshRenderers);
		}

		private void getSkinnedMeshRenderersAndRootBone(out List<SkinnedMeshRenderer> skinnedMeshRenderers, out Transform rootBone)
		{
			skinnedMeshRenderers = new List<SkinnedMeshRenderer>();
			Transform transform = View.transform;
			Transform transform2 = null;
			for (int i = 0; i < transform.childCount; i++)
			{
				GameObject gameObject = transform.GetChild(i).gameObject;
				SkinnedMeshRenderer component = gameObject.GetComponent<SkinnedMeshRenderer>();
				if (component != null)
				{
					skinnedMeshRenderers.Add(component);
				}
				else
				{
					Component[] components = gameObject.GetComponents<Component>();
					if (components.Length == 1)
					{
						if (transform2 != null)
						{
							throw new Exception("Multiple root transforms found. Please make sure there is only 1 root bone (child with a single transform component).");
						}
						transform2 = (Transform)components[0];
					}
				}
				GameObject gameObject2 = ((!(transform2 != null)) ? null : transform2.gameObject);
				if (gameObject != gameObject2 && gameObject.transform.childCount != 0)
				{
					throw new Exception("Avatar view GameObject must have exactly 1 level of children except for root bone.");
				}
			}
			if (transform2 == null || skinnedMeshRenderers.Count == 0)
			{
				throw new Exception("Avatar view GameObject must have exactly 1 root bone and 1 or more SkinnedMeshRenderer children.");
			}
			rootBone = transform2;
		}

		private void setupSlots(List<SkinnedMeshRenderer> skinnedMeshRenderers)
		{
			SlotNamesToModels = new Dictionary<string, Slot>();
			for (int i = 0; i < skinnedMeshRenderers.Count; i++)
			{
				string name = skinnedMeshRenderers[i].gameObject.name;
				Slot value = new Slot(skinnedMeshRenderers[i], name);
				SlotNamesToModels.Add(name, value);
			}
		}

		public List<Equipment> ApplyEquipment(Equipment equipment)
		{
			if (AppliedEquipment.Contains(equipment))
			{
				return new List<Equipment>();
			}
			AppliedEquipment.Add(equipment);
			equipment.ResetEjectionInfo();
			for (int i = 0; i < equipment.Parts.Length; i++)
			{
				EquipmentPart equipmentPart = equipment.Parts[i];
				Slot value;
				if (!SlotNamesToModels.TryGetValue(equipmentPart.TargetSlotName, out value))
				{
					throw new InvalidSlotTypeException(string.Format("EquipmentPart's mesh name {0} must be the same as the avatar submesh it is replacing.", equipmentPart.TargetSlotName));
				}
				value.ApplyPart(equipmentPart);
			}
			addAdditionalBonesForEquipmentParts(equipment.Parts);
			return getEquipmentWithEjectedParts();
		}

		private void addAdditionalBonesForEquipmentParts(EquipmentPart[] equipmentParts)
		{
			if (IsReferencingBones)
			{
				return;
			}
			for (int i = 0; i < equipmentParts.Length; i++)
			{
				List<Transform> list = new List<Transform>();
				EquipmentPart equipmentPart = equipmentParts[i];
				List<Transform> bones = equipmentPart.Bones;
				BindPoseTransform[] bindPoses = equipmentPart.BindPoses;
				for (int j = 0; j < bones.Count; j++)
				{
					int value = -1;
					if (!_bonesData.BoneNamesToIndices.TryGetValue(bones[j].gameObject.name, out value))
					{
						_bonesData.Bones.Add(bones[j]);
						_bonesData.BoneBindPoses.Add(bindPoses[j]);
						_bonesData.BoneNamesToIndices.Add(bones[j].gameObject.name, _bonesData.Bones.Count - 1);
						list.Add(bones[j]);
					}
				}
				if (list.Count > 0)
				{
					_bonesData.EquipmentPartToAdditionalBones.Add(equipmentPart, list);
				}
			}
		}

		private void removeAdditionalBonesForEquipmentParts(List<EquipmentPart> equipmentParts)
		{
			if (IsReferencingBones)
			{
				return;
			}
			for (int i = 0; i < equipmentParts.Count; i++)
			{
				EquipmentPart key = equipmentParts[i];
				List<Transform> value;
				if (!_bonesData.EquipmentPartToAdditionalBones.TryGetValue(key, out value))
				{
					continue;
				}
				int num = _bonesData.BoneNamesToIndices[value[0].gameObject.name];
				_bonesData.Bones.RemoveRange(num, value.Count);
				_bonesData.BoneBindPoses.RemoveRange(num, value.Count);
				for (int j = 0; j < value.Count; j++)
				{
					_bonesData.BoneNamesToIndices.Remove(value[j].gameObject.name);
				}
				foreach (KeyValuePair<string, int> boneNamesToIndex in _bonesData.BoneNamesToIndices)
				{
					if (boneNamesToIndex.Value >= num + value.Count)
					{
						_bonesData.BoneNamesToIndices[boneNamesToIndex.Key] -= value.Count;
					}
				}
				_bonesData.EquipmentPartToAdditionalBones.Remove(key);
			}
		}

		public List<Equipment> RemoveEquipment(Equipment equipment)
		{
			if (!AppliedEquipment.Contains(equipment))
			{
				throw new ArgumentException("Attempting to remove equipment that is not applied: " + equipment.gameObject.name, "equipment");
			}
			equipment.EjectAllParts();
			return getEquipmentWithEjectedParts();
		}

		private List<Equipment> getEquipmentWithEjectedParts()
		{
			List<Equipment> list = new List<Equipment>();
			for (int num = AppliedEquipment.Count - 1; num >= 0; num--)
			{
				if (AppliedEquipment[num].EjectedParts.Count > 0)
				{
					removeAdditionalBonesForEquipmentParts(AppliedEquipment[num].EjectedParts);
					list.Add(AppliedEquipment[num]);
					if (AppliedEquipment[num].AllPartsEjected)
					{
						AppliedEquipment.RemoveAt(num);
					}
				}
			}
			return list;
		}

		public List<Equipment> RemoveEquipmentByName(string equipmentGameObjectName)
		{
			Equipment equipment = AppliedEquipment.Find((Equipment x) => x.gameObject.name.Equals(equipmentGameObjectName));
			if (equipment == null)
			{
				return new List<Equipment>();
			}
			return RemoveEquipment(equipment);
		}

		public List<Equipment> RemoveAllEquipment()
		{
			List<Equipment> list = new List<Equipment>();
			while (AppliedEquipment.Count >= 1)
			{
				List<Equipment> list2 = RemoveEquipment(AppliedEquipment[AppliedEquipment.Count - 1]);
				for (int i = 0; i < list2.Count; i++)
				{
					if (list2[i].AllPartsEjected)
					{
						list.Add(list2[i]);
					}
				}
			}
			return list;
		}
	}
}
