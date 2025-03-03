using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using System;

namespace ClubPenguin.ClothingDesigner
{
	[Serializable]
	public class EquipmentInstanceRewardDefinition : AbstractStaticGameDataRewardDefinition<TemplateDefinition>
	{
		public TemplateDefinition Definition;

		public override IRewardable Reward
		{
			get
			{
				CustomEquipment value = default(CustomEquipment);
				value.definitionId = Definition.Id;
				value.parts = new CustomEquipmentPart[0];
				return new EquipmentInstanceReward(value);
			}
		}

		protected override TemplateDefinition getField()
		{
			return Definition;
		}
	}
}
