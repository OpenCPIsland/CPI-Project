using System.Reflection;
using UnityEngine;

namespace hg.ApiWebKit.converters
{
	public class SerializeTexture2DToJpg : IValueConverter
	{
		public object Convert(object input, FieldInfo targetField, out bool successful, params object[] parameters)
		{
			successful = false;
			if (input == null)
			{
				return null;
			}
			try
			{
				byte[] result = ((Texture2D)input).EncodeToJPG();
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
