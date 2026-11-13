using System.Text;

namespace DI.Storage
{
	public class ByteDocument : IDocument
	{
		private string reference;

		private string name;

		private byte[] data;

		private Encoding encoding = Encoding.UTF8;

		public ByteDocument()
		{
		}

		public ByteDocument(string reference, string name, byte[] data)
		{
		}

		public void setReference(string reference)
		{
			this.reference = reference;
		}

		public void setName(string name)
		{
			this.name = name;
		}

		public void setContents(string contents)
		{
			data = ((contents == null) ? null : encoding.GetBytes(contents));
		}

		public void setEncoding(Encoding encoding)
		{
			this.encoding = encoding;
		}

		public void setData(byte[] data)
		{
			this.data = data;
		}

		public string getReference()
		{
			return reference;
		}

		public string getName()
		{
			return name;
		}

		public string getContents()
		{
			return (data == null) ? null : encoding.GetString(data);
		}

		public byte[] getData()
		{
			return data;
		}
	}
}
