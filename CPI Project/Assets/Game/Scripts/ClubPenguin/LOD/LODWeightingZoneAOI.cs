using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.LOD
{
	public class LODWeightingZoneAOI : LODWeightingRule
	{
		public LODWeightingZoneAOIData Data;

		[SerializeField]
		private Transform target;

		private Vector3 halfEdgeAOI;

		public override void Setup()
		{
			base.Setup();
			GameObject gameObject = GameObject.FindWithTag(Data.TargetTag);
			if (gameObject == null)
			{
				throw new NullReferenceException("Unable to find GameObject with tag: " + Data.TargetTag);
			}
			target = gameObject.transform;
			halfEdgeAOI = (Service.Get<ZoneTransitionService>().CurrentZone.DefaultAOI + Data.Padding) * 0.5f;
		}

		public override void OnDisable()
		{
			base.OnDisable();
			target = null;
		}

		protected override float UpdateWeighting()
		{
			float result = Data.WeightingOutside;
			if (target != null && request.Data.PositionData != null && Mathf.Abs(request.Data.PositionData.Position.x - target.position.x) <= halfEdgeAOI.x && Mathf.Abs(request.Data.PositionData.Position.y - target.position.y) <= halfEdgeAOI.y && Mathf.Abs(request.Data.PositionData.Position.z - target.position.z) <= halfEdgeAOI.z)
			{
				result = Data.WeighingInside;
			}
			return result;
		}
	}
}
