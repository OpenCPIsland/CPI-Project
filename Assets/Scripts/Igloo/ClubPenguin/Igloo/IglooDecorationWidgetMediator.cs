#define UNITY_ASSERTIONS
using ClubPenguin.ObjectManipulation;
using ClubPenguin.SceneManipulation;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace ClubPenguin.Igloo
{
	internal class IglooDecorationWidgetMediator : MonoBehaviour
	{
		public SceneManipulationService SceneManipulationServiceRef;

		public PrefabContentKey PrefabRemoveDecorationContentKey;

		private IglooRemoveDecorationUIController decorationOptionsUIInstance;

		private EventChannel eventChannel;

		private void Awake()
		{
			Assert.IsNotNull(SceneManipulationServiceRef);
			Assert.IsNotNull(PrefabRemoveDecorationContentKey);
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<IglooUIEvents.ShowSelectedUIWidget>(onShowSelectedUIWidget);
			SceneManipulationServiceRef.ObjectDeselected += onObjectedDeselected;
			SceneManipulationServiceRef.ObjectRemoved += onObjectRemoved;
			Content.LoadAsync(onRemoveDecorationPrefabLoaded, PrefabRemoveDecorationContentKey);
		}

		private void Start()
		{
		}

		private void onRemoveDecorationPrefabLoaded(string key, GameObject asset)
		{
			GameObject gameObject = Object.Instantiate(asset, base.transform);
			decorationOptionsUIInstance = gameObject.GetComponent<IglooRemoveDecorationUIController>();
		}

		private bool onShowSelectedUIWidget(IglooUIEvents.ShowSelectedUIWidget evt)
		{
			if (decorationOptionsUIInstance == null)
			{
				Content.LoadAsync(onRemoveDecorationPrefabLoaded, PrefabRemoveDecorationContentKey);
				CoroutineRunner.Start(delayTillLoadComplete(evt.ManipulatableObject, evt.BoundsForCameraTarget, evt.MinCameraDistance), this, "Delay for Widget load");
			}
			else
			{
				setNewDecorationDetails(evt.ManipulatableObject, evt.BoundsForCameraTarget, evt.MinCameraDistance);
			}
			return false;
		}

		private IEnumerator delayTillLoadComplete(ManipulatableObject manipulatableObject, Bounds boundsForCameraTarget, float minCameraDistance)
		{
			while (decorationOptionsUIInstance == null)
			{
				yield return null;
			}
			setNewDecorationDetails(manipulatableObject, boundsForCameraTarget, minCameraDistance);
		}

		private void setNewDecorationDetails(ManipulatableObject manipulatableObject, Bounds boundsForCameraTarget, float minCameraDistance)
		{
			if (!decorationOptionsUIInstance.IsInitialized)
			{
				decorationOptionsUIInstance.Init(SceneManipulationServiceRef, boundsForCameraTarget, minCameraDistance);
			}
			decorationOptionsUIInstance.SetNewDecoration(manipulatableObject);
		}

		private void onObjectRemoved(ManipulatableObject mo)
		{
			hideDecorationUIWidget();
		}

		private void onObjectedDeselected(ManipulatableObject mo)
		{
			hideDecorationUIWidget();
		}

		private void hideDecorationUIWidget()
		{
			if (decorationOptionsUIInstance != null)
			{
				decorationOptionsUIInstance.UnsetDecoration();
			}
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			eventChannel.RemoveAllListeners();
			SceneManipulationServiceRef.ObjectDeselected -= onObjectedDeselected;
			SceneManipulationServiceRef.ObjectRemoved -= onObjectRemoved;
			if (decorationOptionsUIInstance != null)
			{
				Object.Destroy(decorationOptionsUIInstance.gameObject);
			}
		}
	}
}
