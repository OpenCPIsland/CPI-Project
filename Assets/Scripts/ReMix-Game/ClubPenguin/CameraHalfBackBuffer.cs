using ClubPenguin.Configuration;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(Camera))]
	[DisallowMultipleComponent]
	public class CameraHalfBackBuffer : MonoBehaviour
	{
		private new Camera camera;

		private RenderTexture scaledRT = null;

		public void Awake()
		{
			camera = GetComponent<Camera>();
			base.enabled = Service.Get<ConditionalConfiguration>().Get("Camera.HalfBackBuffer.property", false);
		}

		public void OnEnable()
		{
			if (scaledRT == null)
			{
				scaledRT = new RenderTexture(Screen.width / 2, Screen.height / 2, 24);
				scaledRT.filterMode = FilterMode.Bilinear;
			}
		}

		public void OnDisable()
		{
			if (scaledRT != null)
			{
				scaledRT.Release();
				scaledRT = null;
			}
		}

		public void OnPreRender()
		{
			if (scaledRT != null)
			{
				camera.targetTexture = scaledRT;
			}
		}

		public void OnPostRender()
		{
			if (scaledRT != null)
			{
				camera.targetTexture = null;
				Graphics.Blit((Texture)scaledRT, (RenderTexture)null);
			}
		}
	}
}
