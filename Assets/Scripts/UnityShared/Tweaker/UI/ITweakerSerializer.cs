using System;

namespace Tweaker.UI
{
	public interface ITweakerSerializer
	{
		string Serialize(object objectToSerialize);

		object Deserialize(string stringToDeserialize, Type objectType);

		T Deserialize<T>(string stringToDeserialize);
	}
}
