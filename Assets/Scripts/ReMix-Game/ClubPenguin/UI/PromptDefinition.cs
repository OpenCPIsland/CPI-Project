using ClubPenguin.Core.StaticGameData;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using System;
using UnityEngine;

namespace ClubPenguin.UI
{
	[Serializable]
	[CreateAssetMenu]
	public class PromptDefinition : StaticGameDataDefinition
	{
		[StaticGameDataDefinitionId]
		public string Id;

		public PrefabContentKey PrefabContentKey;

		[LocalizationToken]
		public string i18nTitleText;

		[LocalizationToken]
		public string i18nBodyText;

		public DPrompt.ButtonFlags[] Buttons;

		public CustomButtonKey[] CustomButtonKeys = new CustomButtonKey[0];

		public SpriteContentKey SpriteContentKey;

		public bool IsModal = true;

		public bool AutoClose = true;

		public bool UseCloseButton;
	}
}
