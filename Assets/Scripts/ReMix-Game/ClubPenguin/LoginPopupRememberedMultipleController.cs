using ClubPenguin.Mix;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InteractableGroup))]
	public class LoginPopupRememberedMultipleController : MonoBehaviour
	{
		[Header("Scroll List")]
		public Transform RememberedAccountsParent;

		public PrefabContentKey RememberButtonContentKey;

		private List<LoginRememberMeListButton> rememberedButtons;

		private GameObject buttonPrefab;

		private InteractableGroup interactableGroup;

		public void OnValidate()
		{
		}

		public void Awake()
		{
			interactableGroup = GetComponent<InteractableGroup>();
			rememberedButtons = new List<LoginRememberMeListButton>();
		}

		public void Start()
		{
			Service.Get<MixLoginCreateService>().LogoutLastSession();
			Service.Get<RememberMeService>().ResetCurrentUsername();
			loadButtonPrefab();
		}

		private void loadButtonPrefab()
		{
			Content.LoadAsync(onLoadButtonComplete, RememberButtonContentKey);
		}

		private void onLoadButtonComplete(string key, GameObject prefab)
		{
			buttonPrefab = prefab;
			displayRememberedAccounts();
		}

		private void displayRememberedAccounts()
		{
			RememberMeService rememberMeService = Service.Get<RememberMeService>();
			List<string> usernames = rememberMeService.Usernames;
			int count = usernames.Count;
			List<RememberMeAccountData> list = new List<RememberMeAccountData>();
			for (int i = 0; i < count; i++)
			{
				RememberMeAccountData item = rememberMeService.LoadAccountData(usernames[i]);
				list.Add(item);
			}
			list.Sort((RememberMeAccountData x, RememberMeAccountData y) => string.Compare(x.DisplayName, y.DisplayName));
			for (int i = 0; i < count; i++)
			{
				createRememberedButton(list[i]);
			}
		}

		private void createRememberedButton(RememberMeAccountData accountData)
		{
			GameObject gameObject = Object.Instantiate(buttonPrefab, RememberedAccountsParent, false);
			LoginRememberMeListButton component = gameObject.GetComponent<LoginRememberMeListButton>();
			component.OnToggleInteraction += onToggleInteraction;
			component.OnRemoveAccount += onRemoveAccount;
			rememberedButtons.Add(component);
			component.LoadData(accountData);
		}

		private void onToggleInteraction(bool isInteractable)
		{
			this.interactableGroup.IsInteractable = isInteractable;
			int count = rememberedButtons.Count;
			for (int i = 0; i < count; i++)
			{
				InteractableGroup interactableGroup = rememberedButtons[i].InteractableGroup;
				if (interactableGroup != null)
				{
					interactableGroup.IsInteractable = isInteractable;
				}
			}
		}

		private void onRemoveAccount(string username, LoginRememberMeListButton button)
		{
			rememberedButtons.Remove(button);
			button.gameObject.SetActive(false);
			Object.Destroy(button.gameObject);
			RememberMeService rememberMeService = Service.Get<RememberMeService>();
			rememberMeService.RemoveUsername(username);
			MulticoloredList componentInChildren = GetComponentInChildren<MulticoloredList>();
			if (componentInChildren != null)
			{
				componentInChildren.Refresh();
			}
		}
	}
}
