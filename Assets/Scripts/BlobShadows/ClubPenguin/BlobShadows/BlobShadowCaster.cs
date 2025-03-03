using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.BlobShadows
{
	public class BlobShadowCaster : MonoBehaviour
	{
		private const float AVATAR_BLOB_SHADOW_SCALE = 0.6f;

		public float ScaleX = 1f;

		public float ScaleZ = 1f;

		[HideInInspector]
		public bool GeoVisible = false;

		private BlobShadowRenderer blobShadowRenderer;

		private EventDispatcher eventDispatcher;

		public bool ShadowCamVisible
		{
			get
			{
				if (blobShadowRenderer != null)
				{
					return blobShadowRenderer.IsShadowsVisible;
				}
				return true;
			}
		}

		private void Awake()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
		}

		public void Start()
		{
			if (SceneRefs.IsSet<BlobShadowRenderer>())
			{
				blobShadowRenderer = SceneRefs.Get<BlobShadowRenderer>();
				SetIsActive(blobShadowRenderer.IsShadowsVisible);
			}
			eventDispatcher.AddListener<BlobShadowEvents.DisableBlobShadows>(onDisableBlobShadows);
			eventDispatcher.AddListener<BlobShadowEvents.EnableBlobShadows>(onEnableBlobShadows);
			ScaleX = 0.6f;
			ScaleZ = 0.6f;
		}

		public void OnDestroy()
		{
			eventDispatcher.RemoveListener<BlobShadowEvents.DisableBlobShadows>(onDisableBlobShadows);
			eventDispatcher.RemoveListener<BlobShadowEvents.EnableBlobShadows>(onEnableBlobShadows);
			if (blobShadowRenderer != null)
			{
				blobShadowRenderer.ShadowCasters.Remove(this);
			}
			GeoVisible = false;
		}

		public void SetIsActiveIfVisible()
		{
			SetIsActive(ShadowCamVisible);
		}

		public void SetIsActive(bool isActive)
		{
			if (blobShadowRenderer != null && isActive && !blobShadowRenderer.ShadowCasters.Contains(this))
			{
				blobShadowRenderer.ShadowCasters.Add(this);
			}
			GeoVisible = isActive;
		}

		internal void SetBlobShadowCam(BlobShadowRenderer blobShadowCam)
		{
			if (!(base.gameObject == null))
			{
				blobShadowRenderer = blobShadowCam;
				SetIsActive(blobShadowCam.IsShadowsVisible);
			}
		}

		private void Update()
		{
			if (!GeoVisible && blobShadowRenderer != null && !blobShadowRenderer.ShadowCasters.Contains(this))
			{
				blobShadowRenderer.ShadowCasters.Add(this);
				SetIsActiveIfVisible();
			}
		}

		private bool onEnableBlobShadows(BlobShadowEvents.EnableBlobShadows evt)
		{
			SetIsActive(true);
			return false;
		}

		private bool onDisableBlobShadows(BlobShadowEvents.DisableBlobShadows evt)
		{
			if (!evt.IncludeLocalPlayerShadow && CompareTag("Player"))
			{
				return false;
			}
			SetIsActive(false);
			return false;
		}
	}
}
