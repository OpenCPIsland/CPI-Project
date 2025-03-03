using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class ChatPhrasePanelStateHandler : MonoBehaviour
	{
		public Transform[] ContentParents;

		public Transform[] ContextualContentParents;

		public Sprite ContextualWordBubble;

		private GlobalChatPhrasesDefinition globalChatPhrasesDefinition;

		private ChatPhraseDefinitionList contextualChatPhraseDefinitionSet;

		private GameObject chatItemPrefab;

		private GameObject chatItemContextualPrefab;

		private Dictionary<int, SizzleClipDefinition> sizzleClips;

		private static PrefabContentKey chatInstantItemContentKey = new PrefabContentKey("ChatPhrasePrefabs/ChatPhrasePanelItem");

		private static PrefabContentKey chatInstantItemContextualContentKey = new PrefabContentKey("ChatPhrasePrefabs/ChatPhrasePanelItemContextual");

		private float[] contentSize;

		private float[] contextualContentSize;

		public void Awake()
		{
			contentSize = new float[ContentParents.Length];
			contextualContentSize = new float[ContextualContentParents.Length];
		}

		public void Start()
		{
			sizzleClips = Service.Get<GameData>().Get<Dictionary<int, SizzleClipDefinition>>();
			loadContent();
			setScrollBar();
		}

		private void loadContent()
		{
			ManifestContentKey manifestContentKey = StaticGameDataUtils.GetManifestContentKey(typeof(GlobalChatPhrasesDefinition));
			Content.LoadAsync(onManifestLoadComplete, manifestContentKey);
		}

		private void onManifestLoadComplete(string key, Manifest instantChatManifest)
		{
			globalChatPhrasesDefinition = (instantChatManifest.Assets[0] as GlobalChatPhrasesDefinition);
			CoroutineRunner.Start(loadPrefabs(), this, "loadPrefabs");
		}

		private IEnumerator loadPrefabs()
		{
			AssetRequest<GameObject> request2 = Content.LoadAsync(chatInstantItemContentKey);
			yield return request2;
			chatItemPrefab = request2.Asset;
			request2 = Content.LoadAsync(chatInstantItemContextualContentKey);
			yield return request2;
			chatItemContextualPrefab = request2.Asset;
			createChatItems();
		}

		private void setContextualChatTokenDefinition()
		{
			DataEntityHandle dataEntityHandle = Service.Get<CPDataEntityCollection>().FindEntityByName("PhraseChatData");
			if (!dataEntityHandle.IsNull)
			{
				contextualChatPhraseDefinitionSet = Service.Get<CPDataEntityCollection>().GetComponent<PhraseChatData>(dataEntityHandle).Peek();
			}
		}

		private void createChatItems()
		{
			for (int i = 0; i < globalChatPhrasesDefinition.ChatPhraseDefinitions.Count; i++)
			{
				int num = identifySmallestIndex(contentSize);
				float num2 = createChatItem(globalChatPhrasesDefinition.ChatPhraseDefinitions[i].Token, globalChatPhrasesDefinition.ChatPhraseDefinitions[i].SizzleClipKey.Id, ContentParents[num], false);
				contentSize[num] += num2;
			}
			setContextualChatTokenDefinition();
			if (contextualChatPhraseDefinitionSet != null)
			{
				for (int i = 0; i < contextualChatPhraseDefinitionSet.ChatPhraseDefinitions.Count; i++)
				{
					int num = identifySmallestIndex(contextualContentSize);
					float num2 = createChatItem(contextualChatPhraseDefinitionSet.ChatPhraseDefinitions[i].Token, contextualChatPhraseDefinitionSet.ChatPhraseDefinitions[i].SizzleClipKey.Id, ContextualContentParents[num], true);
					contextualContentSize[num] += num2;
				}
			}
		}

		private float createChatItem(string itemToken, int sizzleClipId, Transform itemParent, bool contextual)
		{
			SizzleClipDefinition value = null;
			sizzleClips.TryGetValue(sizzleClipId, out value);
			GameObject gameObject = UnityEngine.Object.Instantiate(contextual ? chatItemContextualPrefab : chatItemPrefab, itemParent);
			ChatPhraseItem component = gameObject.GetComponent<ChatPhraseItem>();
			component.ClickAction = (Action<string, SizzleClipDefinition>)Delegate.Combine(component.ClickAction, new Action<string, SizzleClipDefinition>(onItemClick));
			component.LoadText(itemToken, value);
			float num = 0f;
			Text componentInChildren = gameObject.GetComponentInChildren<Text>();
			num += ((componentInChildren != null) ? componentInChildren.preferredWidth : 0f);
			HorizontalLayoutGroup component2 = itemParent.GetComponent<HorizontalLayoutGroup>();
			num += ((component2 != null) ? component2.spacing : 0f);
			LayoutGroup component3 = gameObject.GetComponent<LayoutGroup>();
			return num + ((component3 != null) ? ((float)(component3.padding.left + component3.padding.right)) : 0f);
		}

		private void onItemClick(string itemText, SizzleClipDefinition sizzleClip)
		{
			Service.Get<EventDispatcher>().DispatchEvent(new ChatMessageSender.SendChatMessage(itemText, sizzleClip, true));
		}

		private void setScrollBar()
		{
			ChatScreenPanel componentInParent = GetComponentInParent<ChatScreenPanel>();
			if (componentInParent != null)
			{
				ScrollRect componentInChildren = GetComponentInChildren<ScrollRect>();
				Scrollbar scrollbar2 = componentInChildren.horizontalScrollbar = componentInParent.GetComponentsInChildren<Scrollbar>(true)[0];
				componentInChildren.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
				ScrollerAccessibilitySettings[] componentsInChildren = componentInParent.GetComponentsInChildren<ScrollerAccessibilitySettings>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].Scrollbar = scrollbar2;
				}
			}
		}

		private int identifySmallestIndex(float[] arrayToCheck)
		{
			int result = 0;
			float num = float.MaxValue;
			int num2 = arrayToCheck.Length;
			for (int i = 0; i < num2; i++)
			{
				if (arrayToCheck[i] < num)
				{
					result = i;
					num = arrayToCheck[i];
				}
			}
			return result;
		}
	}
}
