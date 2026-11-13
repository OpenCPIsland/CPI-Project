using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.Kelowna.Common.Tests;
using System.Collections;

namespace ClubPenguin.Tests
{
	public class DataEntityCollectionEventsTest : BaseIntegrationTest
	{
		private DataEntityCollection dataEntityCollection;

		protected override IEnumerator setup()
		{
			dataEntityCollection = new DataEntityCollectionDictionaryImpl();
			yield return null;
		}

		protected override IEnumerator runTest()
		{
			bool addEntityPassed = false;
			bool removeEntityPassed = false;
			bool addComponentPassed = false;
			bool removeComponentPassed = false;
			string testEntity1 = "TestEntity1";
			dataEntityCollection.EventDispatcher.AddListener(delegate(DataEntityEvents.EntityAddedEvent evt)
			{
				if (evt.EntityHandle == dataEntityCollection.FindEntityByName(testEntity1))
				{
					addEntityPassed = true;
				}
				return false;
			});
			dataEntityCollection.EventDispatcher.AddListener(delegate(DataEntityEvents.EntityRemovedEvent evt)
			{
				if (evt.EntityHandle.Id == testEntity1)
				{
					removeEntityPassed = true;
				}
				return false;
			});
			dataEntityCollection.EventDispatcher.AddListener(delegate(DataEntityEvents.ComponentAddedEvent<MockComponent1> evt)
			{
				if (evt.Handle == dataEntityCollection.FindEntityByName(testEntity1) && evt.Component != null)
				{
					addComponentPassed = true;
				}
				return false;
			});
			dataEntityCollection.EventDispatcher.AddListener(delegate(DataEntityEvents.ComponentRemovedEvent evt)
			{
				if (dataEntityCollection.GetEntityByComponent(evt.Component) == dataEntityCollection.FindEntityByName(testEntity1) && evt.Component is MockComponent1)
				{
					removeComponentPassed = true;
				}
				return false;
			});
			DataEntityHandle handle = dataEntityCollection.AddEntity(testEntity1);
			dataEntityCollection.AddComponent<MockComponent1>(handle);
			dataEntityCollection.RemoveComponent<MockComponent1>(handle);
			dataEntityCollection.RemoveEntityByName(testEntity1);
			yield return null;
			IntegrationTestEx.FailIf(!addEntityPassed, "Add Entity Failed");
			IntegrationTestEx.FailIf(!removeEntityPassed, "Remove Entity Failed");
			IntegrationTestEx.FailIf(!addComponentPassed, "Add Component Failed");
			IntegrationTestEx.FailIf(!removeComponentPassed, "Remove Component Failed");
		}

		protected override void tearDown()
		{
		}
	}
}
