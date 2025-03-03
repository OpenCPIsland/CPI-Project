using DI.JSON;
using System.Collections.Generic;

namespace DI.Storage
{
	public class JSONDocument : IJSONDocument, IDocument
	{
		private IDocument inner;

		private IJSONParser parser;

		public JSONDocument(IDocument inner, IJSONParser parser)
		{
			if (parser == null)
			{
				throw new StorageException("A JSON parser is required when creating a JSONDocument.");
			}
			this.inner = inner;
			this.parser = parser;
		}

		public string getReference()
		{
			return (inner == null) ? null : inner.getReference();
		}

		public string getName()
		{
			return (inner == null) ? null : inner.getName();
		}

		public string getContents()
		{
			return (inner == null) ? null : inner.getContents();
		}

		public byte[] getData()
		{
			return (inner == null) ? null : inner.getData();
		}

		public IDictionary<string, object> getDocument()
		{
			if (inner != null && parser.Parse(inner.getContents()))
			{
				return parser.AsDictionary();
			}
			return null;
		}
	}
}
