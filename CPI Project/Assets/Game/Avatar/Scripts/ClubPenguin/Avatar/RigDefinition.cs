#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Assertions;

namespace ClubPenguin.Avatar
{
	[Serializable]
	public class RigDefinition
	{
		[Serializable]
		public class Bone
		{
			public string Name;

			public Vector3 Position;

			public Quaternion Rotation;

			public int ChildCount;

			public Bone(Transform source)
			{
				Name = source.name;
				Position = source.localPosition;
				Rotation = source.localRotation;
				ChildCount = source.childCount;
			}

			public Transform ToTransform(Transform parent = null)
			{
				Transform transform = new GameObject(Name).transform;
				transform.localPosition = Position;
				transform.localRotation = Rotation;
				if (parent != null)
				{
					transform.SetParent(parent, false);
				}
				return transform;
			}

			public override string ToString()
			{
				return string.Format("[Bone: Name={0}, Position={1}, Rotation={2}, ChildCount={3}]", Name, Position, Rotation, ChildCount);
			}
		}

		public Bone[] Bones;

		public int GetHash()
		{
			StructHash sh = default(StructHash);
			for (int i = 0; i < Bones.Length; i++)
			{
				sh.Combine(Bones[i].Name);
				sh.Combine(Bones[i].Position);
				sh.Combine(Bones[i].Rotation);
				sh.Combine(Bones[i].ChildCount);
			}
			return sh;
		}

		public RigDefinition()
		{
			Bones = new Bone[0];
		}

		[Conditional("UNITY_EDITOR")]
		public void FromTransform(Transform source)
		{
			List<Bone> list = new List<Bone>();
			Bones = list.ToArray();
		}

		[Conditional("UNITY_EDITOR")]
		private void addBone(Transform source, List<Bone> bones)
		{
			bones.Add(new Bone(source));
			for (int i = 0; i < source.childCount; i++)
			{
			}
		}

		public Transform ToTransform(Transform[] boneList = null)
		{
			Transform transform = Bones[0].ToTransform();
			createTransforms(transform, 0, boneList);
			return transform;
		}

		private int createTransforms(Transform parent, int index, Transform[] boneList)
		{
			if (boneList != null)
			{
				boneList[index] = parent;
			}
			Bone bone = Bones[index++];
			for (int i = 0; i < bone.ChildCount; i++)
			{
				Transform parent2 = Bones[index].ToTransform(parent);
				index = createTransforms(parent2, index, boneList);
			}
			return index;
		}

		public int FindBoneIndex(Bone rhsBone)
		{
			Assert.IsNotNull(rhsBone);
			Assert.IsNotNull(Bones);
			int num = Bones.Length;
			for (int i = 0; i < num; i++)
			{
				Bone bone = Bones[i];
				if (bone.Name == rhsBone.Name)
				{
					num = i;
				}
			}
			return num;
		}
	}
}
