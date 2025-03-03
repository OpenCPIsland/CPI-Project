using ClubPenguin.Core;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace ClubPenguin.PartyGames
{
	[Serializable]
	[JsonConverter(typeof(PartyGameDataDefinitionJsonConverter))]
	public abstract class PartyGameDataDefinition : ScriptableObject, ICustomExportScriptableObject
	{
		public class PartyGameDataDefinitionJsonConverter : JsonConverter
		{
			public override bool CanConvert(Type objectType)
			{
				return typeof(PartyGameDataDefinition).IsAssignableFrom(objectType);
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
			{
				throw new NotImplementedException();
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				serializer.Serialize(writer, JsonUtility.ToJson(value));
			}
		}
	}
}
