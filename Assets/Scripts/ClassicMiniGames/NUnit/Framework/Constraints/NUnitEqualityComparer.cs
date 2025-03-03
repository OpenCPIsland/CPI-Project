using System;
using System.Collections;
using System.IO;

namespace NUnit.Framework.Constraints
{
	public class NUnitEqualityComparer
	{
		public class FailurePoint
		{
			public int Position;

			public object ExpectedValue;

			public object ActualValue;

			public bool ExpectedHasData;

			public bool ActualHasData;
		}

		private bool caseInsensitive;

		private bool compareAsCollection;

		private ArrayList externalComparers = new ArrayList();

		private ObjectList failurePoints;

		private static readonly int BUFFER_SIZE = 4096;

		public static NUnitEqualityComparer Default
		{
			get
			{
				return new NUnitEqualityComparer();
			}
		}

		public bool IgnoreCase
		{
			get
			{
				return caseInsensitive;
			}
			set
			{
				caseInsensitive = value;
			}
		}

		public bool CompareAsCollection
		{
			get
			{
				return compareAsCollection;
			}
			set
			{
				compareAsCollection = value;
			}
		}

		public IList ExternalComparers
		{
			get
			{
				return externalComparers;
			}
		}

		public IList FailurePoints
		{
			get
			{
				return failurePoints;
			}
		}

		public bool AreEqual(object x, object y, ref Tolerance tolerance)
		{
			failurePoints = new ObjectList();
			if (x == null && y == null)
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}
			if (object.ReferenceEquals(x, y))
			{
				return true;
			}
			Type type = x.GetType();
			Type type2 = y.GetType();
			EqualityAdapter externalComparer = GetExternalComparer(x, y);
			if (externalComparer != null)
			{
				return externalComparer.AreEqual(x, y);
			}
			if (type.IsArray && type2.IsArray && !compareAsCollection)
			{
				return ArraysEqual((Array)x, (Array)y, ref tolerance);
			}
			if (x is IDictionary && y is IDictionary)
			{
				return DictionariesEqual((IDictionary)x, (IDictionary)y, ref tolerance);
			}
			if (x is IEnumerable && y is IEnumerable && (!(x is string) || !(y is string)))
			{
				return EnumerablesEqual((IEnumerable)x, (IEnumerable)y, ref tolerance);
			}
			if (x is string && y is string)
			{
				return StringsEqual((string)x, (string)y);
			}
			if (x is Stream && y is Stream)
			{
				return StreamsEqual((Stream)x, (Stream)y);
			}
			if (Numerics.IsNumericType(x) && Numerics.IsNumericType(y))
			{
				return Numerics.AreEqual(x, y, ref tolerance);
			}
			if (tolerance != null && tolerance.Value is TimeSpan)
			{
				TimeSpan t = (TimeSpan)tolerance.Value;
				if (x is DateTime && y is DateTime)
				{
					return ((DateTime)x - (DateTime)y).Duration() <= t;
				}
				if (x is TimeSpan && y is TimeSpan)
				{
					return ((TimeSpan)x - (TimeSpan)y).Duration() <= t;
				}
			}
			return x.Equals(y);
		}

		private EqualityAdapter GetExternalComparer(object x, object y)
		{
			foreach (EqualityAdapter externalComparer in externalComparers)
			{
				if (externalComparer.CanCompare(x, y))
				{
					return externalComparer;
				}
			}
			return null;
		}

		private bool ArraysEqual(Array x, Array y, ref Tolerance tolerance)
		{
			int rank = x.Rank;
			if (rank != y.Rank)
			{
				return false;
			}
			for (int i = 1; i < rank; i++)
			{
				if (x.GetLength(i) != y.GetLength(i))
				{
					return false;
				}
			}
			return EnumerablesEqual(x, y, ref tolerance);
		}

		private bool DictionariesEqual(IDictionary x, IDictionary y, ref Tolerance tolerance)
		{
			if (x.Count != y.Count)
			{
				return false;
			}
			CollectionTally collectionTally = new CollectionTally(this, x.Keys);
			if (!collectionTally.TryRemove(y.Keys) || collectionTally.Count > 0)
			{
				return false;
			}
			foreach (object key in x.Keys)
			{
				if (!AreEqual(x[key], y[key], ref tolerance))
				{
					return false;
				}
			}
			return true;
		}

		private bool CollectionsEqual(ICollection x, ICollection y, ref Tolerance tolerance)
		{
			IEnumerator enumerator = x.GetEnumerator();
			IEnumerator enumerator2 = y.GetEnumerator();
			int num = 0;
			bool flag2;
			bool flag3;
			while (true)
			{
				bool flag = true;
				flag2 = enumerator.MoveNext();
				flag3 = enumerator2.MoveNext();
				if (!flag2 && !flag3)
				{
					return true;
				}
				if (flag2 != flag3 || !AreEqual(enumerator.Current, enumerator2.Current, ref tolerance))
				{
					break;
				}
				num++;
			}
			FailurePoint failurePoint = new FailurePoint();
			failurePoint.Position = num;
			failurePoint.ExpectedHasData = flag2;
			if (flag2)
			{
				failurePoint.ExpectedValue = enumerator.Current;
			}
			failurePoint.ActualHasData = flag3;
			if (flag3)
			{
				failurePoint.ActualValue = enumerator2.Current;
			}
			failurePoints.Insert(0, failurePoint);
			return false;
		}

		private bool StringsEqual(string x, string y)
		{
			string text = caseInsensitive ? x.ToLower() : x;
			string value = caseInsensitive ? y.ToLower() : y;
			return text.Equals(value);
		}

		private bool EnumerablesEqual(IEnumerable x, IEnumerable y, ref Tolerance tolerance)
		{
			IEnumerator enumerator = x.GetEnumerator();
			IEnumerator enumerator2 = y.GetEnumerator();
			int num = 0;
			bool flag2;
			bool flag3;
			while (true)
			{
				bool flag = true;
				flag2 = enumerator.MoveNext();
				flag3 = enumerator2.MoveNext();
				if (!flag2 && !flag3)
				{
					return true;
				}
				if (flag2 != flag3 || !AreEqual(enumerator.Current, enumerator2.Current, ref tolerance))
				{
					break;
				}
				num++;
			}
			FailurePoint failurePoint = new FailurePoint();
			failurePoint.Position = num;
			failurePoint.ExpectedHasData = flag2;
			if (flag2)
			{
				failurePoint.ExpectedValue = enumerator.Current;
			}
			failurePoint.ActualHasData = flag3;
			if (flag3)
			{
				failurePoint.ActualValue = enumerator2.Current;
			}
			failurePoints.Insert(0, failurePoint);
			return false;
		}

		private bool StreamsEqual(Stream x, Stream y)
		{
			if (x == y)
			{
				return true;
			}
			if (!x.CanRead)
			{
				throw new ArgumentException("Stream is not readable", "expected");
			}
			if (!y.CanRead)
			{
				throw new ArgumentException("Stream is not readable", "actual");
			}
			if (!x.CanSeek)
			{
				throw new ArgumentException("Stream is not seekable", "expected");
			}
			if (!y.CanSeek)
			{
				throw new ArgumentException("Stream is not seekable", "actual");
			}
			if (x.Length != y.Length)
			{
				return false;
			}
			byte[] array = new byte[BUFFER_SIZE];
			byte[] array2 = new byte[BUFFER_SIZE];
			BinaryReader binaryReader = new BinaryReader(x);
			BinaryReader binaryReader2 = new BinaryReader(y);
			long position = x.Position;
			long position2 = y.Position;
			try
			{
				binaryReader.BaseStream.Seek(0L, SeekOrigin.Begin);
				binaryReader2.BaseStream.Seek(0L, SeekOrigin.Begin);
				for (long num = 0L; num < x.Length; num += BUFFER_SIZE)
				{
					binaryReader.Read(array, 0, BUFFER_SIZE);
					binaryReader2.Read(array2, 0, BUFFER_SIZE);
					for (int i = 0; i < BUFFER_SIZE; i++)
					{
						if (array[i] != array2[i])
						{
							failurePoints.Insert(0, num + i);
							return false;
						}
					}
				}
			}
			finally
			{
				x.Position = position;
				y.Position = position2;
			}
			return true;
		}
	}
}
