using Disney.MobileNetwork;
using System;

namespace ClubPenguin.Configuration.DynamicTiers
{
	public class LODPenguinQualityIndexDynamicTier : ConditionalTier_Int
	{
		private CustomGraphicsService customGraphics;

		public override int DynamicValue
		{
			get
			{
				if (customGraphics == null && Service.IsSet<CustomGraphicsService>())
				{
					customGraphics = Service.Get<CustomGraphicsService>();
					customGraphics.LodPenguinQualityLevel.EChanged += OnValueChanged;
				}
				if (customGraphics != null)
				{
					return (int)customGraphics.LodPenguinQualityLevel.Value;
				}
				return StaticValue;
			}
			internal set
			{
				if (Enum.IsDefined(typeof(QualityLevel), value))
				{
					customGraphics.LodPenguinQualityLevel.SetValue((QualityLevel)value);
				}
			}
		}

		private void OnValueChanged(QualityLevel level)
		{
			DispatchDynamicChanged();
		}

		private void OnDestroy()
		{
			if (customGraphics != null)
			{
				customGraphics.LodPenguinQualityLevel.EChanged -= OnValueChanged;
			}
		}
	}
}
