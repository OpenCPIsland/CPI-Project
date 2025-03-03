using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitCoreServicesAction))]
	public class InitBootPopupCanvas : InitActionComponent
	{
		public PopupManager PopupCanvas;

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
			yield break;
		}
	}
}
