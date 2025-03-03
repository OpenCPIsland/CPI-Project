using ClubPenguin.Avatar;
using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(ImageBuilderCameraData))]
	public class AvatarImageComponent : MonoBehaviour
	{
		private struct RenderRequest
		{
			public DataEntityHandle Handle;

			public AvatarDetailsData AvatarDetailsData;

			public string Context;

			public AvatarAnimationFrame AvatarAnimationFrame;

			public RenderRequest(DataEntityHandle handle)
			{
				Handle = handle;
				AvatarDetailsData = null;
				Context = null;
				AvatarAnimationFrame = null;
			}
		}

		private const int LOD_INDEX = 1;

		private static PrefabContentKey uiAvatarPrefabContentKey = new PrefabContentKey("Prefabs/UIAvatarPenguin");

		public bool UseCache;

		private bool isRenderingActive = true;

		public Action<DataEntityHandle, Texture2D> OnImageReady;

		private DataEntityCollection dataEntityCollection;

		private ImageBuilderCameraData imageBuilderCameraData;

		private OtherPlayerDetailsRequestBatcher otherPlayerDetailsRequestBatcher;

		private AvatarImageCacher avatarImageCacher;

		private Dictionary<string, RenderRequest> pendingRenderRequests;

		private HashSet<string> pausedRenderRequests;

		private GameObject avatarPrefab;

		private bool isLoadingAvatarPrefab;

		public bool IsRenderingActive
		{
			set
			{
				bool flag = value && !isRenderingActive;
				isRenderingActive = value;
				if (flag)
				{
					startPausedRenders();
				}
			}
		}

		private void Awake()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			imageBuilderCameraData = GetComponent<ImageBuilderCameraData>();
			dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<AvatarDetailsData>>(onAvatarDetailsDataAdded);
			pendingRenderRequests = new Dictionary<string, RenderRequest>();
			pausedRenderRequests = new HashSet<string>();
			otherPlayerDetailsRequestBatcher = Service.Get<OtherPlayerDetailsRequestBatcher>();
			otherPlayerDetailsRequestBatcher.ResetFirstRequestStatus();
			isLoadingAvatarPrefab = false;
			if (UseCache)
			{
				avatarImageCacher = new AvatarImageCacher();
			}
		}

		public void RequestImage(DataEntityHandle handle, AvatarAnimationFrame avatarAnimationFrame = null, string context = null)
		{
			if (UseCache && string.IsNullOrEmpty(context))
			{
				throw new ArgumentException("If using the cache, the context string must not be null");
			}
			string displayName = dataEntityCollection.GetComponent<DisplayNameData>(handle).DisplayName;
			if (!pendingRenderRequests.ContainsKey(displayName))
			{
				RenderRequest renderRequest = new RenderRequest(handle);
				renderRequest.AvatarAnimationFrame = avatarAnimationFrame;
				renderRequest.Context = context;
				pendingRenderRequests.Add(displayName, renderRequest);
				AvatarDetailsData component;
				if (!dataEntityCollection.TryGetComponent(handle, out component))
				{
					otherPlayerDetailsRequestBatcher.RequestOtherPlayerDetails(handle);
					return;
				}
				renderRequest.AvatarDetailsData = component;
				getImage(displayName, renderRequest);
			}
		}

		public bool IsRenderInProgress(string displayName)
		{
			return pendingRenderRequests.ContainsKey(displayName);
		}

		public bool CancelRender(string displayName)
		{
			return pendingRenderRequests.Remove(displayName);
		}

		public bool ContainsCachedImage(DataEntityHandle handle, string context = null)
		{
			return avatarImageCacher.ContainsImage(handle, context);
		}

		private bool onAvatarDetailsDataAdded(DataEntityEvents.ComponentAddedEvent<AvatarDetailsData> evt)
		{
			string displayName = dataEntityCollection.GetComponent<DisplayNameData>(evt.Handle).DisplayName;
			if (pendingRenderRequests.ContainsKey(displayName))
			{
				evt.Component.OnInitialized += onAvatarDetailsInitialized;
			}
			return false;
		}

		private void onAvatarDetailsInitialized(AvatarDetailsData avatarDetailsData)
		{
			avatarDetailsData.OnInitialized -= onAvatarDetailsInitialized;
			DataEntityHandle entityByComponent = dataEntityCollection.GetEntityByComponent(avatarDetailsData);
			string displayName = dataEntityCollection.GetComponent<DisplayNameData>(entityByComponent).DisplayName;
			if (pendingRenderRequests.ContainsKey(displayName))
			{
				RenderRequest renderRequest = pendingRenderRequests[displayName];
				renderRequest.AvatarDetailsData = avatarDetailsData;
				getImage(displayName, renderRequest);
			}
		}

		private void getImage(string displayName, RenderRequest renderRequest)
		{
			if (UseCache)
			{
				if (!string.IsNullOrEmpty(renderRequest.Context))
				{
					Texture2D icon;
					if (avatarImageCacher.TryGetCachedImage(renderRequest.Handle, out icon, renderRequest.Context))
					{
						pendingRenderRequests.Remove(displayName);
						dispatchImageReady(renderRequest.Handle, icon);
						return;
					}
				}
				else
				{
					Log.LogErrorFormatted(this, "Could not find a context string to check the cache with for display name: {0}", displayName);
				}
			}
			if (isRenderingActive)
			{
				renderImage(renderRequest);
			}
			else
			{
				pausedRenderRequests.Add(displayName);
			}
		}

		private void startPausedRenders()
		{
			foreach (string pausedRenderRequest in pausedRenderRequests)
			{
				resumeRender(pausedRenderRequest);
			}
			pausedRenderRequests.Clear();
		}

		private void resumeRender(string displayName)
		{
			if (pendingRenderRequests.ContainsKey(displayName))
			{
				RenderRequest renderRequest = pendingRenderRequests[displayName];
				if (renderRequest.AvatarDetailsData != null || dataEntityCollection.TryGetComponent(renderRequest.Handle, out renderRequest.AvatarDetailsData))
				{
					getImage(displayName, renderRequest);
				}
				else
				{
					otherPlayerDetailsRequestBatcher.RequestOtherPlayerDetails(renderRequest.Handle);
				}
			}
		}

		private void renderImage(RenderRequest renderRequest)
		{
			CoroutineRunner.Start(loadRenderGameObjects(renderRequest.AvatarDetailsData.Outfit, renderRequest.AvatarDetailsData.BodyColor, renderRequest.AvatarAnimationFrame, renderRequest), this, "RenderAvatar_loadRenderGameObjects");
		}

		private IEnumerator loadRenderGameObjects(DCustomEquipment[] outfit, Color bodyColor, AvatarAnimationFrame avatarFrame, RenderRequest renderRequest)
		{
			if (avatarPrefab == null && isLoadingAvatarPrefab)
			{
				yield return null;
			}
			if (avatarPrefab == null)
			{
				isLoadingAvatarPrefab = true;
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
					avatarPrefab = assetRequest.Asset;
					isLoadingAvatarPrefab = false;
				}
			}
			if (avatarPrefab != null)
			{
				GameObject avatarInstance = UnityEngine.Object.Instantiate(avatarPrefab);
				IconRenderLightingRig lightingRig = IconRenderLightingRig.Acquire();
				while (!lightingRig.IsReady)
				{
					yield return null;
				}
				DCustomOutfit avatarOutfit = default(DCustomOutfit);
				avatarOutfit.Equipment = outfit;
				AvatarModel avatarModel = avatarInstance.GetComponent<AvatarModel>();
				yield return AvatarRenderer.RenderOutfit(avatarOutfit, avatarModel.BeakColor, bodyColor, avatarModel.BellyColor, imageBuilderCameraData, avatarInstance, (ModelRenderer modelRenderer) => onProcessAnimationFrame(modelRenderer, renderRequest.Handle, avatarInstance), avatarFrame);
				IconRenderLightingRig.Release();
			}
			else
			{
				Log.LogErrorFormatted(this, "Unabe to load the UI Avatar Prefab at path {0}", uiAvatarPrefabContentKey.Key);
			}
		}

		private bool onProcessAnimationFrame(ModelRenderer modelRenderer, DataEntityHandle handle, GameObject avatarInstance)
		{
			string displayName = dataEntityCollection.GetComponent<DisplayNameData>(handle).DisplayName;
			if (pendingRenderRequests.ContainsKey(displayName))
			{
				Texture2D image = modelRenderer.Image;
				if (UseCache)
				{
					if (!string.IsNullOrEmpty(pendingRenderRequests[displayName].Context))
					{
						avatarImageCacher.SaveTextureToCache(handle, pendingRenderRequests[displayName].Context, image);
					}
					else
					{
						Log.LogErrorFormatted(this, "Could not add image to cache, context string not found for display name: {0}", displayName);
					}
				}
				pendingRenderRequests.Remove(displayName);
				dispatchImageReady(handle, image);
			}
			modelRenderer.Destroy();
			UnityEngine.Object.Destroy(avatarInstance);
			return false;
		}

		private void dispatchImageReady(DataEntityHandle handle, Texture2D icon)
		{
			if (OnImageReady != null)
			{
				OnImageReady.InvokeSafe(handle, icon);
			}
		}

		private void OnDestroy()
		{
			OnImageReady = null;
			CoroutineRunner.StopAllForOwner(this);
			if (avatarImageCacher != null)
			{
				avatarImageCacher.ClearCache();
			}
			pendingRenderRequests.Clear();
			dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<AvatarDetailsData>>(onAvatarDetailsDataAdded);
			avatarPrefab = null;
		}
	}
}
