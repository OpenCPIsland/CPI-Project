using UnityEngine;

namespace ClubPenguin.Core
{
	public class OnEnableChangeLayerOnChildren : MonoBehaviour
	{
		[Tooltip("Layer to change to. Choose ONE layer in the mask or else this will break")]
		public LayerMask LayerMaskToSwitchTo;

		[Tooltip("Layer to find. Choose ONE layer in the mask or else this will break")]
		public LayerMask LayerMaskToFind;

		public GameObject ParentContainer;

		public void OnEnable()
		{
			ChangeLayerOfChildren();
		}

		public void ChangeLayerOfChildren()
		{
			if (ParentContainer != null)
			{
				int value = LayerMaskToFind.value;
				GameObjectExtensions.ChangeLayersRecursively(layerOnly: (value != 0) ? ((int)Mathf.Log(value, 2f)) : (-1), trans: ParentContainer.transform, layer: (int)Mathf.Log(LayerMaskToSwitchTo.value, 2f));
			}
		}

		public void OnValidate()
		{
		}

		private static bool IsPowerOfTwo(int x)
		{
			return (x & (x - 1)) == 0;
		}
	}
}
