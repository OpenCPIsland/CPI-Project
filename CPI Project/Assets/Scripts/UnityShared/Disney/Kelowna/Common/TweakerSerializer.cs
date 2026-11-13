using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using Tweaker.AssemblyScanner;
using Tweaker.UI;

namespace Disney.Kelowna.Common
{
	public class TweakerSerializer : ITweakerSerializer
	{
		private Dictionary<Type, CustomTypeSerializer> customSerializers;

		public TweakerSerializer(IScanner scanner)
		{
			customSerializers = new Dictionary<Type, CustomTypeSerializer>();
			CustomSerializerProcessor processor = new CustomSerializerProcessor();
			scanner.AddProcessor(processor);
			IScanResultProvider<CustomSerializerResult> resultProvider = scanner.GetResultProvider<CustomSerializerResult>();
			resultProvider.ResultProvided += CustomSerializerFound;
		}

		private void CustomSerializerFound(object sender, ScanResultArgs<CustomSerializerResult> e)
		{
			CustomTypeSerializer customTypeSerializer = Activator.CreateInstance(e.result.type, this) as CustomTypeSerializer;
			customSerializers.Add(customTypeSerializer.CustomType, customTypeSerializer);
		}

		public string Serialize(object objectToSerialize)
		{
			CustomTypeSerializer value;
			customSerializers.TryGetValue(objectToSerialize.GetType(), out value);
			if (value != null)
			{
				return value.Serialize(objectToSerialize);
			}
			return Service.Get<JsonService>().Serialize(objectToSerialize);
		}

		public object Deserialize(string stringToDeserialize, Type objectType)
		{
			CustomTypeSerializer value;
			customSerializers.TryGetValue(objectType, out value);
			if (value != null)
			{
				return value.Deserialize(stringToDeserialize);
			}
			return Service.Get<JsonService>().Deserialize(stringToDeserialize, objectType);
		}

		public T Deserialize<T>(string stringToDeserialize)
		{
			return (T)Deserialize(stringToDeserialize, typeof(T));
		}
	}
}
