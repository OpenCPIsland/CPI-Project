using System.Collections.Generic;

namespace DI.JSON
{
	public interface IJSONParser
	{
		bool Parse(string document);

		void SetParsed(object parsed);

		object GetParsed();

		IDictionary<string, object> AsDictionary();

		string GetDescription();
	}
}
