using ClubPenguin.Core.StaticGameData;
using System;
using System.Collections.Generic;

namespace ClubPenguin.UI
{
	[Serializable]
	public class GlobalChatPhrasesDefinition : StaticGameDataDefinition
	{
		public List<ChatPhraseDefinition> ChatPhraseDefinitions = new List<ChatPhraseDefinition>();
	}
}
