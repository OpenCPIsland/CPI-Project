using System;
using System.Collections.Generic;
using UnityEngine;

namespace MixAvatar
{
	public class AvatarUtils
	{
		public static SkinnedMeshRenderer CombineMeshToTargetGameObject(AvatarMeshCombineData avatarMeshCombineData, GameObject targetMeshGameObject, GameObject targetBonesGameObject, out int[] numVerticesPerSubMesh, out List<SkinnedMeshRenderer> combinedSubMeshes, out Transform rootBoneCopy)
		{
			combinedSubMeshes = new List<SkinnedMeshRenderer>();
			foreach (KeyValuePair<string, Slot> slotNamesToModel in avatarMeshCombineData.SlotNamesToModels)
			{
				Slot value = slotNamesToModel.Value;
				for (int i = 0; i < value.VisibleSkinnedMeshRenderers.Count; i++)
				{
					combinedSubMeshes.Add(value.VisibleSkinnedMeshRenderers[i]);
				}
			}
			List<Transform> list;
			if (avatarMeshCombineData.IsReferencingBones)
			{
				list = avatarMeshCombineData.BonesData.Bones;
				rootBoneCopy = avatarMeshCombineData.BonesData.RootBone;
			}
			else
			{
				list = cloneAvatarBones(targetBonesGameObject, avatarMeshCombineData.BonesData.Bones, avatarMeshCombineData.BonesData.RootBone, avatarMeshCombineData.BonesData.BoneNamesToIndices);
				rootBoneCopy = list[avatarMeshCombineData.BonesData.BoneNamesToIndices[avatarMeshCombineData.BonesData.RootBone.gameObject.name]];
				rootBoneCopy.SetParent(targetBonesGameObject.transform, false);
				cloneAnimators(avatarMeshCombineData.BonesData.Bones, list);
			}
			MeshUtils.ResetBonesToBindPoses(list, avatarMeshCombineData.BonesData.BoneBindPoses);
			return MeshUtils.MergeSkinnedMeshes(combinedSubMeshes, list, avatarMeshCombineData.BonesData.BoneNamesToIndices, targetMeshGameObject, avatarMeshCombineData.View.transform.worldToLocalMatrix, out numVerticesPerSubMesh);
		}

		private static List<Transform> cloneAvatarBones(GameObject container, List<Transform> allBones, Transform rootBone, Dictionary<string, int> BoneNamesToIndices)
		{
			List<Transform> list = new List<Transform>();
			for (int i = 0; i < allBones.Count; i++)
			{
				Transform transform = findChild(container.transform, allBones[i].gameObject.name);
				if (transform != null)
				{
					list.Add(transform);
					continue;
				}
				GameObject gameObject = new GameObject();
				gameObject.name = allBones[i].gameObject.name;
				gameObject.transform.localPosition = allBones[i].localPosition;
				gameObject.transform.localRotation = allBones[i].localRotation;
				gameObject.transform.localScale = allBones[i].localScale;
				list.Add(gameObject.transform);
			}
			for (int j = 0; j < allBones.Count; j++)
			{
				Transform parent = allBones[j].parent;
				int value = -1;
				if (BoneNamesToIndices.TryGetValue(parent.gameObject.name, out value))
				{
					list[j].SetParent(list[value], false);
				}
			}
			return list;
		}

		private static Transform findChild(Transform transform, string name)
		{
			int childCount = transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (child.name == name)
				{
					return child;
				}
				Transform transform2 = findChild(child, name);
				if (transform2 != null)
				{
					return transform2;
				}
			}
			return null;
		}

		private static void cloneAnimators(List<Transform> sourceBones, List<Transform> destinationBones)
		{
			for (int i = 0; i < sourceBones.Count; i++)
			{
				Animator component = sourceBones[i].GetComponent<Animator>();
				if (component != null)
				{
					Animator animator = destinationBones[i].gameObject.AddComponent<Animator>();
					animator.runtimeAnimatorController = component.runtimeAnimatorController;
				}
			}
		}

		public static void ReplaceAndMergeEquipmentBonesWithAvatarBones(Equipment equipment, AvatarMeshCombineData avatarMeshCombineData)
		{
			if (!avatarMeshCombineData.AppliedEquipment.Contains(equipment))
			{
				throw new Exception("Equipment must be applied to AvatarMeshCombineData before having its bones replaced.");
			}
			List<Transform> bones = avatarMeshCombineData.BonesData.Bones;
			Dictionary<string, int> boneNamesToIndices = avatarMeshCombineData.BonesData.BoneNamesToIndices;
			for (int i = 0; i < equipment.Parts.Length; i++)
			{
				EquipmentPart equipmentPart = equipment.Parts[i];
				Transform[] bones2 = equipmentPart.SkinnedMeshRenderer.bones;
				Transform[] array = new Transform[bones2.Length];
				for (int j = 0; j < bones2.Length; j++)
				{
					string name = bones2[j].gameObject.name;
					int index = boneNamesToIndices[name];
					array[j] = bones[index];
				}
				equipmentPart.SkinnedMeshRenderer.bones = array;
			}
			for (int k = 0; k < equipment.Parts.Length; k++)
			{
				EquipmentPart key = equipment.Parts[k];
				List<Transform> value;
				if (!avatarMeshCombineData.BonesData.EquipmentPartToAdditionalBones.TryGetValue(key, out value))
				{
					continue;
				}
				for (int l = 0; l < value.Count; l++)
				{
					Transform transform = value[l];
					if (!(transform.parent == null))
					{
						string name2 = transform.parent.gameObject.name;
						if (!boneNamesToIndices.ContainsKey(name2))
						{
							throw new Exception("Failed to reparent additional bone named: " + transform.gameObject.name + ". Existing avatar bones don't contain its parent bone named: " + name2);
						}
						Transform transform2 = bones[boneNamesToIndices[name2]];
						if (transform.parent != transform2)
						{
							transform.SetParent(transform2, false);
						}
					}
				}
			}
		}

		public static void RevertEquipmentPartBonesToOriginals(List<EquipmentPart> equipmentParts)
		{
			for (int i = 0; i < equipmentParts.Count; i++)
			{
				EquipmentPart equipmentPart = equipmentParts[i];
				List<Transform> bones = equipmentPart.Bones;
				Dictionary<string, int> boneNamesToIndices = equipmentPart.BoneNamesToIndices;
				Transform[] bones2 = equipmentPart.SkinnedMeshRenderer.bones;
				Transform[] array = new Transform[bones2.Length];
				for (int j = 0; j < bones2.Length; j++)
				{
					string name = bones2[j].gameObject.name;
					int index = boneNamesToIndices[name];
					array[j] = bones[index];
				}
				equipmentPart.SkinnedMeshRenderer.bones = array;
				for (int k = 0; k < bones.Count; k++)
				{
					Transform transform = bones[k];
					if (transform.parent == null)
					{
						continue;
					}
					int value = -1;
					if (boneNamesToIndices.TryGetValue(transform.parent.gameObject.name, out value))
					{
						Transform transform2 = bones[value];
						if (transform.parent != transform2)
						{
							transform.SetParent(transform2, false);
						}
					}
				}
			}
		}

		public static void UpdateSlotVisibilities(AvatarMeshCombineData avatarMeshCombineData)
		{
			foreach (KeyValuePair<string, Slot> slotNamesToModel in avatarMeshCombineData.SlotNamesToModels)
			{
				slotNamesToModel.Value.DefaultSkinnedMeshRenderer.gameObject.SetActive(false);
				for (int i = 0; i < slotNamesToModel.Value.VisibleSkinnedMeshRenderers.Count; i++)
				{
					if (slotNamesToModel.Value.VisibleSkinnedMeshRenderers[i] != null)
					{
						slotNamesToModel.Value.VisibleSkinnedMeshRenderers[i].gameObject.SetActive(true);
					}
					else
					{
						slotNamesToModel.Value.VisibleSkinnedMeshRenderers.RemoveAt(i);
					}
				}
			}
		}
	}
}
