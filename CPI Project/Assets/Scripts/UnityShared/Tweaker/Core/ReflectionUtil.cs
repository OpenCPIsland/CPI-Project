using System.Reflection;

namespace Tweaker.Core
{
	public static class ReflectionUtil
	{
		public static BindingFlags GetBindingFlags(object instance)
		{
			BindingFlags bindingFlags = BindingFlags.Public;
			if (instance == null)
			{
				return bindingFlags | BindingFlags.Static;
			}
			return bindingFlags | BindingFlags.Instance;
		}
	}
}
