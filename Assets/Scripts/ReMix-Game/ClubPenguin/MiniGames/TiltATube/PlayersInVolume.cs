using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.MiniGames.TiltATube
{
	public class PlayersInVolume : MonoBehaviour
	{
		[Tooltip("If not blank, events will be sent using this identifier")]
		public string VolumeId;

		private CPDataEntityCollection dataEntityCollection;

		private EventDispatcher dispatcher;

		private HashSet<string> players = new HashSet<string>();

		private HashSet<GameObject> objects = new HashSet<GameObject>();

		private HashSet<RemotePlayerData> playerRemovedListeners = new HashSet<RemotePlayerData>();

		private void Awake()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			dispatcher = Service.Get<EventDispatcher>();
		}

		private void OnDestroy()
		{
			foreach (RemotePlayerData playerRemovedListener in playerRemovedListeners)
			{
				playerRemovedListener.PlayerRemoved -= onPlayerDisconnect;
			}
			playerRemovedListeners.Clear();
		}

		private void OnTriggerEnter(Collider coll)
		{
			if (coll.gameObject.CompareTag("Player") || coll.gameObject.CompareTag("RemotePlayer"))
			{
				GameObject gameObject = coll.gameObject;
				increasePlayers(gameObject);
			}
		}

		private void OnTriggerExit(Collider coll)
		{
			if (coll.gameObject.CompareTag("Player") || coll.gameObject.CompareTag("RemotePlayer"))
			{
				GameObject gameObject = coll.gameObject;
				decreasePlayers(gameObject);
			}
		}

		private void increasePlayers(GameObject playerObj)
		{
			string playerName = getPlayerName(playerObj);
			players.Add(playerName);
			if (playerObj.CompareTag("RemotePlayer"))
			{
				AvatarDataHandle component = playerObj.GetComponent<AvatarDataHandle>();
				if (component != null)
				{
					addRemotePlayer(component.Handle);
				}
			}
			objects.Add(playerObj);
			if (dispatcher != null && !string.IsNullOrEmpty(VolumeId))
			{
				dispatcher.DispatchEvent(new TiltATubesEvents.AddPlayer(VolumeId, playerObj, isTubing(playerObj)));
			}
		}

		private void decreasePlayers(GameObject playerObj)
		{
			string playerName = getPlayerName(playerObj);
			if (players.Contains(playerName))
			{
				players.Remove(playerName);
			}
			if (playerObj.CompareTag("RemotePlayer"))
			{
				AvatarDataHandle component = playerObj.GetComponent<AvatarDataHandle>();
				if (component != null)
				{
					deleteRemotePlayer(component.Handle);
				}
			}
			if (dispatcher != null && !string.IsNullOrEmpty(VolumeId))
			{
				dispatcher.DispatchEvent(new TiltATubesEvents.RemovePlayer(VolumeId, playerObj, isTubing(playerObj)));
			}
			objects.Remove(playerObj);
		}

		private string getPlayerName(GameObject playerObj)
		{
			DataEntityHandle handle;
			DisplayNameData component;
			if (AvatarDataHandle.TryGetPlayerHandle(playerObj, out handle) && dataEntityCollection.TryGetComponent(handle, out component))
			{
				return component.DisplayName;
			}
			return "Error: name not found";
		}

		private string getPlayerName(DataEntityHandle playerHandle)
		{
			DisplayNameData component;
			if (dataEntityCollection.TryGetComponent(playerHandle, out component))
			{
				return component.DisplayName;
			}
			return "Error: name not found";
		}

		private string getPlayerName(RemotePlayerData remotePlayerData)
		{
			DataEntityHandle entityByComponent = dataEntityCollection.GetEntityByComponent(remotePlayerData);
			DisplayNameData component;
			if (dataEntityCollection.TryGetComponent(entityByComponent, out component))
			{
				return component.DisplayName;
			}
			return "Error: name not found";
		}

		public int PlayerCount()
		{
			return players.Count;
		}

		public int ActiveTubeCount()
		{
			int num = 0;
			foreach (GameObject @object in objects)
			{
				if (isTubing(@object))
				{
					num++;
				}
			}
			return num;
		}

		private bool isTubing(GameObject playerObj)
		{
			if (playerObj != null && LocomotionHelper.IsCurrentControllerOfType<SlideController>(playerObj))
			{
				return true;
			}
			return false;
		}

		private void onPlayerDisconnect(RemotePlayerData remotePlayerData)
		{
			if (deleteRemotePlayer(remotePlayerData))
			{
				DataEntityHandle entityByComponent = dataEntityCollection.GetEntityByComponent(remotePlayerData);
				GameObjectReferenceData component;
				DisplayNameData component2;
				if (Service.Get<CPDataEntityCollection>().TryGetComponent(entityByComponent, out component))
				{
					decreasePlayers(component.GameObject);
				}
				else if (dataEntityCollection.TryGetComponent(entityByComponent, out component2) && dispatcher != null && !string.IsNullOrEmpty(VolumeId))
				{
					dispatcher.DispatchEvent(new TiltATubesEvents.DisconnectPlayer(VolumeId, component2.DisplayName));
				}
			}
		}

		private bool addRemotePlayer(DataEntityHandle remotePlayerHandle)
		{
			RemotePlayerData component = dataEntityCollection.GetComponent<RemotePlayerData>(remotePlayerHandle);
			component.PlayerRemoved += onPlayerDisconnect;
			return playerRemovedListeners.Add(component);
		}

		private bool addRemotePlayer(RemotePlayerData remotePlayerData)
		{
			remotePlayerData.PlayerRemoved += onPlayerDisconnect;
			return playerRemovedListeners.Add(remotePlayerData);
		}

		private bool deleteRemotePlayer(DataEntityHandle remotePlayerHandle)
		{
			RemotePlayerData component = dataEntityCollection.GetComponent<RemotePlayerData>(remotePlayerHandle);
			component.PlayerRemoved -= onPlayerDisconnect;
			return playerRemovedListeners.Remove(component);
		}

		private bool deleteRemotePlayer(RemotePlayerData remotePlayerData)
		{
			remotePlayerData.PlayerRemoved -= onPlayerDisconnect;
			return playerRemovedListeners.Remove(remotePlayerData);
		}
	}
}
