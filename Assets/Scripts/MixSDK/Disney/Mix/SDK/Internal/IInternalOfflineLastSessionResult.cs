namespace Disney.Mix.SDK.Internal
{
	public interface IInternalOfflineLastSessionResult : IOfflineLastSessionResult
	{
		IInternalSession InternalSession
		{
			get;
		}
	}
}
