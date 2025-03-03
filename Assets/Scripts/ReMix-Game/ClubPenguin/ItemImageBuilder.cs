using ClubPenguin.Avatar;
using ClubPenguin.Cinematography;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Foundation.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class ItemImageBuilder : AbstractImageBuilder
	{
		private enum LoadingTriState
		{
			IN_PROGRESS,
			SUCCESS,
			FAILURE
		}

		private class ItemRenderParams : RenderParams
		{
			public DCustomEquipment equipment;

			public Color bodyColor;
		}

		private const string UNLIT_SHADER_NAME = "CpRemix/Particles/Unlit Color";

		private static readonly PrefabContentKey PENGUIN_KEY = new PrefabContentKey("Prefabs/ClothingDesigner/ItemRenderPenguin");

		private GameObject penguin;

		private AvatarModel avatarModel;

		private AvatarView avatarView;

		private AvatarController avatarController;

		private Color defaultBodyColor;

		private LoadingTriState penguinPreload = LoadingTriState.IN_PROGRESS;

		private static ItemImageBuilder instance;

		private static int refCount = 0;

		public static ItemImageBuilder acquire()
		{
			if (instance == null)
			{
				instance = new ItemImageBuilder();
			}
			refCount++;
			return instance;
		}

		public static void release()
		{
			if (--refCount <= 0)
			{
				instance.destroy();
				instance = null;
			}
		}

		public void RemoveImageFromCache(DCustomEquipment equipment)
		{
			string hashName = calcHash(equipment, Color.clear, Color.clear);
			if (imageCache.ContainsImage(hashName))
			{
				imageCache.RemoveImage(hashName);
			}
		}

		public void RequestImage(DCustomEquipment equipment, RequestImageCallback callback)
		{
			CallbackToken callbackToken = default(CallbackToken);
			callbackToken.Id = equipment.Id;
			callbackToken.DefinitionId = equipment.DefinitionId;
			RequestImage(equipment, callback, callbackToken, Color.clear, Color.clear);
		}

		public void RequestImage(DCustomEquipment equipment, RequestImageCallback callback, CallbackToken callbackToken, Color backgroundColor, Color penguinColor)
		{
			string imageHash = calcHash(equipment, backgroundColor, penguinColor);
			ItemRenderParams itemRenderParams = new ItemRenderParams();
			itemRenderParams.CallbackToken = callbackToken;
			itemRenderParams.equipment = equipment;
			itemRenderParams.BackgroundColor = backgroundColor;
			itemRenderParams.bodyColor = penguinColor;
			itemRenderParams.ImageHash = imageHash;
			ItemRenderParams renderParams = itemRenderParams;
			RequestImage(renderParams, callback);
		}

		private ItemImageBuilder()
		{
			adjustSceneCullingMasks();
			CoroutineRunner.Start(preloadPenguinPrefab(), this, "preloadPenguinPrefab");
		}

		protected override IEnumerator processPendingRequests()
		{
			while (penguinPreload == LoadingTriState.IN_PROGRESS)
			{
				yield return null;
			}
			penguin.SetActive(true);
			yield return base.processPendingRequests();
			if (penguin != null)
			{
				penguin.SetActive(false);
			}
		}

		protected override IEnumerator processRequest(RenderParams renderParam)
		{
			ItemRenderParams param = renderParam as ItemRenderParams;
			if (penguinPreload != LoadingTriState.FAILURE)
			{
				Color bodyColor = (param.bodyColor != Color.clear) ? param.bodyColor : defaultBodyColor;
				penguin.transform.localPosition = new Vector3(10f, 0f, 0f);
				avatarModel.ClearAllEquipment();
				try
				{
					avatarModel.ApplyOutfit(createOutfit(param.equipment));
				}
				catch (Exception ex)
				{
					Log.LogErrorFormatted(this, "When applying an outfit to the avatar model an error occurred. Icon not rendered. Message: {0}", ex);
					yield break;
				}
				avatarModel.BeakColor = bodyColor;
				avatarModel.BellyColor = bodyColor;
				avatarModel.BodyColor = bodyColor;
				while (!avatarView.IsReady)
				{
					yield return null;
				}
				LightCullingMaskHelper.SetLayerIncludingChildren(penguin.transform, "IconRender");
				modifyMaterials(bodyColor);
				yield return renderToTexture(param);
			}
		}

		private IEnumerator renderToTexture(ItemRenderParams param)
		{
			param.OutputTexture = null;
			DCustomEquipment equipment = param.equipment;
			int equipmentTemplateId = equipment.DefinitionId;
			Dictionary<int, TemplateDefinition> templates = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>();
			if (!templates.ContainsKey(equipmentTemplateId))
			{
				Log.LogErrorFormatted(this, "Unable to locate template {0} in template definitions with id {1}.", equipment.Name, equipmentTemplateId);
				yield break;
			}
			TemplateDefinition templateDefinition = templates[equipmentTemplateId];
			AssetRequest<TemplateRenderData> templateRequest;
			try
			{
				templateRequest = Content.LoadAsync(templateDefinition.RenderDataKey);
			}
			catch (Exception)
			{
				Log.LogErrorFormatted(this, "Could not load render data for template definition {0} at {1}.", templateDefinition.Name, templateDefinition.RenderDataKey.Key);
				yield break;
			}
			if (templateRequest != null)
			{
				yield return templateRequest;
				TemplateRenderData templateRenderData = templateRequest.Asset;
				penguin.transform.rotation = templateRenderData.ItemRotation;
				ModelRendererConfig config = new ModelRendererConfig(penguin.transform, templateRenderData.ItemPosition, new Vector2(256f, 256f))
				{
					FieldOfView = templateRenderData.CameraFOV,
					FrameObjectInCamera = false,
					UseOcclusionCulling = false,
					AutoDestroyObjectToRender = false
				};
				if (param.BackgroundColor != Color.clear)
				{
					config.CameraBackgroundColor = param.BackgroundColor;
					config.UseSolidBackground = true;
				}
				ModelRenderer modelRenderer = new ModelRenderer(config);
				modelRenderer.RotateCamera(templateRenderData.CameraRotation.eulerAngles);
				yield return new WaitForEndOfFrame();
				param.OutputTexture = modelRenderer.Image;
				modelRenderer.Destroy();
			}
		}

		private static DCustomOutfit createOutfit(DCustomEquipment equipment)
		{
			DCustomOutfit result = default(DCustomOutfit);
			result.LodLevel = 0;
			result.Equipment = new DCustomEquipment[1]
			{
				equipment
			};
			return result;
		}

		private static string calcHash(DCustomEquipment data, Color backgroundColor, Color penguinColor)
		{
			StructHash sh = default(StructHash);
			sh.Combine(data.GetFullHash());
			sh.Combine(backgroundColor);
			sh.Combine(penguinColor);
			return Convert.ToString(sh);
		}

		private IEnumerator preloadPenguinPrefab()
		{
			AssetRequest<GameObject> assetRequest;
			try
			{
				assetRequest = Content.LoadAsync(PENGUIN_KEY);
				if (assetRequest == null)
				{
					throw new NullReferenceException();
				}
			}
			catch (Exception)
			{
				penguinPreload = LoadingTriState.FAILURE;
				Log.LogError(this, string.Format("Could not load penguin asset {0}.", PENGUIN_KEY.Key));
				yield break;
			}
			if (assetRequest != null)
			{
				yield return assetRequest;
			}
			try
			{
				penguin = UnityEngine.Object.Instantiate(assetRequest.Asset);
				avatarView = penguin.GetComponent<AvatarView>();
				avatarModel = penguin.GetComponent<AvatarModel>();
				avatarController = penguin.GetComponent<AvatarController>();
				if (!avatarController || !avatarView || !avatarModel)
				{
					throw new NullReferenceException();
				}
			}
			catch (Exception)
			{
				penguinPreload = LoadingTriState.FAILURE;
				Log.LogError(this, string.Format("Could not instantiate penguin MVC components. {0}", PENGUIN_KEY.Key));
				yield break;
			}
			penguin.SetActive(false);
			defaultBodyColor = avatarModel.BodyColor;
			penguinPreload = LoadingTriState.SUCCESS;
		}

		private void modifyMaterials(Color bodyColor)
		{
			Shader shader = Shader.Find("CpRemix/Particles/Unlit Color");
			Material material = new Material(shader);
			material.color = bodyColor;
			SkinnedMeshRenderer[] componentsInChildren = penguin.GetComponentsInChildren<SkinnedMeshRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Material sharedMaterial = componentsInChildren[i].sharedMaterial;
				if (sharedMaterial != null)
				{
					if (sharedMaterial.shader.name == AvatarService.EquipmentPreviewShader.name)
					{
						sharedMaterial.shader = AvatarService.EquipmentScreenshotShader;
					}
					else
					{
						ComponentExtensions.DestroyIfInstance(componentsInChildren[i].sharedMaterial);
						componentsInChildren[i].sharedMaterial = material;
					}
				}
				else
				{
					Log.LogErrorFormatted(this, "Null material in {0}", componentsInChildren[i].name);
				}
				componentsInChildren[i].updateWhenOffscreen = true;
			}
		}

		private void adjustSceneCullingMasks()
		{
			CameraCullingMaskHelper.HideLayer(Camera.main, "IconRender");
		}

		protected override void destroy()
		{
			if (penguin != null)
			{
				UnityEngine.Object.Destroy(penguin);
			}
			base.destroy();
		}
	}
}
