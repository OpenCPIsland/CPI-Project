using ClubPenguin.Net.Domain;
using Disney.LaunchPadFramework;
using Sfs2X.Entities;
using System;

namespace ClubPenguin.Net.Client.Smartfox.Item
{
	public class ItemFactory
	{
		public static CPMMOItem Create(IMMOItem sfsItem, Func<int?, long> sessionIdMapper)
		{
			ItemCategory intValue = (ItemCategory)sfsItem.GetVariable(SocketItemVars.CATEGORY.GetKey()).GetIntValue();
			CPMMOItem cPMMOItem;
			switch (intValue)
			{
			case ItemCategory.Consumable:
				cPMMOItem = createConsumableItem(sfsItem);
				break;
			case ItemCategory.ScheduledRoomObject:
				cPMMOItem = createStatefulWorldObject(sfsItem);
				break;
			case ItemCategory.IslandTarget:
				cPMMOItem = createIslandTargetMMOItem(sfsItem);
				break;
			case ItemCategory.IslandTargetGroup:
				cPMMOItem = createIslandTargetGroupMMOItem(sfsItem);
				break;
			case ItemCategory.IslandTargetPlaygroundStats:
				cPMMOItem = createIslandTargetPlaygroundStatsMMOItem(sfsItem);
				break;
			case ItemCategory.PartyGamelobby:
				cPMMOItem = createPartyGameLobbyMMOItem(sfsItem);
				break;
			case ItemCategory.DanceBattle:
				cPMMOItem = createDanceBattleMMOItem(sfsItem);
				break;
			default:
				cPMMOItem = new CPMMOItem();
				Log.LogError(typeof(CPMMOItem), "Unknown item category: " + intValue);
				break;
			}
			cPMMOItem.Id = new CPMMOItemId(sfsItem.Id, CPMMOItemId.CPMMOItemParent.WORLD);
			cPMMOItem.CreatorId = sessionIdMapper(sfsItem.GetVariable(SocketItemVars.CREATOR.GetKey()).GetIntValue());
			return cPMMOItem;
		}

		private static CPMMOItem createIslandTargetPlaygroundStatsMMOItem(IMMOItem sfsItem)
		{
			return new IslandTargetPlaygroundStatsMMOItem(sfsItem);
		}

		private static CPMMOItem createIslandTargetGroupMMOItem(IMMOItem sfsItem)
		{
			return new IslandTargetGroupMMOItem(sfsItem);
		}

		private static CPMMOItem createIslandTargetMMOItem(IMMOItem sfsItem)
		{
			return new IslandTargetMMOItem(sfsItem);
		}

		private static CPMMOItem createPartyGameLobbyMMOItem(IMMOItem sfsItem)
		{
			return new PartygameLobbyMmoItem(sfsItem);
		}

		private static CPMMOItem createDanceBattleMMOItem(IMMOItem sfsItem)
		{
			return new DanceBattleMmoItem(sfsItem);
		}

		private static ConsumableItem createConsumableItem(IMMOItem sfsItem)
		{
			string stringValue = sfsItem.GetVariable(SocketItemVars.TEMPLATE.GetKey()).GetStringValue();
			ConsumableItem consumableItem;
			switch (stringValue)
			{
			case "timed":
				consumableItem = new TimedItem(sfsItem);
				break;
			case "actioned":
				consumableItem = new ActionedItem(sfsItem);
				break;
			default:
				consumableItem = new ConsumableItem();
				Log.LogError(typeof(CPMMOItem), "Unknown consumable template: " + stringValue);
				break;
			}
			consumableItem.Type = sfsItem.GetVariable(SocketItemVars.TYPE.GetKey()).GetStringValue();
			return consumableItem;
		}

		private static StatefulWorldObject createStatefulWorldObject(IMMOItem sfsItem)
		{
			return new StatefulWorldObject(sfsItem);
		}
	}
}
