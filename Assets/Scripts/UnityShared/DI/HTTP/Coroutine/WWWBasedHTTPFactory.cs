using UnityEngine;

namespace DI.HTTP.Coroutine
{
	public class WWWBasedHTTPFactory : IHTTPFactory
	{
		private static WWWBasedHTTPFactory factory = null;

		private static MonoBehaviour _context = null;

		private MonoBehaviour context = null;

		private WWWBasedHTTPFactory(MonoBehaviour context)
		{
			this.context = context;
		}

		public static void setContext(MonoBehaviour context)
		{
			_context = context;
		}

		public static WWWBasedHTTPFactory getFactory()
		{
			if (factory == null)
			{
				if (_context == null)
				{
					throw new HTTPException("Context (MonoBehaviour) must be set prior to requesting a WWW based factory.");
				}
				factory = new WWWBasedHTTPFactory(_context);
			}
			return factory;
		}

		public IHTTPClient getClient()
		{
			return new WWWBasedHTTPClient(this);
		}

		public MonoBehaviour getContext()
		{
			return context;
		}
	}
}
