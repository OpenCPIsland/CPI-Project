using System;
using UnityEngine;

namespace ClubPenguin.LOD
{
	public class LODWeightingDistanceRange : LODWeightingRule
	{
		public LODWeightingDistanceRangeData Data;

		[SerializeField]
		private Transform target;

		public override void Setup()
		{
			base.Setup();
			GameObject gameObject = GameObject.FindWithTag(Data.TargetTag);
			if (gameObject == null)
			{
				throw new NullReferenceException("Unable to find GameObject with tag: " + Data.TargetTag);
			}
			target = gameObject.transform;
		}

		public override void OnDisable()
		{
			base.OnDisable();
			target = null;
		}

		protected override float UpdateWeighting()
		{
			float result = Data.DefaultWeighing;
			if (target != null && request.Data.PositionData != null)
			{
				float num = Vector3.Distance(target.position, request.Data.PositionData.Position);
				int num2 = Data.Ranges.Length;
				for (int i = 0; i < num2; i++)
				{
					LODWeightingDistanceRangeData.Range range = Data.Ranges[i];
					if (num <= (float)range.Distance)
					{
						result = range.Weighting;
						break;
					}
				}
			}
			return result;
		}
	}
}
