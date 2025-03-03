namespace DI.HTTP
{
	public class HTTPBaseDocumentImpl : IHTTPDocument
	{
		private byte[] data = null;

		public HTTPBaseDocumentImpl()
		{
		}

		public HTTPBaseDocumentImpl(byte[] data)
		{
			setData(data);
		}

		protected void setData(byte[] data)
		{
			this.data = data;
		}

		public byte[] getData()
		{
			return data;
		}
	}
}
