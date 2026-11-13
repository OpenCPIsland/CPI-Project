using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Domain.Decoration;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin.DecorationInventory
{
	[DisallowMultipleComponent]
	public class DecorationInventoryService
	{
		public class DecorationNameGenerator : NamedToggleValueAttribute.NamedToggleValueGenerator
		{
			public IEnumerable<NamedToggleValueAttribute.NamedToggleValue> GetNameToggleValues()
			{
				List<NamedToggleValueAttribute.NamedToggleValue> list = new List<NamedToggleValueAttribute.NamedToggleValue>();
				if (Service.IsSet<IGameData>())
				{
					Dictionary<int, DecorationDefinition> dictionary = Service.Get<IGameData>().Get<Dictionary<int, DecorationDefinition>>();
					foreach (KeyValuePair<int, DecorationDefinition> item in dictionary)
					{
						list.Add(new NamedToggleValueAttribute.NamedToggleValue(Service.Get<Localizer>().GetTokenTranslation(item.Value.Name), item.Value.Id));
					}
				}
				return list.OrderBy((NamedToggleValueAttribute.NamedToggleValue x) => x.Name).ToList();
			}
		}

		public class StructureNameGenerator : NamedToggleValueAttribute.NamedToggleValueGenerator
		{
			public IEnumerable<NamedToggleValueAttribute.NamedToggleValue> GetNameToggleValues()
			{
				List<NamedToggleValueAttribute.NamedToggleValue> list = new List<NamedToggleValueAttribute.NamedToggleValue>();
				if (Service.IsSet<IGameData>())
				{
					Dictionary<int, StructureDefinition> dictionary = Service.Get<IGameData>().Get<Dictionary<int, StructureDefinition>>();
					foreach (KeyValuePair<int, StructureDefinition> item in dictionary)
					{
						list.Add(new NamedToggleValueAttribute.NamedToggleValue(Service.Get<Localizer>().GetTokenTranslation(item.Value.Name), item.Value.Id));
					}
				}
				return list.OrderBy((NamedToggleValueAttribute.NamedToggleValue x) => x.Name).ToList();
			}
		}

		private DecorationInventoryData decorationInventory;

		private List<KeyValuePair<DecorationDefinition, int>> cachedCalculatedDecorationInventory;

		private Dictionary<int, int> cachedAvailableDecorationCounts;

		private StructureInventoryData structureInventory;

		private List<KeyValuePair<StructureDefinition, int>> cachedCalculatedStructureInventory;

		private Dictionary<int, int> cachedAvailableStructureCounts;

		private SceneLayoutData sceneLayout;

		private Dictionary<int, DecorationDefinition> decorationDefinitions;

		private Dictionary<int, StructureDefinition> structureDefinitions;

		private Dictionary<int, int> pendingDecorationRewards = new Dictionary<int, int>();

		private Dictionary<int, int> pendingStructureRewards = new Dictionary<int, int>();

		private HashSet<string> reconciledQuestCompletionRewards = new HashSet<string>();

		private bool decorationInventoryLoadRequested;

		public event Action<DecorationType> onInventoryLoaded;

		public event System.Action InventoryUpdated;

		public DecorationInventoryService()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			decorationDefinitions = Service.Get<IGameData>().Get<Dictionary<int, DecorationDefinition>>();
			structureDefinitions = Service.Get<IGameData>().Get<Dictionary<int, StructureDefinition>>();
			cPDataEntityCollection.Whenever<DecorationInventoryData>("LocalPlayer", onDecorationsLoaded, onDecorationsUnLoaded);
			cPDataEntityCollection.Whenever<StructureInventoryData>("LocalPlayer", onStructuresLoaded, onStructuresUnLoaded);
			cPDataEntityCollection.Whenever<SceneLayoutData>("ActiveSceneData", onSceneLayoutLoaded, onSceneLayoutUnLoaded);
			Service.Get<EventDispatcher>().AddListener<IglooServiceEvents.DecorationsLoaded>(onIglooServiceEventsDecorationsLoaded);
			Service.Get<EventDispatcher>().AddListener<IglooServiceEvents.DecorationUpdated>(onDecorationUpdated);
			Service.Get<EventDispatcher>().AddListener<RewardServiceEvents.MyRewardEarned>(onMyRewardEarned);
		}

		public int GetTotalInventoryCountForDecoration(int decorationId)
		{
			if (decorationInventory == null)
			{
				return -1;
			}
			return decorationInventory.Decorations[decorationId];
		}

		public List<KeyValuePair<DecorationDefinition, int>> GetAvailableDecorations()
		{
			HashSet<string> hashSet = new HashSet<string>();
			if (cachedCalculatedDecorationInventory == null)
			{
				if (decorationInventory == null)
				{
					Log.LogError(this, "Attempting to access the Decoration inventory before it is loaded");
					return new List<KeyValuePair<DecorationDefinition, int>>();
				}
				if (sceneLayout == null)
				{
					Log.LogError(this, "Attempting to access the Decoration inventory before scene is loaded");
					return new List<KeyValuePair<DecorationDefinition, int>>();
				}
				cachedAvailableDecorationCounts = new Dictionary<int, int>(decorationInventory.Decorations);
				foreach (DecorationLayoutData item in sceneLayout.GetLayoutEnumerator())
				{
					bool flag = true;
					if (item.Type == DecorationLayoutData.DefinitionType.Decoration && cachedAvailableDecorationCounts.ContainsKey(item.DefinitionId))
					{
						if (item.CustomProperties.ContainsKey("partner") && item.CustomProperties.ContainsKey("guid"))
						{
							flag = !hashSet.Contains(item.CustomProperties["partner"]);
							hashSet.Add(item.CustomProperties["guid"]);
						}
						if (flag)
						{
							cachedAvailableDecorationCounts[item.DefinitionId]--;
						}
					}
				}
				cachedCalculatedDecorationInventory = new List<KeyValuePair<DecorationDefinition, int>>();
				foreach (KeyValuePair<int, int> cachedAvailableDecorationCount in cachedAvailableDecorationCounts)
				{
					if (decorationDefinitions.ContainsKey(cachedAvailableDecorationCount.Key))
					{
						cachedCalculatedDecorationInventory.Add(new KeyValuePair<DecorationDefinition, int>(decorationDefinitions[cachedAvailableDecorationCount.Key], cachedAvailableDecorationCount.Value));
					}
				}
			}
			return cachedCalculatedDecorationInventory;
		}

		public int GetAvailableDecorationCount(int id)
		{
			if (cachedCalculatedDecorationInventory == null)
			{
				GetAvailableDecorations();
			}
			if (cachedAvailableDecorationCounts != null && cachedAvailableDecorationCounts.ContainsKey(id))
			{
				return cachedAvailableDecorationCounts[id];
			}
			return -1;
		}

		public List<KeyValuePair<StructureDefinition, int>> GetAvailableStructures()
		{
			if (cachedCalculatedStructureInventory == null)
			{
				if (structureInventory == null)
				{
					Log.LogError(this, "Attempting to access the Structure inventory before it is loaded");
					return new List<KeyValuePair<StructureDefinition, int>>();
				}
				if (sceneLayout == null)
				{
					Log.LogError(this, "Attempting to access the Structure inventory before scene is loaded");
					return new List<KeyValuePair<StructureDefinition, int>>();
				}
				cachedAvailableStructureCounts = new Dictionary<int, int>(structureInventory.Structures);
				foreach (DecorationLayoutData item in sceneLayout.GetLayoutEnumerator())
				{
					if (item.Type == DecorationLayoutData.DefinitionType.Structure && cachedAvailableStructureCounts.ContainsKey(item.DefinitionId))
					{
						cachedAvailableStructureCounts[item.DefinitionId]--;
					}
				}
				cachedCalculatedStructureInventory = new List<KeyValuePair<StructureDefinition, int>>();
				foreach (KeyValuePair<int, int> cachedAvailableStructureCount in cachedAvailableStructureCounts)
				{
					if (structureDefinitions.ContainsKey(cachedAvailableStructureCount.Key))
					{
						cachedCalculatedStructureInventory.Add(new KeyValuePair<StructureDefinition, int>(structureDefinitions[cachedAvailableStructureCount.Key], cachedAvailableStructureCount.Value));
					}
				}
			}
			return cachedCalculatedStructureInventory;
		}

		public int GetAvailableStructureCount(int id)
		{
			if (cachedCalculatedStructureInventory == null)
			{
				GetAvailableStructures();
			}
			if (cachedAvailableStructureCounts != null && cachedAvailableStructureCounts.ContainsKey(id))
			{
				return cachedAvailableStructureCounts[id];
			}
			return -1;
		}

		public DecorationDefinition GetDecorationDefinition(int decorationId)
		{
			if (decorationDefinitions.ContainsKey(decorationId))
			{
				return decorationDefinitions[decorationId];
			}
			return null;
		}

		public StructureDefinition GetStructureDefinition(int structureId)
		{
			if (structureDefinitions.ContainsKey(structureId))
			{
				return structureDefinitions[structureId];
			}
			return null;
		}

		private void onDecorationsLoaded(DecorationInventoryData data)
		{
			decorationInventory = data;
			decorationInventory.OnDecorationsChanged += onDecorationsChanged;
			this.onInventoryLoaded.InvokeSafe(DecorationType.Decoration);
		}

		private bool onDecorationUpdated(IglooServiceEvents.DecorationUpdated evt)
		{
			if (evt.DecorationId.type == DecorationType.Decoration)
			{
				if (decorationInventory != null)
				{
					decorationInventory.AddDecoration(evt.DecorationId.definitionId, evt.Count);
				}
			}
			else if (evt.DecorationId.type == DecorationType.Structure && structureInventory != null)
			{
				structureInventory.AddStructure(evt.DecorationId.definitionId, evt.Count);
			}
			this.InventoryUpdated.InvokeSafe();
			return false;
		}

		private bool onMyRewardEarned(RewardServiceEvents.MyRewardEarned evt)
		{
			if (evt.Source == RewardSource.QUEST_COMPLETED)
			{
				EnsureQuestCompletionReward(evt.SourceId, evt.Reward);
				return false;
			}
			DecorationInstanceReward decorationReward;
			if (evt.Reward.TryGetValue(out decorationReward))
			{
				foreach (KeyValuePair<int, int> decoration in decorationReward.Decorations)
				{
					addDecorationReward(decoration.Key, decoration.Value);
				}
			}
			StructureInstanceReward structureReward;
			if (evt.Reward.TryGetValue(out structureReward))
			{
				foreach (KeyValuePair<int, int> structure in structureReward.Decorations)
				{
					addStructureReward(structure.Key, structure.Value);
				}
			}
			if ((pendingDecorationRewards.Count > 0 || pendingStructureRewards.Count > 0) && !decorationInventoryLoadRequested)
			{
				decorationInventoryLoadRequested = true;
				Service.Get<INetworkServicesManager>().IglooService.GetDecorations();
			}
			return false;
		}

		public void EnsureQuestCompletionReward(string questId, Reward reward)
		{
			if (!reconciledQuestCompletionRewards.Add(questId))
			{
				return;
			}
			DecorationInstanceReward decorationReward;
			if (reward.TryGetValue(out decorationReward))
			{
				foreach (KeyValuePair<int, int> decoration in decorationReward.Decorations)
				{
					ensureDecorationReward(decoration.Key, decoration.Value);
				}
			}
			StructureInstanceReward structureReward;
			if (reward.TryGetValue(out structureReward))
			{
				foreach (KeyValuePair<int, int> structure in structureReward.Decorations)
				{
					ensureStructureReward(structure.Key, structure.Value);
				}
			}
			if ((pendingDecorationRewards.Count > 0 || pendingStructureRewards.Count > 0) && !decorationInventoryLoadRequested)
			{
				decorationInventoryLoadRequested = true;
				Service.Get<INetworkServicesManager>().IglooService.GetDecorations();
			}
		}

		private void addDecorationReward(int definitionId, int count)
		{
			if (decorationInventory != null)
			{
				decorationInventory.AddDecoration(definitionId, count);
				this.InventoryUpdated.InvokeSafe();
				return;
			}
			addPendingReward(pendingDecorationRewards, definitionId, count);
		}

		private void addStructureReward(int definitionId, int count)
		{
			if (structureInventory != null)
			{
				structureInventory.AddStructure(definitionId, count);
				this.InventoryUpdated.InvokeSafe();
				return;
			}
			addPendingReward(pendingStructureRewards, definitionId, count);
		}

		private void ensureDecorationReward(int definitionId, int count)
		{
			ensureReward(pendingDecorationRewards, decorationInventory == null ? null : decorationInventory.Decorations, definitionId, count, addDecorationReward);
		}

		private void ensureStructureReward(int definitionId, int count)
		{
			ensureReward(pendingStructureRewards, structureInventory == null ? null : structureInventory.Structures, definitionId, count, addStructureReward);
		}

		private void ensureReward(Dictionary<int, int> pendingRewards, Dictionary<int, int> inventory, int definitionId, int count, Action<int, int> addReward)
		{
			if (inventory == null)
			{
				int pendingCount;
				pendingRewards.TryGetValue(definitionId, out pendingCount);
				pendingRewards[definitionId] = Math.Max(pendingCount, count);
				return;
			}
			int currentCount;
			inventory.TryGetValue(definitionId, out currentCount);
			if (currentCount < count)
			{
				addReward(definitionId, count - currentCount);
			}
		}

		private void addPendingReward(Dictionary<int, int> pendingRewards, int definitionId, int count)
		{
			if (pendingRewards.ContainsKey(definitionId))
			{
				pendingRewards[definitionId] += count;
			}
			else
			{
				pendingRewards[definitionId] = count;
			}
		}

		private void applyPendingRewards()
		{
			applyPendingRewards(pendingDecorationRewards, decorationInventory == null ? null : decorationInventory.Decorations, addDecorationReward);
			applyPendingRewards(pendingStructureRewards, structureInventory == null ? null : structureInventory.Structures, addStructureReward);
		}

		private void applyPendingRewards(Dictionary<int, int> pendingRewards, Dictionary<int, int> inventory, Action<int, int> addReward)
		{
			if (inventory == null)
			{
				return;
			}
			foreach (KeyValuePair<int, int> reward in pendingRewards)
			{
				int currentCount;
				inventory.TryGetValue(reward.Key, out currentCount);
				if (currentCount < reward.Value)
				{
					addReward(reward.Key, reward.Value - currentCount);
				}
			}
			pendingRewards.Clear();
		}

		private void onDecorationsUnLoaded(DecorationInventoryData data)
		{
			decorationInventory.OnDecorationsChanged -= onDecorationsChanged;
			decorationInventory = null;
			cachedCalculatedDecorationInventory = null;
			pendingDecorationRewards.Clear();
			pendingStructureRewards.Clear();
			reconciledQuestCompletionRewards.Clear();
			decorationInventoryLoadRequested = false;
		}

		private void onDecorationsChanged(Dictionary<int, int> data)
		{
			cachedCalculatedDecorationInventory = null;
		}

		private void onStructuresLoaded(StructureInventoryData data)
		{
			structureInventory = data;
			structureInventory.OnStructuresChanged += onStructuresChanged;
			this.onInventoryLoaded.InvokeSafe(DecorationType.Structure);
		}

		private void onStructuresUnLoaded(StructureInventoryData data)
		{
			structureInventory.OnStructuresChanged -= onStructuresChanged;
			structureInventory = null;
			cachedCalculatedStructureInventory = null;
		}

		private void onStructuresChanged(Dictionary<int, int> data)
		{
			cachedCalculatedStructureInventory = null;
		}

		private void onSceneLayoutLoaded(SceneLayoutData data)
		{
			sceneLayout = data;
			sceneLayout.SceneLayoutDataUpdated += onSceneLayoutChanged;
		}

		private void onSceneLayoutUnLoaded(SceneLayoutData data)
		{
			sceneLayout.SceneLayoutDataUpdated -= onSceneLayoutChanged;
			sceneLayout = null;
			onSceneLayoutChanged(data);
		}

		private void onSceneLayoutChanged(SceneLayoutData data)
		{
			cachedCalculatedStructureInventory = null;
			cachedCalculatedDecorationInventory = null;
		}

		private bool onIglooServiceEventsDecorationsLoaded(IglooServiceEvents.DecorationsLoaded evt)
		{
			decorationInventoryLoadRequested = false;
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			int count = evt.DecorationInventory.items.Count;
			DecorationInventoryData component;
			if (!cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				component = cPDataEntityCollection.AddComponent<DecorationInventoryData>(cPDataEntityCollection.LocalPlayerHandle);
			}
			Dictionary<int, int> dictionary = new Dictionary<int, int>(count);
			for (int i = 0; i < count; i++)
			{
				DecorationId decorationId = evt.DecorationInventory.items[i].decorationId;
				if (decorationId.type == DecorationType.Decoration && !decorationId.customId.HasValue)
				{
					dictionary.Add(decorationId.definitionId, evt.DecorationInventory.items[i].count);
				}
			}
			component.Decorations = dictionary;
			applyPendingRewards();
			StructureInventoryData component2;
			if (!cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component2))
			{
				component2 = cPDataEntityCollection.AddComponent<StructureInventoryData>(cPDataEntityCollection.LocalPlayerHandle);
			}
			Dictionary<int, int> dictionary2 = new Dictionary<int, int>(count);
			for (int i = 0; i < count; i++)
			{
				DecorationId decorationId = evt.DecorationInventory.items[i].decorationId;
				if (decorationId.type == DecorationType.Structure && !decorationId.customId.HasValue)
				{
					dictionary2.Add(decorationId.definitionId, evt.DecorationInventory.items[i].count);
				}
			}
			component2.Structures = dictionary2;
			applyPendingRewards();
			return false;
		}

		public void MarkDecorationsDirty()
		{
			cachedCalculatedDecorationInventory = null;
		}

		public void MarkStructuresDirty()
		{
			cachedCalculatedStructureInventory = null;
		}

		[PublicTweak]
		[Invokable("Inventory.Decoration.AddDecorations", Description = "Add decorations to your inventory")]
		private void qaAddDecorations([NamedToggleValue(typeof(DecorationNameGenerator), 0u)] int definitionId, int count = 1)
		{
			Service.Get<INetworkServicesManager>().IglooService.QA_AddDecorations(definitionId, DecorationType.Decoration, count);
			if (decorationInventory != null)
			{
				decorationInventory.AddDecoration(definitionId, count);
			}
		}

		[PublicTweak]
		[Invokable("Inventory.Decoration.AddStructures", Description = "Add structures to your inventory")]
		private void qaAddStructures([NamedToggleValue(typeof(StructureNameGenerator), 0u)] int definitionId, int count = 1)
		{
			Service.Get<INetworkServicesManager>().IglooService.QA_AddDecorations(definitionId, DecorationType.Structure, count);
			if (structureInventory != null)
			{
				structureInventory.AddStructure(definitionId, count);
			}
		}

		[PublicTweak]
		[Invokable("Inventory.Decoration.DeleteAllDecorationsAndStructures", Description = "Delete all decorations for the current player.")]
		private void qaDeleteAllDecorations()
		{
			Service.Get<INetworkServicesManager>().IglooService.QA_DeleteAllDecorations();
			if (decorationInventory != null && structureInventory != null)
			{
				decorationInventory.Decorations = new Dictionary<int, int>();
				structureInventory.Structures = new Dictionary<int, int>();
			}
		}

		[PublicTweak]
		[Invokable("Inventory.Decoration.SetCountForAll", Description = "Sets your furniture inventory count for all known furniture items")]
		private void qaSetFurnitureCount(int count = 10)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
			Reward reward = new Reward();
			foreach (int key in Service.Get<IGameData>().Get<Dictionary<int, DecorationDefinition>>().Keys)
			{
				reward.Add(new DecorationInstanceReward(key, count));
				dictionary.Add(key, count);
			}
			foreach (int key2 in Service.Get<IGameData>().Get<Dictionary<int, StructureDefinition>>().Keys)
			{
				reward.Add(new StructureInstanceReward(key2, count));
				dictionary2.Add(key2, count);
			}
			Service.Get<INetworkServicesManager>().RewardService.QA_SetReward(reward);
			if (decorationInventory != null && structureInventory != null)
			{
				decorationInventory.Decorations = dictionary;
				structureInventory.Structures = dictionary2;
			}
		}
	}
}
