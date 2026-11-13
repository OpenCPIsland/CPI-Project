using ClubPenguin.DecorationInventory;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitGameDataAction))]
	[RequireComponent(typeof(InitCoreServicesAction))]
	[RequireComponent(typeof(InitDataModelAction))]
	public class InitDecorationInventoryServiceAction : InitActionComponent
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
			Service.Set(new DecorationInventoryService());
			yield break;
		}
	}
}
