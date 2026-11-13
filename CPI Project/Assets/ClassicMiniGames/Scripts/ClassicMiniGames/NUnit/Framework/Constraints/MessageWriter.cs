using System.Collections;
using System.Globalization;
using System.IO;

namespace NUnit.Framework.Constraints
{
	public abstract class MessageWriter : StringWriter
	{
		public abstract int MaxLineLength
		{
			get;
			set;
		}

		protected MessageWriter()
			: base(CultureInfo.InvariantCulture)
		{
		}

		public void WriteMessageLine(string message, params object[] args)
		{
			WriteMessageLine(0, message, args);
		}

		public abstract void WriteMessageLine(int level, string message, params object[] args);

		public abstract void DisplayDifferences(Constraint constraint);

		public abstract void DisplayDifferences(object expected, object actual);

		public abstract void DisplayDifferences(object expected, object actual, Tolerance tolerance);

		public abstract void DisplayStringDifferences(string expected, string actual, int mismatch, bool ignoreCase, bool clipping);

		public abstract void WriteConnector(string connector);

		public abstract void WritePredicate(string predicate);

		public abstract void WriteExpectedValue(object expected);

		public abstract void WriteModifier(string modifier);

		public abstract void WriteActualValue(object actual);

		public abstract void WriteValue(object val);

		public abstract void WriteCollectionElements(IEnumerable collection, int start, int max);
	}
}
