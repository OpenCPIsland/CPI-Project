using ClubPenguin.Adventure;
using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Interactables.Domain;
using ClubPenguin.Net;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native.iOS;
using Fabric;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Collectibles
{
	public abstract class Collectible : ProximityBroadcaster
	{
		public string AudioEvent = "SFX/UI/Collectable/Collect";

		public string Tag = "Player";

		[Tooltip("Required. Click the 'circle-dot' to the right of this field for an asset list")]
		public RewardDefinition RewardDef = null;

		public string QuestEventName = "";

		private string _path;

		private string _interactionPath = "";

		public Action<bool> OnCollected;

		protected CPDataEntityCollection dataEntityCollection;

		protected EventDispatcher dispatcher;

		protected CollectiblesData collectiblesData;

		protected bool isInitialized = false;

		private bool isListenerActive = false;

		public string Path
		{
			get
			{
				if (string.IsNullOrEmpty(_path))
				{
					_path = base.gameObject.GetPath();
				}
				return _path;
			}
			private set
			{
			}
		}

		public string InteractionPath
		{
			get
			{
				return _interactionPath;
			}
			protected set
			{
				_interactionPath = value;
			}
		}

		public abstract void RespawnCollectible();

		public override void Awake()
		{
			base.Awake();
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			dispatcher = Service.Get<EventDispatcher>();
			if (string.IsNullOrEmpty(Path))
			{
				Log.LogError(this, string.Format("Can't get value for Path"));
			}
		}

		public override void Start()
		{
			base.Start();
			if (dataEntityCollection.HasComponent<CollectiblesData>(dataEntityCollection.LocalPlayerHandle))
			{
				initializeCollectibleData();
			}
			else
			{
				dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<CollectiblesData>>(onCollectiblesReady);
				isListenerActive = true;
			}
			SceneRefs.CollectibleController.RegisterCollectible(this);
		}

		protected void playAudioEvent()
		{
			if (!string.IsNullOrEmpty(AudioEvent))
			{
				EventManager.Instance.PostEvent(AudioEvent, EventAction.PlaySound);
			}
		}

		protected void registerToRespawn(long initialRespawnTime = long.MaxValue)
		{
			long gameTimeMilliseconds = Service.Get<INetworkServicesManager>().GameTimeMilliseconds;
			if (!(RewardDef != null))
			{
				return;
			}
			List<CollectibleRewardDefinition> definitions = RewardDef.GetDefinitions<CollectibleRewardDefinition>();
			if (definitions.Count <= 0)
			{
				return;
			}
			long respawnTime;
			if (definitions[0].Collectible.SpawnCategory == SpawnCategory.ElapsedTime)
			{
				if (initialRespawnTime != long.MaxValue)
				{
					respawnTime = initialRespawnTime;
				}
				else
				{
					long num = GetTime.SecondsToMS(definitions[0].Collectible.RespawnSeconds);
					respawnTime = gameTimeMilliseconds + num;
				}
			}
			else
			{
				respawnTime = long.MaxValue;
			}
			SceneRefs.CollectibleController.RegisterRespawn(this, respawnTime);
		}

		protected void sendCollectedEventLocal()
		{
			if (collectiblesData == null || !(RewardDef != null))
			{
				return;
			}
			List<CollectibleRewardDefinition> definitions = RewardDef.GetDefinitions<CollectibleRewardDefinition>();
			if (definitions.Count <= 0)
			{
				return;
			}
			string collectibleType = definitions[0].Collectible.CollectibleType;
			int num = 0;
			int num2 = CoinRewardableDefinition.Coins(RewardDef);
			if (num2 > 0)
			{
				num = num2;
				Service.Get<ICPSwrveService>().CoinsGiven(num, "collected", collectibleType);
				Service.Get<CPDataEntityCollection>().GetComponent<CoinsData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).AddCoins(num, true);
				dispatcher.DispatchEvent(new InteractablesEvents.InWorldItemCollected(collectibleType, num));
				return;
			}
			num = definitions[0].Amount;
			if (!string.IsNullOrEmpty(collectibleType) && num != 0)
			{
				Service.Get<iOSHapticFeedback>().TriggerSelectionFeedback();
				Service.Get<EventDispatcher>().DispatchEvent(new CollectibleEvents.CollectibleAdd(collectibleType, num));
			}
			else
			{
				Log.LogError(this, string.Format("{0} doesn't have all required data, type = '{1}, amount = {2}", Path, collectibleType, num));
			}
			collectiblesData.AddCollectible(collectibleType, num);
			if (collectibleType == "RainbowDrop")
			{
				Service.Get<ICPSwrveService>().Action("rainbow", "drop_collected");
			}
		}

		protected void sendCollectedEventServer(string path, Vector3 pos)
		{
			if (RewardDef != null)
			{
				List<CollectibleRewardDefinition> definitions = RewardDef.GetDefinitions<CollectibleRewardDefinition>();
				if (definitions.Count > 0)
				{
					string collectibleType = definitions[0].Collectible.CollectibleType;
					Service.Get<INetworkServicesManager>().TaskService.Pickup(path, collectibleType, pos);
				}
			}
		}

		protected void sendQuestEvent()
		{
			if (!string.IsNullOrEmpty(QuestEventName))
			{
				Service.Get<QuestService>().SendEvent(QuestEventName);
			}
		}

		protected bool onCollectiblesReady(DataEntityEvents.ComponentAddedEvent<CollectiblesData> evt)
		{
			dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<CollectiblesData>>(onCollectiblesReady);
			isListenerActive = false;
			initializeCollectibleData();
			return false;
		}

		private void initializeCollectibleData()
		{
			collectiblesData = dataEntityCollection.GetComponent<CollectiblesData>(dataEntityCollection.LocalPlayerHandle);
		}

		public virtual void StartCollectible(RespawnResponse respawnResponse)
		{
			Log.LogError(this, string.Format("You shouldn't see this message. Method 'startCollectible' must be overidden in the extended class"));
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			if (isListenerActive)
			{
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<CollectiblesData>>(onCollectiblesReady);
			}
		}
	}
}
