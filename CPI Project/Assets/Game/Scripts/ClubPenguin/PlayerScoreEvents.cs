using UnityEngine;

namespace ClubPenguin
{
	public class PlayerScoreEvents
	{
		public enum ParticleType
		{
			None,
			Coins,
			XP
		}

		public struct ShowPlayerScore
		{
			public readonly long PlayerId;

			public readonly string Score;

			public readonly ParticleType ParticleEffectType;

			public readonly Color XPTintColor;

			public ShowPlayerScore(long playerId, string score, ParticleType particleType = ParticleType.None, Color xpTintColor = default(Color))
			{
				PlayerId = playerId;
				Score = score;
				ParticleEffectType = particleType;
				XPTintColor = xpTintColor;
			}
		}

		public struct RemovePlayerScore
		{
			public readonly long PlayerId;

			public readonly GameObject PlayerScoreObject;

			public RemovePlayerScore(long playerId, GameObject playerScoreObject)
			{
				PlayerId = playerId;
				PlayerScoreObject = playerScoreObject;
			}
		}
	}
}
