#define UNITY_ASSERTIONS
using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.Assertions;

namespace ClubPenguin.Igloo
{
	public class TriggerDynamicLightmap : MonoBehaviour
	{
		private const bool DESTROY_ON_COMPLETE = true;

		private const bool DONT_DESTROY_ON_COMPLETE = false;

		public CreateDepthMap depthMapCreator;

		[Header("Default clear color of the plane")]
		public Color clearColor = Color.grey;

		private DataEventListener dataEventListener;

		private SceneStateData sceneStateData;

		private EventChannel eventChannel;

		private SceneStateData.SceneState previousState = SceneStateData.SceneState.Create;

		private bool isPreviousStateSet;

		public void Start()
		{
			Assert.IsNotNull(depthMapCreator);
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			dataEventListener = cPDataEntityCollection.When<SceneStateData>("ActiveSceneData", onSceneStateDataAdded);
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<SceneTransitionEvents.LayoutGameObjectsLoaded>(onLayoutGameObjectsLoaded);
			eventChannel.AddListener<IglooLightingEvents.RenderIglooDynamicLightmap>(onRenderIglooDynamicLightmap);
		}

		private void onSceneStateDataAdded(SceneStateData sceneStateData)
		{
			this.sceneStateData = sceneStateData;
			sceneStateData.OnStateChanged += onSceneStateDataChanged;
			onSceneStateDataChanged(sceneStateData.State);
		}

		private void onSceneStateDataChanged(SceneStateData.SceneState state)
		{
			if (!isPreviousStateSet || state != previousState)
			{
				switch (state)
				{
				case SceneStateData.SceneState.Play:
				case SceneStateData.SceneState.Preview:
				case SceneStateData.SceneState.Create:
					render(false);
					break;
				case SceneStateData.SceneState.Edit:
					depthMapCreator.Clear(clearColor);
					break;
				}
				previousState = state;
				isPreviousStateSet = true;
			}
		}

		private bool onLayoutGameObjectsLoaded(SceneTransitionEvents.LayoutGameObjectsLoaded evt)
		{
			if (sceneStateData != null && sceneStateData.State == SceneStateData.SceneState.Play)
			{
				render(false);
			}
			return false;
		}

		private bool onRenderIglooDynamicLightmap(IglooLightingEvents.RenderIglooDynamicLightmap evt)
		{
			render(false);
			return false;
		}

		private void render(bool renderOnce)
		{
			depthMapCreator.Render(renderOnce);
		}

		private void OnDestroy()
		{
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
			if (dataEventListener != null)
			{
				dataEventListener.StopListening();
			}
			if (sceneStateData != null)
			{
				sceneStateData.OnStateChanged -= onSceneStateDataChanged;
			}
		}
	}
}
