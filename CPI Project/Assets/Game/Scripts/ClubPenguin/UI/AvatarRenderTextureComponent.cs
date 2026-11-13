using ClubPenguin.Avatar;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(ImageBuilderCameraData))]
	public class AvatarRenderTextureComponent : MonoBehaviour
	{
		private static PrefabContentKey uiAvatarPrefabContentKey = new PrefabContentKey("Prefabs/UIAvatarPenguin");

		public GameObject AvatarPreloader;

		public GameObject AvatarContainer;

		public RawImage AvatarIcon;

		private ImageBuilderCameraData imageBuilderCameraData;

		private bool avatarImageSet;

		private bool continueAvatarRender;

		private GameObject avatarInstance;

		private IconRenderLightingRig lightingRig;

		private ModelRenderer modelRenderer;

		private bool useBodyColor;

		private Color bodyColor;

		private bool updateRotation;

		private float rotationAmount;

		public event System.Action OnAvatarImageSet;

		private void Awake()
		{
			imageBuilderCameraData = GetComponent<ImageBuilderCameraData>();
			hideAvatar();
			useBodyColor = false;
		}

		private void OnEnable()
		{
			lightingRig = IconRenderLightingRig.Acquire();
		}

		private void OnDisable()
		{
			if (lightingRig != null)
			{
				IconRenderLightingRig.Release();
			}
		}

		private void OnDestroy()
		{
			cleanupRenderObjects();
			continueAvatarRender = false;
			this.OnAvatarImageSet = null;
		}

		private void cleanupRenderObjects()
		{
			if (modelRenderer != null)
			{
				modelRenderer.Destroy();
				modelRenderer = null;
			}
			if (avatarInstance != null)
			{
				UnityEngine.Object.Destroy(avatarInstance);
			}
		}

		public void RenderAvatar(AvatarDetailsData avatarDetails, AvatarAnimationFrame avatarFrame = null)
		{
			RenderAvatar(avatarDetails.Outfit, avatarDetails.BodyColor, avatarFrame);
		}

		public void RenderAvatar(DCustomEquipment[] outfit, Color bodyColor, AvatarAnimationFrame avatarFrame = null)
		{
			this.bodyColor = bodyColor;
			useBodyColor = true;
			RenderAvatar(outfit, avatarFrame);
		}

		public void RenderAvatar(DCustomEquipment[] outfit, AvatarAnimationFrame avatarFrame = null)
		{
			hideAvatar();
			CoroutineRunner.Start(loadRenderGameObjects(outfit, avatarFrame), this, "RenderAvatar_loadRenderGameObjects");
		}

		private IEnumerator loadRenderGameObjects(DCustomEquipment[] outfit, AvatarAnimationFrame avatarFrame)
		{
			if (avatarInstance != null)
			{
				continueAvatarRender = false;
				cleanupRenderObjects();
				yield return null;
			}
			while (lightingRig != null && !lightingRig.IsReady)
			{
				yield return null;
			}
			AssetRequest<GameObject> assetRequest = null;
			try
			{
				assetRequest = Content.LoadAsync(uiAvatarPrefabContentKey);
			}
			catch (Exception ex)
			{
				Log.LogError(this, string.Format("Could not load UI Avatar penguin asset {0}. Message: {1}", uiAvatarPrefabContentKey.Key, ex.Message));
			}
			if (assetRequest != null)
			{
				yield return assetRequest;
				avatarInstance = UnityEngine.Object.Instantiate(assetRequest.Asset);
				DCustomOutfit avatarOutfit = default(DCustomOutfit);
				avatarOutfit.Equipment = outfit;
				avatarImageSet = false;
				continueAvatarRender = true;
				AvatarModel avatarModel = avatarInstance.GetComponent<AvatarModel>();
				yield return AvatarRenderer.RenderOutfit(beakColor: avatarModel.BeakColor, bellyColor: avatarModel.BellyColor, bodyColor: useBodyColor ? bodyColor : avatarModel.BodyColor, outfit: avatarOutfit, cameraData: imageBuilderCameraData, avatarGO: avatarInstance, onProcessModel: onProcessAnimationFrame, animationFrame: avatarFrame);
			}
		}

		private bool onProcessAnimationFrame(ModelRenderer modelRenderer)
		{
			if (!avatarImageSet)
			{
				this.modelRenderer = modelRenderer;
				AvatarIcon.enabled = true;
				AvatarIcon.texture = modelRenderer.RenderTexture;
				AvatarPreloader.SetActive(false);
				AvatarContainer.SetActive(true);
				avatarImageSet = true;
				if (this.OnAvatarImageSet != null)
				{
					this.OnAvatarImageSet();
				}
			}
			if (updateRotation)
			{
				Quaternion modelRotation = modelRenderer.GetModelRotation();
				modelRotation *= Quaternion.Euler(0f, rotationAmount * Time.deltaTime, 0f);
				modelRenderer.SetModelRotation(modelRotation);
				updateRotation = false;
			}
			return continueAvatarRender;
		}

		public void PlayAnimation(AvatarAnimationFrame animationFrame)
		{
			if (avatarInstance != null)
			{
				Animator component = avatarInstance.GetComponent<Animator>();
				component.Play(animationFrame.StateName, animationFrame.Layer, animationFrame.Time);
			}
		}

		private void hideAvatar()
		{
			AvatarPreloader.SetActive(true);
			AvatarContainer.SetActive(false);
		}

		public void RotateModel(float rotationAmount)
		{
			this.rotationAmount = rotationAmount;
			updateRotation = true;
		}
	}
}
