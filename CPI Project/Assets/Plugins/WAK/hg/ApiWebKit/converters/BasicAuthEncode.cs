using System;
using System.Reflection;
using System.Text;

namespace hg.ApiWebKit.converters
{
	public class BasicAuthEncode : IValueConverter
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
				string result = "Basic " + System.Convert.ToBase64String(Encoding.UTF8.GetBytes((string)input), Base64FormattingOptions.None);
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
