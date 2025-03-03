using System;

namespace LitJson
{
	[Flags]
	public enum JsonIgnoreWhen
	{
		Never = 0x0,
		Serializing = 0x1,
		Deserializing = 0x2
	}
}
