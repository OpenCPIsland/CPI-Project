using System;

namespace hg.LitJson
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class JsonIgnoreAttribute : Attribute
	{
	}
}
