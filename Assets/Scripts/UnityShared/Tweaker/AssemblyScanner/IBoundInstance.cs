using System;

namespace Tweaker.AssemblyScanner
{
	public interface IBoundInstance
	{
		object Instance
		{
			get;
		}

		uint UniqueId
		{
			get;
		}

		Type Type
		{
			get;
		}
	}
}
