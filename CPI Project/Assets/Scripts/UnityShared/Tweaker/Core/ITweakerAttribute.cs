using System;

namespace Tweaker.Core
{
	public interface ITweakerAttribute
	{
		string Name
		{
			get;
		}

		Guid Guid
		{
			get;
		}
	}
}
