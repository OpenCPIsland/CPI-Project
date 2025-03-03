using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Tweaker.AssemblyScanner;

namespace Tweaker.UI.Testbed
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
			StringWriter stringWriter = new StringWriter();
			JsonSerializer jsonSerializer = new JsonSerializer();
			jsonSerializer.Serialize(stringWriter, objectToSerialize);
			string result = stringWriter.GetStringBuilder().ToString();
			stringWriter.Close();
			return result;
		}

		public object Deserialize(string stringToDeserialize, Type objectType)
		{
			CustomTypeSerializer value;
			customSerializers.TryGetValue(objectType, out value);
			if (value != null)
			{
				return value.Deserialize(stringToDeserialize);
			}
			StringReader stringReader = new StringReader(stringToDeserialize);
			JsonSerializer jsonSerializer = new JsonSerializer();
			object result = jsonSerializer.Deserialize(stringReader, objectType);
			stringReader.Close();
			return result;
		}

		public T Deserialize<T>(string stringToDeserialize)
		{
			CustomTypeSerializer value;
			customSerializers.TryGetValue(typeof(T), out value);
			if (value != null)
			{
				return (T)value.Deserialize(stringToDeserialize);
			}
			StringReader stringReader = new StringReader(stringToDeserialize);
			JsonSerializer jsonSerializer = new JsonSerializer();
			T result = (T)jsonSerializer.Deserialize(stringReader, typeof(T));
			stringReader.Close();
			return result;
		}
	}
}
