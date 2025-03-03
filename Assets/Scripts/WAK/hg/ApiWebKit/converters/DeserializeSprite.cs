using System.Reflection;
using UnityEngine;

namespace hg.ApiWebKit.converters
{
	public class DeserializeSprite : DeserializeTexture2D
	{
		public override object Convert(object input, FieldInfo targetField, out bool successful, params object[] parameters)
		{
			successful = false;
			if (input == null)
			{
				return null;
			}
			try
			{
				Texture2D texture2D = (Texture2D)base.Convert(input, targetField, out successful, parameters);
				Sprite result = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
				successful = true;
				return result;
			}
			catch
			{
				return null;
			}
		}
	}
}
