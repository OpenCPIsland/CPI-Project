using ClubPenguin.Net.Domain;
using UnityEngine;

namespace ClubPenguin
{
	public class SpawnPlayerParams
	{
		public class SpawnPlayerParamsBuilder
		{
			private Vector3 position;

			private Quaternion rotation;

			private ZoneDefinition zone;

			private string sceneName = "";

			private SpawnedAction spawnedAction;

			private bool nudgePlayer;

			private bool getOutOfSwimming;

			private Reward pendingReward;

			public SpawnPlayerParamsBuilder(Vector3 position)
			{
				this.position = position;
			}

			public SpawnPlayerParamsBuilder Rotation(Quaternion rotation)
			{
				this.rotation = rotation;
				return this;
			}

			public SpawnPlayerParamsBuilder Zone(ZoneDefinition zone)
			{
				this.zone = zone;
				return this;
			}

			public SpawnPlayerParamsBuilder SceneName(string sceneName)
			{
				this.sceneName = sceneName;
				return this;
			}

			public SpawnPlayerParamsBuilder SpawnedAction(SpawnedAction spawnedAction)
			{
				this.spawnedAction = spawnedAction;
				return this;
			}

			public SpawnPlayerParamsBuilder NudgePlayer(bool nudgePlayer)
			{
				this.nudgePlayer = nudgePlayer;
				return this;
			}

			public SpawnPlayerParamsBuilder GetOutOfSwimming(bool getOutOfSwimming)
			{
				this.getOutOfSwimming = getOutOfSwimming;
				return this;
			}

			public SpawnPlayerParamsBuilder PendingReward(Reward pendingReward)
			{
				this.pendingReward = pendingReward;
				return this;
			}

			public SpawnPlayerParams Build()
			{
				return new SpawnPlayerParams(position, rotation, zone, sceneName, spawnedAction, nudgePlayer, getOutOfSwimming, pendingReward);
			}
		}

		public readonly Vector3 Position;

		public readonly Quaternion Rotation;

		public readonly ZoneDefinition Zone;

		public readonly string SceneName;

		public readonly SpawnedAction SpawnedAction;

		public readonly bool NudgePlayer;

		public readonly bool GetOutOfSwimming;

		public readonly Reward PendingReward;

		private SpawnPlayerParams(Vector3 position, Quaternion rotation, ZoneDefinition zone, string sceneName, SpawnedAction spawnedAction, bool nudgePlayer, bool getOutOfSwimming, Reward pendingReward)
		{
			Position = position;
			Rotation = rotation;
			Zone = zone;
			SceneName = sceneName;
			SpawnedAction = spawnedAction;
			NudgePlayer = nudgePlayer;
			GetOutOfSwimming = getOutOfSwimming;
			PendingReward = pendingReward;
		}
	}
}
