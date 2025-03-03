using System;

namespace ZenFulcrum.EmbeddedBrowser
{
	public struct RejectHandler2
	{
		public Action<Exception> callback;

		public IRejectable2 rejectable;
	}
}
