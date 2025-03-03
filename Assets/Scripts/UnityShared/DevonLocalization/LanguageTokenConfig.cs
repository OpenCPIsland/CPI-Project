using DevonLocalization.Core;
using System;
using UnityEngine;

namespace DevonLocalization
{
	public class LanguageTokenConfig : ScriptableObject
	{
		[Serializable]
		public struct LanguageToken
		{
			public Language Language;

			public string Token;
		}

		public LanguageToken[] LanguageTokens;
	}
}
