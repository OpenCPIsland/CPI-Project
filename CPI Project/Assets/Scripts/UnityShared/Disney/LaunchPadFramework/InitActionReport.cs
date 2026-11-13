using System;
using System.Collections.Generic;

namespace Disney.LaunchPadFramework
{
	public class InitActionReport
	{
		public struct Summary
		{
			public string Name;

			public InitManagerComponent.ActionDuration Duration;
		}

		private readonly List<Summary> actions = new List<Summary>();

		private TimeSpan totalDuration = new TimeSpan(0L);

		public int ActionCount
		{
			get
			{
				return actions.Count;
			}
		}

		public TimeSpan TotalDuration
		{
			get
			{
				return totalDuration;
			}
		}

		public TimeSpan TotalDurationWithOverhead
		{
			get;
			set;
		}

		public TimeSpan GetComponentsDuration
		{
			get;
			set;
		}

		public TimeSpan SortComponentsDuration
		{
			get;
			set;
		}

		public int FrameCount
		{
			get;
			set;
		}

		public void AddSummary(string name, InitManagerComponent.ActionDuration duration)
		{
			Summary summary = default(Summary);
			summary.Name = name;
			summary.Duration = duration;
			Summary item = summary;
			actions.Add(item);
			totalDuration += duration.GetTotalDuration();
		}

		public void LogReportTable()
		{
		}
	}
}
