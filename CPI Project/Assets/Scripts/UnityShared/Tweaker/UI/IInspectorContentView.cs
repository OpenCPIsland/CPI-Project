using System;

namespace Tweaker.UI
{
	public interface IInspectorContentView
	{
		event Action Destroyed;

		void DestroySelf();
	}
}
