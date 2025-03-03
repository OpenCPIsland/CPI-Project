using ClubPenguin.Core;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Tubes
{
	public class TubeButtonIconLoader : MonoBehaviour
	{
		private const int DISABLED_ICON_INDEX = 0;

		private const int OFF_ICON_INDEX = 1;

		private const int ON_ICON_INDEX = 2;

		private const int HIGHLIGHT_ICON_INDEX = 3;

		private const int DEFAULT_TUBE_ID = 0;

		private TrayInputButton trayInputButton;

		private TubeData tubeData;

		private Dictionary<int, TubeDefinition> tubeDefinitions;

		private void Start()
		{
			trayInputButton = GetComponentInParent<TrayInputButton>();
			getTubeData();
			getTubeDefinitions();
			if (tubeData != null)
			{
				onTubeSelected(tubeData.SelectedTubeId);
				return;
			}
			onTubeSelected(0);
			Service.Get<EventDispatcher>().AddListener<NetworkControllerEvents.LocalPlayerDataReadyEvent>(onLocalPlayerDataReady);
		}

		private void getTubeData()
		{
			if (Service.Get<CPDataEntityCollection>().TryGetComponent(Service.Get<CPDataEntityCollection>().LocalPlayerHandle, out tubeData))
			{
				tubeData.OnTubeSelected += onTubeSelected;
			}
		}

		private void getTubeDefinitions()
		{
			tubeDefinitions = Service.Get<GameData>().Get<Dictionary<int, TubeDefinition>>();
		}

		private void onTubeSelected(int tubeId)
		{
			TubeDefinition tubeDefinition;
			if (tryGetTubeDefinitionById(tubeId, out tubeDefinition))
			{
				CoroutineRunner.Start(loadIconsForTubeDefinition(tubeDefinition), this, "");
			}
		}

		private bool tryGetTubeDefinitionById(int tubeId, out TubeDefinition tubeDefinition)
		{
			foreach (KeyValuePair<int, TubeDefinition> tubeDefinition2 in tubeDefinitions)
			{
				if (tubeDefinition2.Key == tubeId)
				{
					tubeDefinition = tubeDefinition2.Value;
					return true;
				}
			}
			tubeDefinition = null;
			return false;
		}

		private IEnumerator loadIconsForTubeDefinition(TubeDefinition tubeDefinition)
		{
			CoroutineGroup coroutineGroup = new CoroutineGroup();
			addCoroutineToGroup(CoroutineRunner.Start(loadIcon(0, tubeDefinition.ButtonIconKeyDisabled.Key), this, ""), coroutineGroup);
			addCoroutineToGroup(CoroutineRunner.Start(loadIcon(1, tubeDefinition.ButtonIconKeyOff.Key), this, ""), coroutineGroup);
			addCoroutineToGroup(CoroutineRunner.Start(loadIcon(2, tubeDefinition.ButtonIconKeyOn.Key), this, ""), coroutineGroup);
			addCoroutineToGroup(CoroutineRunner.Start(loadIcon(3, tubeDefinition.ButtonIconKeyOn.Key), this, ""), coroutineGroup);
			while (!coroutineGroup.IsFinished)
			{
				yield return null;
			}
			coroutineGroup.Clear();
			trayInputButton.Icon.sprite = trayInputButton.IconSprite.Sprites[(int)trayInputButton.CurrentViewState];
		}

		private void addCoroutineToGroup(ICoroutine coroutine, CoroutineGroup group)
		{
			if (!coroutine.Disposed && !coroutine.Cancelled && !coroutine.Completed)
			{
				group.Add(coroutine);
			}
		}

		private IEnumerator loadIcon(int iconIndex, string iconPath)
		{
			if (this != null)
			{
				AssetRequest<Sprite> assetRequest = Content.LoadAsync<Sprite>(iconPath);
				yield return assetRequest;
				trayInputButton.IconSprite.Sprites[iconIndex] = assetRequest.Asset;
			}
		}

		private bool onLocalPlayerDataReady(NetworkControllerEvents.LocalPlayerDataReadyEvent evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<NetworkControllerEvents.LocalPlayerDataReadyEvent>(onLocalPlayerDataReady);
			getTubeData();
			return false;
		}

		private void OnDestroy()
		{
			if (tubeData != null)
			{
				tubeData.OnTubeSelected -= onTubeSelected;
			}
			CoroutineRunner.StopAllForOwner(this);
			Service.Get<EventDispatcher>().RemoveListener<NetworkControllerEvents.LocalPlayerDataReadyEvent>(onLocalPlayerDataReady);
		}
	}
}
