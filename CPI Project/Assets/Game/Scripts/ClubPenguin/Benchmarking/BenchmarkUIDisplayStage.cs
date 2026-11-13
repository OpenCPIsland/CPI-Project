using Disney.Kelowna.Common.GameObjectTree;
using UnityEngine;

namespace ClubPenguin.Benchmarking
{
	public class BenchmarkUIDisplayStage : BenchmarkTestStage
	{
		[Header("UI Display Settings")]
		public TreeNodeDefinitionContentKey RootNodeDefinitionContentKey;

		protected override void performBenchmark()
		{
			GameObject gameObject = GameObject.Find("UIRoot");
			if (gameObject != null)
			{
				Object.Destroy(gameObject);
			}
			GameObject gameObject2 = new GameObject("UIRoot");
			gameObject2.SetActive(false);
			gameObject2.AddComponent<TreeController>().RootNodeDefinitionContentKey = RootNodeDefinitionContentKey;
			gameObject2.SetActive(true);
			onFinish();
		}
	}
}
