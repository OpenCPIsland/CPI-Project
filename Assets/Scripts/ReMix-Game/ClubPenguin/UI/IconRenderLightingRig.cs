using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using System;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class IconRenderLightingRig
	{
		private static readonly PrefabContentKey LIGHTS_KEY = new PrefabContentKey("Lighting/IconRenderLighting");

		private static IconRenderLightingRig instance;

		private static int refCount = 0;

		private GameObject lightingRig;

		private bool cancelLoading;

		public bool IsReady
		{
			get;
			private set;
		}

		public static IconRenderLightingRig Acquire()
		{
			if (instance == null || instance.lightingRig == null)
			{
				instance = new IconRenderLightingRig();
			}
			instance.cancelLoading = false;
			refCount++;
			return instance;
		}

		public static void Release()
		{
			if (--refCount <= 0 && instance != null)
			{
				instance.cancelLoading = true;
				instance.destroy();
				instance = null;
			}
		}

		private IconRenderLightingRig()
		{
			IsReady = false;
			cancelLoading = false;
			Light[] array = UnityEngine.Object.FindObjectsOfType<Light>();
			for (int i = 0; i < array.Length; i++)
			{
				LightCullingMaskHelper.DisableLayer(array[i], "IconRender");
			}
			Content.LoadAsync(onLightPrefabLoaded, LIGHTS_KEY);
		}

		private void onLightPrefabLoaded(string path, GameObject prefab)
		{
			if (!cancelLoading)
			{
				try
				{
					lightingRig = UnityEngine.Object.Instantiate(prefab);
					lightingRig.name = "IconRender_LightingRig";
					Light[] componentsInChildren = lightingRig.GetComponentsInChildren<Light>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						LightCullingMaskHelper.SetSingleLayer(componentsInChildren[i], "IconRender");
					}
					lightingRig.SetActive(true);
					IsReady = true;
				}
				catch (Exception)
				{
					Log.LogError(this, string.Format("Could not instantiate icon render light asset {0}.", LIGHTS_KEY.Key));
				}
			}
		}

		private void destroy()
		{
			if (lightingRig != null)
			{
				UnityEngine.Object.Destroy(lightingRig);
			}
			lightingRig = null;
		}
	}
}
