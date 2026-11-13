using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Igloo.UI
{
	public abstract class AbstractIglooUIStateController : MonoBehaviour
	{
		protected CPDataEntityCollection dataEntityCollection;

		protected EventDispatcher eventDispatcher;

		protected EventChannel eventChannel;

		private DataEventListener stateDataListener;

		protected SceneStateData sceneStateData;

		protected virtual void Awake()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			eventChannel = new EventChannel(eventDispatcher);
			stateDataListener = dataEntityCollection.When<SceneStateData>(Service.Get<SceneLayoutDataManager>().GetActiveHandle(), onSceneStateData);
		}

		protected virtual void OnDestroy()
		{
			eventChannel.RemoveAllListeners();
			if (stateDataListener != null)
			{
				stateDataListener.StopListening();
			}
			StopListeningToStateChange();
			CoroutineRunner.StopAllForOwner(this);
		}

		protected virtual void onSceneStateData(SceneStateData sceneStateData)
		{
			this.sceneStateData = sceneStateData;
			StartListeningToStateChange();
		}

		public virtual void StartListeningToStateChange()
		{
			if (sceneStateData != null)
			{
				sceneStateData.OnStateChanged += onStateChanged;
			}
		}

		public virtual void StopListeningToStateChange()
		{
			if (sceneStateData != null)
			{
				sceneStateData.OnStateChanged -= onStateChanged;
			}
		}

		protected virtual void onStateChanged(SceneStateData.SceneState state)
		{
		}

		public void SetState(SceneStateData.SceneState state)
		{
			sceneStateData.State = state;
		}
	}
}
