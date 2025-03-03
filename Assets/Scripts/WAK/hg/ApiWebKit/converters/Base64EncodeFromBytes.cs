using System;
using System.Reflection;

namespace hg.ApiWebKit.converters
{
	public class Base64EncodeFromBytes : IValueConverter
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
				string result = System.Convert.ToBase64String((byte[])input, Base64FormattingOptions.None);
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
