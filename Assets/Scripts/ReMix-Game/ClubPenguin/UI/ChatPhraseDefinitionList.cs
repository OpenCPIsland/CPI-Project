using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	[Serializable]
	public class ChatPhraseDefinitionList : ScriptableObject
	{
		public List<ChatPhraseDefinition> ChatPhraseDefinitions = new List<ChatPhraseDefinition>();
	}
}
