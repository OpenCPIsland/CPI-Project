using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.Kelowna.Common.Tests;
using System.Collections;
using System.Collections.Generic;

namespace ClubPenguin.Tests
{
	public class DataEntityHandleEqualityTests : BaseIntegrationTest
	{
		private DataEntityCollection dataEntityCollection;

		protected override IEnumerator setup()
		{
			dataEntityCollection = new DataEntityCollectionDictionaryImpl();
			yield return null;
		}

		protected override IEnumerator runTest()
		{
			string entityName = "TestEntity1";
			string entityName2 = "TestEntity2";
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			List<DataEntityHandle> list = new List<DataEntityHandle>();
			list.Add(dataEntityCollection.AddEntity(entityName));
			list.Add(dataEntityCollection.AddEntity(entityName2));
			if (dataEntityCollection.FindEntityByName(entityName) == dataEntityCollection.FindEntityByName(entityName))
			{
				flag = true;
			}
			if (dataEntityCollection.FindEntityByName(entityName) != dataEntityCollection.FindEntityByName(entityName2))
			{
				flag2 = true;
			}
			if (list.Contains(dataEntityCollection.FindEntityByName(entityName2)))
			{
				flag3 = true;
			}
			IntegrationTestEx.FailIf(!flag, "DataEntityHandle Equality Test Failed");
			IntegrationTestEx.FailIf(!flag2, "DataEntityHandle Inequality Test Failed");
			IntegrationTestEx.FailIf(!flag3, "DataEntityHandle List.Contains() Test Failed");
			yield break;
		}

		protected override void tearDown()
		{
		}
	}
}
