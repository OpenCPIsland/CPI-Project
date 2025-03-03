using System.Collections;
using System.Collections.Generic;

namespace Disney.LaunchPadFramework
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
				Log.LogFatal(obj, "Attempting to access a dictionary using a context does not contain a dictionary is not a dictionary");
			}
			return result;
		}
	}
}
