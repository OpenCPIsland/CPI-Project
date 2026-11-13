using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitCoreServicesAction))]
	[RequireComponent(typeof(InitAdventureSystemAction))]
	[RequireComponent(typeof(InitNetworkControllerAction))]
	[RequireComponent(typeof(InitDataModelAction))]
	[RequireComponent(typeof(InitZoneTransitionServiceAction))]
	public class InitGameStateControllerAction : InitActionComponent
	{
		public GameStateController GameStateController;

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
			if (GameStateController == null)
			{
				throw new MissingReferenceException("Game state controller is not set");
			}
			Service.Set(GameStateController);
			GameStateController.gameObject.SetActive(true);
			yield break;
		}
	}
}
