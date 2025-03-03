using ClubPenguin.Core;
using ClubPenguin.UI;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Igloo.UI
{
	public class IglooTrayUIDisabler : MonoBehaviour
	{
		[Header("Ids to be disabled when the state is in Preview state.")]
		public string[] PreviewStateDisabledIds;

		[Header("Ids to be disabled when the state is in any state but Preview.")]
		public string[] NonPreviewDisabledIds;

		private EventDispatcher eventDispatcher;

		private SceneStateData sceneStateData;

		private DataEventListener sceneStateDataListener;

		private void Awake()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
		}

		private IEnumerator Start()
		{
			yield return null;
			CPDataEntityCollection dataEntityCollection = Service.Get<CPDataEntityCollection>();
			sceneStateDataListener = dataEntityCollection.When<SceneStateData>("ActiveSceneData", onSceneDataAdded);
		}

		private void onSceneDataAdded(SceneStateData stateData)
		{
			sceneStateData = stateData;
			sceneStateData.OnStateChanged += onSceneStateDataChanged;
			changeElementDisabledState(sceneStateData.State);
		}

		private void onSceneStateDataChanged(SceneStateData.SceneState state)
		{
			changeElementDisabledState(state);
		}

		private void changeElementDisabledState(SceneStateData.SceneState state)
		{
			if (state == SceneStateData.SceneState.Preview)
			{
				enableTrayUIElements(NonPreviewDisabledIds);
				disableTrayUIElements(PreviewStateDisabledIds);
			}
			else
			{
				enableTrayUIElements(PreviewStateDisabledIds);
				disableTrayUIElements(NonPreviewDisabledIds);
			}
		}

		private void disableTrayUIElements(string[] ids)
		{
			for (int i = 0; i < ids.Length; i++)
			{
				eventDispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElement(ids[i]));
			}
		}

		private void enableTrayUIElements(string[] ids)
		{
			for (int i = 0; i < ids.Length; i++)
			{
				eventDispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElement(ids[i]));
			}
		}

		private void OnDestroy()
		{
			if (sceneStateData != null)
			{
				sceneStateData.OnStateChanged -= onSceneStateDataChanged;
			}
			if (sceneStateDataListener != null)
			{
				sceneStateDataListener.StopListening();
			}
		}
	}
}
