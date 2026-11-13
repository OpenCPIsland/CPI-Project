using ClubPenguin.Core;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace ClubPenguin.PartyGames
{
	[Serializable]
	[JsonConverter(typeof(PartyGameLobbyDefinitionJsonConverter))]
	public abstract class PartyGameLobbyDefinition : ScriptableObject, ICustomExportScriptableObject
	{
		public class PartyGameLobbyDefinitionJsonConverter : JsonConverter
		{
			public override bool CanConvert(Type objectType)
			{
				return typeof(PartyGameLobbyDefinition).IsAssignableFrom(objectType);
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
