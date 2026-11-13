using System;
using System.Collections.Generic;
using UnityEngine;

namespace MixAvatar
{
	public class AvatarBonesData
	{
		public List<Transform> Bones { get; private set; }

		public List<BindPoseTransform> BoneBindPoses { get; private set; }

		public Dictionary<string, int> BoneNamesToIndices { get; private set; }

		public Transform RootBone { get; private set; }

		public Dictionary<EquipmentPart, List<Transform>> EquipmentPartToAdditionalBones { get; private set; }

		public AvatarBonesData(Transform rootBone)
		{
			RootBone = rootBone;
			Bones = new List<Transform>();
			BoneNamesToIndices = new Dictionary<string, int>();
			BoneBindPoses = new List<BindPoseTransform>();
			setupBones(RootBone);
			EquipmentPartToAdditionalBones = new Dictionary<EquipmentPart, List<Transform>>();
		}

		private void setupBones(Transform boneTransform)
		{
			int value = -1;
			if (BoneNamesToIndices.TryGetValue(boneTransform.gameObject.name, out value))
			{
				throw new ArgumentException("Duplicate bone name found: " + boneTransform.gameObject.name);
			}
			Bones.Add(boneTransform);
			BindPoseTransform item = new BindPoseTransform(boneTransform.localPosition, boneTransform.localRotation);
			BoneBindPoses.Add(item);
			BoneNamesToIndices.Add(boneTransform.gameObject.name, Bones.Count - 1);
			for (int i = 0; i < boneTransform.childCount; i++)
			{
				Transform child = boneTransform.GetChild(i);
				setupBones(child);
			}
		}
	}
}
