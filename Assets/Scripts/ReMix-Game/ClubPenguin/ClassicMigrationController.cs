using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common.DataModel;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin
{
	public class ClassicMigrationController : MonoBehaviour
	{
		public string MigrateSuccessEvent;

		protected StateMachine rootStateMachine;

		protected INetworkServicesManager networkServiceManager;

		public event Action<PlayerStateServiceErrors.LegacyAccountMigrationError> OnLoginError;

		private void Start()
		{
			networkServiceManager = Service.Get<INetworkServicesManager>();
			rootStateMachine = GetComponent<StateMachine>();
		}

		private void OnDestroy()
		{
			this.OnLoginError = null;
		}

		public void Login(DLoginPayload payload)
		{
			CPIDCredentials cpidCreds = default(CPIDCredentials);
			cpidCreds.username = payload.Username;
			cpidCreds.password = payload.Password;
			Service.Get<EventDispatcher>().AddListener<PlayerStateServiceEvents.MigrationDataRecieved>(onLoginSuccess);
			Service.Get<EventDispatcher>().AddListener<PlayerStateServiceErrors.LegacyAccountMigrationError>(onLoginFailed);
			if (networkServiceManager != null)
			{
				if (networkServiceManager.PlayerStateService != null)
				{
					networkServiceManager.PlayerStateService.MigrateLegacy(cpidCreds);
				}
				else
				{
					Log.LogError(this, "networkServiceManager.PlayerStateService not initialized for classic migration login");
				}
			}
			else
			{
				Log.LogError(this, "NetworkServiceManager not initialized for classic migration login");
			}
		}

		private bool onLoginSuccess(PlayerStateServiceEvents.MigrationDataRecieved recievedData)
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle localPlayerHandle = cPDataEntityCollection.LocalPlayerHandle;
			if (recievedData.Data != null && !localPlayerHandle.IsNull)
			{
				ProfileData component;
				if (!cPDataEntityCollection.TryGetComponent(localPlayerHandle, out component))
				{
					component = cPDataEntityCollection.AddComponent<ProfileData>(localPlayerHandle);
				}
				component.IsMigratedPlayer = true;
				LegacyProfileData component2;
				if (!cPDataEntityCollection.TryGetComponent(localPlayerHandle, out component2))
				{
					component2 = cPDataEntityCollection.AddComponent<LegacyProfileData>(localPlayerHandle);
				}
				component2.IsMember = recievedData.Data.legacyAccountData.member;
				component2.Username = recievedData.Data.legacyAccountData.username;
				component2.CreatedDate = recievedData.Data.legacyAccountData.createdDate;
				component2.MigratedDate = recievedData.Data.migratedDate;
			}
			rootStateMachine.SendEvent(MigrateSuccessEvent);
			return false;
		}

		private bool onLoginFailed(PlayerStateServiceErrors.LegacyAccountMigrationError migrationError)
		{
			if (this.OnLoginError != null)
			{
				this.OnLoginError(migrationError);
			}
			return false;
		}
	}
}
