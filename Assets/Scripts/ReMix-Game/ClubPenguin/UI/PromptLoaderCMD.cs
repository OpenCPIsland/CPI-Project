using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class PromptLoaderCMD
	{
		private DPrompt _promptData;

		private object owner;

		private Action<PromptLoaderCMD> onCompleteHandler;

		private string titleTextInsert;

		private string bodyTextInsert;

		public DPrompt PromptData
		{
			get
			{
				return _promptData;
			}
		}

		public PromptController Prefab
		{
			get;
			private set;
		}

		public PromptDefinition PromptDefinition
		{
			get;
			private set;
		}

		public Action<DPrompt.ButtonFlags> PromptCallback
		{
			get;
			private set;
		}

		public PromptLoaderCMD(object owner, PromptDefinition promptDefinition, Action<PromptLoaderCMD> onCompleteHandler, Action<DPrompt.ButtonFlags> promptCallback = null)
		{
			this.owner = owner;
			PromptDefinition = promptDefinition;
			PromptCallback = promptCallback;
			this.onCompleteHandler = onCompleteHandler;
		}

		public PromptLoaderCMD(object owner, PromptDefinition promptDefinition, string titleTextInsert, string bodyTextInsert, Action<PromptLoaderCMD> onCompleteHandler, Action<DPrompt.ButtonFlags> promptCallback = null)
		{
			this.owner = owner;
			PromptDefinition = promptDefinition;
			PromptCallback = promptCallback;
			this.onCompleteHandler = onCompleteHandler;
			this.titleTextInsert = titleTextInsert;
			this.bodyTextInsert = bodyTextInsert;
		}

		public void ExecuteImmediate()
		{
			DPrompt.ButtonFlags buttonFlags = DPrompt.ButtonFlags.None;
			for (int i = 0; i < PromptDefinition.Buttons.Length; i++)
			{
				buttonFlags |= PromptDefinition.Buttons[i];
			}
			Sprite image = null;
			if (!string.IsNullOrEmpty(PromptDefinition.SpriteContentKey.Key))
			{
				image = Content.LoadImmediate(PromptDefinition.SpriteContentKey);
			}
			if (!string.IsNullOrEmpty(PromptDefinition.PrefabContentKey.Key))
			{
				PromptController component = Content.LoadImmediate(PromptDefinition.PrefabContentKey).GetComponent<PromptController>();
				if (component != null)
				{
					Prefab = component;
				}
			}
			string i18nTitleText = PromptDefinition.i18nTitleText;
			string i18nBodyText = PromptDefinition.i18nBodyText;
			bool flag = !string.IsNullOrEmpty(titleTextInsert) || !string.IsNullOrEmpty(bodyTextInsert);
			Localizer localizer = Service.Get<Localizer>();
			string text = string.Format("{0}.Desktop", i18nTitleText);
			string text2 = string.Format("{0}.Desktop", i18nBodyText);
			i18nTitleText = (localizer.tokens.ContainsKey(text) ? text : i18nTitleText);
			i18nBodyText = (localizer.tokens.ContainsKey(text2) ? text2 : i18nBodyText);
			if (flag)
			{
				i18nTitleText = (string.IsNullOrEmpty(titleTextInsert) ? localizer.GetTokenTranslation(i18nTitleText) : string.Format(localizer.GetTokenTranslation(i18nTitleText), titleTextInsert));
				i18nBodyText = (string.IsNullOrEmpty(bodyTextInsert) ? localizer.GetTokenTranslation(i18nBodyText) : string.Format(localizer.GetTokenTranslation(i18nBodyText), bodyTextInsert));
			}
			_promptData = new DPrompt(i18nTitleText, i18nBodyText, buttonFlags, image, PromptDefinition.IsModal, PromptDefinition.AutoClose, flag, PromptDefinition.UseCloseButton);
			List<PromptController.CustomButtonDefinition> list = new List<PromptController.CustomButtonDefinition>(PromptDefinition.CustomButtonKeys.Length);
			for (int i = 0; i < PromptDefinition.CustomButtonKeys.Length; i++)
			{
				CustomButton customButton = Content.LoadImmediate(PromptDefinition.CustomButtonKeys[i]);
				if (customButton != null)
				{
					list.Add(customButton.Definition);
				}
			}
			_promptData.CustomButtons = list.ToArray();
			if (onCompleteHandler != null)
			{
				onCompleteHandler(this);
			}
		}

		public void Execute()
		{
			CoroutineRunner.Start(loadPrompt(), owner, "PromptLoaderCMD");
		}

		private IEnumerator loadPrompt()
		{
			DPrompt.ButtonFlags buttonFlags = DPrompt.ButtonFlags.None;
			for (int j = 0; j < PromptDefinition.Buttons.Length; j++)
			{
				buttonFlags |= PromptDefinition.Buttons[j];
			}
			Sprite image = null;
			if (!string.IsNullOrEmpty(PromptDefinition.SpriteContentKey.Key))
			{
				AssetRequest<Sprite> spriteRequest = Content.LoadAsync(PromptDefinition.SpriteContentKey);
				yield return spriteRequest;
				image = spriteRequest.Asset;
			}
			if (!string.IsNullOrEmpty(PromptDefinition.PrefabContentKey.Key))
			{
				AssetRequest<GameObject> prefabRequest = Content.LoadAsync(PromptDefinition.PrefabContentKey);
				yield return prefabRequest;
				if ((bool)prefabRequest.Asset.GetComponent<PromptController>())
				{
					Prefab = prefabRequest.Asset.GetComponent<PromptController>();
				}
			}
			string titleText2 = PromptDefinition.i18nTitleText;
			string bodyText2 = PromptDefinition.i18nBodyText;
			bool translate = !string.IsNullOrEmpty(titleTextInsert) || !string.IsNullOrEmpty(bodyTextInsert);
			Localizer localizer = Service.Get<Localizer>();
			string titleDesktopText = string.Format("{0}.Desktop", titleText2);
			string bodyDesktopText = string.Format("{0}.Desktop", bodyText2);
			titleText2 = (localizer.tokens.ContainsKey(titleDesktopText) ? titleDesktopText : titleText2);
			bodyText2 = (localizer.tokens.ContainsKey(bodyDesktopText) ? bodyDesktopText : bodyText2);
			if (translate)
			{
				titleText2 = (string.IsNullOrEmpty(titleTextInsert) ? localizer.GetTokenTranslation(titleText2) : string.Format(localizer.GetTokenTranslation(titleText2), titleTextInsert));
				bodyText2 = (string.IsNullOrEmpty(bodyTextInsert) ? localizer.GetTokenTranslation(bodyText2) : string.Format(localizer.GetTokenTranslation(bodyText2), bodyTextInsert));
			}
			_promptData = new DPrompt(titleText2, bodyText2, buttonFlags, image, PromptDefinition.IsModal, PromptDefinition.AutoClose, translate, PromptDefinition.UseCloseButton);
			List<PromptController.CustomButtonDefinition> customButtons = new List<PromptController.CustomButtonDefinition>(PromptDefinition.CustomButtonKeys.Length);
			for (int i = 0; i < PromptDefinition.CustomButtonKeys.Length; i++)
			{
				AssetRequest<CustomButton> request = Content.LoadAsync(PromptDefinition.CustomButtonKeys[i]);
				yield return request;
				if (request.Asset != null)
				{
					customButtons.Add(request.Asset.Definition);
				}
			}
			_promptData.CustomButtons = customButtons.ToArray();
			if (onCompleteHandler != null)
			{
				onCompleteHandler(this);
			}
		}
	}
}
