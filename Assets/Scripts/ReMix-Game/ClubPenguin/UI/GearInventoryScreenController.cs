using ClubPenguin.Core;
using ClubPenguin.Durable;
using ClubPenguin.Props;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class GearInventoryScreenController : AbstractProgressionLockedItems<PropDefinition>
	{
		protected override void createItem(PropDefinition itemDefinition, GameObject item, ItemGroup.LockedState lockedState)
		{
			item.GetComponent<GearInventoryButton>().Init(itemDefinition);
		}

		protected override PropDefinition[] getRewards(RewardDefinition rewardDefinition)
		{
			return AbstractStaticGameDataRewardDefinition<PropDefinition>.ToDefinitionArray(rewardDefinition.GetDefinitions<DurableRewardDefinition>());
		}

		protected override PropDefinition[] filterDefinitions(PropDefinition[] definitions)
		{
			List<PropDefinition> list = new List<PropDefinition>();
			if (definitions != null && definitions.Length > 0)
			{
				list.AddRange(definitions);
				list.RemoveAll((PropDefinition definition) => definition.PropType != PropDefinition.PropTypes.Durable);
			}
			return list.ToArray();
		}
	}
}
