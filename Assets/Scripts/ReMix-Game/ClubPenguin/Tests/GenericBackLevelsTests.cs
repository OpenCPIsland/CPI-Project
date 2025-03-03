using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.Tests;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ClubPenguin.Tests
{
	public class GenericBackLevelsTests : BaseBackButtonControllerIntegrationTest
	{
		private BackButtonController backButtonController;

		private List<Action> level1ExpectedCallback = new List<Action>();

		private List<Action> level2ExpectedCallbacks = new List<Action>();

		private List<Action> level3ExpectedCallbacks = new List<Action>();

		protected override IEnumerator runTest()
		{
			yield return StartCoroutine(base.runTest());
			backButtonController = Service.Get<BackButtonController>();
			backButtonController.Add(Level1BackCallback);
			IntegrationTestEx.FailIf(backButtonController.NumCallbacks() != 1, "FAILED: Level 1 back button controller not properly added.");
			level1ExpectedCallback.Add(Level1BackCallback);
			backButtonController.Execute();
			backButtonController.Add(Level2aBackCallback);
			backButtonController.Add(Level2bBackCallback);
			IntegrationTestEx.FailIf(backButtonController.NumCallbacks() != 2, "FAILED: Level 2 back button controller not properly added.");
			level2ExpectedCallbacks.Add(Level2aBackCallback);
			level2ExpectedCallbacks.Add(Level2bBackCallback);
			backButtonController.Execute();
			backButtonController.Execute();
			backButtonController.Add(Level3aBackCallback);
			backButtonController.Add(Level3bBackCallback);
			backButtonController.Add(Level3cBackCallback);
			IntegrationTestEx.FailIf(backButtonController.NumCallbacks() != 3, "FAILED: Level 3 back button controller not properly added.");
			level3ExpectedCallbacks.Add(Level3aBackCallback);
			level3ExpectedCallbacks.Add(Level3bBackCallback);
			level3ExpectedCallbacks.Add(Level3cBackCallback);
			backButtonController.Execute();
			backButtonController.Execute();
			backButtonController.Execute();
			backButtonController.Add(TestRemovingACallback);
			backButtonController.Remove(TestRemovingACallback);
			IntegrationTestEx.FailIf(backButtonController.NumCallbacks() > 0, "FAILED: callback not removed from back button controller.");
			backButtonController.Add(null);
			backButtonController.Remove(null, true);
			IntegrationTestEx.FailIf(backButtonController.NumCallbacks() > 0, "FAILED: Null callback not removed from back button controller.");
		}

		private void Level1BackCallback()
		{
			MethodBase currentMethod = MethodBase.GetCurrentMethod();
			Action action = level1ExpectedCallback[0];
			IntegrationTestEx.FailIf(!string.Equals(action.Method.Name, currentMethod.Name), "FAILED: Level 1 back button controller called " + currentMethod.Name + " instead of expected callback " + action.Method.Name);
		}

		private void Level2aBackCallback()
		{
			MethodBase currentMethod = MethodBase.GetCurrentMethod();
			Action action = level2ExpectedCallbacks[0];
			IntegrationTestEx.FailIf(!string.Equals(action.Method.Name, currentMethod.Name), "FAILED: Level 2 back button controller called " + currentMethod.Name + " instead of expected callback " + action.Method.Name);
		}

		private void Level2bBackCallback()
		{
			MethodBase currentMethod = MethodBase.GetCurrentMethod();
			Action action = level2ExpectedCallbacks[1];
			IntegrationTestEx.FailIf(!string.Equals(action.Method.Name, currentMethod.Name), "FAILED: Level 2 back button controller called " + currentMethod.Name + " instead of expected callback " + action.Method.Name);
		}

		private void Level3aBackCallback()
		{
			MethodBase currentMethod = MethodBase.GetCurrentMethod();
			Action action = level3ExpectedCallbacks[0];
			IntegrationTestEx.FailIf(!string.Equals(action.Method.Name, currentMethod.Name), "FAILED: Level 3 back button controller called " + currentMethod.Name + " instead of expected callback " + action.Method.Name);
		}

		private void Level3bBackCallback()
		{
			MethodBase currentMethod = MethodBase.GetCurrentMethod();
			Action action = level3ExpectedCallbacks[1];
			IntegrationTestEx.FailIf(!string.Equals(action.Method.Name, currentMethod.Name), "FAILED: Level 3 back button controller called " + currentMethod.Name + " instead of expected callback " + action.Method.Name);
		}

		private void Level3cBackCallback()
		{
			MethodBase currentMethod = MethodBase.GetCurrentMethod();
			Action action = level3ExpectedCallbacks[2];
			IntegrationTestEx.FailIf(!string.Equals(action.Method.Name, currentMethod.Name), "FAILED: Level 3 back button controller called " + currentMethod.Name + " instead of expected callback " + action.Method.Name);
		}

		private void TestRemovingACallback()
		{
		}
	}
}
