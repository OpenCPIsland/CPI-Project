using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class ScrollRectBackground : MonoBehaviour
	{
		public Camera ActiveCamera;

		public Image SourceImage;

		public void TakeScreenShot()
		{
			if (ActiveCamera == null)
			{
				ActiveCamera = Camera.main;
			}
			int num = (int)((float)Screen.width * ActiveCamera.rect.width);
			int num2 = (int)((float)Screen.height * ActiveCamera.rect.height);
			RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
			renderTexture.isPowerOfTwo = false;
			renderTexture.filterMode = FilterMode.Bilinear;
			renderTexture.useMipMap = false;
			renderTexture.Create();
			renderTexture.name = "ScrollRect Background Render Texture: " + base.name;
			ActiveCamera.targetTexture = renderTexture;
			ActiveCamera.Render();
			RenderTexture.active = renderTexture;
			Texture2D texture2D = new Texture2D(num, num2, TextureFormat.ARGB32, false);
			texture2D.filterMode = FilterMode.Bilinear;
			texture2D.ReadPixels(new Rect(0f, Screen.height - num2, num, num2), 0, 0);
			texture2D.Apply();
			RenderTexture.active = null;
			Color c = new Color(0.8f, 0.9f, 1f, 0.9f);
			Color32[] pixels = texture2D.GetPixels32(0);
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i] = Color32.Lerp(pixels[i], c, 0.66f);
			}
			texture2D.SetPixels32(pixels, 0);
			texture2D.Apply(false);
			RenderTexture.active = null;
			Object.Destroy(renderTexture);
			Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, num, num2), default(Vector2));
			sprite.name = string.Format("bgSprite-{0}", Time.frameCount.ToString());
			SourceImage.sprite = sprite;
		}
	}
}
