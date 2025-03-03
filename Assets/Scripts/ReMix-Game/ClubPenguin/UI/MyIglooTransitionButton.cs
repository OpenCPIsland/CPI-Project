using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Igloo;
using DevonLocalization.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class MyIglooTransitionButton : MonoBehaviour
	{
		private enum ButtonState
		{
			Enabled,
			Disabled
		}

		private DataEventListener savedIglooListener;

		private SavedIgloosMetaData savedIgloosMetaData;

		private CPDataEntityCollection dataEntityCollection;

		private bool isFirstIglooLoad;

		private bool isInOwnIgloo;

		private void Awake()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			savedIglooListener = dataEntityCollection.When<SavedIgloosMetaData>(dataEntityCollection.LocalPlayerHandle, onSavedIgloosAdded);
		}

		private void Start()
		{
			isInOwnIgloo = IglooButtonUtils.SetButtonState(dataEntityCollection.LocalPlayerHandle, base.gameObject);
		}

		private void OnDestroy()
		{
			savedIglooListener.StopListening();
		}

		public void OnButton()
		{
			if (!isInOwnIgloo)
			{
				Service.Get<ActionConfirmationService>().ConfirmAction(typeof(MyIglooTransitionButton), null, sendPlayerToIgloo);
			}
		}

		public void OnActionGraphButton(GameObject sender)
		{
			if (!isInOwnIgloo && sender.CompareTag("Player"))
			{
				sendPlayerToIgloo();
			}
		}

		private void onSavedIgloosAdded(SavedIgloosMetaData savedIgloosMetaData)
		{
			this.savedIgloosMetaData = savedIgloosMetaData;
			this.savedIgloosMetaData.SavedIgloosUpdated += onSavedIgloosUpdated;
			isFirstIglooLoad = (!savedIgloosMetaData.ActiveIglooId.HasValue || savedIgloosMetaData.ActiveIglooId == 0);
		}

		private void onSavedIgloosUpdated(List<SavedIglooMetaData> savedIglooMetaDataList)
		{
			isFirstIglooLoad = (savedIglooMetaDataList.Count == 0);
		}

		private void sendPlayerToIgloo()
		{
			SceneStateData.SceneState sceneState = SceneStateData.SceneState.Play;
			if (isFirstIglooLoad)
			{
				sceneState = SceneStateData.SceneState.Create;
			}
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			ProfileData component;
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				Service.Get<ZoneTransitionService>().LoadIgloo(component.ZoneId, Service.Get<Localizer>().Language, sceneState);
				Service.Get<ICPSwrveService>().Action("igloo", "igloo_list_visit", "player");
			}
			else
			{
				Log.LogError(this, "Unable to find profileData to load into local players igloo.");
			}
		}
	}
}
