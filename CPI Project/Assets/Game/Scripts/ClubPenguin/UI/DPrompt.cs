using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	public struct DPrompt
	{
		public struct TextData
		{
			public string I18nText;

			public bool IsTranslated;

			public TextData(string i18nText, bool isTranslated = false)
			{
				I18nText = i18nText;
				IsTranslated = isTranslated;
			}
		}

		[Flags]
		public enum ButtonFlags
		{
			None = 0x0,
			CANCEL = 0x1,
			OK = 0x2,
			NO = 0x4,
			YES = 0x8,
			CLOSE = 0x10,
			RETRY = 0x20
		}

		public static string PROMPT_TEXT_TITLE = "Prompt.Text.Title";

		public static string PROMPT_TEXT_BODY = "Prompt.Text.Body";

		public static string PROMPT_TEXT_INFO = "Prompt.Text.Info";

		public static string PROMPT_IMAGE_DEFAULT = "Prompt.Image.Default";

		public readonly ButtonFlags Buttons;

		public readonly bool IsModal;

		public readonly bool AutoClose;

		public readonly bool UseCloseButton;

		public PromptController.CustomButtonDefinition[] CustomButtons;

		public Dictionary<string, TextData> TextFields;

		public Dictionary<string, Sprite> Images;

		public static bool IsConfirmation(ButtonFlags flag)
		{
			return flag == ButtonFlags.OK || flag == ButtonFlags.YES;
		}

		public DPrompt(string titleText, string bodyText, ButtonFlags buttons = ButtonFlags.OK, Sprite image = null, bool isModal = true, bool autoClose = true, bool isTranslated = false, bool useCloseButton = false)
		{
			TextFields = new Dictionary<string, TextData>();
			Images = new Dictionary<string, Sprite>();
			Buttons = buttons;
			IsModal = isModal;
			AutoClose = autoClose;
			UseCloseButton = useCloseButton;
			CustomButtons = null;
			TextFields.Add(PROMPT_TEXT_TITLE, new TextData(titleText, isTranslated));
			TextFields.Add(PROMPT_TEXT_BODY, new TextData(bodyText, isTranslated));
			Images.Add(PROMPT_IMAGE_DEFAULT, image);
		}

		public void SetText(string key, string i18nText, bool isTranslated = false)
		{
			TextData value;
			if (TextFields.TryGetValue(key, out value))
			{
				TextFields[key] = new TextData(i18nText, isTranslated);
			}
			else
			{
				TextFields.Add(key, new TextData(i18nText, isTranslated));
			}
		}

		public void SetImage(string key, Sprite sprite)
		{
			Sprite value;
			if (Images.TryGetValue(key, out value))
			{
				Images[key] = sprite;
			}
			else
			{
				Images.Add(key, sprite);
			}
		}
	}
}
