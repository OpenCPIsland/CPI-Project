using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.DCE
{
	[DisallowMultipleComponent]
	public class Rig : MonoBehaviour
	{
		[SerializeField]
		private Transform rootBone;

		private Transform[] bones;

		private Dictionary<string, int> boneIndexLookup = new Dictionary<string, int>();

		public Transform[] Bones
		{
			get
			{
				if (bones == null)
				{
					if (rootBone == null)
					{
						rootBone = FindRootBone(base.gameObject);
					}
					bones = FindBones(rootBone);
					for (int i = 0; i < bones.Length; i++)
					{
						boneIndexLookup[bones[i].name] = i;
					}
				}
				return bones;
			}
		}

		public Transform RootBone
		{
			get
			{
				return Bones[0];
			}
		}

		public Transform this[int index]
		{
			get
			{
				return Bones[index];
			}
		}

		public Transform this[string name]
		{
			get
			{
				return Bones[boneIndexLookup[name]];
			}
		}

		public int Length
		{
			get
			{
				return Bones.Length;
			}
		}

		public static Transform FindRootBone(GameObject go)
		{
			Transform transform = null;
			int num = 0;
			while (transform == null && num < go.transform.childCount)
			{
				Transform child = go.transform.GetChild(num);
				if (child.childCount > 0)
				{
					transform = child;
				}
				num++;
			}
			return transform;
		}

		public static Transform[] FindBones(Transform rootBone)
		{
			return rootBone.GetComponentsInChildren<Transform>();
		}

		public static void SetupSkinnedMeshRenderer(SkinnedMeshRenderer smr)
		{
			smr.rootBone = FindRootBone(smr.gameObject);
			smr.bones = FindBones(smr.rootBone);
		}

		public void OnDrawGizmosSelected()
		{
			Transform transform = rootBone;
			if (transform == null)
			{
				transform = FindRootBone(base.gameObject);
			}
			if (transform != null)
			{
				Gizmos.color = Color.red;
				Transform[] componentsInChildren = transform.GetComponentsInChildren<Transform>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					Gizmos.DrawLine(componentsInChildren[i].position, componentsInChildren[i].parent.position);
				}
			}
		}
	}
}
