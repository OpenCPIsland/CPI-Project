#define ENABLE_PROFILER
using Disney.LaunchPadFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Profiling;

namespace Foundation.Unity
{
	public class FibreService : MonoBehaviour
	{
		public delegate IEnumerator<bool> FibreFactory();

		[Serializable]
		public class Fibre
		{
			public readonly string Name;

			private int timeSliceTicks;

			private readonly int maxSlicePerFrameTicks;

			private long forcedProcessingTicks;

			public IEnumerator<bool> Enumerator;

			public FibreFactory factory;

			public Fibre(string name, float timeslice, FibreFactory factory)
			{
				Name = name;
				this.factory = factory;
				Restart();
				timeSliceTicks = (int)(timeslice * (float)MS_TO_TICKS);
				maxSlicePerFrameTicks = (int)(timeslice * (float)MS_TO_TICKS);
			}

			public void Restart()
			{
				Enumerator = factory();
			}

			public long BeforeProcessing(long startTicks)
			{
				if (forcedProcessingTicks < startTicks)
				{
					timeSliceTicks = maxSlicePerFrameTicks;
				}
				return startTicks + timeSliceTicks;
			}

			public void AfterProcessing(long startTicks)
			{
				if (timeSliceTicks > 0)
				{
					forcedProcessingTicks = startTicks + MaxProcessingDelayMs * MS_TO_TICKS;
				}
				int num = (int)(Stopwatch.GetTimestamp() - startTicks);
				timeSliceTicks = Mathf.Min(timeSliceTicks + maxSlicePerFrameTicks - num, maxSlicePerFrameTicks);
			}
		}

		public const bool CONTINUE_FRAME = true;

		public const bool DONE_FRAME = false;

		public static int MaxProcessingDelayMs = 200;

		private static readonly int MS_TO_TICKS = (int)((float)Stopwatch.Frequency / 1000f);

		public List<Fibre> Fibres = new List<Fibre>();

		private readonly Dictionary<string, float> budget = new Dictionary<string, float>();

		public void SetBudget(FibreBudget.TimeSlice[] slices)
		{
			budget.Clear();
			if (slices != null)
			{
				for (int i = 0; i < slices.Length; i++)
				{
					budget[slices[i].Name] = slices[i].DurationMs;
				}
			}
		}

		public void AddFibre(string name, float timeSliceMs, FibreFactory factory)
		{
			float value;
			if (budget.TryGetValue(name, out value))
			{
				timeSliceMs = value;
			}
			Fibres.Add(new Fibre(name, timeSliceMs, factory));
		}

		public void Update()
		{
			for (int num = Fibres.Count - 1; num >= 0; num--)
			{
				Fibre fibre = Fibres[num];
				try
				{
					Profiler.BeginSample(fibre.Name);
					long timestamp = Stopwatch.GetTimestamp();
					long num2 = fibre.BeforeProcessing(timestamp);
					while (Stopwatch.GetTimestamp() <= num2)
					{
						if (!fibre.Enumerator.MoveNext())
						{
							Fibres.RemoveAt(num);
							break;
						}
						if (!fibre.Enumerator.Current)
						{
							break;
						}
					}
					fibre.AfterProcessing(timestamp);
				}
				catch (Exception ex)
				{
					Log.LogError(this, "Exception caught executing fibre: " + fibre.Name);
					Log.LogException(this, ex);
					Fibres[num].Restart();
				}
				finally
				{
					Profiler.EndSample();
				}
			}
		}
	}
}
