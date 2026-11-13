using System;
using System.Collections.Generic;
using UnityEngine;

namespace MixAvatar
{
	public class EquipmentPart : MonoBehaviour
	{
		private SkinnedMeshRenderer _skinnedMeshRenderer;

		[NonSerialized]
		private List<Transform> _bones;

		[NonSerialized]
		private Dictionary<string, int> _boneNamesToIndices;

		[NonSerialized]
		private BindPoseTransform[] _bindPoses;

		[NonSerialized]
		private string _targetSlotName;

		public EquipmentPartType PartType;

		public bool EjectSiblingsOnOverlap = true;

		[HideInInspector]
		public Slot OccupiedSlot;

		public SkinnedMeshRenderer SkinnedMeshRenderer
		{
			get
			{
				if (_skinnedMeshRenderer == null && base.transform.childCount != 0)
				{
					for (int i = 0; i < base.transform.childCount; i++)
					{
						SkinnedMeshRenderer component = base.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>();
						if (component != null)
						{
							if (_skinnedMeshRenderer != null)
							{
								throw new Exception("Equipment part may only have 1 SkinnedMeshRenderer child.");
							}
							_skinnedMeshRenderer = component;
						}
					}
				}
				return _skinnedMeshRenderer;
			}
		}

		public List<Transform> Bones
		{
			get
			{
				if (_bones == null)
				{
					_bones = new List<Transform>();
					if (SkinnedMeshRenderer != null)
					{
						getAllBones(SkinnedMeshRenderer.rootBone, _bones);
					}
				}
				return _bones;
			}
		}

		public Dictionary<string, int> BoneNamesToIndices
		{
			get
			{
				if (_boneNamesToIndices == null)
				{
					_boneNamesToIndices = new Dictionary<string, int>();
					List<Transform> bones = Bones;
					for (int i = 0; i < bones.Count; i++)
					{
						_boneNamesToIndices.Add(bones[i].gameObject.name, i);
					}
				}
				return _boneNamesToIndices;
			}
		}

		public BindPoseTransform[] BindPoses
		{
			get
			{
				if (_bindPoses == null)
				{
					_bindPoses = new BindPoseTransform[Bones.Count];
					List<Transform> bones = Bones;
					for (int i = 0; i < bones.Count; i++)
					{
						Transform transform = bones[i];
						BindPoseTransform bindPoseTransform = new BindPoseTransform(transform.localPosition, transform.localRotation);
						_bindPoses[i] = bindPoseTransform;
					}
				}
				return _bindPoses;
			}
		}

		public string TargetSlotName
		{
			get
			{
				if (string.IsNullOrEmpty(_targetSlotName))
				{
					Renderer renderer = null;
					for (int i = 0; i < base.transform.childCount; i++)
					{
						GameObject gameObject = base.transform.GetChild(i).gameObject;
						Renderer component = gameObject.GetComponent<Renderer>();
						if (component != null)
						{
							if (renderer != null)
							{
								throw new Exception("EquipmentPart may only have 1 MeshRenderer child");
							}
							renderer = component;
						}
					}
					_targetSlotName = renderer.gameObject.name;
				}
				return _targetSlotName;
			}
		}

		public event Action<EquipmentPart> EjectedFromSlot;

		private void Awake()
		{
			if (_bindPoses == null)
			{
				_bindPoses = BindPoses;
			}
		}

		public void Eject()
		{
			if (this.EjectedFromSlot != null)
			{
				this.EjectedFromSlot(this);
			}
		}

		private void getAllBones(Transform bone, List<Transform> allBones)
		{
			allBones.Add(bone);
			for (int i = 0; i < bone.childCount; i++)
			{
				getAllBones(bone.GetChild(i), allBones);
			}
		}

		public void OnValidate()
		{
		}
	}
}
