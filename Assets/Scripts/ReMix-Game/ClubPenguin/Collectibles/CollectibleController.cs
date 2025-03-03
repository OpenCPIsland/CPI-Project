using ClubPenguin.Core;
using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Collectibles
{
	public class CollectibleController : MonoBehaviour
	{
		private SortedList<long, List<Collectible>> respawnList = new SortedList<long, List<Collectible>>();

		private Dictionary<string, Collectible> pendingCollectiblesToRegister = new Dictionary<string, Collectible>();

		private InZoneCollectiblesData currentRoomState;

		private List<long> keysToExpire = new List<long>();

		private void Awake()
		{
			SceneRefs.SetCollectibleController(this);
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out currentRoomState))
			{
				initCurrentRoomState();
			}
			else
			{
				cPDataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<InZoneCollectiblesData>>(onInZoneCollectiblesDataAdded);
			}
		}

		private void Start()
		{
			InvokeRepeating("collectibleHeartbeat", 0f, 1f);
		}

		public void OnDestroy()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			if (cPDataEntityCollection != null)
			{
				cPDataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<InZoneCollectiblesData>>(onInZoneCollectiblesDataAdded);
			}
		}

		private bool onInZoneCollectiblesDataAdded(DataEntityEvents.ComponentAddedEvent<InZoneCollectiblesData> evt)
		{
			currentRoomState = evt.Component;
			initCurrentRoomState();
			return false;
		}

		private void initCurrentRoomState()
		{
			CoroutineRunner.Start(processCollectibleQueue(), this, "onRewardDataChanged: Keep CPU usage spike low during gameplay");
		}

		private bool isCollectibleDataLoaded()
		{
			return currentRoomState != null;
		}

		public bool RegisterRespawn(Collectible scriptObj, long respawnTime)
		{
			if (scriptObj.RewardDef == null)
			{
				respawnTime = long.MaxValue;
			}
			updateRespawnList(scriptObj, respawnTime);
			return false;
		}

		public bool RegisterCollectible(Collectible scriptObj)
		{
			if (isCollectibleDataLoaded())
			{
				setCollectibleState(scriptObj);
			}
			else
			{
				string path = scriptObj.Path;
				if (pendingCollectiblesToRegister.ContainsKey(path))
				{
					Log.LogErrorFormatted(scriptObj, "'{0}' is a duplicate path. All collectibles must have a unique name", path);
				}
				else
				{
					pendingCollectiblesToRegister[path] = scriptObj;
				}
			}
			return false;
		}

		private void setCollectibleState(Collectible scriptObj)
		{
			RespawnResponse respawnState = getRespawnState(scriptObj);
			scriptObj.StartCollectible(respawnState);
		}

		private IEnumerator processCollectibleQueue()
		{
			int processed = 0;
			foreach (Collectible scriptObj in pendingCollectiblesToRegister.Values)
			{
				setCollectibleState(scriptObj);
				if (processed++ % 20 == 0)
				{
					yield return null;
				}
			}
			pendingCollectiblesToRegister.Clear();
		}

		private void updateRespawnList(Collectible scriptObj, long respawnTime)
		{
			if (respawnList.ContainsKey(respawnTime))
			{
				respawnList[respawnTime].Add(scriptObj);
				return;
			}
			List<Collectible> list = new List<Collectible>();
			list.Add(scriptObj);
			respawnList.Add(respawnTime, list);
		}

		private RespawnResponse getRespawnState(Collectible scriptObj)
		{
			string text = scriptObj.Path;
			if (scriptObj.GetType() == typeof(SceneryCollectible) || scriptObj.GetType() == typeof(RewardCollectible))
			{
				text = scriptObj.InteractionPath;
			}
			RewardDefinition rewardDef = scriptObj.RewardDef;
			List<CollectibleRewardDefinition> list = null;
			if (rewardDef != null)
			{
				list = rewardDef.GetDefinitions<CollectibleRewardDefinition>();
			}
			if (rewardDef == null || list.Count < 1)
			{
				Log.LogErrorFormatted(scriptObj, "{0} has no CollectibleDefinition", text);
				return new RespawnResponse(RespawnState.NOT_AVAILABLE, 0L);
			}
			if (!currentRoomState.HasBeenCollected(text))
			{
				return new RespawnResponse(RespawnState.READY_FOR_PICKUP, 0L);
			}
			if (list[0].Collectible.SpawnCategory == SpawnCategory.Daily)
			{
				return new RespawnResponse(RespawnState.NOT_AVAILABLE, 0L);
			}
			long respawnTime = currentRoomState.GetRespawnTime(text);
			long num = GetTime.SecondsToMS(list[0].Collectible.RespawnSeconds);
			long num2 = respawnTime + num;
			long gameTimeMilliseconds = Service.Get<INetworkServicesManager>().GameTimeMilliseconds;
			if (num2 < gameTimeMilliseconds)
			{
				return new RespawnResponse(RespawnState.READY_FOR_PICKUP, 0L);
			}
			return new RespawnResponse(RespawnState.WAITING_TO_RESPAWN, num2);
		}

		private void collectibleHeartbeat()
		{
			if (!isCollectibleDataLoaded())
			{
				return;
			}
			long num = Service.Get<INetworkServicesManager>().GameTimeMilliseconds - 15000;
			int num2 = 0;
			IList<long> keys = respawnList.Keys;
			for (int i = 0; i < keys.Count; i++)
			{
				long num3 = keys[i];
				if (num3 > num)
				{
					break;
				}
				keysToExpire.Add(num3);
				List<Collectible> list = respawnList[num3];
				for (int j = 0; j < list.Count; j++)
				{
					if (list[j] != null)
					{
						setCollectibleState(list[j]);
					}
				}
			}
			num2 = keysToExpire.Count;
			for (int j = 0; j < num2; j++)
			{
				respawnList.Remove(keysToExpire[j]);
			}
			keysToExpire.Clear();
		}
	}
}
