using System;

namespace ICSharpCode.SharpZipLib.Core
{
	public class ProgressEventArgs : EventArgs
	{
		private string name_;

		private long processed_;

		private long target_;

		private bool continueRunning_ = true;

		public string Name
		{
			get
			{
				return name_;
			}
		}

		public bool ContinueRunning
		{
			get
			{
				return continueRunning_;
			}
			set
			{
				continueRunning_ = value;
			}
		}

		public float PercentComplete
		{
			get
			{
				if (target_ <= 0)
				{
					return 0f;
				}
				return (float)processed_ / (float)target_ * 100f;
			}
		}

		public long Processed
		{
			get
			{
				return processed_;
			}
		}

		public long Target
		{
			get
			{
				return target_;
			}
		}

		public ProgressEventArgs(string name, long processed, long target)
		{
			name_ = name;
			processed_ = processed;
			target_ = target;
		}
	}
}
