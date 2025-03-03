using Disney.Kelowna.Common.DataModel;
using Disney.Kelowna.Common.Tests;
using System.Collections;

namespace ClubPenguin.Tests
{
	public class AddFriendsDataTests : BaseMixIntegrationTest
	{
		private DataEntityCollection dataEntityCollection;

		protected override IEnumerator setup()
		{
			dataEntityCollection = new DataEntityCollectionDictionaryImpl();
			yield return null;
		}

		protected override IEnumerator runTest()
		{
			yield break;
		}

		protected override void tearDown()
		{
			base.tearDown();
		}
	}
}
