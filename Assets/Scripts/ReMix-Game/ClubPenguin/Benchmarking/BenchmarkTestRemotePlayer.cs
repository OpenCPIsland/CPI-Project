using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Benchmarking
{
	public class BenchmarkTestRemotePlayer : BenchmarkTestStage
	{
		[Header("Remote Player Settings")]
		public long LastSessionID;

		public int Bots;

		[Range(0f, 100f)]
		public int SpawnDelayFrames;

		public Vector3 SpawnPos;

		[Range(0f, 10f)]
		public float SpawnRadius;

		private RemotePlayerBotGenerator generator;

		protected override void performBenchmark()
		{
			Service.Get<CoroutineRunner>().StartCoroutine(remotePlayerJoin());
		}

		private IEnumerator remotePlayerJoin()
		{
			generator.enabled = true;
			while (generator.enabled)
			{
				yield return null;
			}
			onFinish();
		}

		protected override void setup()
		{
			generator = new GameObject("RemotePlayerBotGenerator").AddComponent<RemotePlayerBotGenerator>();
			generator.transform.position = SpawnPos;
			generator.LastSessionID = LastSessionID;
			generator.BotsToSpawn = Bots;
			generator.SpawnDelay = SpawnDelayFrames;
			generator.SpawnRadius = SpawnRadius;
		}

		protected override void teardown()
		{
			Object.Destroy(generator.gameObject);
		}
	}
}
