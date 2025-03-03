using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Mix.Native;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitCoreServicesAction))]
	public class InitKeyboardManagerAction : InitActionComponent
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
			GameObject gameObject = new GameObject();
			NativeKeyboardManager nativeKeyboardManager = gameObject.AddComponent<NativeKeyboardManager>();
			nativeKeyboardManager.Init();
			Service.Set(nativeKeyboardManager);
			gameObject.transform.SetParent(Service.Get<GameObject>().transform);
			yield break;
		}
	}
}
