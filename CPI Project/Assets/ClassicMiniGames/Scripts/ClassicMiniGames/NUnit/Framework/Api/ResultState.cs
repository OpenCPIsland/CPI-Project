namespace NUnit.Framework.Api
{
	public class ResultState
	{
		private readonly TestStatus status;

		private readonly string label;

		public static readonly ResultState Inconclusive = new ResultState(TestStatus.Inconclusive);

		public static readonly ResultState NotRunnable = new ResultState(TestStatus.Skipped, "Invalid");

		public static readonly ResultState Skipped = new ResultState(TestStatus.Skipped);

		public static readonly ResultState Ignored = new ResultState(TestStatus.Skipped, "Ignored");

		public static readonly ResultState Success = new ResultState(TestStatus.Passed);

		public static readonly ResultState Failure = new ResultState(TestStatus.Failed);

		public static readonly ResultState Error = new ResultState(TestStatus.Failed, "Error");

		public static readonly ResultState Cancelled = new ResultState(TestStatus.Failed, "Cancelled");

		public TestStatus Status
		{
			get
			{
				return status;
			}
		}

		public string Label
		{
			get
			{
				return label;
			}
		}

		public ResultState(TestStatus status)
			: this(status, string.Empty)
		{
		}

		public ResultState(TestStatus status, string label)
		{
			this.status = status;
			this.label = ((label == null) ? string.Empty : label);
		}

		public override string ToString()
		{
			string text = status.ToString();
			return (label == null || label.Length == 0) ? text : string.Format("{0}:{1}", text, label);
		}
	}
}
