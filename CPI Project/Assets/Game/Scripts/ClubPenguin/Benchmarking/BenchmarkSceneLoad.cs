using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClubPenguin.Benchmarking
{
	public class BenchmarkSceneLoad : BenchmarkTestStage
	{
		[Header("Scene Load Settings")]
		public string SceneName;

		protected override void performBenchmark()
		{
			Service.Get<CoroutineRunner>().StartCoroutine(sceneLoad());
		}

		private IEnumerator sceneLoad()
		{
			yield return SceneManager.LoadSceneAsync(SceneName);
			onFinish();
		}
	}
}
