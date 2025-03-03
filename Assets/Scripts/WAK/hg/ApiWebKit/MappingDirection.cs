using System;

namespace hg.ApiWebKit
{
	[Flags]
	public enum MappingDirection
	{
		NONE = 0x1,
		REQUEST = 0x2,
		RESPONSE = 0x4,
		ALL = 0x8
	}
}
