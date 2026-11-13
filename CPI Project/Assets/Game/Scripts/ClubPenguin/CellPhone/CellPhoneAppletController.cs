using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.CellPhone
{
	public class CellPhoneAppletController : MonoBehaviour
	{
		public const string APPLET_SCENE_PREFAB_TO_LOAD = "appletToLaunch";

		public GameObject CanvasContainer;

		private EventChannel events;

		private PresenceData localPresenceData;

		public void OnValidate()
		{
			if (CanvasContainer == null)
			{
				throw new NullReferenceException("CellPhoneAppletController does not have a canvas container assigned via the editor inspector.");
			}
		}

		public void Start()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			localPresenceData = cPDataEntityCollection.GetComponent<PresenceData>(cPDataEntityCollection.LocalPlayerHandle);
			if (localPresenceData != null)
			{
				localPresenceData.IsNotInCurrentRoomsScene = true;
			}
			else
			{
				Log.LogError(this, "Unable to set IsNotInCurrentRoomsScene. Jump to friends may be broken.");
			}
			PrefabContentKey prefabContentKey = (PrefabContentKey)Service.Get<SceneTransitionService>().GetSceneArg("appletToLaunch");
			if (prefabContentKey == null)
			{
				CloseApplet();
				throw new InvalidOperationException("CellPhoneAppletController was added in a scene that did not have a APPLET_SCENE_ARG_KEY value");
			}
			events = new EventChannel(Service.Get<EventDispatcher>());
			events.AddListenerOneShot<CellPhoneEvents.ReturnToHomeScreen>(onCloseApplet);
			CoroutineRunner.Start(loadAndInitPrefab(prefabContentKey), this, "loadAndInitPrefab");
		}

		public void OnDestroy()
		{
			if (localPresenceData != null)
			{
				localPresenceData.IsNotInCurrentRoomsScene = false;
			}
			events.RemoveAllListeners();
		}

		private IEnumerator loadAndInitPrefab(PrefabContentKey prefabKey)
		{
			AssetRequest<GameObject> request = Content.LoadAsync(prefabKey);
			yield return request;
			UnityEngine.Object.Instantiate(request.Asset, CanvasContainer.transform, false);
			yield return null;
			Service.Get<LoadingController>().RemoveLoadingSystem(CellPhoneController.LoadingSystemObject);
		}

		private bool onCloseApplet(CellPhoneEvents.ReturnToHomeScreen evt)
		{
			CloseApplet();
			return false;
		}

		public void CloseApplet()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add(SceneTransitionService.SceneArgs.ShowCellPhoneOnEnterScene.ToString(), true);
			Dictionary<string, object> sceneArgs = dictionary;
			Service.Get<LoadingController>().AddLoadingSystem(CellPhoneController.LoadingSystemObject);
			Service.Get<GameStateController>().ReturnToZoneScene(sceneArgs);
		}
	}
}
