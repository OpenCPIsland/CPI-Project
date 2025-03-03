namespace Disney.Mix.SDK.Internal
{
	public interface ISessionFactory
	{
		IInternalSession Create(string swid);
	}
}
