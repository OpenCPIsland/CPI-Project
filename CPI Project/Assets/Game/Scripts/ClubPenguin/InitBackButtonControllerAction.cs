using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitCoreServicesAction))]
	public class InitBackButtonControllerAction : InitActionComponent
	{
		[SerializeField]
		private BackButtonController backButtonController = null;

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

		private void OnValidate()
		{
		}

		public override IEnumerator PerformFirstPass()
		{
			Service.Set(backButtonController);
			backButtonController.gameObject.SetActive(true);
			yield return null;
		}
	}
}
