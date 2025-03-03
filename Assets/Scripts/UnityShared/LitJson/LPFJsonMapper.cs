using System.Collections;
using System.Collections.Generic;

namespace LitJson
{
	public class LPFJsonMapper : JsonMapper
	{
		public static object ToObjectSimple(string json)
		{
			JsonReader reader = new JsonReader(json);
			return ReadValueSimple(reader);
		}

		private static object ReadValueSimple(JsonReader reader)
		{
			reader.Read();
			if (reader.Token == JsonToken.ArrayEnd || reader.Token == JsonToken.Null)
			{
				return null;
			}
			if (reader.Token == JsonToken.Double || reader.Token == JsonToken.Int || reader.Token == JsonToken.Long || reader.Token == JsonToken.String || reader.Token == JsonToken.Boolean)
			{
				return reader.Value;
			}
			if (reader.Token == JsonToken.ArrayStart)
			{
				IList list = new ArrayList();
				while (true)
				{
					bool flag = true;
					object obj = ReadValueSimple(reader);
					if (obj == null && reader.Token == JsonToken.ArrayEnd)
					{
						break;
					}
					list.Add(obj);
				}
				return list;
			}
			if (reader.Token == JsonToken.ObjectStart)
			{
				IDictionary<string, object> dictionary = new Dictionary<string, object>();
				while (true)
				{
					bool flag = true;
					reader.Read();
					if (reader.Token == JsonToken.ObjectEnd)
					{
						break;
					}
					string key = (string)reader.Value;
					dictionary[key] = ReadValueSimple(reader);
				}
				return dictionary;
			}
			throw new JsonException(string.Format("Unknown JSON type while parsing. Not a double, int, long, string, boolean, array, or object"));
		}
	}
}
