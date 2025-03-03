using ClubPenguin.Analytics;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.SEDFSM;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class LoginRememberMeListButton : LoginPopupContentController
	{
		[Header("Display")]
		public Text DisplayName;

		[Header("Other Components")]
		public Button LoginButton;

		public Button RemoveButton;

		public AvatarRenderTextureComponent AvatarIcon;

		public SpriteSelector MembershipSelector;

		public Image LoginSpinner;

		[Header("Banned Info - Avatar")]
		public GameObject BannedText;

		[Range(0f, 1f)]
		public float BannedAlpha;

		[Header("Banned Info - Button")]
		public Sprite BannedButton;

		public Sprite BannedSpinner;

		[Header("FSM Events")]
		public string FSMTarget;

		public string RememberMeSingleEvent;

		private RememberMeAccountData accountData;

		public InteractableGroup InteractableGroup
		{
			get
			{
				return interactableGroup;
			}
		}

		public event Action<bool> OnToggleInteraction;

		public event Action<string, LoginRememberMeListButton> OnRemoveAccount;

		public void OnValidate()
		{
		}

		public override void OnEnable()
		{
			LoginButton.onClick.AddListener(onLoginClicked);
			RemoveButton.onClick.AddListener(onRemoveClicked);
			base.OnEnable();
		}

		public void OnDisable()
		{
			LoginButton.onClick.RemoveListener(onLoginClicked);
			RemoveButton.onClick.RemoveListener(onRemoveClicked);
		}

		public void LoadData(RememberMeAccountData accountData)
		{
			this.accountData = accountData;
			DisplayName.text = accountData.DisplayName;
			if (AvatarIcon != null)
			{
				AvatarIcon.RenderAvatar(accountData.Outfit, accountData.BodyColor);
			}
			if (MembershipSelector != null)
			{
				int index = 0;
				switch (accountData.MembershipType)
				{
				case MembershipType.Member:
					index = 1;
					break;
				case MembershipType.None:
				case MembershipType.AllAccessEventMember:
				{
					AllAccessService allAccessService = Service.Get<AllAccessService>();
					if (allAccessService.IsAllAccessActive() && AllAccessHelper.HasSeenAllAccessFlow(allAccessService.GetAllAccessEventKey(), accountData.DisplayName))
					{
						index = 2;
					}
					break;
				}
				}
				MembershipSelector.SelectSprite(index);
			}
			if (accountData.Banned.HasValue)
			{
				if (!accountData.Banned.Value.ExpirationDate.HasValue)
				{
					displayBannedState();
				}
				else if ((accountData.Banned.Value.ExpirationDate - DateTime.Now).Value.TotalHours >= 0.0)
				{
					displayBannedState();
				}
			}
		}

		protected override void toggleInteraction(bool isInteractable)
		{
			base.toggleInteraction(isInteractable);
			if (this.OnToggleInteraction != null)
			{
				this.OnToggleInteraction(isInteractable);
			}
		}

		private void displayBannedState()
		{
			if (BannedText != null)
			{
				BannedText.SetActive(true);
			}
			if (AvatarIcon != null)
			{
				showBannedOverlay();
			}
			if (LoginSpinner != null && BannedSpinner != null)
			{
				LoginSpinner.sprite = BannedSpinner;
			}
			if (BannedButton != null)
			{
				LoginButton.image.sprite = BannedButton;
				LoginButton.spriteState = default(SpriteState);
			}
		}

		private void showBannedOverlay()
		{
			RawImage componentInChildren = AvatarIcon.GetComponentInChildren<RawImage>();
			if (componentInChildren != null)
			{
				Color color = componentInChildren.color;
				componentInChildren.color = new Color(color.r, color.g, color.b, BannedAlpha);
			}
			else
			{
				AvatarIcon.OnAvatarImageSet += onAvatarImageSet;
			}
		}

		private void onAvatarImageSet()
		{
			AvatarIcon.OnAvatarImageSet -= onAvatarImageSet;
			showBannedOverlay();
		}

		private void onRemoveClicked()
		{
			Service.Get<PromptManager>().ShowPrompt("RemoveRememberedAccountPrompt", onRemoveAccountPromptButtonClicked);
		}

		private void onRemoveAccountPromptButtonClicked(DPrompt.ButtonFlags pressed)
		{
			if (pressed == DPrompt.ButtonFlags.YES || pressed == DPrompt.ButtonFlags.OK)
			{
				this.OnRemoveAccount.InvokeSafe(accountData.Username, this);
			}
		}

		private void onLoginClicked()
		{
			if (string.IsNullOrEmpty(accountData.Password))
			{
				transitionToSingle();
				return;
			}
			string tier = "";
			for (int i = 0; i < Service.Get<RememberMeService>().Usernames.Count; i++)
			{
				if (Service.Get<RememberMeService>().Usernames[i] == accountData.Username)
				{
					tier = (i + 1).ToString();
					break;
				}
			}
			string message = Service.Get<RememberMeService>().SavedUsernameCount.ToString();
			Service.Get<ICPSwrveService>().Action("login_list", tier, null, null, null, message);
			performLogin(new DLoginPayload(accountData.Username, accountData.Password));
		}

		protected override void showGeneralError(string error)
		{
			base.showGeneralError(error);
			RemoveButton.gameObject.SetActive(!GeneralErrorBox.gameObject.activeSelf);
		}

		protected override void onInvalidCredentialsError(string error)
		{
			transitionToSingle(error);
		}

		protected override void onMissingInfoError(string error)
		{
			transitionToSingle(error);
		}

		protected override void onTemporaryBanError()
		{
			if (!accountData.Banned.HasValue)
			{
				accountData.Banned = new RememberMeAccountData.BannedInfo(AlertType.Unknown, null);
				Service.Get<RememberMeService>().SaveAccountData(accountData);
			}
			showAccountBannedPrompt(accountData.Banned.Value.Category, accountData.Banned.Value.ExpirationDate);
		}

		protected override void onAccountBannedPromptLoaded(PromptLoaderCMD promptLoader, AlertType category, DateTime? expirationDate)
		{
			base.onAccountBannedPromptLoaded(promptLoader, category, expirationDate);
			displayBannedState();
		}

		private void transitionToSingle(string error = "")
		{
			RememberMeService rememberMeService = Service.Get<RememberMeService>();
			RememberMeData rememberMeData = rememberMeService.GetRememberMeData();
			rememberMeData.AccountData = accountData;
			rememberMeData.GeneralErrorMessage = error;
			if (!string.IsNullOrEmpty(FSMTarget))
			{
				StateMachineContext componentInParent = GetComponentInParent<StateMachineContext>();
				componentInParent.SendEvent(new ExternalEvent(FSMTarget, RememberMeSingleEvent));
			}
			else
			{
				StateMachine componentInParent2 = GetComponentInParent<StateMachine>();
				componentInParent2.SendEvent(RememberMeSingleEvent);
			}
		}
	}
}
