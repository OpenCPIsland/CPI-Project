using System.Reflection;
using System.Text;

namespace hg.ApiWebKit.converters
{
	public class StringToBytes : IValueConverter
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
				byte[] bytes = Encoding.UTF8.GetBytes((string)input);
				successful = true;
				return bytes;
			}
			catch
			{
				return null;
			}
		}
	}
}
