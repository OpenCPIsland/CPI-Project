using ClubPenguin.Avatar;
using ClubPenguin.Cinematography;
using Disney.Kelowna.Common;
using System;
using UnityEngine;

namespace ClubPenguin.Props
{
	[RequireComponent(typeof(Prop))]
	public class PropAccessory : MonoBehaviour
	{
		public string TargetBoneName = "r_wrist_jnt";

		public PrefabContentKey PrefabContentKey;

		public bool Enabled = true;

		private bool isLoadingComplete;

		private GameObject propAccessory;

		public bool IsLoadingComplete
		{
			get
			{
				return isLoadingComplete;
			}
		}

		public GameObject PropAccessoryGO
		{
			get
			{
				return propAccessory;
			}
		}

		private void Start()
		{
			Content.LoadAsync(onPropAccessoryLoaded, PrefabContentKey);
		}

		private void onPropAccessoryLoaded(string path, GameObject prefab)
		{
			if (base.gameObject.IsDestroyed())
			{
				return;
			}
			if (prefab != null)
			{
				propAccessory = UnityEngine.Object.Instantiate(prefab);
				Prop component = GetComponent<Prop>();
				if (component != null && component.PropUserRef != null)
				{
					Rig component2 = component.PropUserRef.GetComponent<Rig>();
					parentPropAccessoryToTargetBone(component2);
					CameraCullingMaskHelper.SetLayerRecursive(component.transform, LayerMask.LayerToName(component.gameObject.layer));
				}
			}
			isLoadingComplete = true;
		}

		private void parentPropAccessoryToTargetBone(Rig rig)
		{
			Transform transform = rig[TargetBoneName];
			if (transform == null)
			{
				throw new InvalidOperationException("Could not find user bone with name: " + TargetBoneName);
			}
			propAccessory.transform.SetParent(transform, false);
			propAccessory.SetActive(Enabled);
		}

		public void OnDestroy()
		{
			if (propAccessory != null)
			{
				UnityEngine.Object.Destroy(propAccessory);
			}
		}
	}
}
