using DevonLocalization.Core;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClubPenguin.Chat
{
	public static class EmoteManager
	{
		private static Dictionary<int, EmoteDefinition> characterCodesToDefinitions;

		public static bool IsEmoteCharacter(char character)
		{
			setUpDictionary();
			int key = Convert.ToInt32(character);
			return characterCodesToDefinitions != null && characterCodesToDefinitions.ContainsKey(key);
		}

		public static char[] GetAllEmoteCharacters()
		{
			setUpDictionary();
			char[] array = new char[characterCodesToDefinitions.Count];
			int num = 0;
			foreach (int key in characterCodesToDefinitions.Keys)
			{
				array[num++] = (char)key;
			}
			return array;
		}

		public static string GetToken(string emoteString)
		{
			char emoteChar = Convert.ToChar(emoteString);
			return GetToken(emoteChar);
		}

		public static string GetToken(char emoteChar)
		{
			EmoteDefinition emoteDefinition = GetEmoteDefinition(emoteChar);
			if (emoteDefinition != null)
			{
				return emoteDefinition.Token;
			}
			return null;
		}

		public static EmoteDefinition GetEmoteDefinition(char emoteChar)
		{
			setUpDictionary();
			int key = Convert.ToInt32(emoteChar);
			EmoteDefinition value;
			if (characterCodesToDefinitions.TryGetValue(key, out value))
			{
				return value;
			}
			return null;
		}

		public static string GetEmoteString(EmoteDefinition emoteDefinition)
		{
			return GetEmoteChar(emoteDefinition).ToString();
		}

		public static char GetEmoteChar(EmoteDefinition emoteDefinition)
		{
			return Convert.ToChar(emoteDefinition.CharacterCode);
		}

		public static string GetMessageWithLocalizedEmotes(string message)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c in message)
			{
				if (IsEmoteCharacter(c))
				{
					string token = GetToken(c);
					string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation(token);
					stringBuilder.Append(tokenTranslation + " ");
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString().Trim();
		}

		public static EmoteDefinition[] GetEmoteDefinitionsFromMessage(string message)
		{
			List<EmoteDefinition> list = new List<EmoteDefinition>();
			foreach (char c in message)
			{
				if (IsEmoteCharacter(c))
				{
					EmoteDefinition emoteDefinition = GetEmoteDefinition(c);
					if (emoteDefinition != null)
					{
						list.Add(emoteDefinition);
					}
				}
			}
			return list.ToArray();
		}

		private static void setUpDictionary()
		{
			if (characterCodesToDefinitions == null)
			{
				Dictionary<string, EmoteDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<string, EmoteDefinition>>();
				if (dictionary != null)
				{
					characterCodesToDefinitions = new Dictionary<int, EmoteDefinition>();
					foreach (EmoteDefinition value in dictionary.Values)
					{
						characterCodesToDefinitions.Add(value.CharacterCode, value);
					}
				}
			}
		}

		internal static EmoteDefinition GetEmoteFromCharacter(char characterCode)
		{
			if (characterCodesToDefinitions.ContainsKey(characterCode))
			{
				EmoteDefinition emoteDefinition = characterCodesToDefinitions[characterCode];
			}
			return characterCodesToDefinitions[characterCode];
		}
	}
}
