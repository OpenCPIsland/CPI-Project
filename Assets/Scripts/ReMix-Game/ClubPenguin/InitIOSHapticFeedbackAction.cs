using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native.iOS;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitCoreServicesAction))]
	public class InitIOSHapticFeedbackAction : InitActionComponent
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
			iOSHapticFeedback instance = Service.Get<GameObject>().AddComponent<iOSHapticFeedback>();
			Service.Set(instance);
			yield break;
		}
	}
}
