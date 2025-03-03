using System.Collections;
using System.Collections.Generic;

namespace DisneyMobile.CoreUnitySystems
{
	public static class ObjectExtension
	{
		public static IDictionary<string, object> AsDic(this object obj)
		{
			IDictionary<string, object> result = null;
			if (obj is IDictionary)
			{
				result = (obj as IDictionary<string, object>);
			}
			else
			{
				Logger.LogFatal(obj, "Attempting to access a dictionary using a context does not contain a dictionary is not a dictionary", Logger.TagFlags.CORE | Logger.TagFlags.INIT);
			}
			return result;
		}
	}
}
