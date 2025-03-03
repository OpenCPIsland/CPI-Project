using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.Core
{
	public class ExtendedPathFilter : PathFilter
	{
		private long minSize_;

		private long maxSize_ = long.MaxValue;

		private DateTime minDate_ = DateTime.MinValue;

		private DateTime maxDate_ = DateTime.MaxValue;

		public long MinSize
		{
			get
			{
				return minSize_;
			}
			set
			{
				if (value < 0 || maxSize_ < value)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				minSize_ = value;
			}
		}

		public long MaxSize
		{
			get
			{
				return maxSize_;
			}
			set
			{
				if (value < 0 || minSize_ > value)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				maxSize_ = value;
			}
		}

		public DateTime MinDate
		{
			get
			{
				return minDate_;
			}
			set
			{
				if (value > maxDate_)
				{
					throw new ArgumentOutOfRangeException("value", "Exceeds MaxDate");
				}
				minDate_ = value;
			}
		}

		public DateTime MaxDate
		{
			get
			{
				return maxDate_;
			}
			set
			{
				if (minDate_ > value)
				{
					throw new ArgumentOutOfRangeException("value", "Exceeds MinDate");
				}
				maxDate_ = value;
			}
		}

		public ExtendedPathFilter(string filter, long minSize, long maxSize)
			: base(filter)
		{
			MinSize = minSize;
			MaxSize = maxSize;
		}

		public ExtendedPathFilter(string filter, DateTime minDate, DateTime maxDate)
			: base(filter)
		{
			MinDate = minDate;
			MaxDate = maxDate;
		}

		public ExtendedPathFilter(string filter, long minSize, long maxSize, DateTime minDate, DateTime maxDate)
			: base(filter)
		{
			MinSize = minSize;
			MaxSize = maxSize;
			MinDate = minDate;
			MaxDate = maxDate;
		}

		public override bool IsMatch(string name)
		{
			bool flag = base.IsMatch(name);
			if (flag)
			{
				FileInfo fileInfo = new FileInfo(name);
				flag = (MinSize <= fileInfo.Length && MaxSize >= fileInfo.Length && MinDate <= fileInfo.LastWriteTime && MaxDate >= fileInfo.LastWriteTime);
			}
			return flag;
		}
	}
}
