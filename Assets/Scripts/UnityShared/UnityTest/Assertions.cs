using UnityEngine;

namespace UnityTest
{
	public static class Assertions
	{
		public static void CheckAssertions()
		{
			AssertionComponent[] assertions = Object.FindObjectsOfType(typeof(AssertionComponent)) as AssertionComponent[];
			CheckAssertions(assertions);
		}

		public static void CheckAssertions(AssertionComponent assertion)
		{
			CheckAssertions(new AssertionComponent[1]
			{
				assertion
			});
		}

		public static void CheckAssertions(GameObject gameObject)
		{
			CheckAssertions(gameObject.GetComponents<AssertionComponent>());
		}

		public static void CheckAssertions(AssertionComponent[] assertions)
		{
			if (!Debug.isDebugBuild)
			{
				return;
			}
			foreach (AssertionComponent assertionComponent in assertions)
			{
				assertionComponent.checksPerformed++;
				if (!assertionComponent.Action.Compare())
				{
					assertionComponent.hasFailed = true;
					assertionComponent.Action.Fail(assertionComponent);
				}
			}
		}
	}
}
