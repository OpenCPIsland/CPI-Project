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
	public class EmojiFontTest : BaseIntegrationTest
	{
		public TextAsset EmojiJsonAsset;

		public GameObject FontPrefab;

		private Text fontText;

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
			CreateChatBlockLocalText();
			TestEmojiFont();
			yield break;
		}

		private void CreateChatBlockLocalText()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(FontPrefab);
			fontText = gameObject.GetComponentInChildren<Text>();
			if (fontText == null)
			{
				IntegrationTest.Fail("No text component found in FontPrefab");
			}
		}

		private void TestEmojiFont()
		{
			for (int i = 0; i < emoteList.Length; i++)
			{
				string emojiString = emoteList.ToString();
				TestEmojiInText(fontText, emojiString);
			}
		}

		private void TestEmojiInText(Text textComponent, string emojiString)
		{
			textComponent.text = emojiString;
			IntegrationTest.Assert(textComponent.preferredWidth > 0f);
		}
	}
}
