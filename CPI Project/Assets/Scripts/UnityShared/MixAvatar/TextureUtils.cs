using System.Collections.Generic;
using UnityEngine;

namespace MixAvatar
{
	public class TextureUtils
	{
		private static int HELPER_TEX_DIMENSION = 8;

		private static Texture2D ALPHA0_TEX;

		private static Texture2D WHITE_TEX;

		public static Texture2D GetAlpha0Texture()
		{
			if (ALPHA0_TEX == null)
			{
				ALPHA0_TEX = new Texture2D(HELPER_TEX_DIMENSION, HELPER_TEX_DIMENSION, TextureFormat.ARGB32, false);
				ALPHA0_TEX.filterMode = FilterMode.Point;
				Color32[] array = new Color32[HELPER_TEX_DIMENSION * HELPER_TEX_DIMENSION];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = new Color32(0, 0, 0, 0);
				}
				ALPHA0_TEX.SetPixels32(array);
				ALPHA0_TEX.Apply(false, true);
				ALPHA0_TEX.name = "TexUtilsAlpha0Tex";
			}
			return ALPHA0_TEX;
		}

		public static Texture2D GetWhiteTexture()
		{
			if (WHITE_TEX == null)
			{
				WHITE_TEX = new Texture2D(HELPER_TEX_DIMENSION, HELPER_TEX_DIMENSION, TextureFormat.ARGB32, false);
				WHITE_TEX.filterMode = FilterMode.Point;
				Color32[] array = new Color32[HELPER_TEX_DIMENSION * HELPER_TEX_DIMENSION];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
				}
				WHITE_TEX.SetPixels32(array);
				WHITE_TEX.Apply(false, true);
				WHITE_TEX.name = "TexUtilsWhiteTex";
			}
			return WHITE_TEX;
		}

		public static Rect GetLargestTextureDimensions(List<Texture2D> textures)
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < textures.Count; i++)
			{
				if (textures[i].width > num)
				{
					num = textures[i].width;
				}
				if (textures[i].height > num2)
				{
					num2 = textures[i].height;
				}
			}
			return new Rect(0f, 0f, num, num2);
		}
	}
}
