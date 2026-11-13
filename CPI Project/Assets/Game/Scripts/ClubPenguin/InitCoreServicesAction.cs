#define UNITY_ASSERTIONS
using ClubPenguin.Core;
using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace ClubPenguin
{
	public class InitCoreServicesAction : InitActionComponent
	{
		public GameObject ServicesContainer;

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

		public void OnValidate()
		{
			Assert.IsFalse(ServicesContainer == null);
		}

		public override IEnumerator PerformFirstPass()
		{
			Screen.autorotateToLandscapeLeft = false;
			Screen.autorotateToLandscapeRight = false;
			Screen.autorotateToPortrait = true;
			Screen.autorotateToPortraitUpsideDown = true;
			Screen.orientation = ScreenOrientation.AutoRotation;
			AppWindowUtil.StartCustomWindowManager();
			Service.Set(new EventDispatcher());
			ConnectionManager instance = ServicesContainer.AddComponent<ConnectionManager>();
			Service.Set(instance);
			Object.DontDestroyOnLoad(ServicesContainer);
			Service.Set(ServicesContainer);
			GameSettings instance2 = new GameSettings();
			Application.targetFrameRate = 30;
			Service.Set(instance2);
			Service.Set((ICommonGameSettings)instance2);
			Service.Set((JsonService)new LitJsonService());
			Service.Set(new ActionConfirmationService());
			yield break;
		}
	}
}
