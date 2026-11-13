using System.Reflection;

namespace hg.ApiWebKit.converters
{
	public class ToLowerCase : ChangeCase
	{
		public override object Convert(object input, FieldInfo targetField, out bool successful, params object[] parameters)
		{
			return base.Convert(input, targetField, out successful, "lower");
		}
	}
}
