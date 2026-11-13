using System;

namespace Disney.Manimal.Common.Annotations
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
	public class JsonIgnoreAttribute : Attribute
	{
	}
}
