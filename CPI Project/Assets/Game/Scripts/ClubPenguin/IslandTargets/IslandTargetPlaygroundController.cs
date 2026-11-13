using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native.iOS;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace ClubPenguin.IslandTargets
{
	public class IslandTargetPlaygroundController : MonoBehaviour
	{
		[Tooltip("Delay time between rounds")]
		[Range(0.5f, 6f)]
		public float delayBetweenRounds = 3f;

		[Tooltip("Tier1 name to log the BI under")]
		[Header("BI Logging")]
		public string BI_Tier1Name = "crate_co_game";

		private HashSet<string> childrenPaths = new HashSet<string>();

		private Dictionary<DataEntityHandle, ServerObjectItemData> dataModelHandleListenersMap;

		private Dictionary<long, Transform> gameObjectIdToTransformMap;

		private Dictionary<long, IslandTarget> gameObjectIdToIslandTargetComponentMap;

		private long currentGameMMOItemId;

		private DataEntityCollection dataEntityCollection;

		private EventDispatcher dispatcher;

		public EventDispatcher EventDispatcher
		{
			get
			{
				if (dispatcher == null)
				{
					dispatcher = new EventDispatcher();
				}
				return dispatcher;
			}
		}

		public int TargetsRemaining
		{
			get;
			private set;
		}

		public int TotalTargets
		{
			get;
			private set;
		}

		public int CurrentRound
		{
			get;
			private set;
		}

		public int TotalRounds
		{
			get;
			private set;
		}

		public void Awake()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			dataModelHandleListenersMap = new Dictionary<DataEntityHandle, ServerObjectItemData>();
			gameObjectIdToTransformMap = new Dictionary<long, Transform>();
			gameObjectIdToIslandTargetComponentMap = new Dictionary<long, IslandTarget>();
			if (dispatcher == null)
			{
				dispatcher = new EventDispatcher();
			}
			childrenPaths = new HashSet<string>(base.gameObject.GetChildrenPaths());
			if (string.IsNullOrEmpty(BI_Tier1Name))
			{
				BI_Tier1Name = "crate_co_game";
				Log.LogError(this, string.Format("Error: Tier1 name for BI is not set on '{0}'", base.gameObject.GetPath()));
			}
		}

		public void Start()
		{
			dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<ServerObjectItemData>>(onServerObjectItemAdded);
			dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.EntityRemovedEvent>(onServerObjectRemoved);
			EventDispatcher.AddListener<IslandTargetsEvents.LocalPlayerHitTargetEvent>(onLocalPlayerHitTarget);
			DataEntityHandle[] entitiesByType = dataEntityCollection.GetEntitiesByType<ServerObjectItemData>();
			foreach (DataEntityHandle handle in entitiesByType)
			{
				onServerObjectItemAdded(handle, dataEntityCollection.GetComponent<ServerObjectItemData>(handle));
			}
		}

		public void OnDestroy()
		{
			dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<ServerObjectItemData>>(onServerObjectItemAdded);
			dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.EntityRemovedEvent>(onServerObjectRemoved);
			EventDispatcher.RemoveListener<IslandTargetsEvents.LocalPlayerHitTargetEvent>(onLocalPlayerHitTarget);
			CoroutineRunner.StopAllForOwner(this);
		}

		private bool onServerObjectItemAdded(DataEntityEvents.ComponentAddedEvent<ServerObjectItemData> evt)
		{
			onServerObjectItemAdded(evt.Handle, evt.Component);
			return false;
		}

		private void onServerObjectItemAdded(DataEntityHandle handle, ServerObjectItemData obj)
		{
			IslandTargetGroupMMOItem islandTargetGroupMMOItem = obj.Item as IslandTargetGroupMMOItem;
			if (islandTargetGroupMMOItem != null)
			{
				onIslandGroupMMOItemAdded(handle, obj, islandTargetGroupMMOItem);
				return;
			}
			IslandTargetMMOItem islandTargetMMOItem = obj.Item as IslandTargetMMOItem;
			if (islandTargetMMOItem != null)
			{
				onIslandTargetMMOItemAdded(handle, obj, islandTargetMMOItem);
				return;
			}
			IslandTargetPlaygroundStatsMMOItem islandTargetPlaygroundStatsMMOItem = obj.Item as IslandTargetPlaygroundStatsMMOItem;
			if (islandTargetPlaygroundStatsMMOItem != null)
			{
				onIslandTargetPlaygroundStatsMMOItemAdded(handle, obj, islandTargetPlaygroundStatsMMOItem);
			}
		}

		private void onIslandTargetPlaygroundStatsMMOItemAdded(DataEntityHandle handle, ServerObjectItemData serverObjectItemData, IslandTargetPlaygroundStatsMMOItem item)
		{
			if (!dataModelHandleListenersMap.ContainsKey(handle) && item.GetPath() == base.transform.GetPath())
			{
				dataModelHandleListenersMap[handle] = serverObjectItemData;
				serverObjectItemData.ItemChanged += onIslandTargetsPlaygroundStatsChanged;
				dispatcher.DispatchEvent(new IslandTargetsEvents.StatsUpdated(item.GetBestWinStreakToday(), item.GetCurrentWinStreakToday()));
			}
		}

		private void onIslandTargetsPlaygroundStatsChanged(CPMMOItem obj)
		{
			IslandTargetPlaygroundStatsMMOItem islandTargetPlaygroundStatsMMOItem = obj as IslandTargetPlaygroundStatsMMOItem;
			dispatcher.DispatchEvent(new IslandTargetsEvents.StatsUpdated(islandTargetPlaygroundStatsMMOItem.GetBestWinStreakToday(), islandTargetPlaygroundStatsMMOItem.GetCurrentWinStreakToday()));
		}

		private void onIslandTargetMMOItemAdded(DataEntityHandle handle, ServerObjectItemData serverObjectItemData, IslandTargetMMOItem islandTargetMmoItem)
		{
			if (dataModelHandleListenersMap.ContainsKey(handle) || !childrenPaths.Contains(islandTargetMmoItem.Path))
			{
				return;
			}
			Transform transform = base.gameObject.transform.Find(islandTargetMmoItem.Path);
			if (!(transform != null))
			{
				return;
			}
			transform.gameObject.SetActive(true);
			dataModelHandleListenersMap[handle] = serverObjectItemData;
			IslandTarget component = transform.gameObject.GetComponent<IslandTarget>();
			if (component != null)
			{
				gameObjectIdToIslandTargetComponentMap[islandTargetMmoItem.Id.Id] = component;
				if (!islandTargetMmoItem.IsAnnihilated())
				{
					serverObjectItemData.ItemChanged += onTargetMMOItemChanged;
					component.DamageCapacity = islandTargetMmoItem.HitCapacity;
					component.ServerDamageCount = islandTargetMmoItem.HitCount;
					CoroutineRunner.Start(DelayTargetAppearance(delayBetweenRounds, component, islandTargetMmoItem), this, "DelayTargetAppearanceBetweenRounds");
					TargetsRemaining++;
					dispatcher.DispatchEvent(new IslandTargetsEvents.TargetsRemainingUpdated(TargetsRemaining, TotalTargets));
				}
			}
		}

		private void onTargetMMOItemChanged(CPMMOItem obj)
		{
			IslandTargetMMOItem islandTargetMMOItem = obj as IslandTargetMMOItem;
			if (islandTargetMMOItem == null || !gameObjectIdToIslandTargetComponentMap.ContainsKey(islandTargetMMOItem.Id.Id))
			{
				return;
			}
			IslandTarget islandTarget = gameObjectIdToIslandTargetComponentMap[islandTargetMMOItem.Id.Id];
			if (!(islandTarget != null))
			{
				return;
			}
			if (islandTargetMMOItem.IsAnnihilated())
			{
				islandTarget.CueDestroyTargetByServer();
				TargetsRemaining--;
				dispatcher.DispatchEvent(new IslandTargetsEvents.TargetsRemainingUpdated(TargetsRemaining, TotalTargets));
				if (TargetsRemaining == 0)
				{
					dispatcher.DispatchEvent(default(IslandTargetsEvents.GameRoundEnded));
					LogWaveComplete();
					currentGameMMOItemId = -1L;
				}
			}
			else
			{
				islandTarget.DamageCapacity = islandTargetMMOItem.HitCapacity;
				islandTarget.ServerDamageCount = islandTargetMMOItem.HitCount;
				islandTarget.UpdateDamageSlider();
				dispatcher.DispatchEvent(new IslandTargetsEvents.TargetHit(islandTargetMMOItem.HitCapacity, islandTargetMMOItem.HitCount));
			}
		}

		private void onIslandGroupMMOItemAdded(DataEntityHandle handle, ServerObjectItemData serverObjectItemData, IslandTargetGroupMMOItem islandTargetGroupMmoItem)
		{
			if (dataModelHandleListenersMap.ContainsKey(handle) || !childrenPaths.Contains(islandTargetGroupMmoItem.Path))
			{
				return;
			}
			Transform transform = base.gameObject.transform.Find(islandTargetGroupMmoItem.Path);
			if (transform != null)
			{
				transform.gameObject.SetActive(true);
				if (transform.gameObject.activeInHierarchy)
				{
				}
				dataModelHandleListenersMap[handle] = serverObjectItemData;
				serverObjectItemData.ItemChanged += onServerIslandTargetGroupChanged;
				gameObjectIdToTransformMap[islandTargetGroupMmoItem.Id.Id] = transform;
				currentGameMMOItemId = islandTargetGroupMmoItem.Id.Id;
				TotalTargets = transform.childCount;
				bool isFinalRound = transform.GetSiblingIndex() == transform.parent.childCount - 1;
				CurrentRound++;
				TotalRounds = transform.parent.childCount;
				CoroutineRunner.Start(DelayNextRoundStart(delayBetweenRounds, islandTargetGroupMmoItem, isFinalRound), this, "DelayBetweenCrateCoRounds");
			}
		}

		private bool onServerObjectRemoved(DataEntityEvents.EntityRemovedEvent evt)
		{
			if (dataModelHandleListenersMap.ContainsKey(evt.EntityHandle))
			{
				ServerObjectItemData serverObjectItemData = dataModelHandleListenersMap[evt.EntityHandle];
				serverObjectItemData.ItemChanged -= onServerIslandTargetGroupChanged;
				if (gameObjectIdToTransformMap.ContainsKey(serverObjectItemData.Item.Id.Id))
				{
					Transform transform = gameObjectIdToTransformMap[serverObjectItemData.Item.Id.Id];
					if (transform != null && !gameObjectIdToIslandTargetComponentMap.ContainsKey(serverObjectItemData.Item.Id.Id))
					{
						IslandTargetGroup component = transform.gameObject.GetComponent<IslandTargetGroup>();
						CoroutineRunner.Start(DelayDisable(waitTime: (!(component != null) || !(component.GroupAnimDelay > 0f)) ? 4f : component.GroupAnimDelay, g: transform.gameObject), this, "TargetAnimateOutDelayed");
					}
					if (currentGameMMOItemId == serverObjectItemData.Item.Id.Id)
					{
						dispatcher.DispatchEvent(default(IslandTargetsEvents.TargetGameTimeOut));
						LogWaveFailed();
						TargetsRemaining = 0;
						dispatcher.DispatchEvent(new IslandTargetsEvents.TargetsRemainingUpdated(TargetsRemaining, TotalTargets));
						CurrentRound = 0;
						currentGameMMOItemId = -1L;
					}
				}
				gameObjectIdToIslandTargetComponentMap.Remove(serverObjectItemData.Item.Id.Id);
				gameObjectIdToTransformMap.Remove(serverObjectItemData.Item.Id.Id);
				dataModelHandleListenersMap.Remove(evt.EntityHandle);
			}
			return false;
		}

		private IEnumerator DelayDisable(GameObject g, float waitTime)
		{
			yield return new WaitForSeconds(waitTime);
			g.SetActive(false);
		}

		private IEnumerator DelayNextRoundStart(float waitTime, IslandTargetGroupMMOItem islandTargetGroupMmoItem, bool isFinalRound)
		{
			yield return new WaitForSeconds(waitTime);
			dispatcher.DispatchEvent(new IslandTargetsEvents.GameRoundStarted(islandTargetGroupMmoItem.Starts, islandTargetGroupMmoItem.Expires, isFinalRound));
		}

		private IEnumerator DelayTargetAppearance(float waitTime, IslandTarget targetComponent, IslandTargetMMOItem islandTargetMmoItem)
		{
			yield return new WaitForSeconds(waitTime);
			targetComponent.Initialize(islandTargetMmoItem.HitCount, islandTargetMmoItem.HitCapacity, this);
		}

		private void onServerIslandTargetGroupChanged(CPMMOItem obj)
		{
		}

		private void UpdateWorldObject(IslandTargetGroupMMOItem islandTargetGroupMmoItem)
		{
			if (gameObjectIdToTransformMap.ContainsKey(islandTargetGroupMmoItem.Id.Id))
			{
				Transform x = gameObjectIdToTransformMap[islandTargetGroupMmoItem.Id.Id];
				if (!(x != null))
				{
				}
			}
		}

		private bool onLocalPlayerHitTarget(IslandTargetsEvents.LocalPlayerHitTargetEvent evt)
		{
			Service.Get<iOSHapticFeedback>().TriggerImpactFeedback(iOSHapticFeedback.ImpactFeedbackStyle.Light);
			string tier = evt.Target.DamagePercent.ToString("F", CultureInfo.InvariantCulture);
			Service.Get<ICPSwrveService>().Action(BI_Tier1Name, "hit_target", tier);
			return false;
		}

		private void LogWaveComplete()
		{
			if (CurrentRound != 0)
			{
				Service.Get<ICPSwrveService>().Action(BI_Tier1Name, "wave_success", CurrentRound.ToString());
			}
		}

		private void LogWaveFailed()
		{
			if (CurrentRound != 0)
			{
				Service.Get<ICPSwrveService>().Action(BI_Tier1Name, "wave_failure", CurrentRound.ToString());
			}
		}
	}
}
