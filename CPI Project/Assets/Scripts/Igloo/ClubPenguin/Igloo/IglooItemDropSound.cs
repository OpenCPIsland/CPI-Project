using ClubPenguin.Core;
using ClubPenguin.ObjectManipulation.Input;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Igloo
{
	internal class IglooItemDropSound : MonoBehaviour
	{
		public string SoundPath = "SFX/UI/Igloo/ItemDrop";

		private ObjectManipulationInputController objectManipulationInputController;

		private InteractionState previousState = InteractionState.DisabledInput;

		private void OnEnable()
		{
			objectManipulationInputController = SceneRefs.Get<ObjectManipulationInputController>();
			objectManipulationInputController.InteractionStateChanged += onObjectManipulationInputControllerInteractionStateChanged;
		}

		private void OnDisable()
		{
			objectManipulationInputController.InteractionStateChanged -= onObjectManipulationInputControllerInteractionStateChanged;
		}

		private void onObjectManipulationInputControllerInteractionStateChanged(InteractionState state)
		{
			if (state == InteractionState.ActiveSelectedItem && previousState == InteractionState.DragItem)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new IglooEvents.PlayIglooSound(SoundPath));
			}
			previousState = state;
		}
	}
}
