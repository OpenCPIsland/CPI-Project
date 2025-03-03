namespace DI.HTTP
{
	public interface IHTTPClient
	{
		IHTTPFactory getFactory();

		IHTTPRequest getRequest();

		void setListener(IHTTPListener listener);

		IHTTPListener getListener();
	}
}
