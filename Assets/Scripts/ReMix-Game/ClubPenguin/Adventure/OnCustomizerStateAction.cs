using ClubPenguin.ClothingDesigner.ItemCustomizer;
using Disney.LaunchPadFramework;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("GUI")]
	public class OnCustomizerStateAction : FsmStateAction
	{
		public CustomizerState CustomizerState;

		private CustomizerModel model;

		public override void OnEnter()
		{
			CustomizationContext customizationContext = Object.FindObjectOfType<CustomizationContext>();
			if (customizationContext == null)
			{
				Disney.LaunchPadFramework.Log.LogErrorFormatted(this, "Unable to find CustomizationContext, assuming we already in state {0}", CustomizerState);
				Finish();
			}
			else if (customizationContext.CustomizerState == CustomizerState)
			{
				Finish();
			}
			else
			{
				CustomizationContext.EventBus.AddListener<CustomizerModelEvents.CustomizerStateChangedEvent>(onStateChanged);
			}
		}

		private bool onStateChanged(CustomizerModelEvents.CustomizerStateChangedEvent evt)
		{
			if (evt.NewState == CustomizerState)
			{
				Finish();
			}
			return false;
		}

		public override void OnExit()
		{
			CustomizationContext.EventBus.RemoveListener<CustomizerModelEvents.CustomizerStateChangedEvent>(onStateChanged);
		}
	}
}
