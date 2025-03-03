using ClubPenguin.Core;
using ClubPenguin.Input;
using ClubPenguin.UI;
using Disney.Kelowna.Common.SEDFSM;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class LoginPopupRememberedSingleController : LoginPopupContentController
	{
		[Header("Credentials")]
		public InputFieldValidator PasswordField;

		[Header("Buttons")]
		public Button RemoveButton;

		public Button PlayButton;

		[Header("Other Controls")]
		public Toggle RememberPassword;

		public Text DisplayName;

		public SpriteSelector MembershipSelector;

		public GameObject BannedOverlay;

		public OnOffGameObjectSelector SoftLoginSelector;

		[Header("FSM Events")]
		public string FSMTarget;

		[FormerlySerializedAs("BackEvent")]
		public string RemovedEvent;

		private RememberMeData rememberedData;

		private bool setPasswordValid;

		public void OnValidate()
		{
		}

		public override void OnEnable()
		{
			RemoveButton.onClick.AddListener(onRemoveClicked);
			PlayButton.GetComponent<ButtonClickListener>().OnClick.AddListener(onPlayClicked);
			RememberPassword.onValueChanged.AddListener(onRememberPasswordToggled);
			base.OnEnable();
		}

		public void OnDisable()
		{
			RemoveButton.onClick.RemoveListener(onRemoveClicked);
			PlayButton.GetComponent<ButtonClickListener>().OnClick.RemoveListener(onPlayClicked);
			RememberPassword.onValueChanged.RemoveListener(onRememberPasswordToggled);
		}

		public override void Start()
		{
			RememberMeService rememberMeService = Service.Get<RememberMeService>();
			rememberedData = rememberMeService.GetRememberMeData();
			if (rememberedData.AccountData == null)
			{
				string text = rememberMeService.CurrentUsername;
				if (string.IsNullOrEmpty(text))
				{
					text = rememberMeService.Usernames[0];
				}
				rememberedData.AccountData = rememberMeService.LoadAccountData(text);
			}
			string username = rememberedData.AccountData.Username;
			SoftLoginSelector.IsOn = (username == rememberMeService.CurrentUsername && PlatformUtils.GetPlatformType() != PlatformType.Mobile);
			showGeneralError(rememberedData.GeneralErrorMessage);
			rememberedData.GeneralErrorMessage = string.Empty;
			setPasswordValid = false;
			PasswordField.TextInput.text = rememberedData.AccountData.Password;
			RememberPassword.isOn = !string.IsNullOrEmpty(PasswordField.TextInput.text);
			if (!string.IsNullOrEmpty(PasswordField.TextInput.text))
			{
				PasswordField.HasError = false;
				PasswordField.IsValidationComplete = true;
				setPasswordValid = true;
			}
			AvatarRenderTextureComponent componentInChildren = GetComponentInChildren<AvatarRenderTextureComponent>();
			if (componentInChildren != null)
			{
				componentInChildren.RenderAvatar(rememberedData.AccountData.Outfit, rememberedData.AccountData.BodyColor);
			}
			if (DisplayName != null)
			{
				DisplayName.text = rememberedData.AccountData.DisplayName;
			}
			if (MembershipSelector != null)
			{
				int index = 0;
				switch (rememberedData.AccountData.MembershipType)
				{
				case MembershipType.Member:
					index = 1;
					break;
				case MembershipType.None:
				case MembershipType.AllAccessEventMember:
				{
					AllAccessService allAccessService = Service.Get<AllAccessService>();
					if (allAccessService.IsAllAccessActive() && AllAccessHelper.HasSeenAllAccessFlow(allAccessService.GetAllAccessEventKey(), rememberedData.AccountData.DisplayName))
					{
						index = 2;
					}
					break;
				}
				}
				MembershipSelector.SelectSprite(index);
			}
			bool isBanned = false;
			if (rememberedData.AccountData.Banned.HasValue)
			{
				if (!rememberedData.AccountData.Banned.Value.ExpirationDate.HasValue)
				{
					isBanned = true;
				}
				else if ((rememberedData.AccountData.Banned.Value.ExpirationDate - DateTime.Now).Value.TotalHours >= 0.0)
				{
					isBanned = true;
				}
			}
			updateBannedState(isBanned);
			base.Start();
		}

		private void Update()
		{
			if (PasswordField.gameObject.activeInHierarchy && setPasswordValid)
			{
				PasswordField.OnValidationSuccess.Invoke();
				setPasswordValid = false;
			}
		}

		private void updateBannedState(bool isBanned)
		{
			if (BannedOverlay != null)
			{
				BannedOverlay.SetActive(isBanned);
			}
		}

		protected override void showPasswordError(string error)
		{
			PasswordField.HasError = true;
			PasswordField.ShowError(error);
		}

		public void OnLoginClicked()
		{
			if (RememberPassword.isOn)
			{
				rememberedData.AccountData.Password = PasswordField.TextInput.text;
			}
			else
			{
				rememberedData.AccountData.Password = string.Empty;
			}
			performLogin(new DLoginPayload(rememberedData.AccountData.Username, PasswordField.TextInput.text));
		}

		private void onRemoveClicked()
		{
			Service.Get<PromptManager>().ShowPrompt("RemoveRememberedAccountPrompt", onRemoveAccountPromptButtonClicked);
		}

		private void onPlayClicked(ButtonClickListener.ClickType interactedType)
		{
			toggleInteraction(false);
			Service.Get<GameStateController>().EnterGame();
		}

		private void onRemoveAccountPromptButtonClicked(DPrompt.ButtonFlags pressed)
		{
			if (pressed == DPrompt.ButtonFlags.YES || pressed == DPrompt.ButtonFlags.OK)
			{
				RememberMeService rememberMeService = Service.Get<RememberMeService>();
				rememberMeService.RemoveUsername(rememberedData.AccountData.Username);
				GetComponentInParent<BackButtonStateHandler>().MarkCurrentStateInvalid();
				if (!string.IsNullOrEmpty(FSMTarget))
				{
					StateMachineContext componentInParent = GetComponentInParent<StateMachineContext>();
					componentInParent.SendEvent(new ExternalEvent(FSMTarget, RemovedEvent));
				}
				else
				{
					StateMachine componentInParent2 = GetComponentInParent<StateMachine>();
					componentInParent2.SendEvent(RemovedEvent);
				}
			}
		}

		private void onRememberPasswordToggled(bool value)
		{
			if (value)
			{
				Service.Get<PromptManager>().ShowPrompt("SavePasswordPrompt", onRememberPasswordPromptButtonClicked);
			}
		}

		private void onRememberPasswordPromptButtonClicked(DPrompt.ButtonFlags pressed)
		{
			RememberPassword.isOn = (pressed == DPrompt.ButtonFlags.YES || pressed == DPrompt.ButtonFlags.OK);
		}

		protected override void onInvalidCredentialsError(string error)
		{
			if (RememberPassword.isOn)
			{
				rememberedData.AccountData.Password = string.Empty;
			}
		}

		protected override void onTemporaryBanError()
		{
			if (!rememberedData.AccountData.Banned.HasValue)
			{
				rememberedData.OnAccountBanned(AlertType.Unknown, null);
			}
			showAccountBannedPrompt(rememberedData.AccountData.Banned.Value.Category, rememberedData.AccountData.Banned.Value.ExpirationDate);
		}

		protected override void onAccountBannedPromptLoaded(PromptLoaderCMD promptLoader, AlertType category, DateTime? expirationDate)
		{
			base.onAccountBannedPromptLoaded(promptLoader, category, expirationDate);
			updateBannedState(true);
		}
	}
}
