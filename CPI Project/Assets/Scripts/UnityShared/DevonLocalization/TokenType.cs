using Disney.Kelowna.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DevonLocalization
{
	public class TokenType
	{
		public static string TOKEN_TYPE_ASSET = "TokenTypes";

		public IDictionary<string, string[]> tokenTypes;

		public static int MAX_CHARACTER_COUNT = 200;

		public static int MIN_CHARACTER_COUNT = 10;

		public static string EMPTY_SUBTYPE = "None";

		public JsonService jsonService;

		private void initializeTokenTypes()
		{
			jsonService = new LitJsonService();
			TextAsset textAsset = Resources.Load<TextAsset>(TOKEN_TYPE_ASSET);
			if (!(textAsset == null) && !string.IsNullOrEmpty(textAsset.text))
			{
				string text = textAsset.text;
				tokenTypes = jsonService.Deserialize<Dictionary<string, string[]>>(text);
				if (tokenTypes.Count > 0)
				{
				}
			}
		}

		public string[] GetTokenTypes()
		{
			if (tokenTypes == null)
			{
				initializeTokenTypes();
			}
			return tokenTypes.Keys.ToArray();
		}

		public string[] GetTokenSubTypes(string tokenType)
		{
			if (tokenTypes == null)
			{
				initializeTokenTypes();
			}
			string[] value;
			if (tokenTypes.TryGetValue(tokenType, out value))
			{
				return value;
			}
			return new string[0];
		}

		public bool validateTokenType(string tokenType)
		{
			if (tokenTypes == null)
			{
				initializeTokenTypes();
			}
			return tokenTypes.ContainsKey(tokenType);
		}

		public bool validateSubTokenType(string tokenType, string subTokenType)
		{
			if (tokenTypes == null)
			{
				initializeTokenTypes();
			}
			string[] value;
			if (tokenTypes.TryGetValue(tokenType, out value))
			{
				return Array.IndexOf(value, subTokenType) > -1;
			}
			return false;
		}
	}
}
