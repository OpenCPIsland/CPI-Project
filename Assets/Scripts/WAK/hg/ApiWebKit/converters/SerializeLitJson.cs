using hg.LitJson;
using System;
using System.Reflection;

namespace hg.ApiWebKit.converters
{
	public class SerializeLitJson : IValueConverter
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
				string result = JsonMapper.ToJson(input);
				successful = true;
				return result;
			}
			catch (Exception ex)
			{
				Configuration.Log("(SerializeLitJson)(Convert) Failure on field '" + targetField.Name + "' : " + ex.Message, LogSeverity.ERROR);
				if (ex.InnerException != null)
				{
					Configuration.Log("(SerializeLitJson)(Convert) Failure-Inner : " + ex.InnerException.Message, LogSeverity.ERROR);
				}
				return null;
			}
		}
	}
}
