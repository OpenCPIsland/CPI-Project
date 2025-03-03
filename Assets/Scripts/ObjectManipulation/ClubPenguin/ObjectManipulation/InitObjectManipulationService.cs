using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;

namespace ClubPenguin.ObjectManipulation
{
	internal class InitObjectManipulationService : InitActionComponent
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
			Service.Set(new ObjectManipulationService());
			yield break;
		}
	}
}
