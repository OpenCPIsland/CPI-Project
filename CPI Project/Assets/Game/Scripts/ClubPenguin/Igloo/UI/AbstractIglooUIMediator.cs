using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Igloo.UI
{
	[RequireComponent(typeof(StateMachineContextListener))]
	public abstract class AbstractIglooUIMediator : MonoBehaviour
	{
		protected StateMachineContextListener contextListener;

		protected StateMachineContext context;

		protected SceneStateData sceneStateData;

		protected SceneLayoutDataManager layoutManager;

		protected CPDataEntityCollection dataEntityCollection;

		protected EventDispatcher eventDispatcher;

		protected EventChannel eventChannel;

		private DataEventListener stateDataListener;

		protected virtual void Awake()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			layoutManager = Service.Get<SceneLayoutDataManager>();
			eventDispatcher = Service.Get<EventDispatcher>();
			contextListener = GetComponent<StateMachineContextListener>();
			stateDataListener = dataEntityCollection.When<SceneStateData>(layoutManager.GetActiveHandle(), onSceneStateData);
			eventChannel = new EventChannel(eventDispatcher);
		}

		protected virtual void OnDestroy()
		{
			if (stateDataListener != null)
			{
				stateDataListener.StopListening();
			}
			if (sceneStateData != null)
			{
				sceneStateData.OnStateChanged -= onStateChanged;
			}
			eventChannel.RemoveAllListeners();
			CoroutineRunner.StopAllForOwner(this);
		}

		protected virtual void onSceneStateData(SceneStateData sceneStateData)
		{
			this.sceneStateData = sceneStateData;
			this.sceneStateData.OnStateChanged += onStateChanged;
		}

		protected virtual void onStateChanged(SceneStateData.SceneState state)
		{
		}

		protected void setState(SceneStateData.SceneState state)
		{
			sceneStateData.State = state;
		}

		protected void resetUI()
		{
			if (context != null)
			{
				context.SendEvent(new ExternalEvent("IglooScreenContainerContent", "igloonone"));
			}
		}
	}
}
