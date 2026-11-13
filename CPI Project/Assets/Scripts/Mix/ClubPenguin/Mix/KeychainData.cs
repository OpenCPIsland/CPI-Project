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

			bool needsNewKey = true;
			if (!string.IsNullOrEmpty(text))
			{
				try
				{
					byte[] decoded = Convert.FromBase64String(text);
					if (decoded != null && decoded.Length == 32)
					{
						localStorageKey = decoded;
						needsNewKey = false;
					}
				}
				catch
				{
					needsNewKey = true;
				}
			}

			if (needsNewKey)
			{
				try
				{
					string mixSdkPath = Path.Combine(Application.persistentDataPath, "MixSDK");
					string keyValueDbPath = Path.Combine(Application.persistentDataPath, "KeyValueDatabase");
					bool hasExistingDb = Directory.Exists(mixSdkPath) || Directory.Exists(keyValueDbPath);
					if (hasExistingDb)
					{
						try
						{
							if (Directory.Exists(mixSdkPath))
							{
								Directory.Delete(mixSdkPath, true);
							}
							if (Directory.Exists(keyValueDbPath))
							{
								Directory.Delete(keyValueDbPath, true);
							}
						}
						catch (Exception ex)
						{
							Log.LogError(this, "Unable to delete existing encrypted databases after SessionUnlockKey was missing/invalid");
							Log.LogException(this, ex);
						}
						if (this.OnKeyGenWithExistingDBError != null)
						{
							this.OnKeyGenWithExistingDBError();
						}
					}

					new RNGCryptoServiceProvider().GetBytes(localStorageKey);
					keyChainManager.PutString("SessionUnlockKey", Convert.ToBase64String(localStorageKey));
				}
				catch (Exception ex)
				{
					Log.LogError(this, "Unable to save SessionUnlockKey");
					Log.LogException(this, ex);
				}
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
