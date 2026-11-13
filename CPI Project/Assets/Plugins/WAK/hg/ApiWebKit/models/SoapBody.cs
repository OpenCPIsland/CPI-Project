namespace hg.ApiWebKit.models
{
	public class SoapBody<T> where T : SoapMessage
	{
		public T Message;

		public SoapBody()
		{
		}

		public SoapBody(T message)
		{
			Message = message;
		}
	}
}
