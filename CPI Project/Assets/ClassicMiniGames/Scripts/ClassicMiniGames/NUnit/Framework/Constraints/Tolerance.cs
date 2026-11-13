using System;

namespace NUnit.Framework.Constraints
{
	public class Tolerance
	{
		private const string ModeMustFollowTolerance = "Tolerance amount must be specified before setting mode";

		private const string MultipleToleranceModes = "Tried to use multiple tolerance modes at the same time";

		private const string NumericToleranceRequired = "A numeric tolerance is required";

		private readonly ToleranceMode mode;

		private readonly object amount;

		public static Tolerance Empty
		{
			get
			{
				return new Tolerance(0, ToleranceMode.None);
			}
		}

		public ToleranceMode Mode
		{
			get
			{
				return mode;
			}
		}

		public object Value
		{
			get
			{
				return amount;
			}
		}

		public Tolerance Percent
		{
			get
			{
				CheckLinearAndNumeric();
				return new Tolerance(amount, ToleranceMode.Percent);
			}
		}

		public Tolerance Ulps
		{
			get
			{
				CheckLinearAndNumeric();
				return new Tolerance(amount, ToleranceMode.Ulps);
			}
		}

		public Tolerance Days
		{
			get
			{
				CheckLinearAndNumeric();
				return new Tolerance(TimeSpan.FromDays(Convert.ToDouble(amount)));
			}
		}

		public Tolerance Hours
		{
			get
			{
				CheckLinearAndNumeric();
				return new Tolerance(TimeSpan.FromHours(Convert.ToDouble(amount)));
			}
		}

		public Tolerance Minutes
		{
			get
			{
				CheckLinearAndNumeric();
				return new Tolerance(TimeSpan.FromMinutes(Convert.ToDouble(amount)));
			}
		}

		public Tolerance Seconds
		{
			get
			{
				CheckLinearAndNumeric();
				return new Tolerance(TimeSpan.FromSeconds(Convert.ToDouble(amount)));
			}
		}

		public Tolerance Milliseconds
		{
			get
			{
				CheckLinearAndNumeric();
				return new Tolerance(TimeSpan.FromMilliseconds(Convert.ToDouble(amount)));
			}
		}

		public Tolerance Ticks
		{
			get
			{
				CheckLinearAndNumeric();
				return new Tolerance(TimeSpan.FromTicks(Convert.ToInt64(amount)));
			}
		}

		public bool IsEmpty
		{
			get
			{
				return mode == ToleranceMode.None;
			}
		}

		public Tolerance(object amount)
			: this(amount, ToleranceMode.Linear)
		{
		}

		private Tolerance(object amount, ToleranceMode mode)
		{
			this.amount = amount;
			this.mode = mode;
		}

		private void CheckLinearAndNumeric()
		{
			if (mode != ToleranceMode.Linear)
			{
				throw new InvalidOperationException((mode == ToleranceMode.None) ? "Tolerance amount must be specified before setting mode" : "Tried to use multiple tolerance modes at the same time");
			}
			if (!Numerics.IsNumericType(amount))
			{
				throw new InvalidOperationException("A numeric tolerance is required");
			}
		}
	}
}
