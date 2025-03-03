using ClubPenguin.ObjectManipulation;
using ClubPenguin.UI;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin
{
	internal class IglooPromptManager : MonoBehaviour
	{
		public PromptDefinitionKey confirmObjectRemovalPrompt;

		private void Awake()
		{
			Service.Get<ObjectManipulationService>().ConfirmObjectRemoval = showObjectRemovalConfirmation;
		}

		private void OnDestroy()
		{
			Service.Get<ObjectManipulationService>().ConfirmObjectRemoval = null;
		}

		private void showObjectRemovalConfirmation(int count, Action<bool> callback)
		{
			Service.Get<PromptManager>().ShowPrompt(confirmObjectRemovalPrompt.Id, count.ToString(), count.ToString(), delegate(DPrompt.ButtonFlags result)
			{
				callback(result == DPrompt.ButtonFlags.YES);
			});
		}
	}
}
