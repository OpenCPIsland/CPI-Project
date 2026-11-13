using System.Collections.Generic;

namespace DI.Storage
{
	public interface IJSONDocument : IDocument
	{
		IDictionary<string, object> getDocument();
	}
}
