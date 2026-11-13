using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using System;

namespace ClubPenguin.ClothingDesigner
{
	[Serializable]
	public class EquipmentTemplateRewardDefinition : AbstractStaticGameDataRewardDefinition<TemplateDefinition>
	{
		public TemplateDefinition Definition;

		public override IRewardable Reward
		{
			get
			{
				return new EquipmentTemplateReward(Definition.Id);
			}
		}

		protected override TemplateDefinition getField()
		{
			return Definition;
		}
	}
}
