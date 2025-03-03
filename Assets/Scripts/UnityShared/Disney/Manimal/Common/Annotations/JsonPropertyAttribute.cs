using System;

namespace Disney.Manimal.Common.Annotations
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
	public class JsonPropertyAttribute : Attribute
	{
		public string Name
		{
			get;
			set;
		}

		public JsonPropertyAttribute(string name)
		{
			Name = name;
		}
	}
}
