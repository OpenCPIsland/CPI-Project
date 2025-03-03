using UnityEngine;

namespace ClubPenguin
{
	public static class LightCullingMaskHelper
	{
		public static void EnableLayer(Light light, string layer)
		{
			light.cullingMask |= 1 << LayerMask.NameToLayer(layer);
		}

		public static void DisableLayer(Light light, string layer)
		{
			light.cullingMask &= ~(1 << LayerMask.NameToLayer(layer));
		}

		public static void SetSingleLayer(Light light, string layer)
		{
			light.cullingMask = 1 << LayerMask.NameToLayer(layer);
		}

		public static void EnableAllLayers(Light light)
		{
			int num3 = light.cullingMask = (light.cullingMask = -1);
		}

		public static void DisableAllLayers(Light light)
		{
			int num3 = light.cullingMask = (light.cullingMask = 0);
		}

		public static void SetLayerIncludingChildren(Transform parentTransform, string layer)
		{
			int layer2 = LayerMask.NameToLayer(layer);
			parentTransform.gameObject.layer = layer2;
			Transform[] componentsInChildren = parentTransform.GetComponentsInChildren<Transform>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.layer = layer2;
			}
		}
	}
}
