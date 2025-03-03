using Disney.Manimal.Async.Unity;

namespace Disney.Manimal.Http
{
	public class HttpRequestFactory
	{
		public static IHttpRequest CreateHttpRequest(UnitySynchronizationContext defaultSyncContext)
		{
			return new HttpRequest();
		}
	}
}
