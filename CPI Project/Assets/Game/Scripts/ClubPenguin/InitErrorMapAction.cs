using ClubPenguin.Error;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitContentSystemAction))]
	[RequireComponent(typeof(InitCoreServicesAction))]
	public class InitErrorMapAction : InitActionComponent
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
			ErrorsMap errorsMap = new ErrorsMap();
			errorsMap.LoadErrorJson("Errors/errors.json");
			Service.Set(errorsMap);
			yield break;
		}
	}
}
