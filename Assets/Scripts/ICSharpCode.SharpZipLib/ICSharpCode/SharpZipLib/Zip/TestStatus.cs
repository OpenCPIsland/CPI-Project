namespace ICSharpCode.SharpZipLib.Zip
{
	public class TestStatus
	{
		private ZipFile file_;

		private ZipEntry entry_;

		private bool entryValid_;

		private int errorCount_;

		private long bytesTested_;

		private TestOperation operation_;

		public TestOperation Operation
		{
			get
			{
				return operation_;
			}
		}

		public ZipFile File
		{
			get
			{
				return file_;
			}
		}

		public ZipEntry Entry
		{
			get
			{
				return entry_;
			}
		}

		public int ErrorCount
		{
			get
			{
				return errorCount_;
			}
		}

		public long BytesTested
		{
			get
			{
				return bytesTested_;
			}
		}

		public bool EntryValid
		{
			get
			{
				return entryValid_;
			}
		}

		public TestStatus(ZipFile file)
		{
			file_ = file;
		}

		internal void AddError()
		{
			errorCount_++;
			entryValid_ = false;
		}

		internal void SetOperation(TestOperation operation)
		{
			operation_ = operation;
		}

		internal void SetEntry(ZipEntry entry)
		{
			entry_ = entry;
			entryValid_ = true;
			bytesTested_ = 0L;
		}

		internal void SetBytesTested(long value)
		{
			bytesTested_ = value;
		}
	}
}
