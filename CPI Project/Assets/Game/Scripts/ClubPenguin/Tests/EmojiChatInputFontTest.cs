using ClubPenguin.Chat;
using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.Tests;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Tests
{
	public class EmojiChatInputFontTest : BaseIntegrationTest
	{
		public TextAsset EmojiJsonAsset;

		public GameObject ChatBarPrefab;

		private Text chatInputText;

		private InputField chatInputField;

		private char[] emoteList;

		protected override IEnumerator runTest()
		{
			Service.Set(new Content(ContentManifestUtility.FromDefinitionFile("Configuration/embedded_content_manifest")));
			GameData gameData = new GameData();
			Service.Set(gameData);
			Service.Set((IGameData)gameData);
			gameData.Init(new Type[1]
			{
				typeof(EmoteDefinition)
			});
			emoteList = EmoteManager.GetAllEmoteCharacters();
			CreateChatBarText();
			yield return EmojiFontTest();
		}

		private void CreateChatBarText()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(ChatBarPrefab);
			chatInputField = gameObject.GetComponentInChildren<InputField>();
			if (chatInputField == null)
			{
				IntegrationTest.Fail("chatInputField == null");
			}
			chatInputText = chatInputField.textComponent;
			if (chatInputText == null)
			{
				IntegrationTest.Fail("chatInputText == null");
			}
		}

		private IEnumerator EmojiFontTest()
		{
			for (int i = 0; i < emoteList.Length; i++)
			{
				yield return StartCoroutine(TestEmojiInText(emojiString: emoteList.ToString(), textComponent: chatInputText));
			}
		}

		private IEnumerator TestEmojiInText(Text textComponent, string emojiString)
		{
			chatInputField.text = emojiString;
			yield return null;
			IntegrationTest.Assert(textComponent.preferredWidth > 0f, "Failed for emojiString: " + emojiString);
		}
	}
}
