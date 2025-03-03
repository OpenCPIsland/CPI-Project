using Disney.Kelowna.Common.DataModel;
using UnityEngine;

namespace ClubPenguin.LOD
{
	public class LODWeightingGeneratedCurve : LODWeightingRule
	{
		public LODWeightingGeneratedCurveData Data;

		public override void Setup()
		{
			base.Setup();
			request.Data.OnGameObjectGeneratedEvent += onGameObjectGenerated;
		}

		public override void OnDisable()
		{
			base.OnDisable();
			request.Data.OnGameObjectGeneratedEvent -= onGameObjectGenerated;
		}

		protected override float UpdateWeighting()
		{
			return 0f;
		}

		private void onGameObjectGenerated(GameObject remotePlayer, DataEntityHandle remotePlayerHandle, LODRequestData requestData)
		{
			LODWeightingCurve lODWeightingCurve = base.gameObject.AddComponent<LODWeightingCurve>();
			lODWeightingCurve.CurveData = Data.CurveData;
			lODWeightingCurve.Setup();
		}
	}
}
