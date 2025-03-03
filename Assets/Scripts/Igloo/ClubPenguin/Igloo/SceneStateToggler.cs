using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.Igloo
{
	public class SceneStateToggler : MonoBehaviour
	{
		[Tooltip("Component and Gameobject state will change only when the state is the active one")]
		public SceneStateData.SceneState State;

		public Behaviour[] BehavioursEnabledOnState;

		public Behaviour[] BehavioursDisabledOnState;

		public GameObject[] GamedObjectsEnabledOnState;

		public GameObject[] GamedObjectsDisabledOnState;

		private CPDataEntityCollection dataEntityCollection;

		private SceneStateData sceneStateData;

		private DataEventListener sceneStateListener;

		public event Action<SceneStateToggler> OnToggleComplete;

		public void Awake()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			sceneStateListener = dataEntityCollection.When<SceneStateData>("ActiveSceneData", onSceneStateData);
		}

		private void onSceneStateData(SceneStateData sceneStateData)
		{
			this.sceneStateData = sceneStateData;
			setState(sceneStateData.State);
			StartListeningToStateChange();
		}

		public void OnDestroy()
		{
			if (sceneStateData != null)
			{
				sceneStateData.OnStateChanged -= setState;
			}
			if (sceneStateListener != null)
			{
				sceneStateListener.StopListening();
			}
			this.OnToggleComplete = null;
		}

		public void StartListeningToStateChange()
		{
			sceneStateData.OnStateChanged += setState;
		}

		public void StopListeningToStateChange()
		{
			sceneStateData.OnStateChanged -= setState;
		}

		private void setState(SceneStateData.SceneState sceneState)
		{
			if (sceneState != State)
			{
				return;
			}
			int num = BehavioursDisabledOnState.Length;
			for (int i = 0; i < num; i++)
			{
				if (BehavioursDisabledOnState[i] != null)
				{
					BehavioursDisabledOnState[i].enabled = false;
				}
				else
				{
					Log.LogErrorFormatted(this, "Null game object in disabled behaviour object list for SceneStateToggler in state:  {0}", State);
				}
			}
			num = GamedObjectsDisabledOnState.Length;
			for (int i = 0; i < num; i++)
			{
				if (GamedObjectsDisabledOnState[i] != null)
				{
					GamedObjectsDisabledOnState[i].SetActive(false);
				}
				else
				{
					Log.LogErrorFormatted(this, "Null game object in disable game object list for SceneStateToggler in state:  {0}", State);
				}
			}
			num = GamedObjectsEnabledOnState.Length;
			for (int i = 0; i < num; i++)
			{
				if (GamedObjectsEnabledOnState[i] != null)
				{
					GamedObjectsEnabledOnState[i].SetActive(true);
				}
				else
				{
					Log.LogErrorFormatted(this, "Null game object in enabled game object list for SceneStateToggler in state:  {0}", State);
				}
			}
			num = BehavioursEnabledOnState.Length;
			for (int i = 0; i < num; i++)
			{
				if (BehavioursEnabledOnState[i] != null)
				{
					BehavioursEnabledOnState[i].enabled = true;
				}
				else
				{
					Log.LogErrorFormatted(this, "Null game object in enabled behaviour object list for SceneStateToggler in state:  {0}", State);
				}
			}
			if (this.OnToggleComplete != null)
			{
				this.OnToggleComplete.InvokeSafe(this);
			}
		}
	}
}
