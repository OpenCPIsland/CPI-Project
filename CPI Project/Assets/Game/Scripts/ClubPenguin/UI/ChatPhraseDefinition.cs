using ClubPenguin.Core.StaticGameData;
using System;
using UnityEngine;

namespace ClubPenguin.UI
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/Chat/Phrase")]
	public class ChatPhraseDefinition : StaticGameDataDefinition
	{
		[StaticGameDataDefinitionId]
		public string Token;

		public SizzleClipDefinitionKey SizzleClipKey;
	}
}
