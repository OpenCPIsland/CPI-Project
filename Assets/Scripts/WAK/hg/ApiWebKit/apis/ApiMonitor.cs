using hg.ApiWebKit.core.http;

namespace hg.ApiWebKit.apis
{
	public class ApiMonitor : Singleton<ApiMonitor>
	{
		public float BytesSent;

		public float BytesReceived;

		public float SucceededCalls;

		public float FailedCalls;

		public float FaultedCalls;

		public float NetworkTime;

		public void Aggregate(HttpResponse response)
		{
			if (response == null)
			{
				FailedCalls += 1f;
				return;
			}
			BytesSent += response.Request.RequestModelResult.Data.Length;
			BytesReceived += ((response.Data != null) ? response.Data.Length : 0);
			SucceededCalls += ((!response.HasError) ? 1 : 0);
			FailedCalls += (response.HasError ? 1 : 0);
			FaultedCalls += (response.Request.RequestModelResult.Operation.IsFaulted ? 1 : 0);
			NetworkTime += response.TimeToComplete;
		}
	}
}
