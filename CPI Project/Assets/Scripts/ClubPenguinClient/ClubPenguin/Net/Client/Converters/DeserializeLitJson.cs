using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using hg.ApiWebKit;
using System;
using System.Globalization;
using System.Reflection;
using System.Threading;

namespace ClubPenguin.Net.Client.Converters
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
				object result;
				if (input is string)
				{
					CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
					Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
					result = Service.Get<JsonService>().Deserialize((string)input, targetField.FieldType);
					Thread.CurrentThread.CurrentCulture = currentCulture;
				}
				else
				{
					result = input;
				}
				successful = true;
				return result;
			}
			catch (Exception ex)
			{
				Configuration.Log("(DeserializeLitJson)(Convert) Failure on field '" + targetField.Name + "' : " + ex.Message, LogSeverity.ERROR);
				if (ex.InnerException != null)
				{
					Configuration.Log("(DeserializeLitJson)(Convert) Failure-Inner : " + ex.InnerException.Message, LogSeverity.ERROR);
					if (ex.InnerException.InnerException != null)
					{
						Configuration.Log("(DeserializeLitJson)(Convert) Failure-Inner-Inner : " + ex.InnerException.InnerException.Message, LogSeverity.ERROR);
					}
				}
				return null;
			}
		}
	}
}
