using ClubPenguin.Core;
using ClubPenguin.Mix;
using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System.Collections.Generic;
using Tweaker.Core;

namespace ClubPenguin
{
	public class RememberMeService
	{
		private const string DEFAULT_ACCOUNT_DATA = "{}";

		private KeyChainCacheableString rememberedUsernamesKeyChain;

		private KeyChainCacheableString currentUsernameKeyChain;

		public List<string> Usernames;

		private string currentUsername = string.Empty;

		public string CurrentUsername
		{
			get
			{
				return currentUsername;
			}
			private set
			{
				currentUsername = value;
				currentUsernameKeyChain.SetValue(currentUsername);
			}
		}

		public int SavedUsernameCount
		{
			get
			{
				return (Usernames != null) ? Usernames.Count : 0;
			}
		}

		public RememberMeService()
		{
			rememberedUsernamesKeyChain = new KeyChainCacheableString("cp.rememberme", "");
			currentUsernameKeyChain = new KeyChainCacheableString("cp.remembermecurrent", "");
			try
			{
				Usernames = Service.Get<JsonService>().Deserialize<List<string>>(rememberedUsernamesKeyChain.GetValue());
				CurrentUsername = currentUsernameKeyChain.GetValue();
				validateStoredUsernames();
			}
			catch
			{
			}
			if (Usernames == null)
			{
				Usernames = new List<string>();
			}
			Service.Get<EventDispatcher>().AddListener<SessionEvents.SessionStartedEvent>(onSessionStarted);
			Service.Get<EventDispatcher>().AddListener<SessionEvents.SessionLogoutEvent>(onLogout);
			Service.Get<MixLoginCreateService>().OnSoftLoginFailed += onSoftLoginFailed;
		}

		private void validateStoredUsernames()
		{
			List<string> list = new List<string>();
			int count = Usernames.Count;
			for (int i = 0; i < count; i++)
			{
				string text = Usernames[i];
				if (!isUserNameValid(text))
				{
					list.Add(text);
				}
			}
			count = list.Count;
			for (int i = 0; i < count; i++)
			{
				RemoveUsername(list[i]);
			}
			if (!isUserNameValid(CurrentUsername))
			{
				CurrentUsername = string.Empty;
			}
		}

		private bool isUserNameValid(string username)
		{
			CacheableType<string> accountDataCache = getAccountDataCache(username);
			return !string.IsNullOrEmpty(accountDataCache.GetValue()) && !(accountDataCache.GetValue() == "{}");
		}

		public RememberMeData GetRememberMeData()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			RememberMeData component;
			if (!cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				component = cPDataEntityCollection.AddComponent<RememberMeData>(cPDataEntityCollection.LocalPlayerHandle);
				component.OnAccountDataUpdated += onAccountDataUpdated;
			}
			return component;
		}

		public void SaveAccountData(RememberMeAccountData accountData)
		{
			string value = Service.Get<JsonService>().Serialize(accountData);
			accountData.StoredData.SetValue(value);
		}

		private void onAccountDataUpdated(RememberMeAccountData accountData)
		{
			SaveAccountData(accountData);
		}

		private void saveUsernames()
		{
			string value = Service.Get<JsonService>().Serialize(Usernames);
			rememberedUsernamesKeyChain.SetValue(value);
		}

		private bool onSessionStarted(SessionEvents.SessionStartedEvent evt)
		{
			ILocalUser localUser = Service.Get<SessionManager>().LocalUser;
			if (string.IsNullOrEmpty(localUser.RegistrationProfile.Username))
			{
				localUser.RefreshProfile(onProfileUpdated);
			}
			else
			{
				setupSession();
			}
			return false;
		}

		private void onSoftLoginFailed(IRestoreLastSessionResult result)
		{
			CurrentUsername = string.Empty;
		}

		public void ResetCurrentUsername()
		{
			CurrentUsername = string.Empty;
		}

		private bool onLogout(SessionEvents.SessionLogoutEvent evt)
		{
			CurrentUsername = string.Empty;
			return false;
		}

		private void onProfileUpdated(IRefreshProfileResult result)
		{
			if (result.Success)
			{
				setupSession();
			}
		}

		private void setupSession()
		{
			ILocalUser localUser = Service.Get<SessionManager>().LocalUser;
			string username = localUser.RegistrationProfile.Username;
			if (!string.IsNullOrEmpty(username))
			{
				CurrentUsername = username;
				addUsername(username);
				RememberMeData rememberMeData = GetRememberMeData();
				rememberMeData.AccountData = LoadAccountData(username);
				rememberMeData.ResetAccountBan();
			}
		}

		private CacheableType<string> getAccountDataCache(string username)
		{
			return new CacheableType<string>(string.Format("cp.rememberme.{0}.data", username), "{}");
		}

		private KeyChainCacheableString getCredentialDataCache(string username)
		{
			return new KeyChainCacheableString(string.Format("cp.rememberme.{0}.credential", username), string.Empty);
		}

		public RememberMeAccountData LoadAccountData(string username)
		{
			CacheableType<string> accountDataCache = getAccountDataCache(username);
			RememberMeAccountData rememberMeAccountData = null;
			try
			{
				rememberMeAccountData = Service.Get<JsonService>().Deserialize<RememberMeAccountData>(accountDataCache.GetValue());
			}
			catch
			{
			}
			if (rememberMeAccountData == null)
			{
				rememberMeAccountData = new RememberMeAccountData();
			}
			rememberMeAccountData.StoredData = accountDataCache;
			rememberMeAccountData.Username = username;
			rememberMeAccountData.KeychainCredential = getCredentialDataCache(username);
			return rememberMeAccountData;
		}

		[Invokable("Debug.RememberMe.AddUsername")]
		private void addUsername(string username)
		{
			if (Usernames.IndexOf(username) < 0)
			{
				Usernames.Add(username);
				saveUsernames();
			}
		}

		[Invokable("Debug.RememberMe.RemoveUsername")]
		public void RemoveUsername(string username)
		{
			if (Usernames.Remove(username))
			{
				if (CurrentUsername == username)
				{
					CurrentUsername = string.Empty;
				}
				CacheableType<string> accountDataCache = getAccountDataCache(username);
				accountDataCache.SetValue(string.Empty);
				KeyChainCacheableString credentialDataCache = getCredentialDataCache(username);
				credentialDataCache.SetValue(string.Empty);
				saveUsernames();
			}
		}
	}
}
