using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

namespace ClubPenguin.Mix
{
	public class KeychainData : IKeychain, IKeychainData
	{
		private KeyChainManager keyChainManager;

		private byte[] localStorageKey = new byte[32];

		public byte[] LocalStorageKey
		{
			get
			{
				return GetOrGenerateLocalStorageKey();
			}
		}

		public byte[] PushNotificationKey
		{
			set
			{
				SetPushNotificationKey(value);
			}
		}

		public event System.Action OnKeyGenWithExistingDBError;

		public KeychainData(KeyChainManager keyChainManager)
		{
			this.keyChainManager = keyChainManager;
		}

		private byte[] GetOrGenerateLocalStorageKey()
		{
			string text = null;
			try
			{
				text = keyChainManager.GetString("SessionUnlockKey");
			}
			catch
			{
			}
			if (string.IsNullOrEmpty(text))
			{
				try
				{
					new RNGCryptoServiceProvider().GetBytes(localStorageKey);
					keyChainManager.PutString("SessionUnlockKey", Convert.ToBase64String(localStorageKey));
					if ((Directory.Exists(Application.persistentDataPath + "/MixSDK/") || Directory.Exists(Application.persistentDataPath + "/KeyValueDatabase/")) && this.OnKeyGenWithExistingDBError != null)
					{
						this.OnKeyGenWithExistingDBError();
					}
				}
				catch (Exception ex)
				{
					Log.LogError(this, "Unable to save SessionUnlockKey");
					Log.LogException(this, ex);
				}
			}
			else
			{
				localStorageKey = Convert.FromBase64String(text);
			}
			return localStorageKey;
		}

		private void SetPushNotificationKey(byte[] aKey)
		{
			if (aKey != null)
			{
				try
				{
					keyChainManager.PutString("PushNotificationUnlockKey", Convert.ToBase64String(aKey));
				}
				catch (Exception ex)
				{
					Log.LogError(this, "Unable to save PushNotificationUnlockKey");
					Log.LogException(this, ex);
				}
			}
			else
			{
				keyChainManager.RemoveString("PushNotificationUnlockKey");
			}
		}
	}
}
