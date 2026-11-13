using UnityEngine;

namespace ClubPenguin.Cinematography
{
	public static class CameraCullingMaskHelper
	{
		public static void ShowLayer(Camera camera, string layer)
		{
			camera.cullingMask |= 1 << LayerMask.NameToLayer(layer);
		}

		public static void HideLayer(Camera camera, string layer)
		{
			camera.cullingMask &= ~(1 << LayerMask.NameToLayer(layer));
		}

		public static void SetSingleLayer(Camera camera, string layer)
		{
			camera.cullingMask = 1 << LayerMask.NameToLayer(layer);
		}

		public static void ShowAllLayers(Camera camera)
		{
			int num3 = camera.cullingMask = (camera.cullingMask = -1);
		}

		public static void HideAllLayers(Camera camera)
		{
			int num3 = camera.cullingMask = (camera.cullingMask = 0);
		}

		public static void SetLayerIncludingChildren(Transform parentTransform, string layer, bool recursive = false)
		{
			if (!recursive)
			{
				parentTransform.gameObject.layer = LayerMask.NameToLayer(layer);
				foreach (Transform item in parentTransform)
				{
					item.gameObject.layer = LayerMask.NameToLayer(layer);
				}
				return;
			}
			Transform[] componentsInChildren = parentTransform.GetComponentsInChildren<Transform>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.layer = LayerMask.NameToLayer(layer);
			}
		}

		public static void SetLayerRecursive(Transform parentTransform, string layer)
		{
			parentTransform.gameObject.layer = LayerMask.NameToLayer(layer);
			foreach (Transform item in parentTransform)
			{
				SetLayerRecursive(item, layer);
			}
		}

		public static bool IsLayerOn(Camera camera, string layer)
		{
			return camera.cullingMask == (camera.cullingMask | (1 << LayerMask.NameToLayer(layer)));
		}
	}
}
