using System;
using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Environment
{
	public class EnvironmentSelector
	{
		private Environment currEnvironment = Environment.PRODUCTION;

		private EnvironmentControl selectionController;

		public event Action<Environment> EnvironmentChanged;

		public void SetCurrentEnvironment(Environment env)
		{
			currEnvironment = env;
		}

		public void DisplaySelector(Transform parent)
		{
			selectionController = UnityEngine.Object.Instantiate(Content.LoadImmediate<EnvironmentControl>("Prefabs/EnvironmentSelector"), parent);
			selectionController.gameObject.SetActive(true);
			selectionController.OnEnvironmentSet = onSelectEnvironment;
			CoroutineRunner.Start(configureController(), this, "configureController");
		}

		public void RemoveSelector()
		{
			UnityEngine.Object.Destroy(selectionController.gameObject);
		}

		private IEnumerator configureController()
		{
			yield return null;
			selectionController.ConfigureSelectEnvironmentButons();
			selectionController.ConfigureCurrentEnvironmentButton(currEnvironment);
		}

		private void onSelectEnvironment(Environment env)
		{
			if (currEnvironment != env && this.EnvironmentChanged != null)
			{
				this.EnvironmentChanged(env);
			}
		}
	}
}
