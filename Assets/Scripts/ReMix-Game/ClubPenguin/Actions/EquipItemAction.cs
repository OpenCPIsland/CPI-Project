using ClubPenguin.Locomotion;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Actions
{
	public class EquipItemAction : Action
	{
		public string TargetBoneName = "r_wrist_jnt";

		public GameObject ItemPrefab;

		public bool StoreExistingProp;

		private GameObject itemInstance;

		protected override void CopyTo(Action _destWarper)
		{
			EquipItemAction equipItemAction = _destWarper as EquipItemAction;
			equipItemAction.TargetBoneName = TargetBoneName;
			equipItemAction.ItemPrefab = ItemPrefab;
			equipItemAction.StoreExistingProp = StoreExistingProp;
			base.CopyTo(_destWarper);
		}

		protected override void Update()
		{
			if (itemInstance == null)
			{
				if (StoreExistingProp)
				{
					LocomotionUtils.UnEquipProp(GetTarget());
				}
				itemInstance = UnityEngine.Object.Instantiate(ItemPrefab);
				Transform transform = GetTarget().transform;
				List<Transform> userBones = new List<Transform>(transform.GetComponentsInChildren<Transform>());
				parentBatToTargetBone(userBones);
			}
			Completed(itemInstance);
		}

		private void parentBatToTargetBone(List<Transform> userBones)
		{
			Transform transform = null;
			for (int i = 0; i < userBones.Count; i++)
			{
				if (userBones[i].name == TargetBoneName)
				{
					transform = userBones[i];
					break;
				}
			}
			if (transform == null)
			{
				throw new ArgumentException("EquipItemAction: Could not find user bone with name: " + TargetBoneName);
			}
			itemInstance.transform.SetParent(transform, false);
		}
	}
}
