using System;
using System.Collections.Generic;

namespace SwrveUnity.ResourceManager
{
	public class SwrveResource
	{
		public readonly Dictionary<string, string> Attributes;

		public SwrveResource(Dictionary<string, string> attributes)
		{
			Attributes = attributes;
		}

		public T GetAttribute<T>(string attributeName, T defaultValue)
		{
			if (Attributes.ContainsKey(attributeName))
			{
				string text = Attributes[attributeName];
				if (text != null)
				{
					return (T)Convert.ChangeType(text, typeof(T));
				}
			}
			return defaultValue;
		}
	}
}
