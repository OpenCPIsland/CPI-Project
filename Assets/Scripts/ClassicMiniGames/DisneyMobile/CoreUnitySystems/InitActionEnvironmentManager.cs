using System.Collections;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems
{
	public class InitActionEnvironmentManager : CrossPlatformInitAction
	{
		public InitActionEnvironmentManager()
			: base(typeof(EnvironmentManager), true, true)
		{
			AddType(RuntimePlatform.IPhonePlayer, typeof(EnvironmentManagerIOS));
			AddType(RuntimePlatform.Android, typeof(EnvironmentManagerAndroid));
		}

		public override IEnumerator Perform()
		{
			CreateInstance();
			if (base.Instance != null)
			{
				EnvironmentManager environmentManager = base.Instance as EnvironmentManager;
				environmentManager.Initialize();
			}
			OnComplete();
			yield return null;
		}
	}
}
