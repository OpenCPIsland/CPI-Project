using ClubPenguin.Chat;
using ClubPenguin.Core;
using ClubPenguin.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[Serializable]
	[CreateAssetMenu(menuName = "Watcher/Chat")]
	public class EmoteTextAndExpressionWatcher : TaskWatcher
	{
		public enum ExpressionMatchType
		{
			ANY,
			ALL,
			ONLY
		}

		public enum ExpressionMatchCounting
		{
			ONE_PER_MESSAGE,
			MULTI_PER_MESSAGE
		}

		[Header("Match Behaviour")]
		public ExpressionMatchType MatchType;

		public ExpressionMatchCounting MatchCounting;

		[Header("Matching (case insensitive with word boundary matching)")]
		public EmoteDefinition[] Emotes;

		public string[] Texts;

		[Tooltip("Chat message is sent with any of these sizzle clips")]
		public SizzleClipDefinition[] SizzleClips;

		public override void OnActivate()
		{
			base.OnActivate();
			base.dispatcher.AddListener<ChatMessageSender.SendChatMessage>(onSendChatMessage);
		}

		public override void OnDeactivate()
		{
			base.OnDeactivate();
			base.dispatcher.RemoveListener<ChatMessageSender.SendChatMessage>(onSendChatMessage);
		}

		private bool onSendChatMessage(ChatMessageSender.SendChatMessage evt)
		{
			int num = 0;
			switch (MatchType)
			{
			case ExpressionMatchType.ANY:
			{
				string text = trimWhitSpaceAndEmotes(evt.Message);
				for (int i = 0; i < Emotes.Length; i++)
				{
					num += evt.Message.Split(EmoteManager.GetEmoteChar(Emotes[i])).Length - 1;
				}
				for (int i = 0; i < Texts.Length; i++)
				{
					string text2 = trimWhitSpaceAndEmotes(Texts[i]);
					num += text.Split(new string[1]
					{
						text2
					}, StringSplitOptions.None).Length - 1;
				}
				if (!(evt.SizzleClip != null))
				{
					break;
				}
				for (int i = 0; i < SizzleClips.Length; i++)
				{
					if (evt.SizzleClip.Id == SizzleClips[i].Id)
					{
						num++;
						break;
					}
				}
				break;
			}
			case ExpressionMatchType.ALL:
			{
				string text = trimWhitSpaceAndEmotes(evt.Message);
				if (Emotes.Length + Texts.Length > 0)
				{
					num = evt.Message.Length;
				}
				else if (SizzleClips.Length > 0)
				{
					num = 1;
				}
				for (int i = 0; i < Emotes.Length; i++)
				{
					if (num <= 0)
					{
						break;
					}
					num = Math.Min(num, evt.Message.Split(EmoteManager.GetEmoteChar(Emotes[i])).Length - 1);
				}
				for (int i = 0; i < Texts.Length; i++)
				{
					if (num <= 0)
					{
						break;
					}
					string text2 = trimWhitSpaceAndEmotes(Texts[i]);
					num = Math.Min(num, text.Split(new string[1]
					{
						text2
					}, StringSplitOptions.None).Length - 1);
				}
				if (!(evt.SizzleClip != null))
				{
					break;
				}
				for (int i = 0; i < SizzleClips.Length; i++)
				{
					if (num <= 0)
					{
						break;
					}
					if (evt.SizzleClip.Id != SizzleClips[i].Id)
					{
						num = 0;
					}
				}
				break;
			}
			case ExpressionMatchType.ONLY:
				if (Texts.Length + Emotes.Length > 1 || SizzleClips.Length > 1 || Texts.Length + Emotes.Length + SizzleClips.Length == 0)
				{
				}
				if (Emotes.Length == 1)
				{
					if (evt.Message.Trim() == EmoteManager.GetEmoteString(Emotes[0]))
					{
						num = 1;
					}
				}
				else if (Texts.Length == 1)
				{
					string text = trimWhitSpace(evt.Message);
					if (text.Equals(trimWhitSpace(Texts[0])))
					{
						num = 1;
					}
				}
				if (SizzleClips.Length == 1)
				{
					if (Texts.Length + Emotes.Length == 0)
					{
						num = 1;
					}
					if (num == 1)
					{
						num = ((evt.SizzleClip != null && SizzleClips[0].Id == evt.SizzleClip.Id) ? 1 : 0);
					}
				}
				break;
			}
			if (num == 0 && Texts.Length == 0 && Emotes.Length == 0 && SizzleClips.Length == 0)
			{
				num = 1;
			}
			if (num > 0)
			{
				if (MatchCounting == ExpressionMatchCounting.ONE_PER_MESSAGE)
				{
					num = 1;
				}
				for (int i = 0; i < num; i++)
				{
					taskIncrement();
				}
			}
			return false;
		}

		private string trimWhitSpaceAndEmotes(string str)
		{
			StringBuilder stringBuilder = new StringBuilder(" ");
			bool flag = true;
			int length = str.Length;
			char[] array = str.ToLower().ToCharArray();
			for (int i = 0; i < length; i++)
			{
				if (EmoteManager.IsEmoteCharacter(array[i]))
				{
					continue;
				}
				if (char.IsWhiteSpace(array[i]))
				{
					if (!flag)
					{
						stringBuilder.Append(" ");
						flag = true;
					}
				}
				else
				{
					stringBuilder.Append(array[i]);
					flag = false;
				}
			}
			if (!flag)
			{
				stringBuilder.Append(" ");
			}
			return stringBuilder.ToString();
		}

		private string trimWhitSpace(string str)
		{
			StringBuilder stringBuilder = new StringBuilder(" ");
			bool flag = true;
			int length = str.Length;
			char[] array = str.ToLower().ToCharArray();
			for (int i = 0; i < length; i++)
			{
				if (char.IsWhiteSpace(array[i]))
				{
					if (!flag)
					{
						stringBuilder.Append(" ");
						flag = true;
					}
				}
				else
				{
					stringBuilder.Append(array[i]);
					flag = false;
				}
			}
			if (!flag)
			{
				stringBuilder.Append(" ");
			}
			return stringBuilder.ToString();
		}

		public override object GetExportParameters()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("matchType", MatchType);
			dictionary.Add("matchCounting", MatchCounting);
			if (Texts != null && Texts.Length > 0)
			{
				dictionary.Add("texts", Texts);
			}
			if (Emotes != null && Emotes.Length > 0)
			{
				List<int> list = new List<int>();
				EmoteDefinition[] emotes = Emotes;
				foreach (EmoteDefinition emoteDefinition in emotes)
				{
					list.Add(emoteDefinition.CharacterCode);
				}
				dictionary.Add("emotes", list);
			}
			if (SizzleClips != null && SizzleClips.Length > 0)
			{
				List<int> list2 = new List<int>();
				SizzleClipDefinition[] sizzleClips = SizzleClips;
				foreach (SizzleClipDefinition sizzleClipDefinition in sizzleClips)
				{
					list2.Add(sizzleClipDefinition.Id);
				}
				dictionary.Add("emotion", list2);
			}
			return dictionary;
		}

		public override string GetWatcherType()
		{
			return "chat";
		}
	}
}
