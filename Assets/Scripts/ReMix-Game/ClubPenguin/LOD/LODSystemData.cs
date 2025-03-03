using ClubPenguin.Configuration;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.LOD
{
	[CreateAssetMenu(menuName = "LOD/System Data")]
	public class LODSystemData : ScriptableObject
	{
		public string MetricName;

		public List<LODWeightingData> RuleData;

		[Header("Generator Data")]
		public PrefabContentKey ContentKey;

		public ConditionalDefinition_Int QualityIndex;

		[Header("Max Count Data")]
		public ConditionalDefinition_IntArray Limits;

		public float LimitMultipler = 1f;

		[Header("Weighting Refresher Data")]
		public ConditionalDefinition_FloatArray RefreshRates;

		public int MaxProcessesPerUpdate = 0;

		public int MaxCount
		{
			get
			{
				if (QualityIndex == null || Limits == null)
				{
					Log.LogErrorFormatted("LimitIndex or Limits conditional properties not set for LOD System {0}. No objects will be spawned.", base.name);
					return 0;
				}
				ConditionalConfiguration conditionalConfiguration = Service.Get<ConditionalConfiguration>();
				int num = conditionalConfiguration.Get(QualityIndex.name, 0);
				int[] array = conditionalConfiguration.Get<int[]>(Limits.name, null);
				if (array == null || num >= array.Length)
				{
					Log.LogErrorFormatted(this, "Failed to configure MaxCount for LOD System {0}. No objects will be spawned.", base.name);
					return 0;
				}
				return (int)((float)array[num] * LimitMultipler);
			}
		}

		public float RefreshRateSeconds
		{
			get
			{
				if (QualityIndex == null || RefreshRates == null)
				{
					Log.LogErrorFormatted("LimitIndex or Limits conditional properties not set for LOD System {0}. Using default of 1.5 seconds.", base.name);
					return 1.5f;
				}
				ConditionalConfiguration conditionalConfiguration = Service.Get<ConditionalConfiguration>();
				int num = conditionalConfiguration.Get(QualityIndex.name, 0);
				float[] array = conditionalConfiguration.Get<float[]>(RefreshRates.name, null);
				if (array == null || num >= array.Length)
				{
					Log.LogErrorFormatted(this, "Failed to configure RefreshRateSeconds for LOD System {0}. Using default of 1.5 seconds", base.name);
					return 1.5f;
				}
				return array[num];
			}
		}
	}
}
