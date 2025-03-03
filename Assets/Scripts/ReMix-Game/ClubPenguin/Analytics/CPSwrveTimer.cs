using System.Diagnostics;

namespace ClubPenguin.Analytics
{
	public class CPSwrveTimer
	{
		public readonly Stopwatch Timer;

		public readonly string TimerID;

		public readonly string Context;

		public readonly string Message;

		public readonly string StepName;

		public CPSwrveTimer(string TimerID, string Context, string Message = null, string StepName = null)
		{
			this.TimerID = TimerID;
			this.Context = Context;
			this.Message = Message;
			this.StepName = StepName;
			Timer = new Stopwatch();
			Timer.Start();
		}

		public void PauseTimer()
		{
			if (Timer != null)
			{
				Timer.Stop();
			}
		}

		public void ResumeTimer()
		{
			if (Timer != null)
			{
				Timer.Start();
			}
		}
	}
}
