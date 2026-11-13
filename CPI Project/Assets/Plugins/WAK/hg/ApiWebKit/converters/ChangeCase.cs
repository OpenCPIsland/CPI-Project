using System;
using System.Reflection;

namespace hg.ApiWebKit.converters
{
	public abstract class ChangeCase : IValueConverter
	{
		public virtual object Convert(object input, FieldInfo targetField, out bool successful, params object[] parameters)
		{
			successful = false;
			if (input == null)
			{
				return null;
			}
			input = System.Convert.ChangeType(input, typeof(string));
			if (parameters == null || parameters[0].ToString().ToLower() == "lower")
			{
				try
				{
					string result = ((string)input).ToLower();
					successful = true;
					return result;
				}
				catch
				{
					return null;
				}
			}
			if (parameters[0].ToString().ToLower() == "upper")
			{
				try
				{
					string result2 = ((string)input).ToUpper();
					successful = true;
					return result2;
				}
				catch
				{
					return null;
				}
			}
			return input;
		}
	}
}
