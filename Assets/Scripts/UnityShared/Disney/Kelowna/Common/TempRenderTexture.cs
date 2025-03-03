using System;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public sealed class TempRenderTexture : IDisposable
	{
		public RenderTexture Texture
		{
			get;
			private set;
		}

		public static implicit operator RenderTexture(TempRenderTexture trt)
		{
			return trt.Texture;
		}

		public void CopyToTexture(Texture2D destTexture)
		{
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = Texture;
			destTexture.ReadPixels(new Rect(0f, 0f, Texture.width, Texture.height), 0, 0);
			destTexture.Apply();
			RenderTexture.active = active;
		}

		public TempRenderTexture(int width, int height, RenderTextureFormat format = RenderTextureFormat.Default, int depthBuffer = 0, RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default, int antiAliasing = 1)
		{
			Texture = RenderTexture.GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing);
		}

		public void Dispose()
		{
			dispose(true);
			GC.SuppressFinalize(this);
		}

		~TempRenderTexture()
		{
			dispose(false);
		}

		private void dispose(bool disposing)
		{
			if (disposing)
			{
			}
			RenderTexture.ReleaseTemporary(Texture);
			Texture = null;
		}
	}
}
