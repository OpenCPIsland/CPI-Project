using System;
using UnityEngine;

namespace ClubPenguin.LOD
{
	public class LODWeightingDistance : LODWeightingRule
	{
		public LODWeightingDistanceData Data;

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
			float result = 0f;
			if (target != null && request.Data.PositionData != null)
			{
				result = Data.StartWeighting - Vector3.Distance(target.position, request.Data.PositionData.Position);
			}
			return result;
		}
	}
}
