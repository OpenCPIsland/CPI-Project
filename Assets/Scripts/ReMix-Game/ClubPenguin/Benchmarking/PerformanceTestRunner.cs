using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

namespace ClubPenguin.Benchmarking
{
	public class PerformanceTestRunner : MonoBehaviour
	{
		public struct Stats
		{
			public readonly float total;

			public readonly float mean;

			public readonly float stdDev;

			public readonly float min;

			public readonly float max;

			public readonly int count;

			public Stats(float[] samples)
			{
				count = samples.Length;
				total = 0f;
				for (int i = 0; i < count; i++)
				{
					total += samples[i];
				}
				mean = total / (float)count;
				stdDev = 0f;
				for (int i = 0; i < count; i++)
				{
					stdDev += (samples[i] - mean) * (samples[i] - mean);
				}
				stdDev = Mathf.Sqrt(stdDev);
				min = Mathf.Min(samples);
				max = Mathf.Max(samples);
			}

			public override string ToString()
			{
				return string.Format("[Stats] total: {0}, mean: {1}, stdDev: {2}, min: {3}, max: {4}, count: {5}", total, mean, stdDev, min, max, count);
			}
		}

		private struct Test
		{
			public readonly string Scene;

			public readonly float[] LoadTimes;

			public readonly float[] RunTimes;

			public readonly float[] FrameTimes;

			public Test(string scene, int runCount, int frameCount)
			{
				Scene = scene;
				LoadTimes = new float[runCount];
				RunTimes = new float[runCount];
				FrameTimes = new float[frameCount * runCount];
			}
		}

		public struct Result
		{
			public readonly Stats LoadTime;

			public readonly Stats RunTime;

			public readonly Stats FrameTime;

			public Result(float[] loadTimes, float[] runTimes, float[] frameTimes)
			{
				LoadTime = new Stats(loadTimes);
				RunTime = new Stats(runTimes);
				FrameTime = new Stats(frameTimes);
			}
		}

		private struct ProfileLogger : IDisposable
		{
			public readonly string Name;

			public ProfileLogger(string name)
			{
				Name = name;
				Print("<" + Name + ">");
			}

			public void Time(string label, float value)
			{
				Print("[" + label + "] : " + value.ToString("R") + " ms");
			}

			public void Print(string txt)
			{
				Console.WriteLine(txt);
			}

			public void Dispose()
			{
				Print("</" + Name + ">");
			}
		}

		public Result[] Results
		{
			get;
			private set;
		}

		public bool IsTesting
		{
			get;
			private set;
		}

		public int CurrentRun
		{
			get;
			private set;
		}

		public void Run(string[] scenes, int runCount, int frameCount, int randomSeed)
		{
			Test[] array = new Test[scenes.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new Test(scenes[i], runCount, frameCount);
			}
			StartCoroutine(TestingRoutine(array, runCount, frameCount, randomSeed));
		}

		private IEnumerator TestingRoutine(Test[] tests, int runCount, int frameCount, int randomSeed)
		{
			Profiler.logFile = "benchmark.log";
			Profiler.enableBinaryLog = true;
			float frequency = (float)Stopwatch.Frequency / 1000f;
			ProfileLogger benchmarkLog = new ProfileLogger("Benchmark");
			try
			{
				ProfileLogger profileLogger = benchmarkLog;
				profileLogger.Print("Ticks per millisecond: " + frequency);
				profileLogger = benchmarkLog;
				profileLogger.Print("Timer resolution: " + (Stopwatch.IsHighResolution ? "high" : "low"));
				profileLogger = benchmarkLog;
				profileLogger.Print("Random seed: " + randomSeed);
				IsTesting = true;
				for (int run = 0; run < runCount; run++)
				{
					ProfileLogger runLog = new ProfileLogger("Run");
					try
					{
						CurrentRun = run;
						for (int i = 0; i < tests.Length; i++)
						{
							ProfileLogger testLog = new ProfileLogger("Test");
							try
							{
								UnityEngine.Random.InitState(randomSeed);
								profileLogger = runLog;
								profileLogger.Print("Loading scene " + tests[i].Scene);
								long startLoad = Stopwatch.GetTimestamp();
								SceneManager.LoadScene(tests[i].Scene, LoadSceneMode.Single);
								tests[i].LoadTimes[run] = (float)(Stopwatch.GetTimestamp() - startLoad) / frequency;
								profileLogger = runLog;
								profileLogger.Time("Load", tests[i].LoadTimes[run]);
								long startFirstFrame = Stopwatch.GetTimestamp();
								yield return null;
								profileLogger = runLog;
								profileLogger.Time("First frame", (float)(Stopwatch.GetTimestamp() - startFirstFrame) / frequency);
								int index = run * frameCount;
								long startRun = Stopwatch.GetTimestamp();
								for (int frame = 0; frame < frameCount; frame++)
								{
									long startFrame = Stopwatch.GetTimestamp();
									yield return null;
									tests[i].FrameTimes[index] = (float)(Stopwatch.GetTimestamp() - startFrame) / frequency;
									profileLogger = testLog;
									profileLogger.Time("Frame", tests[i].FrameTimes[index++]);
								}
								tests[i].RunTimes[run] = (float)(Stopwatch.GetTimestamp() - startRun) / frequency;
								profileLogger = testLog;
								profileLogger.Time("Run", tests[i].RunTimes[run]);
							}
							finally
							{
								ProfileLogger profileLogger2 = testLog;
								((IDisposable)profileLogger2).Dispose();
							}
						}
					}
					finally
					{
						ProfileLogger profileLogger2 = runLog;
						((IDisposable)profileLogger2).Dispose();
					}
				}
				using (ProfileLogger profileLogger3 = new ProfileLogger("Summary"))
				{
					Results = new Result[tests.Length];
					for (int j = 0; j < tests.Length; j++)
					{
						Results[j] = new Result(tests[j].LoadTimes, tests[j].RunTimes, tests[j].FrameTimes);
						profileLogger3.Print(string.Format("Scene: {0}\nLoad: {1}\nRun: {2}\nFrames: {3}\n\n", tests[j].Scene, Results[j].LoadTime, Results[j].RunTime, Results[j].FrameTime));
					}
				}
			}
			finally
			{
				ProfileLogger profileLogger2 = benchmarkLog;
				((IDisposable)profileLogger2).Dispose();
			}
			Application.Quit();
			IsTesting = false;
		}
	}
}
