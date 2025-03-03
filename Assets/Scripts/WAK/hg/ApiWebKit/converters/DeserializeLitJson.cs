using hg.LitJson;
using System;
using System.Reflection;

namespace hg.ApiWebKit.converters
{
	public class DeserializeLitJson : IValueConverter
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
				object result = JsonMapper.ToObject((string)input, targetField.FieldType);
				successful = true;
				return result;
			}
			catch (Exception ex)
			{
				Configuration.Log("(DeserializeLitJson)(Convert) Failure on field '" + targetField.Name + "' : " + ex.Message, LogSeverity.ERROR);
				if (ex.InnerException != null)
				{
					Configuration.Log("(DeserializeLitJson)(Convert) Failure-Inner : " + ex.InnerException.Message, LogSeverity.ERROR);
				}
				return null;
			}
		}
	}
}
