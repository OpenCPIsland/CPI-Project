using System;
using System.Reflection;

namespace hg.ApiWebKit.converters
{
	public class Base64DecodeToBytes : IValueConverter
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
				byte[] result = System.Convert.FromBase64String((string)input);
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
