using UnityEngine;

namespace ClubPenguin.LOD
{
	public class LODWeightingCurve : LODWeightingRule
	{
		public WeightingCurveData CurveData;

		[SerializeField]
		private float timer = 0f;

		public override void Reset()
		{
			base.Reset();
			Object.Destroy(this);
		}

		public override void OnDisable()
		{
			base.OnDisable();
			Object.Destroy(this);
		}

		protected override float UpdateWeighting()
		{
			return CurveData.StartWeighting * CurveData.Curve.Evaluate(timer / CurveData.Length);
		}

		public void Update()
		{
			timer += Time.deltaTime;
			if (timer >= CurveData.Length)
			{
				Object.Destroy(this);
			}
		}
	}
}
