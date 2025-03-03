using ClubPenguin.Core;
using UnityEngine;

namespace ClubPenguin.BlobShadows
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Camera))]
	public class BlobShadowCameraController : MonoBehaviour
	{
		public void OnPreRender()
		{
			BlobShadowRenderer blobShadowRenderer = SceneRefs.Get<BlobShadowRenderer>();
			if (blobShadowRenderer != null)
			{
				blobShadowRenderer.RenderBlobs();
			}
		}
	}
}
