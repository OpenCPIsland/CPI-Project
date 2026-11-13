using System.Collections;

namespace ClubPenguin.Tests
{
	public class ConnectToZoneTest : BaseZoneTranstionTest
	{
		protected override IEnumerator setup()
		{
			yield return base.setup();
			eventDispatcher.AddListener<ZoneTransitionEvents.ZoneTransition>(base.onZoneTransition);
		}

		protected override IEnumerator runTest()
		{
			zts.ConnectToZone(TestZoneDefinition.ZoneName);
			yield return base.runTest();
		}

		protected override void tearDown()
		{
		}
	}
}
