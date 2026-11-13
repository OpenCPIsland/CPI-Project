using ClubPenguin.Core;
using UnityEngine;
using UnityEngine.Rendering;

namespace ClubPenguin.BlobShadows
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Camera))]
	public class BlobShadowCameraController : MonoBehaviour
	{
		private Camera targetCamera;

		private void OnEnable()
		{
			targetCamera = GetComponent<Camera>();
			RenderPipelineManager.beginCameraRendering += onBeginCameraRendering;
		}

		private void OnDisable()
		{
			RenderPipelineManager.beginCameraRendering -= onBeginCameraRendering;
		}

		private void onBeginCameraRendering(ScriptableRenderContext context, Camera camera)
		{
			if (camera != targetCamera)
			{
				return;
			}
			BlobShadowRenderer blobShadowRenderer = SceneRefs.Get<BlobShadowRenderer>();
			if (blobShadowRenderer != null)
			{
				blobShadowRenderer.RenderBlobs(context);
			}
		}
	}
}
