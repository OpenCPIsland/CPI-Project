using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JsonDotNet.Extras.CustomConverters
{
	public class Vector2Converter : JsonConverter
	{
		public override bool CanRead
		{
			get
			{
				return false;
			}
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			JToken jToken = JToken.FromObject(value);
			if (jToken.Type != JTokenType.Object)
			{
				jToken.WriteTo(writer);
				return;
			}
			JObject jObject = (JObject)jToken;
			IList<string> content = (from p in jObject.Properties()
				where p.Name == "x" || p.Name == "y"
				select p.Name).ToList();
			jObject.AddFirst(new JProperty("Keys", new JArray(content)));
			jObject.WriteTo(writer);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Vector2);
		}
	}
}
