using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Domain.Scene;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Igloo
{
	internal class IglooLayoutReloader : MonoBehaviour
	{
		[LocalizationToken]
		public string UpdateNotice;

		[LocalizationToken]
		public string UpdateCountDown;

		[LocalizationToken]
		public string CloseNotice;

		[LocalizationToken]
		public string CloseCountDown;

		public float CountDownSeconds;

		public PrefabContentKey NotificationPrefab;

		public string ForceLeaveTargetZone;

		private EventDispatcher eventDispatcher;

		private EventChannel eventChannel;

		private Localizer localizer;

		private CPDataEntityCollection dataEntityCollection;

		private SceneLayout newLayout;

		private void Awake()
		{
			localizer = Service.Get<Localizer>();
			eventDispatcher = Service.Get<EventDispatcher>();
			eventChannel = new EventChannel(eventDispatcher);
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			eventChannel.AddListener<IglooServiceEvents.ForceLeave>(onForceLeave);
			eventChannel.AddListener<IglooServiceEvents.IglooUpdated>(onIglooUpdated);
		}

		private bool onIglooUpdated(IglooServiceEvents.IglooUpdated evt)
		{
			newLayout = null;
			eventChannel.AddListenerOneShot<IglooServiceEvents.IglooActiveLayoutLoaded>(onActiveLayoutLoaded);
			Service.Get<INetworkServicesManager>().IglooService.GetActiveIglooLayout(evt.IglooId);
			sendNotification(UpdateNotice, UpdateCountDown);
			CoroutineRunner.Start(reloadScene(), this, "Reload igloo scene");
			return false;
		}

		private bool onForceLeave(IglooServiceEvents.ForceLeave evt)
		{
			if (evt.ZoneId == null)
			{
				evt.ZoneId = new ZoneId();
			}
			if (!evt.ZoneId.isEmpty())
			{
				sendNotification(UpdateNotice, UpdateCountDown);
			}
			else
			{
				evt.ZoneId.name = ForceLeaveTargetZone;
				sendNotification(CloseNotice, CloseCountDown);
			}
			CoroutineRunner.Start(changeScene(evt.ZoneId), this, "Change igloo scene");
			return false;
		}

		private IEnumerator changeScene(ZoneId zoneId)
		{
			yield return new WaitForSeconds(CountDownSeconds);
			eventDispatcher.DispatchEvent(new IglooEvents.ChangeZone(zoneId));
		}

		private IEnumerator reloadScene()
		{
			yield return new WaitForSeconds(CountDownSeconds);
			while (newLayout == null)
			{
				yield return null;
			}
			eventDispatcher.DispatchEvent(new IglooEvents.ReloadLayout(newLayout));
		}

		private bool onActiveLayoutLoaded(IglooServiceEvents.IglooActiveLayoutLoaded evt)
		{
			newLayout = evt.Layout;
			return false;
		}

		private void sendNotification(string notice, string countDown)
		{
			string iglooOwnerName = getIglooOwnerName();
			string translatedText = string.Format(localizer.GetTokenTranslation(notice), iglooOwnerName) + "\n" + localizer.GetTokenTranslation(countDown);
			eventDispatcher.DispatchEvent(new IglooUIEvents.ShowNotification(translatedText, CountDownSeconds, NotificationPrefab, true, false));
		}

		private string getIglooOwnerName()
		{
			string result = "";
			DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntityByName("ActiveSceneData");
			SceneOwnerData component;
			if (!dataEntityHandle.IsNull && dataEntityCollection.TryGetComponent(dataEntityHandle, out component))
			{
				result = component.Name;
			}
			return result;
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			eventChannel.RemoveAllListeners();
		}
	}
}
