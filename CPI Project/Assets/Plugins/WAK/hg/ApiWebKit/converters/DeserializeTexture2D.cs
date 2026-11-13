using System.Reflection;
using UnityEngine;

namespace hg.ApiWebKit.converters
{
	public class DeserializeTexture2D : IValueConverter
	{
		public virtual object Convert(object input, FieldInfo targetField, out bool successful, params object[] parameters)
		{
			successful = false;
			if (input == null)
			{
				return null;
			}
			try
			{
				Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
				texture2D.LoadImage((byte[])input);
				successful = true;
				return texture2D;
			}
			catch
			{
				return null;
			}
		}
	}
}
