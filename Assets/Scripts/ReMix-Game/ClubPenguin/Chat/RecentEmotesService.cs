using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Tweaker.Core;

namespace ClubPenguin.Chat
{
	public class RecentEmotesService
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct RecentEmotesUpdated
		{
		}

		private const string KEY = "RecentEmotesServiceList";

		public int RecentEmotesMaxCount;

		private List<EmoteDefinition> emoteDefinitionsList;

		private EventDispatcher eventDispatcher;

		public RecentEmotesService()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			eventDispatcher.AddListener<NetworkControllerEvents.LocalPlayerDataReadyEvent>(onLocalPlayerDataReadyEvent);
			eventDispatcher.AddListener<ChatMessageSender.SendChatMessage>(onSendChatMessage);
		}

		public List<EmoteDefinition> GetEmoteDefinitionsList()
		{
			return new List<EmoteDefinition>(emoteDefinitionsList);
		}

		private bool onLocalPlayerDataReadyEvent(NetworkControllerEvents.LocalPlayerDataReadyEvent evt)
		{
			if (DisplayNamePlayerPrefs.HasKey("RecentEmotesServiceList"))
			{
				List<string> list = DisplayNamePlayerPrefs.GetList<string>("RecentEmotesServiceList");
				emoteDefinitionsList = StringsToDefinitions(list);
			}
			else
			{
				emoteDefinitionsList = new List<EmoteDefinition>();
			}
			return false;
		}

		[Invokable("UI.Chat.ClearRecentEmotes")]
		public static void ClearRecentEmotes()
		{
			DisplayNamePlayerPrefs.DeleteKey("RecentEmotesServiceList");
			Service.Get<RecentEmotesService>().emoteDefinitionsList.Clear();
		}

		private bool onSendChatMessage(ChatMessageSender.SendChatMessage evt)
		{
			bool flag = false;
			EmoteDefinition[] emoteDefinitionsFromMessage = EmoteManager.GetEmoteDefinitionsFromMessage(evt.Message);
			for (int i = 0; i < emoteDefinitionsFromMessage.Length; i++)
			{
				if (emoteDefinitionsList.Contains(emoteDefinitionsFromMessage[i]))
				{
					emoteDefinitionsList.Remove(emoteDefinitionsFromMessage[i]);
				}
				emoteDefinitionsList.Insert(0, emoteDefinitionsFromMessage[i]);
				flag = true;
			}
			while (emoteDefinitionsList.Count > RecentEmotesMaxCount)
			{
				emoteDefinitionsList.RemoveAt(emoteDefinitionsList.Count - 1);
			}
			List<string> value = DefinitionsToStrings(emoteDefinitionsList);
			DisplayNamePlayerPrefs.SetList("RecentEmotesServiceList", value);
			if (flag)
			{
				eventDispatcher.DispatchEvent(default(RecentEmotesUpdated));
			}
			return false;
		}

		private static List<EmoteDefinition> StringsToDefinitions(List<string> strings)
		{
			Dictionary<string, EmoteDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<string, EmoteDefinition>>();
			List<EmoteDefinition> list = new List<EmoteDefinition>();
			for (int i = 0; i < strings.Count; i++)
			{
				EmoteDefinition value;
				if (dictionary.TryGetValue(strings[i], out value))
				{
					list.Add(value);
				}
			}
			return list;
		}

		private static List<string> DefinitionsToStrings(List<EmoteDefinition> emoteDefinitions)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < emoteDefinitions.Count; i++)
			{
				list.Add(emoteDefinitions[i].Id);
			}
			return list;
		}
	}
}
