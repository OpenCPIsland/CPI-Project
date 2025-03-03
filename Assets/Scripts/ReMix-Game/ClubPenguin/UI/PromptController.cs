using ClubPenguin.Core;
using ClubPenguin.Input;
using DevonLocalization.Core;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(PromptControllerInputHandler))]
	[RequireComponent(typeof(ModalBackground))]
	public class PromptController : MonoBehaviour
	{
		[Serializable]
		public struct ButtonDefinition
		{
			public GameObject Prefab;

			[LocalizationToken]
			public string i18nText;
		}

		[Serializable]
		public struct CustomButtonDefinition
		{
			public ButtonDefinition ButtonDefinition;

			public DPrompt.ButtonFlags ButtonFlags;
		}

		[Serializable]
		public struct TextDefinition
		{
			public Text Text;

			public string Key;
		}

		[Serializable]
		public struct ImageDefinition
		{
			public Image Image;

			public string Key;
		}

		private const string DEFAULT_ACCESSIBILITY_TITLE_TOKEN = "Accessibility.Prompt.Title.Generic";

		public ButtonDefinition[] Buttons;

		public Transform ButtonParent;

		public GameObject CloseButton;

		public TextDefinition[] TextFields;

		public ImageDefinition[] Images;

		private StateMachineContext stateMachineContext;

		protected PromptControllerInputHandler inputHandler;

		protected Localizer localizer;

		public void OnValidate()
		{
			if (Buttons == null)
			{
			}
		}

		private void Awake()
		{
			inputHandler = GetComponent<PromptControllerInputHandler>();
		}

		public void ShowPrompt(DPrompt data, Action<DPrompt.ButtonFlags> callback)
		{
			localizer = Service.Get<Localizer>();
			GameObject gameObject = GameObject.FindGameObjectWithTag(UIConstants.Tags.UI_Tray_Root);
			if (gameObject != null)
			{
				stateMachineContext = gameObject.GetComponent<StateMachineContext>();
				if (stateMachineContext != null && stateMachineContext.ContainsStateMachine("Root"))
				{
					stateMachineContext.SendEvent(new ExternalEvent("Root", "popup_open"));
				}
			}
			updateTextFields(data);
			updateImages(data);
			ModalBackground component = GetComponent<ModalBackground>();
			component.enabled = data.IsModal;
			if (data.UseCloseButton)
			{
				if (CloseButton != null)
				{
					CloseButton.SetActive(true);
					ButtonClickListener component2 = CloseButton.GetComponent<ButtonClickListener>();
					component2.OnClick.AddListener(onButtonClicked);
				}
				else
				{
					Log.LogError(this, "CloseButton is null, cannot activate it");
				}
			}
			ButtonClickListener btnAccept = null;
			ButtonClickListener btnCancel = null;
			for (int i = 0; i < Buttons.Length; i++)
			{
				createButton(data, i, callback, ref btnAccept, ref btnCancel);
			}
			inputHandler.Initialize(btnAccept, btnCancel);
			playAccessibilityText();
		}

		protected virtual void createButton(DPrompt data, int buttonIndex, Action<DPrompt.ButtonFlags> callback, ref ButtonClickListener btnAccept, ref ButtonClickListener btnCancel)
		{
			DPrompt.ButtonFlags flag = (DPrompt.ButtonFlags)(1 << buttonIndex);
			if ((data.Buttons & flag) != flag)
			{
				return;
			}
			string i18nText = Buttons[buttonIndex].i18nText;
			GameObject gameObject = createButtonObject(data, buttonIndex, flag, ref i18nText);
			if (!string.IsNullOrEmpty(i18nText))
			{
				Text componentInChildren = gameObject.GetComponentInChildren<Text>();
				if (componentInChildren != null)
				{
					componentInChildren.text = localizer.GetTokenTranslation(i18nText);
				}
			}
			ButtonClickListener component = gameObject.GetComponent<ButtonClickListener>();
			if (component != null)
			{
				if (isBackButton(flag))
				{
					btnCancel = component;
				}
				else
				{
					btnAccept = component;
				}
				if (callback != null)
				{
					component.OnClick.AddListener(delegate
					{
						callback(flag);
					});
				}
				component.OnClick.AddListener(onButtonClicked);
			}
		}

		protected virtual GameObject createButtonObject(DPrompt data, int buttonIndex, DPrompt.ButtonFlags flag, ref string i18nText)
		{
			GameObject prefab = Buttons[buttonIndex].Prefab;
			if (data.CustomButtons != null)
			{
				for (int i = 0; i < data.CustomButtons.Length; i++)
				{
					CustomButtonDefinition customButtonDefinition = data.CustomButtons[i];
					if ((customButtonDefinition.ButtonFlags & flag) == flag)
					{
						if (customButtonDefinition.ButtonDefinition.Prefab != null)
						{
							prefab = customButtonDefinition.ButtonDefinition.Prefab;
						}
						if (!string.IsNullOrEmpty(customButtonDefinition.ButtonDefinition.i18nText))
						{
							i18nText = customButtonDefinition.ButtonDefinition.i18nText;
						}
					}
				}
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab);
			gameObject.transform.SetParent(ButtonParent, false);
			return gameObject;
		}

		private void updateTextFields(DPrompt data)
		{
			Localizer localizer = Service.Get<Localizer>();
			int num = TextFields.Length;
			for (int i = 0; i < num; i++)
			{
				TextDefinition textDefinition = TextFields[i];
				DPrompt.TextData value;
				if (data.TextFields.TryGetValue(textDefinition.Key, out value))
				{
					if (value.IsTranslated)
					{
						textDefinition.Text.text = value.I18nText;
					}
					else
					{
						textDefinition.Text.text = localizer.GetTokenTranslation(value.I18nText);
					}
				}
				bool flag = !string.IsNullOrEmpty(textDefinition.Text.text);
				OnOffGameObjectSelector component = textDefinition.Text.gameObject.GetComponent<OnOffGameObjectSelector>();
				if (component != null)
				{
					component.IsOn = flag;
				}
				else
				{
					textDefinition.Text.gameObject.SetActive(flag);
				}
			}
		}

		private void updateImages(DPrompt data)
		{
			int num = Images.Length;
			for (int i = 0; i < num; i++)
			{
				ImageDefinition imageDefinition = Images[i];
				Sprite value;
				if (data.Images.TryGetValue(imageDefinition.Key, out value) && value != null)
				{
					imageDefinition.Image.sprite = value;
					imageDefinition.Image.gameObject.SetActive(true);
				}
			}
		}

		private void onButtonClicked(ButtonClickListener.ClickType clickType)
		{
			if (stateMachineContext != null)
			{
				stateMachineContext.SendEvent(new ExternalEvent("Root", "popup_closed"));
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private bool isBackButton(DPrompt.ButtonFlags flag)
		{
			return flag == DPrompt.ButtonFlags.None || flag == DPrompt.ButtonFlags.CANCEL || flag == DPrompt.ButtonFlags.NO || flag == DPrompt.ButtonFlags.CLOSE;
		}

		private void playAccessibilityText()
		{
			if (MonoSingleton<NativeAccessibilityManager>.Instance.AccessibilityLevel != NativeAccessibilityLevel.VOICE)
			{
				return;
			}
			string text = string.Empty;
			int num = TextFields.Length;
			for (int i = 0; i < num; i++)
			{
				if (TextFields[i].Text.gameObject.activeInHierarchy)
				{
					text = TextFields[i].Text.text + " ";
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				MonoSingleton<NativeAccessibilityManager>.Instance.Native.Speak(text.Trim());
			}
		}
	}
}
