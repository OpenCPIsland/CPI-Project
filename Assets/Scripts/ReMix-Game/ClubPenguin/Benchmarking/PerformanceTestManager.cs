using UnityEngine;

namespace ClubPenguin.Benchmarking
{
	public class PerformanceTestManager : MonoBehaviour
	{
		public int RandomSeed = 0;

		public int FrameCount = 100;

		public int RunCount = 2;

		public string[] Tests;

		public static PerformanceTestRunner TestRunner
		{
			get;
			private set;
		}

		public void Awake()
		{
			if (TestRunner == null)
			{
				TestRunner = Object.FindObjectOfType<PerformanceTestRunner>();
				if (TestRunner == null)
				{
					GameObject gameObject = new GameObject("__TestRunner");
					Object.DontDestroyOnLoad(gameObject);
					TestRunner = gameObject.AddComponent<PerformanceTestRunner>();
				}
			}
		}

		public void Start()
		{
			RunTests();
		}

		public void RunTests()
		{
			if (!TestRunner.IsTesting)
			{
				TestRunner.Run(Tests, RunCount, FrameCount, RandomSeed);
			}
		}
	}
}
