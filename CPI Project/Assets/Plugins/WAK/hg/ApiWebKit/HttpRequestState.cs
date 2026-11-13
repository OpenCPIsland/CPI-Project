namespace hg.ApiWebKit
{
	public enum HttpRequestState
	{
		NONE,
		IDLE,
		BUSY,
		STARTED,
		COMPLETED,
		TIMEOUT,
		ERROR,
		CANCELLED,
		DISCONNECTED
	}
}
