using System;

namespace Disney.Kelowna.Common
{
	public abstract class JsonService
	{
		public abstract string Serialize<T>(T objectToSerialize);

		public abstract T Deserialize<T>(string stringToDeserialize);

		public abstract object Deserialize(string stringToDeserialize, Type type);
	}
}
