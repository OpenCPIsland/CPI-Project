using System;

namespace hg.LitJson
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class JsonPropertyAttribute : Attribute
	{
		public string Name;

		public JsonPropertyAttribute(string name)
		{
			Name = name;
		}
	}
}
