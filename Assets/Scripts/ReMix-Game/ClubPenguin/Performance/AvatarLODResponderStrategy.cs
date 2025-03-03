using UnityEngine;

namespace ClubPenguin.Performance
{
	public class AvatarLODResponderStrategy : ResponderStrategy
	{
		private const float NEAR_DISTANCE = 5f;

		private const float FAR_DISTANCE = 10f;

		private const float MIN_NORMALIZED_SCORE = 0.5f;

		protected override PerformanceResponderType getExpectedResponseType()
		{
			return PerformanceResponderType.AvatarLod;
		}

		protected override float evaluateScore(FrameData frameData, PerformanceResponder performanceResponder, float maxNormalizedScore)
		{
			float num = 0f;
			float magnitude = (Camera.main.transform.position - performanceResponder.TransformRef.position).magnitude;
			if (magnitude <= 5f)
			{
				return maxNormalizedScore;
			}
			if (magnitude >= 10f)
			{
				return 0.5f;
			}
			float num2 = maxNormalizedScore - 0.5f;
			float num3 = (magnitude - 5f) / 5f;
			return maxNormalizedScore - num3 * num2;
		}
	}
}
