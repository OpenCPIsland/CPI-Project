using ClubPenguin.UI;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using System;
using System.Collections.Generic;

namespace ClubPenguin
{
	[Serializable]
	public class PhraseChatData : ScopedData
	{
		private Stack<ChatPhraseDefinitionList> contextualPhraseChats = new Stack<ChatPhraseDefinitionList>();

		protected override string scopeID
		{
			get
			{
				return CPDataScopes.Zone.ToString();
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(PhraseChatDataMonoBehaviour);
			}
		}

		public void Push(ChatPhraseDefinitionList definition)
		{
			contextualPhraseChats.Push(definition);
		}

		public void Pop(ChatPhraseDefinitionList definition)
		{
			if (contextualPhraseChats.Count > 0)
			{
				contextualPhraseChats.Pop();
			}
		}

		public ChatPhraseDefinitionList Peek()
		{
			if (contextualPhraseChats.Count == 0)
			{
				Log.LogError(this, "Context chat stack is empty, scene is missing contextual chat triggers.");
				return null;
			}
			return contextualPhraseChats.Peek();
		}

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
