using System.Collections.Generic;

namespace Disney.Manimal.Common.Util
{
	public interface ITypeSerializer
	{
		string SerializeToString<T>(T value);

		T DeserializeFromString<T>(string value);

		IDictionary<string, object> ToDictionary(string value);

		T FromInternalRep<T>(object value);
	}
}
