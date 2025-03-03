namespace DI.HTTP
{
	public class HTTPListenerHelper
	{
		private IHTTPListener listener = null;

		public void setListener(IHTTPListener listener)
		{
			this.listener = listener;
		}

		public IHTTPListener getListener()
		{
			return listener;
		}
	}
}
