using System.Reflection;

namespace hg.ApiWebKit
{
	public interface IValueConverter
	{
		object Convert(object input, FieldInfo targetField, out bool successful, params object[] parameters);
	}
}
