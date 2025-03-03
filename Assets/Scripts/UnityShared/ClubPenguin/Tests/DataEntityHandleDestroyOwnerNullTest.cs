using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.Kelowna.Common.Tests;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Tests
{
	public class DataEntityHandleDestroyOwnerNullTest : BaseIntegrationTest
	{
		private DataEntityCollection dataEntityCollection;

		protected override IEnumerator setup()
		{
			dataEntityCollection = new DataEntityCollectionDictionaryImpl();
			yield return null;
		}

		protected override IEnumerator runTest()
		{
			string testEntity1 = "TestEntity1";
			DataEntityHandle handle = dataEntityCollection.AddEntity(testEntity1);
			dataEntityCollection.RemoveEntityByName(testEntity1);
			IntegrationTestEx.FailIf(!handle.IsNull, "DataEntityHandle is still valid");
			GC.Collect();
			yield return null;
			Debug.Log("-*-*-*-*-*-*-*-* Hello there -*-*-*-*-*-*-*");
			IntegrationTestEx.FailIf(!handle.IsNull, "DataEntityHandle is still valid");
		}

		protected override void tearDown()
		{
		}
	}
}
