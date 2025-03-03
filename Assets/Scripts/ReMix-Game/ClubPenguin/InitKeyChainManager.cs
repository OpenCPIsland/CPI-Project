using ClubPenguin.Mix;
using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitCoreServicesAction))]
	public class InitKeyChainManager : InitActionComponent
	{
		public override bool HasSecondPass
		{
			get
			{
				return false;
			}
		}

		public override bool HasCompletedPass
		{
			get
			{
				return false;
			}
		}

		public override IEnumerator PerformFirstPass()
		{
			GameObject gameObject = Service.Get<GameObject>();
			gameObject.AddComponent<KeyChainWindowsManager>();
			KeychainData instance = new KeychainData(Service.Get<KeyChainManager>());
			Service.Set((IKeychain)instance);
			Service.Set((IKeychainData)instance);
			yield break;
		}
	}
}
