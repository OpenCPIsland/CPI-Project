using System;
using System.Reflection;

namespace hg.ApiWebKit.converters
{
	public class DeserializeAssembly : IValueConverter
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
				Assembly result = Assembly.Load((byte[])input);
				successful = true;
				return result;
			}
			catch (Exception ex)
			{
				Configuration.Log("(DeserializeAssembly)(Convert) Failure : " + ex.Message, LogSeverity.ERROR);
				return null;
			}
		}
	}
}
