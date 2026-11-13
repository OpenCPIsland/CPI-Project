using System;

namespace Disney.Manimal.Common.Annotations
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	public class JsonClassAttribute : Attribute
	{
		public string Name
		{
			get;
			private set;
		}

		public JsonClassAttribute(string name)
		{
			Name = name;
		}
	}
}
