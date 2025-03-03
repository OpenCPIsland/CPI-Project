using ClubPenguin.DecorationInventory;
using Disney.Kelowna.Common;
using Foundation.Unity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace ClubPenguin.Core
{
	public class LightingController : MonoBehaviour
	{
		[Serializable]
		public class Trilight
		{
			public Color sky;

			public Color equator;

			public Color ground;

			public static Trilight FromSettings()
			{
				Trilight trilight = new Trilight();
				trilight.sky = RenderSettings.ambientSkyColor;
				trilight.equator = RenderSettings.ambientEquatorColor;
				trilight.ground = RenderSettings.ambientGroundColor;
				return trilight;
			}
		}

		private const float FADE_DURATION = 2f;

		private void Awake()
		{
			RenderSettings.ambientMode = AmbientMode.Trilight;
		}

		private void OnDestroy()
		{
			StopAllCoroutines();
		}

		public void ApplyLighting(LightingDefinition lightingDefinition, bool blendTrilight = true)
		{
			Trilight trilight = new Trilight();
			trilight.sky = lightingDefinition.AmbientSkyColor;
			trilight.equator = lightingDefinition.AmbientEquatorColor;
			trilight.ground = lightingDefinition.AmbientGroundColor;
			Trilight trilight2 = trilight;
			LightingDefinitionOverride lightingDefinitionOverride = UnityEngine.Object.FindObjectOfType<LightingDefinitionOverride>();
			if (lightingDefinitionOverride != null && lightingDefinitionOverride.TrilightOverrides != null)
			{
				for (int i = 0; i < lightingDefinitionOverride.TrilightOverrides.Length; i++)
				{
					if (lightingDefinitionOverride.TrilightOverrides[i].LightingDef.Id == lightingDefinition.Id)
					{
						trilight2.sky = lightingDefinitionOverride.TrilightOverrides[i].Trilight.sky;
						trilight2.equator = lightingDefinitionOverride.TrilightOverrides[i].Trilight.equator;
						trilight2.ground = lightingDefinitionOverride.TrilightOverrides[i].Trilight.ground;
						break;
					}
				}
			}
			setTrilight(trilight2, blendTrilight);
			setSkybox(lightingDefinition.SkyboxMaterialKey);
		}

		private void setTrilight(Trilight destionColors, bool blendTrilight)
		{
			StopAllCoroutines();
			if (blendTrilight)
			{
				Trilight srcColors = Trilight.FromSettings();
				StartCoroutine(fadeColors(srcColors, destionColors));
			}
			else
			{
				RenderSettings.ambientSkyColor = destionColors.sky;
				RenderSettings.ambientEquatorColor = destionColors.equator;
				RenderSettings.ambientGroundColor = destionColors.ground;
			}
		}

		private IEnumerator fadeColors(Trilight srcColors, Trilight dstColors)
		{
			float elapsed = 0f;
			do
			{
				yield return null;
				elapsed += Time.deltaTime;
				float t = Mathf.Clamp01(elapsed / 2f);
				RenderSettings.ambientSkyColor = Color.Lerp(srcColors.sky, dstColors.sky, t);
				RenderSettings.ambientEquatorColor = Color.Lerp(srcColors.equator, dstColors.equator, t);
				RenderSettings.ambientGroundColor = Color.Lerp(srcColors.ground, dstColors.ground, t);
			}
			while (elapsed < 2f);
		}

		private void setSkybox(MaterialContentKey skyboxMaterialKey)
		{
			if (skyboxMaterialKey != null && !string.IsNullOrEmpty(skyboxMaterialKey.Key))
			{
				Texture mainTexture = RenderSettings.skybox.mainTexture;
				if (mainTexture != null)
				{
					RenderSettings.skybox.mainTexture = null;
					ComponentExtensions.DestroyResource(mainTexture);
				}
				Content.LoadAsync(delegate(string path, Material material)
				{
					onSkyboxLoaded(material);
				}, skyboxMaterialKey);
			}
		}

		private void onSkyboxLoaded(Material material)
		{
			RenderSettings.skybox = material;
		}
	}
}
