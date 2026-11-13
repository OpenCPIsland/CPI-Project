using System;

namespace Disney.Manimal.Common.Annotations
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	public class ClientProxyAttribute : Attribute
	{
	}
}
