using ClubPenguin.Actions;
using ClubPenguin.ObjectManipulation;
using UnityEngine;

namespace ReMixGame
{
	[RequireComponent(typeof(PartneredObject))]
	public class RunTimeWarpToActionModifier : MonoBehaviour
	{
		public PartneredObject PartneredObject;

		public void Awake()
		{
			PartneredObject.OtherSet += onOtherPartnerSet;
		}

		public void OnDestroy()
		{
			if (PartneredObject != null)
			{
				PartneredObject.OtherSet -= onOtherPartnerSet;
			}
		}

		public void onOtherPartnerSet(PartneredObject po)
		{
			if (po.Other != null)
			{
				WarpToTransformAction componentInChildren = GetComponentInChildren<WarpToTransformAction>();
				if (componentInChildren != null)
				{
					componentInChildren.TargetTransform = po.Other.transform;
				}
			}
		}
	}
}
